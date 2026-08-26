# コンセプト

## コンパイル時に依存関係をリンクする

tsr-di の利用方法は DI コンテナに似ています。インターフェースをサービス契約とし、実装クラスを登録し、コンストラクターへ依存関係を注入します。

大きな違いは、登録表を実行時に組み立てて探索するのではなく、Source Generator がコンパイル中にサービス宣言と解決要求を読み取り、必要な C# コードを生成する点です。`T` の解決方法が見つからない、または一意に決まらない状態は、実行時の例外ではなくビルド時のエラーとします。

このため tsr-di は、動的なコンテナというよりも、宣言されたサービスと利用箇所を静的に結ぶリンカーに近い性格を持ちます。

## 実行時の柔軟性より静的な明確さ

tsr-di は実行中のサービス登録や登録変更、文字列による動的な名前付き解決、`TryResolve<T>`、リフレクションによる探索、プロパティ注入を提供しません。

代わりに、登録と解決の組み合わせをコンパイル時に確認し、生成コードから直接コンストラクターやメソッドを呼び出します。構成を実行時に差し替える必要があるアプリケーションより、ビルド時に依存関係が決まるアプリケーションに向きます。

## 属性は宣言、Generator が解釈する

`[ServiceClass]` や `[ServiceFunction]` は登録内容を記述するデータです。ライフタイム、名前、delegate 型などの値を Generator が読み取り、組み合わせの妥当性を検査します。

属性インスタンスの getter や setter を実行時に処理して適用することはしません。Roslyn は属性のコンストラクター引数と名前付き引数をデータとして扱うため、仕様の解釈と診断は Generator 側に集約されています。

## クラスと関数を同じ「サービス」として扱う

tsr-di はメソッドを delegate として登録できます。  
インターフェースによる解決では、メソッドだけを利用したい場合でもクラスを公開する必要がありますが、tsr-di ではメソッドのみを公開できます。

```csharp
[ServiceClass(Lifetime = Lifetime.Singleton)]
public sealed class UserStore
{
    [ServiceFunction(ServiceName = "CreateUser")]
    public User Create(UserInfo info) => /* 永続化など */ default!;
}
```

```csharp
using Sample.AutoDefined;

CreateUser createUser = AppServices.Resolve<CreateUser>();
User user = createUser(info);
```

`UserStore` はリソース管理や状態保持を担う実装上の単位であり、`CreateUser` は利用側が必要とする操作の契約です。
操作を公開するためにインターフェースを用意する必要はありません。ドメイン上の要求のみを依存関係にできます。

インスタンスメソッドの場合、親クラスにも `[ServiceClass]` が必要です。メソッドのライフタイムは親オブジェクトのライフタイムによって決まるためです。親クラスが公開インターフェースを実装していなければ、そのクラス自体は解決対象にならず、delegate を提供するための管理対象としてのみ使われます。

## スコープは Resolve の呼び出し単位

`Scoped` は、1 回の公開 `Resolve` または `ResolveAll` の内部で共有されます。複数型を一度に解決するオーバーロードでも、同じ解決コンテキストが使われます。

```csharp
var (reader, writer) = AppServices.Resolve<IReader, IWriter>();
```

両方の依存グラフから同じ Scoped サービスへ到達すれば、その呼び出し内では同じインスタンスです。別の `Resolve` 呼び出しでは別インスタンスになります。

`[FromNewScope]` は、特定のコンストラクター引数以下だけを新しいスコープで解決します。Singleton が Scoped を保持する場合、その Scoped は Singleton の最初の構築に使われた解決スコープで決まります。共有境界を固定したい依存には `[FromNewScope]` を使います。

## 関連

静的 DI / コンパイル時 DI としては、以下のような実装もあります。

- [jab](https://github.com/pakrym/jab) — Jab Compile Time Dependency Injection
- [Pure.DI](https://github.com/DevTeam/Pure.DI) — Pure.DI for .NET
- [StrongInject](https://github.com/YairHalberstadt/stronginject) — Compile Time Dependency Injection For C#
