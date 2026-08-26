# 利用ガイド

## 導入

C# 14 以上のプロジェクトから `tsr-di` を直接参照します。

```xml
<ItemGroup>
  <PackageReference Include="tsr-di" Version="1.*" PrivateAssets="all" />
</ItemGroup>
```

`PrivateAssets="all"` は Generator の参照を下流へ通常の依存関係として公開したくない場合に指定します。生成処理はパッケージを直接参照したプロジェクトでのみ有効なので、指定しなくとも、テストプロジェクトなどの参照先で有効になることはありません。

単一プロジェクトでは必要な属性定義も生成されるため、`tsr-di.Attribute` は不要です。
複数プロジェクトへサービス宣言を分ける場合は、宣言する各プロジェクトから共通の `tsr-di.Attribute` を参照します。リゾルバーを生成するプロジェクトは `tsr-di` も直接参照します。

## リゾルバーとクラスサービス

1 コンパイルに 1 つ、`static partial` リゾルバーを宣言します。

```csharp
using tsr_di;

namespace Sample;

[ServiceResolver]
public static partial class AppServices;

public interface IClock
{
    DateTimeOffset Now { get; }
}

[ServiceClass(Lifetime = Lifetime.Singleton)]
public sealed class SystemClock : IClock
{
    public DateTimeOffset Now => DateTimeOffset.Now;
}

public interface IGreeter
{
    string Greet(string name);
}

[ServiceClass]
public sealed class Greeter(IClock clock) : IGreeter
{
    public string Greet(string name) => $"{clock.Now:t} Hello, {name}";
}
```

```csharp
IGreeter greeter = AppServices.Resolve<IGreeter>();
```

サービスクラスは public で、public コンストラクターが 1 つ必要です。  
実装するすべての public インターフェースが解決対象になります。

## ライフタイム

- `Transient`（既定値）: 要求のたびに生成。同じ解決グラフ内でも共有しません。
- `Scoped`: 1 回の `Resolve` / `ResolveAll` 呼び出し内で共有します。
- `Singleton`: リゾルバー全体で共有します。生成はロックで保護されます。

複数のルートを同じスコープで取得するには、型引数を複数指定します。

```csharp
var (command, query) = AppServices.Resolve<ICommand, IQuery>();
```

以下のように 2 回の `Resolve` に分けた場合は、別スコープです。

```csharp
var command = AppServices.Resolve<ICommand>();
var query = AppServices.Resolve<IQuery>();
```

## 名前付きサービス

```csharp
[ServiceClass(Name = "Primary")]
public sealed class PrimaryStore : IStore;

[ServiceClass(Name = "Backup")]
public sealed class BackupStore : IStore;
```

`Name` は `ServiceKey` の enum メンバーとして生成されます。有効な識別子となる名前を指定してください。  
`Resolve` 呼び出しでは、生成された enum 型で指定します。
enum 値を引数に直接指定する必要があります。enum 値が代入された変数などを指定することはできません。

```csharp
using Sample.AutoDefined;

IStore store = AppServices.Resolve<IStore>(ServiceKey.Primary);
```

コンストラクター注入では `[FromNamed]` を使います。

```csharp
[ServiceClass]
public sealed class Importer([FromNamed("Backup")] IStore store) : IImporter;
```

こちらは文字列による指定ですが、有効な名前か否かは Generator がコンパイル時に検証します。

## コレクション

配列、`IEnumerable<T>`、`List<T>` のコンストラクター引数には、名前の有無にかかわらず指定された型の登録済みサービスがすべて注入されます。

```csharp
[ServiceClass]
public sealed class Pipeline(IEnumerable<IHandler> handlers) : IPipeline;

ReadOnlyCollection<IHandler> handlers = AppServices.ResolveAll<IHandler>();
```

列挙順は保証されません。必要であれば別途ソートしてください。

## 新しいスコープ

`[FromNewScope]` を付けた引数以下は新しい解決コンテキストを使います。

```csharp
[ServiceClass(Lifetime = Lifetime.Singleton)]
public sealed class WorkerHost(
    [FromNamed("Background"), FromNewScope] IWorker worker) : IWorkerHost;
```

付与された引数の配下の Scoped は親と共有されません。属性を付けた引数ごとに独立した新しいスコープが作成されます。  

## 関数サービス

static メソッドへ `[ServiceFunction]` を付けると、そのシグネチャの delegate が生成されます。

```csharp
public static class TextOperations
{
    [ServiceFunction(ServiceName = "NormalizeText")]
    public static string Normalize(string value) => value.Trim();
}
```

```csharp
using Sample.AutoDefined;

NormalizeText normalize = AppServices.Resolve<NormalizeText>();
```

`ServiceName` を省略すると `<メソッド名>Func` になります。既存 delegate を契約にする場合は `ServiceType = typeof(...)` を指定します。

インスタンスメソッドでは親クラスにも `[ServiceClass]` が必要で、その `Lifetime` が状態共有を決めます。  
関数用の親クラスは public インターフェースを実装しなくても構いませんが、`SharingMode.IsolatedPerService` は指定できません。

## delegate の部分適用

引数を持つ登録 delegate には、先頭から引数を固定する `Bind` 拡張メソッドが生成されます。  
固定後に残る引数と戻り値に応じた `Func` または `Action` が返されます。

```csharp
public delegate int Add3(int x, int y, int z);

Add3 add = AppServices.Resolve<Add3>();
Func<int, int> addTo4 = add.Bind(4);
```

すべての引数を固定すると、`Func<TResult>` または `Action` になります。

## 直接解決プロパティ

```csharp
IClock clock = AppServices.Services.Sample.IClock;
IStore primary = AppServices.Services.Sample.IStore_Primary;
```

型の名前空間が静的クラスの階層になり、名前付き登録は `_Name` が付きます。プロパティを読むたびに新しい解決スコープから始まるため、複数ルートで Scoped を共有する場合は複数型の `Resolve` を使います。
