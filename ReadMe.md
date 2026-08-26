# tsr-di (T-Service/Symbol-Resolver)

tsr-di は、C# の型と関数を属性で宣言し、依存関係をコンパイル時に結び付ける Source Generator です。

一般的な DI コンテナと同じように、インターフェースから実装を取得し、コンストラクターへ依存関係を注入できます。一方で、実行時の登録表や汎用的な探索処理は持ちません。Source Generator が必要な生成・呼び出しコードを作り、未登録、登録の競合、不正な名前などをビルド時のエラーとして報告します。

クラスだけでなくメソッドを delegate として登録できることも特徴です。オブジェクト指向で実装された機能を、利用側では `CreateUser` のような操作を表す関数として受け取れます。

## 主な特徴

- 属性によるクラスおよび関数の静的な登録
- コンパイル時に確定する依存関係
- `Transient`、`Scoped`、`Singleton` のライフタイム
- 型安全な `ServiceKey` による名前付き解決
- コンストラクターへの単一サービスおよびコレクションの注入
- delegate の自動生成と先頭引数からの部分適用
- 参照アセンブリに宣言されたサービスの収集
- 生成された静的プロパティによる直接解決

## 最小例

tsr-di パッケージを直接参照した C# 14 以上のプロジェクトで、リゾルバーとサービスを宣言します。

```csharp
using tsr_di;

namespace Sample;

public interface IMessageWriter
{
    void Write(string message);
}

[ServiceClass(Lifetime = Lifetime.Singleton)]
public sealed class ConsoleMessageWriter : IMessageWriter
{
    public void Write(string message) => Console.WriteLine(message);
}

[ServiceResolver]
public static partial class AppServices;
```

```csharp
var writer = AppServices.Resolve<IMessageWriter>();
writer.Write("Hello");
```

`Resolve<IMessageWriter>()` の実装と `ConsoleMessageWriter` の生成処理はコンパイル時に作られます。実装が見つからない場合や、名前なしの実装が複数あって一意に決まらない場合、ビルドは tsr-di の診断エラーになります。

## ドキュメント

- [コンセプト](docs/concepts.md) — tsr-di が解決する問題、一般的な DI との違い、関数をサービスにする意図
- [利用ガイド](docs/getting-started.md) — 導入からクラス、名前付きサービス、スコープ、関数の利用まで
- [API リファレンス](docs/reference.md) — 属性、生成 API、ライフタイム、制約と診断
- [実装の概要](docs/implementation.md) — Source Generator の処理とソース構成

## 動作要件とパッケージ

- C# 14 以上
- `tsr-di`: Source Generator 本体。直接 `PackageReference` したプロジェクトで動作します。
- `tsr-di.Attribute`: 複数プロジェクトでサービス宣言を共有するときに使う属性定義パッケージです。単一プロジェクトでは通常不要です。

tsr-di は生成時に使用される analyzer パッケージであり、通常の実行時ライブラリとしてアプリケーションへ組み込まれるものではありません。

C# 14 は Source Generator が出力するコードをコンパイルするための言語仕様の要件であり、ターゲットフレームワークを同世代の .NET に限定するものではありません。C# 14 を扱える SDK / コンパイラーから、より古い .NET を対象にできます。ただし、利用する言語機能が対象フレームワークにない補助型を要求する場合は、プロジェクト側で互換定義が必要です。たとえば古いターゲットで `record` や `init` を使う場合は、対象フレームワークに存在しないときだけ `System.Runtime.CompilerServices.IsExternalInit` を定義します。

古いターゲット向けの `IsExternalInit` の最小定義例です。この定義は対象フレームワークに同じ型がない場合だけ追加してください。

```csharp
namespace System.Runtime.CompilerServices;

internal static class IsExternalInit;
```

## ライセンス

[MIT License](LICENSE.txt)
