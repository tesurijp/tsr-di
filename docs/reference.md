# API リファレンス

## 前提

- C# 14 以上
- 属性の名前空間: `tsr_di`
- 自動生成型: リゾルバーの名前空間の `.AutoDefined`
- 1 コンパイルにつき `[ServiceResolver]` は 1 つ

C# 14 は言語仕様の要件であり、ターゲットフレームワークを同世代の .NET に限定するものではありません。C# 14 を扱える SDK / コンパイラーから、より古い .NET を対象にできます。  
ただし、利用する言語機能が対象フレームワークにない補助型を要求する場合は、プロジェクト側で互換定義が必要です。たとえば古いターゲットで `record` や `init` を使う場合は、対象フレームワークに存在しない場合に限り、`System.Runtime.CompilerServices.IsExternalInit` の定義が必要です。

## 属性

### `[ServiceResolver]`

サービスを公開するリゾルバークラスへ付与します。  

対象は `partial` が必要で、通常は `public static partial class` として宣言します。

### `[ServiceClass]`

サービス登録するクラスに付与します。  

public クラスが実装するすべての public インターフェースを登録します。  
単一の public コンストラクターが必要です。

`[ServiceFunction]` を提供する親クラスとしてのみ使い、クラスそのものの解決が不要な場合は、public インターフェースを実装しなくても構いません。この場合、クラス自体は解決対象になりません。

複数インターフェースを実装する場合、インターフェースごとにインスタンスを個別に管理するか共有するかを `SharingMode` で指定します。

| プロパティ | 型 | 既定値 | 意味 |
|---|---|---|---|
| `Lifetime` | `Lifetime` | `Transient` | インスタンスの保持範囲 |
| `SharingMode` | `SharingMode` | `Shared` | 複数インターフェース間の共有 |
| `Name` | `string?` | `null` | 名前付き登録のキー。C# の有効な識別子が必要 |

### `[ServiceFunction]`

サービス登録するメソッドに付与します。  

public メソッドを delegate サービスとして登録します。static メソッド、または `[ServiceClass]` のインスタンスメソッドを指定できます。

| プロパティ | 型 | 既定値 | 意味 |
|---|---|---|---|
| `ServiceType` | `Type?` | `null` | 契約に使う既存 delegate 型 |
| `ServiceName` | `string?` | `null` | 自動生成 delegate 型の名前 |
| `Name` | `string?` | `null` | 名前付き登録のキー。C# の有効な識別子が必要 |

`ServiceType` が未指定の場合 delegate を生成します。  
型名には `ServiceName` が使われます。未指定の場合は `<メソッド名>Func` です。

### `[FromNamed("Name")]`

コンストラクターのパラメーターに付与します。  

該当する名前で登録されたサービスを注入します。属性が未指定の場合は、名前なしで登録されたサービスを注入します。

### `[FromNewScope]`

コンストラクターのパラメーターに付与します。  

対象のパラメーター以下を新しい `ResolveContext` で解決します。  
`[FromNamed]` と併用できます。  

## 列挙型

### `Lifetime`

| 値 | 動作 |
|---|---|
| `Transient` | 保持せず、要求のたびに生成 |
| `Scoped` | 1 回の解決コンテキスト内で遅延生成して共有 |
| `Singleton` | static フィールドで遅延生成して共有。共通ロックで保護 |

### `SharingMode`

| 値 | 動作 |
|---|---|
| `Shared` | 同じ保持領域を使う |
| `IsolatedPerService` | 個別の保持領域を使う |

複数のインターフェースを実装するサービスクラスで、インスタンスをどのように保持するかを指定します。  
`Shared` の場合は、どのインターフェースで解決された場合でも同じインスタンスを返します。  
`Lifetime` が `Transient` の場合、いずれを指定しても常に新規生成です。
インスタンス関数をサービスとして定義している場合、親クラスに `IsolatedPerService` は指定できません。

## 生成される API

### `T Resolve<T>(ServiceKey key = ServiceKey.None)`

単一サービスを解決します。  
Generator はソースに現れるリゾルバーの `Resolve<T>` 呼び出しを収集し、使用された型とキー向けの実装を生成します。  
`ServiceKey.Xxx` を呼び出しに直接記述します。変数に代入された値などを指定することはできません。

### `ReadOnlyCollection<T> ResolveAll<T>()`

`T` の名前なし・名前付き全登録を返します。順序は保証されません。

#### 複数型オーバーロード

`Resolve<>()` および `ResolveAll<>()` の型引数は、必要なだけ任意の個数を指定できます。  

```csharp
(T1, T2) Resolve<T1, T2>(
    ServiceKey key1 = ServiceKey.None,
    ServiceKey key2 = ServiceKey.None);

(ReadOnlyCollection<T1>, ReadOnlyCollection<T2>)
    ResolveAll<T1, T2>();
```

1 回の呼び出しでは共通の `ResolveContext` が使われるため、複数のルート要素を同一のスコープで解決する場合に使用します。

### `Services`

全登録に対する直接解決プロパティです。  
プロパティ参照 1 回が、1 つのスコープ（`Resolve()` 呼び出し 1 回）に相当します。

```text
<Resolver>.Services.<namespace classes>.<service type>[_<Name>]
```

例: `AppServices.Services.Sample.IStore_Primary`

### `ServiceKey`

`None` と、`ServiceClass.Name` / `ServiceFunction.Name` の重複を除いた値を持つ enum です。

### delegate と `Bind`

`ServiceType` を指定しなかった関数の delegate と、登録 delegate の先頭引数を 1 個以上固定する拡張メソッドを生成します。

## コレクション注入

コンストラクター引数を `T[]`、`IEnumerable<T>`、`List<T>` 型で指定した場合、`T` 型のインターフェースで登録されたすべてのサービスコレクションが注入されます。

## 主な診断

すべて既定で error です。

| ID | 条件 |
|---|---|
| `TD9001` | C# 14 未満 |
| `TD0001`–`TD0003` | リゾルバーがない、複数、または partial でない |
| `TD0011`–`TD0015` | 未登録、キー不一致、登録競合、引数個数不一致 |
| `TD0021`–`TD0024` | クラスの公開性、契約、コンストラクター、キーが不正 |
| `TD0031`–`TD0036` | 関数の公開性、親クラス、共有モード、名前、delegate 型が不正 |

詳細なエラーメッセージはビルド診断に表示されます。

## 制約

以下の機能は、tsr-di のコンセプトに沿わないためサポートしません。

- 実行時登録、動的探索、プロパティ注入は対応しません。
- `ServiceKey` の値を実行時に参照または計算する使い方は許容されません。
- `Resolve` の型引数としてソースに現れない型向けの汎用解決コードは生成しません。直接解決プロパティは登録全体から生成します。
- コレクションの列挙順は保証しません。
- `IDisposable` / `IAsyncDisposable` は登録可能ですが、専用の破棄スコープなどを管理する仕組みは提供しません。
