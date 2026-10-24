const translations = {
  "copy": {
    "ja": "コピー",
    "en": "Copy"
  },
  "nav1": {
    "ja": "コンセプト",
    "en": "Concept"
  },
  "nav2": {
    "ja": "使い方",
    "en": "Examples"
  },
  "h": {
    "ja": "依存解決は、\nコンパイル時に。",
    "en": "Resolve dependencies.\nAt compile time."
  },
  "lead": {
    "ja": "実行時に登録表を探索する DI から、生成コードで直接呼び出す DI へ。\nクラスだけでなく、メソッドも型安全なサービスとして解決します。",
    "en": "Move from runtime container lookup to direct calls in generated code.\nResolve classes and methods as type-safe services."
  },
  "start": {
    "ja": "使い始める",
    "en": "Get started"
  },
  "note": {
    "ja": "実行時のサービス登録・探索処理は不要。",
    "en": "No runtime service registration or lookup."
  },
  "ct": {
    "ja": "依存解決を、実行時に持ち込まない。",
    "en": "Resolve at build time. Call directly at runtime."
  },
  "cp": {
    "ja": "実行時に登録表を構築・探索する一般的な DI コンテナに対し、tsr-di は Source Generator がコンパイル時に依存関係を確定します。実行時は生成されたコードがコンストラクターやメソッドを直接呼び出すため、登録表の構築・探索やリフレクションによる依存解決の負荷がありません。未登録や登録の競合も、ビルド時に検出します。",
    "en": "Where conventional DI containers build and search a registry at runtime, tsr-di determines dependencies at compile time using a Source Generator. Generated code calls constructors and methods directly, eliminating runtime registry construction, lookup and reflection-based resolution overhead. Missing or conflicting registrations are caught during the build."
  },
  "declare": {
    "ja": "属性で宣言",
    "en": "Declare"
  },
  "generate": {
    "ja": "ビルドで生成",
    "en": "Generate"
  },
  "resolve": {
    "ja": "型で解決",
    "en": "Resolve"
  },
  "et": {
    "ja": "クラスも、メソッドも、解決対象に。",
    "en": "Resolve classes. Resolve methods, too."
  },
  "ep": {
    "ja": "オブジェクトだけでなく、必要な操作そのものを依存関係として宣言できます。",
    "en": "Declare the operation you need as a dependency, alongside object contracts."
  },
  "cct": {
    "ja": "クラスを、生成コードで直接解決",
    "en": "Resolve classes through generated code"
  },
  "ccp": {
    "ja": "属性で実装とライフタイムを宣言。コンストラクターへの依存注入までコンパイル時に結び付けるため、実行時のサービス探索は不要です。",
    "en": "Declare implementations and lifetimes with attributes. Constructor dependencies are wired at compile time, so no runtime service lookup is needed."
  },
  "fct": {
    "ja": "メソッドも、型安全に解決",
    "en": "Resolve methods as type-safe services"
  },
  "fcp": {
    "ja": "メソッドに属性を付けるだけで、専用の delegate 型を自動生成。Resolve<T>() で操作そのものを取得でき、利用側に実装クラスや専用インターフェースを公開する必要はありません。",
    "en": "Annotate a method to generate a dedicated delegate type. Retrieve the operation itself with Resolve<T>(), without exposing its implementation class or creating a dedicated interface for consumers."
  },
  "bind": {
    "ja": "登録 delegate には、先頭引数を固定する Bind 拡張メソッドも生成されます。",
    "en": "Registered delegates also get Bind extensions for partial application of leading arguments."
  },
  "lt": {
    "ja": "共有する範囲も、明示する。",
    "en": "Make sharing boundaries explicit."
  },
  "lp": {
    "ja": "Scoped は、1 回の Resolve / ResolveAll 内で共有。複数のルートを一度に解決すれば、同じスコープを利用できます。",
    "en": "Scoped instances are shared within one Resolve / ResolveAll call. Resolve multiple roots together to share the same scope."
  },
  "Transient": {
    "ja": "要求ごとに新しく生成。既定のライフタイム。",
    "en": "A new instance for each request. The default lifetime."
  },
  "Scoped": {
    "ja": "1 回の解決呼び出し内でインスタンスを共有。",
    "en": "One shared instance within a resolution call."
  },
  "Singleton": {
    "ja": "リゾルバー全体でインスタンスを共有。",
    "en": "One shared instance across the resolver."
  },
  "key": {
    "ja": "名前付きサービスを生成 enum で指定。登録名の不整合はビルド時に検出します。",
    "en": "Select named services with a generated enum. Invalid registration names are caught at build time."
  },
  "all": {
    "ja": "複数の登録をまとめて解決。コンストラクターへのコレクション注入も利用できます。",
    "en": "Resolve multiple registrations together, with collection injection available for constructors."
  },
  "props": {
    "ja": "生成された静的プロパティから直接解決。名前空間に沿った階層をたどれます。",
    "en": "Resolve through generated static properties organized by namespace."
  },
  "st": {
    "ja": "実行時の探索をなくす、\n静的 DI を。",
    "en": "Static DI.\nWithout runtime lookup."
  },
  "sp": {
    "ja": "C# 14 以上のコンパイラーを使用し、リゾルバーを生成するプロジェクトから直接参照します。",
    "en": "Use a C# 14+ compiler and reference the package directly from the project that generates the resolver."
  },
  "attr": {
    "ja": "サービス宣言を複数プロジェクトで共有する場合は、tsr-di.Attribute を利用します。C# の要件は、ターゲット .NET の世代を限定するものではありません。",
    "en": "Use tsr-di.Attribute to share service declarations across projects. The C# language requirement does not restrict you to the same generation of target .NET."
  },
  "guide": {
    "ja": "利用ガイド",
    "en": "Usage guide"
  },
  "conceptlink": {
    "ja": "コンセプト",
    "en": "Concepts"
  },
  "api": {
    "ja": "API リファレンス",
    "en": "API reference"
  },
  "scope": {
    "ja": "設計方針：ビルド時に依存関係が決まるアプリケーション向け。実行中の登録変更・動的な文字列解決・TryResolve・プロパティ注入は提供しません。詳細と制約は公式ドキュメントをご確認ください。",
    "en": "Designed for applications whose dependencies are known at build time. Runtime registration changes, dynamic string resolution, TryResolve and property injection are not provided. See the official documentation for details and constraints."
  },
  "fst": {
    "ja": "F# の関数も、同じ仕組みで。",
    "en": "F# functions. The same resolution model."
  },
  "fsp": {
    "ja": "F# のモジュール関数は .NET の static メソッドとして公開されます。ServiceFunction 属性を付ければ、C# 側では生成された delegate を通して、関数そのものをサービスとして解決できます。",
    "en": "F# module functions are exposed as .NET static methods. Add the ServiceFunction attribute, then resolve the function itself from C# through a generated delegate."
  },
  "fssetup": {
    "ja": "構成：F# のクラスライブラリから tsr-di.Attribute を参照し、C# アプリからその F# プロジェクトを参照します。C# 側では tsr-di と共通の tsr-di.Attribute を直接参照してください。",
    "en": "Setup: reference tsr-di.Attribute from the F# class library, then add a project reference to it from the C# app. Reference both tsr-di and the shared tsr-di.Attribute package directly from the C# project."
  },
  "fsnote": {
    "ja": "F# 側の実装をラッパークラスや専用インターフェースで包まず、利用側は NormalizeFromFSharp という操作の契約に依存できます。以下は別々のプロジェクトに置く例です。",
    "en": "Consumers depend on the NormalizeFromFSharp operation contract, without wrapping the F# implementation in an adapter class or a dedicated interface. The following snippets belong to separate projects."
  }
};
let language = 'ja';
function setLanguage(lang) {
  language = lang === 'en' ? 'en' : 'ja';
  document.documentElement.lang = language;
  document.querySelectorAll('[data-i18n]').forEach(el => { el.textContent = translations[el.dataset.i18n][language]; });
  document.querySelectorAll('[data-lang]').forEach(el => el.setAttribute('aria-pressed', String(el.dataset.lang === language)));
  document.title = language === 'ja' ? 'tsr-di | 依存解決は、コンパイル時に。' : 'tsr-di | Resolve dependencies at compile time.';
  try { localStorage.setItem('tsr-di-language', language); } catch (_) {}
}
document.querySelectorAll('[data-lang]').forEach(el => el.addEventListener('click', () => setLanguage(el.dataset.lang)));
try { setLanguage(localStorage.getItem('tsr-di-language') || 'ja'); } catch (_) { setLanguage('ja'); }
document.querySelectorAll('.copy').forEach(button => button.addEventListener('click', async () => {
  const value = button.closest('.code').querySelector('code').textContent;
  try {
    if (navigator.clipboard && window.isSecureContext) await navigator.clipboard.writeText(value);
    else { const input = document.createElement('textarea'); input.value = value; document.body.append(input); input.select(); if (!document.execCommand('copy')) throw new Error('copy'); input.remove(); }
    button.textContent = language === 'ja' ? 'コピーしました' : 'Copied';
  } catch (_) { button.textContent = language === 'ja' ? '選択してコピーしてください' : 'Select code to copy'; }
  setTimeout(() => { button.textContent = translations.copy[language]; }, 2000);
}));
