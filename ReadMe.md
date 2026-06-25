# tsr-di (T-Service/Symbol-Resolver) 

tsr-di は、サービスおよびシンボルのための静的リゾルバー（DI コンテナ）です。  

一般的な DI コンテナと同様に、インターフェースとその実装クラスを登録し、必要な場所で解決してインスタンスを取得する機能を提供しており、静的 DI としても利用できます。  
また、クラスだけでなく関数もサービスとして登録し、delegate として静的に解決できます。

本質的な特徴として、以下のような仕組みを提供します。

- サービス契約（依存関係）を属性で宣言する。
- クラスや関数を、解決可能なサービスとして属性で登録する。
- ソースジェネレーターが解決コードを事前に自動生成する。
- 実行時にはサービス登録や動的な解決ロジックを持たない。
- コンパイル時にサービス解決の妥当性を検証・保証する。

これらは、動的に解決する一般的な DI というよりも、コンパイル時に依存関係をリンクする「リンカー」に近い発想に基づいています。  
コンパイル時点で `T` の解決方法は特定されており、解決できないサービスが存在する場合は、実行時例外ではなくコンパイルエラーとなります。

コンパイル時に依存グラフの構築を完了し、実行時には動的な解決を行わないという点は [Pure.DI](https://github.com/DevTeam/Pure.DI) と同様ですが、tsr-di はサービスの「利用箇所」と「実装箇所」の定義に特化しており、任意の複雑な依存関係の構成ロジックをコードで自由に記述するわけではないという点が異なります。

## 1. 主な特徴

- **コンパイル時解決**  
  - すべての依存関係を静的に解決します。
  - コンパイル時に未登録のサービスが要求されたり、同じインターフェースに対して適用可能な複数の実装が競合して登録されたりしている場合は、コンパイルエラーになります。
- **属性による登録**  
  - `AddXXX` のようなメソッドによる実行時の登録機能はありません。
  - `[ServiceClass]` と `[ServiceFunction]` の属性によるコンパイル時の静的な登録のみをサポートします。
- **関数の解決**  
  - `[ServiceFunction]` を付与したメソッドを delegate として解決対象にできます。
  - 静的メソッド、または `[ServiceClass]` が付与されたクラスのインスタンスメソッドを登録できます。
- **型安全な名前付き解決**  
  - `Resolve<T>()` での名前付き解決は、自動生成される `ServiceKey` enum を指定します。
  - 実行時引数として文字列を直接渡す名前解決は行いません。
- **複数アセンブリ（プロジェクト）対応**  
  - 自身のプロジェクトだけでなく、参照しているプロジェクトやアセンブリも `ServiceClass` / `ServiceFunction` の検索対象となります。
- **インスタンス共有方法の指定**  
  - 同一のクラスを複数のサービス（インターフェース）に紐づけて登録した際の、インスタンスの共有方法を指定できます。
- **軽量・高速**  
  - 汎用的な解決ロジックは生成せず、コンパイル時に収集した情報のみに基づく極めてシンプルなコードを出力します。
  - 単一プロジェクトの場合、実行時に追加される依存アセンブリはありません。
    - 複数のプロジェクトに分割して利用する場合は、すべてのアセンブリで属性の定義を共通にする必要があります。そのため、属性定義のみを含む共通アセンブリ（`tsr-di.Attribute`）の参照が必要です。

## 利用方法

### 属性と反映結果

#### `[ServiceResolver]`

リゾルバー（DI コンテナに相当するクラス）に付与します。  
`static partial` クラスとして定義する必要があります。  
`static T Resolve<T>(ServiceKey key = ServiceKey.None)` および `static IEnumerable<T> ResolveAll<T>()` の 2 つの解決用メソッドが自動生成されます。

```csharp
[ServiceResolver]
public static partial class AppResolver;
```

#### `[ServiceClass]`

サービスとして登録するクラスに付与します。  
クラスが実装しているすべてのインターフェースがサービス解決の対象となります。

以下のパラメータで登録内容をカスタマイズできます。

- `LifeTime`: 生存期間（ライフタイム）  
  - `Transient` (デフォルト): 必要になるたびにインスタンスを作成します。同一の `Resolve` 呼び出しの内部であっても、必要になるたびに作成します。
  - `Scoped`: 同一の `Resolve` 呼び出しの中で解決されるインスタンスを共有します。
  - `Singleton`: アプリケーション全体で単一のインスタンスを共有します。
- `SharingMode`: 共有方法  
  主に複数のインターフェースを実装する `Singleton` サービスでの共有方法を調整するための設定です。
  - `Shared` (デフォルト): どのインターフェースで解決される場合でも、共通のインスタンスを返します。
  - `IsolatePerService`: インターフェースごとに個別のインスタンスを保持します。インターフェースが共通である場合は、共通のインスタンスになります。
- `Name`: 名前付き解決  
  同じインターフェースに対して複数の実装を登録する場合、この名前で識別します。指定した名前は自動生成される `ServiceKey` enum のメンバーになります。

```csharp
[ServiceClass(LifeTime = LifeTime.Singleton, Name = "MainService")]
public class MyService : IMyService;
```

名前付き登録に指定する名前は、生成される enum のメンバー名として利用できる C# の識別子にしてください。名前なしのサービスは `ServiceKey.None` に対応します。

#### `[ServiceFunction]`

サービスとして登録するメソッドに付与します。  
登録された関数は delegate として解決対象になります。

静的メソッドはそのまま登録できます。インスタンスメソッドを登録する場合は、所属クラスにも `[ServiceClass]` を付与する必要があります。その場合、所属クラスのインスタンス解決と同じ仕組みでメソッドの delegate が作成されます。

以下のパラメータで登録内容をカスタマイズできます。

- `ServiceType`: 解決対象として使う delegate 型を明示します。
- `ServiceName`: `ServiceType` を指定しない場合に、自動生成される delegate 名の元になる名前を指定します。
- `Name`: 名前付き解決用の名前を指定します。指定した名前は `ServiceKey` enum のメンバーになります。

`ServiceType` を指定しない場合は、メソッドのシグネチャから delegate 型が自動生成されます。自動生成された delegate と `ServiceKey` は、リゾルバーと同じ名前空間の `AutoDefined` 名前空間に出力されます。

```csharp
public class Commands
{
    [ServiceFunction(ServiceName = "ExecuteCommand")]
    public static int Execute(string value) => value.Length;
}
```

```csharp
using MyApp.AutoDefined;

var execute = AppResolver.Resolve<IExecuteCommand>();
var length = execute("sample");
```

#### `[FromNamed]`

コンストラクター引数に付与し、どの名前付きサービスを注入するかを指定します。

`Name` で指定された名前を持つサービスを解決し、コンストラクターの該当引数に注入します。  
`[FromNamed]` が付与されていない場合は、`Name` パラメータのない（名前なしの）サービスが注入されます。

`Resolve<T>()` では `ServiceKey` enum を使いますが、`[FromNamed]` は文字列で名前を指定します。`ServiceKey` は利用側の実装コード内に自動生成される型であり、ライブラリ側で定義される `[FromNamed]` 属性からは参照できません。  
また、`Resolve<T>()` の引数は実行時に渡される値であるため、動的な指定の正当性を保護する目的で `ServiceKey` を使います。一方、`[FromNamed]` の文字列はソースジェネレーターがコンパイル時に検証するため、静的に正当性を保証できます。  
このように、機能構成をシンプルに保ちながら十分な正当性保証を行うため、`Resolve<T>()` と `[FromNamed]` では異なる指定方法を採用しています。

```csharp
[ServiceClass]
public class Client([FromNamed("MainService")] IMyService service)
{
    // ...
}
```

### サービス解決と注入

提供される基本機能は、一般的な DI コンテナと大きな違いはありません。  
サービスの取得には `Resolve` メソッドを使用し、依存関係はコンストラクターの引数へ注入されます（プロパティ注入などには対応していません）。

大きな特徴として、メソッドによる解決であれコンストラクター引数への注入であれ、解決できないサービスが存在する場合はコンパイルエラーになります。  
そのため、`T Resolve<T>()` および `IEnumerable<T> ResolveAll<T>()` といった確実に解決できることを前提としたインターフェースのみを提供し、`TryResolve<T>(out T)` や `T? Resolve<T>()` のような実行時に解決結果を確認する必要のあるインターフェースは提供しません。

- **メソッドによる解決**  
  `[ServiceResolver]` を付与したクラスに自動生成される 2 つの静的メソッド `Resolve<T>()` および `ResolveAll<T>()` を利用してサービスを解決します。  
  解決の起点となり、`Resolve` 呼び出しから戻るまでの間に行われる解決全体が、`LifeTime.Scoped` の有効期間となります。  
  - `Resolve<T>()` は `T` 型のインスタンスを単一で取得します。  
    `Name` パラメータが付与されたサービスを解決する場合は、`Resolve<T>(ServiceKey.MainService)` のように `ServiceKey` で指定します。  
    登録されたサービスが1件だけの場合であっても、名前付きで登録されているときは名前の指定が必要です。
  - `ResolveAll<T>()` は、名前の有無に関係なく `T` 型で登録されたすべてのインスタンスを返します（並び順は保証されません）。
- **コンストラクター注入**  
  コンストラクター引数で要求された型に、対応するサービスを注入します。  
  `Resolve<T>()` などで起点として要求された型から、そのコンストラクター引数で要求されている型を再帰的にたどって依存関係を解決し、必要なインスタンスをコンストラクターへ渡します。  
  - **通常注入**: 指定された型のサービスをコンストラクター引数へ注入します。`[FromNamed("Xxx")]` 属性が指定された引数には、対応する名前付きサービスを注入します。`[FromNamed]` が指定されていない引数には、名前なしで登録されたサービスを注入します（`Resolve<T>` 相当）。
  - **コレクション注入**: コンストラクター引数の型として `IEnumerable<T>`、`List<T>`、`T[]` などのコレクションを指定した場合は、その型で登録されたすべてのサービスをまとめて注入します（`ResolveAll<T>()` 相当）。
  - **遅延注入**: 引数の型に `Lazy<T>` を指定することで、インスタンスが必要になった時点で初めて解決される遅延依存関係を注入します。
- **関数の解決と部分適用**  
  `[ServiceFunction]` を付与したメソッドは delegate として解決できます。  
  `ServiceType` を指定した場合はその型で解決され、指定しない場合は自動生成された delegate 型で解決されます。  
  また、自動生成された delegate に対しては、引数を部分適用するための拡張メソッド `Bind` が自動的に生成されます。これにより、解決した関数の引数を一部固定した、新しいデリゲート（`Action` または `Func`）を取得できます。

  例えば、引数を3つ取る `delegate void IXYA(int X, int y, int A)` というデリゲートが自動生成された場合、以下のように部分適用を行うことができます。

  ```csharp
  // 引数を3つ取るデリゲートを解決
  var xya = ResolverClass.Resolve<IXYA>();
  xya(1, 2, 3); // 通常の呼び出し

  // 第1引数を固定（部分適用）し、残りの引数2つを受け取るデリゲート（Action<int, int>）を取得
  var ya = to.Bind(1);
  ya(2, 3); // 内部で xya(1, 2, 3) が呼び出されます
  ```

  `Bind` メソッドは、第1引数から第k引数までを固定するために、引数の個数に応じたオーバーロードが自動的に生成されます。すべての引数をバインドした場合は、引数なしの `Action` または `Func<TReturn>` が返されます。

## 生成されるコードについて

tsr-di は主に以下のファイルを生成します

### `Attribute.g.cs`

`ServiceResolverAttribute`, `ServiceClassAttribute`, `ServiceFunctionAttribute`, `LifeTime` などの属性定義が含まれます。  
複数プロジェクトで利用する場合は、属性クラスの定義を共通化する必要があるため、各プロジェクトから `tsr-di.Attribute` プロジェクト（またはパッケージ）を参照します。  
その場合、生成された本ファイル内の属性定義は無効になります。単一プロジェクトで他のプロジェクトのサービスを検索する必要がない場合は、`tsr-di.Attribute` を追加参照することなく、自動生成される本ファイル内の属性をそのまま利用できます。

### `Properties.g.cs`

`FieldStore` クラスが定義され、その中に登録されたクラスサービスや関数 delegate を管理するプロパティが作成されます。  
ライフタイムの違いはプロパティの内部実装で吸収されるため、利用側はライフタイムの違いを意識する必要がありません。  
`FieldStore` は `Resolve<T>()` / `ResolveAll<T>()` のメソッド内ローカル変数としてインスタンス化されるため、ローカル変数のスコープと DI のスコープが一致します。  
`SharingMode` の指定内容によって、インスタンスを保持するプロパティがサービスごとに分割されるか、あるいは共通化されるかが変わります。

- `Singleton`: `static readonly Lazy<object>` フィールドに実体が保持され、アプリケーション全体で共有されます。
- `Scoped`: インスタンスプロパティの `field` に保持され、同一スコープ内で再度参照された場合は同じインスタンスが再利用されます。
- `Transient`: プロパティの内容は保持されず、毎回 `new()` するコードになります。

### `Resolve.g.cs`

`Resolve<T>` および `ResolveAll<T>` の実体が定義されます。  
ローカル変数として `FieldStore` をインスタンス化し、`typeof(T)` による `if` 分岐や `ServiceKey` による `switch` 式を用いて、`FieldStore` の適切なプロパティを呼び出すコードを生成します。  
リフレクションなどは一切使用せず、コンパイル時に確定した条件分岐のみでコードが生成されます。
ローカル変数 `FieldStore` の寿命と DI としてのスコープが一致するため、スコープ管理専用のコードもありません。

実行時の判断を可能な限り排除することがコンセプトであるため、本来は C++ のテンプレート特殊化のような構造にしたいところですが、C# では実現できないため、`Resolve<T>()` 内で型による分岐を生成しています。
分岐判定の対象は、コード上で実際に `Resolve<T>()` や `ResolveAll<T>()` の型引数として使用されている型 `T` のみに限定しています。  
これにより、`[ServiceClass]` や `[ServiceFunction]` 属性を付与していても実際にはコード上で解決要求（呼び出し）のないサービスやインターフェースへの参照がなくなり、Native AOT 等によるトリムの対象になりやすくなります。

### `TypedEnum.g.cs`

名前付き登録に利用する `ServiceKey` enum が定義されます。  
名前なしのサービスを表す `None` と、`ServiceClass.Name` / `ServiceFunction.Name` で指定された名前に対応する enum メンバーが生成されます。

```csharp
namespace MyApp.AutoDefined;

public enum ServiceKey
{
    None,
    MainService,
}
```

### `Delegates.g.cs`

`ServiceFunction` のために自動生成される delegate 型が定義されます。  
また、各delegateに対応する拡張メソッド Bindが定義されます。
`ServiceFunction.ServiceType` で既存の delegate 型を明示した場合は、その型が解決対象として使われます。`ServiceType` を指定しない場合は、メソッドの戻り値と引数から delegate 型が生成されます。

## 関数の解決機能について

関数、メソッドを解決する機能は、一般的なDIコンテナとしては少し特殊な機能です。  
機能そのものは、解決する対象がクラスのインスタンスではなく、メソッドになるというだけですが、この機能によってオブジェクト指向的な発想で作られたライブラリをDDD的なドメインレベルの要求の解決の対象とすることが出来ます。

```csharp

[ServiceClass]
public class UserDB : IUserDB
{
    [ServiceFunction(ServiceName="RegisterUser")]
    User CreateUser(Info xxxx);
    [ServiceFunction(ServiceName="UnregisterUser")]
    void DeleteUser();
}
```

という典型的なオブジェクト指向で作られたクラス設計のライブラリに対して、

```csharp
var userDB = Service.Resolve<IUserDB>();

var user = userDB.CreateUser(xxxx);
userDB.DeleteUser(user);
```

上記のようなオブジェクト指向的にインターフェースを提供するだけでなく、以下のような利用方法が可能になります。

```csharp
var registerUser = Service.Resolve<IRegisterUser>();
var unregisterUser = Service.Resolve<IUnregisterUser>();

var user = registerUser(xxxx)
unregisterUser(user)
```

registerUser や unregisterUser の実態は、UserDBのインスタンスメソッドであるため、UserDBの実装側で従来通りのオブジェクト指向的なクラス設計でリソース管理などを行なうことが可能です。  
ビジネスロジック部分からはドメイン要求を解決する手段のみが見えている状態で、その実現をしているオブジェクトの存在を意識する必要はありません。Repository.Save()のような妥協も持ち込む必要がなくなります。

## 今後の拡張予定

今後の拡張として、データテーブル、定数値など、多様なリソースや情報の静的解決手段となる機能を追加していく予定です。
