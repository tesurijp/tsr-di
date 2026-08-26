# 実装の概要

この文書は、tsr-di が入力を収集し、解決コードへ変換する概要です。

## ソリューションの構成

```text
tsr-di.slnx
├─ tsr-di/
│  ├─ Generator.cs
│  ├─ Collector.cs
│  ├─ Mapper_*.cs
│  ├─ Emitter.cs
│  ├─ RecordTypes.cs / TypeAlias.cs
│  ├─ DirectPackageReferenceGate.cs
│  ├─ Diagnostic.cs
│  ├─ template/
│  └─ build/tsr-di.targets
├─ tsr-di.Attribute/
└─ tsr-di.test/
```

- `tsr-di`: Incremental Source Generator 本体。
- `tsr-di.Attribute`: 複数アセンブリで同じ属性型を参照するための属性定義
- `tsr-di.test`: テスト

## Generator のパイプライン

`Generator.Initialize` が次の Incremental Generator パイプラインを構成します。

```text
属性定義の初期出力
        ↓
Collector: Roslyn symbol の収集
        ↓
Mapper: 保持領域、解決要求、delegate 宣言へ変換
        ↓
正常値と ErrorItem を分離
        ↓
Emitter: C# ソースと Diagnostic を出力
```

各値は `Combine`、`Collect`、`Select` で構成されます。Mapper は symbol から不変の中間データまたは `ErrorItem` を返し、Emitter は判断済みのデータをテンプレートへ配置します。

## 収集: `Collector.cs`

`ForAttributeWithMetadataName` を利用して、リゾルバー、サービスクラス、サービス関数を収集します。収集対象には参照プロジェクトを含みます。

ローカルソースコード内の `Resolve<T>` / `ResolveAll<T>` は SyntaxProvider で見つけ、呼び出し元、型引数、記述されたキーを Semantic Model から取得します。

参照アセンブリは名前空間から再帰的に走査し、共通の属性型を持つクラスとメソッドを集めます。System、Microsoft、FSharp などの一般的な基盤アセンブリは除外します。

## 変換: `Mapper_*.cs`

- `Mapper_FieldStore.cs`: 生成式、ライフタイム、共有モード、コンストラクター引数を `FieldItem` へ変換
- `Mapper_Resolver.cs`: 使用された `Resolve` の型とキーを検査し、`ResolverItem` と直接解決項目を作成
- `Mapper_Declaration.cs`: delegate と必要な複数型 `Resolve` の型引数数を決定
- `Mapper_util.cs`: 属性値、型表示名、登録 lookup の共通処理

`ResultOrError<T>` により正常値と診断候補を同じ変換から返し、`Generator.cs` の `Split` で別々の Incremental Provider に分けます。

## 出力: `Emitter.cs` と `template/*`

出力全体の骨格は埋め込みリソースのテンプレートです。Emitter はフィールド、switch 式、delegate 宣言などの繰り返し部分を文字列で作り、テンプレートへ挿入します。

## 生成ファイル

### `Attribute.g.cs`

属性パッケージを参照しない単一プロジェクト用の内部属性を定義します。  
このファイルは `tsr-di.Attribute` がある場合は条件付きコンパイルで重複を避けます。

### `InnerResolveContext.g.cs`

各サービスクラスの生成式と保持領域を持つ `ResolveContext` を生成します。
サービスクラスの種類によらず同じ方法で値を返せるよう、プロパティでラップし、内部を次のように分けています。  
フィールド名とプロパティ名には、名前空間を含むクラス名のハッシュ値を加えて一意にします。

- Singleton: ネストした static 保持クラスの static 変数として保持します。
- Scoped: `ResolveContext` 内のスコープ変数内で保持します。
- Transient: 生成物を保持せず毎回生成します。

`[FromNewScope]` は対象引数を `default(ResolveContext)` から取得する式を出力することでスコープを外します。

### `InnerResolver.g.cs`

使用された各 `T` の内部 `IResolver<T>` を実装します。`Resolve` は `ServiceKey` の switch からフィールドを選び、`ResolveAll` は全フィールドから `ReadOnlyCollection<T>` を作ります。
switch 式の候補は、ソースコード上で利用されている値のみで、未使用の値は switch 式内に出てきません。

### `Resolve.g.cs`

公開 `Resolve` / `ResolveAll` を生成します。呼び出しの先頭で値型の `ResolveContext` を default 初期化し、内部 resolver へ `ref` で渡します。複数型オーバーロードはソースで使用された個数だけ追加され、結果間で同じ context を共有します。

### `ResolveProp.g.cs`

全登録を名前空間階層の static partial クラスと直接解決プロパティに変換します。各プロパティは `default(ResolveContext)` から対象フィールドを読みます。

### `TypedEnum.g.cs` 

登録名から `ServiceKey` を生成します。

### `Delegates.g.cs`

必要な delegate と `Bind` 拡張メソッドを生成します。

## 直接参照時だけ動作する仕組み

`build/tsr-di.targets` は `TSR_DI_GENERATOR_ENABLED` を CompilerVisibleProperty として Roslyn へ渡します。`DirectPackageReferenceGate` がこの値を読み、Generator 本体の出力と診断を有効化します。
tsr-di では、このフラグ情報を使い、推移的に参照されたプロジェクトでは生成処理を実行しません。

## 診断

`Diagnostic.cs` が `TD` から始まる error を定義します。Mapper は不正な symbol と位置を `ErrorItem` にし、Emitter が `ReportDiagnostic` へ渡します。

登録漏れや競合に加え、不正な識別子、コンストラクター数、delegate シグネチャ、親クラスの共有モードも同じコンパイル時診断の経路で扱います。
