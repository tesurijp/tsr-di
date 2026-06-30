using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace tsr_di;

internal static class Emitter
{
    internal static void OutputErrors(SourceProductionContext context, ImmutableArray<ErrorItem> errors)
    {
        foreach(var item in errors)
        {
            context.ReportDiagnostic(item);
        }
    }
    internal static void WriteAttribute(IncrementalGeneratorInitializationContext context) =>
        context.RegisterPostInitializationOutput((ctx) => ctx.AddSource("Attribute.g.cs", TemplateReader.AttributeCS));

    internal static void WriteFieldItems(SourceProductionContext context, (ImmutableArray<(string nmspc, string clsnm)> ident, ImmutableArray<FieldItem> fieldItems) param)
    {
        if (param.ident.Any())
        {
            var (nmspc, clsnm) = param.ident.First();
            var fieldsLine = MakeFieldsLines(param.fieldItems);
            var storeCsCode = string.Format(TemplateReader.FieldStoreCS, nmspc, clsnm, string.Join("\r\n", fieldsLine));
            context.AddSource($"Properties.g.cs", storeCsCode);
        }
    }
    internal static void WriteTypedEnum(SourceProductionContext context, (ImmutableArray<(string nmspc, string clsnm)> ident, ImmutableArray<ResolverItem> resolveItems) param)
    {
        if (param.ident.Any())
        {
            var (nmspc, clsnm) = param.ident.First();
            var list = param.resolveItems.Where(i => i.Key is not null).Select(i => i.Key!).Distinct();
            var enumCS = string.Format(TemplateReader.ServiceTypeEnumCS, nmspc, clsnm, string.Join(",\r\n", list));
            context.AddSource($"TypedEnum.g.cs", enumCS);
        }
    }

    internal static void WriteInnerResolve(SourceProductionContext context, (ImmutableArray<(string nmspc, string clsnm)> ident, ImmutableArray<ResolverItem> resolveItems) param)
    {
        if (param.ident.Any())
        {
            var (nmspc, clsnm) = param.ident.First();
            var lookup = param.resolveItems.ToLookup(i => i.IdentName, i => i);
            var resoleveInterfaces = MakeResolveInterfaces(lookup);
            var resoleveFuncs = MakeResolveFunc(lookup);
            var resolveCsCode = string.Format(TemplateReader.InnerResolverCS, nmspc, clsnm, string.Join(",", resoleveInterfaces), string.Join("\r\n", resoleveFuncs));
            context.AddSource($"InnerResolver.g.cs", resolveCsCode);
        }
    }
    internal static void WriteDelegates(SourceProductionContext context, (ImmutableArray<(string nmspc, string clsnm)> ident, ImmutableArray<DelegateItem> delegateItems) param)
    {
        if (param.ident.Any())
        {
            var (nmspc, clsnm) = param.ident.First();
            var delegateLines = MakeDelegatesLines(param.delegateItems);
            var extensionLines = MakeExtensionLines(param.delegateItems);
            var delegatesCsCode = string.Format(TemplateReader.DelegatesCS, nmspc, clsnm, string.Join("\r\n", delegateLines), string.Join("\r\n", extensionLines));
            context.AddSource($"Delegates.g.cs", delegatesCsCode);
        }
    }

    internal static void WriteResolveFunc(SourceProductionContext context, (ImmutableArray<(string nmspc, string clsnm)> ident, ImmutableArray<int> count) param)
    {
        if (param.ident.Any())
        {
            var (nmspc, clsnm) = param.ident.First();
            var extendResolveLine = MakeExtendResolveFunc(param.count);
            var resolveCsCode = string.Format(TemplateReader.ResolveMethodCS, nmspc, clsnm, string.Join("\r\n", extendResolveLine));
            context.AddSource($"Resolve.g.cs", resolveCsCode);
        }
    }

    private static IEnumerable<string> MakeFieldsLines(IEnumerable<FieldItem> items)
    {
        foreach (var item in items)
        {
            var getterFrom = item.LifeTime switch
            {
                LifeTime.Singleton => $"_{item.FieldName} ??= ",
                LifeTime.Scoped => $"field ??= ",
                LifeTime.Transient => "",
                _ => throw new System.InvalidOperationException($"Unknown lifetime: {item.LifeTime}")
            };
            if (LifeTime.Singleton == item.LifeTime)
            {
                yield return $"    private static {item.TypeName}? _{item.FieldName};";
                yield return $"    private static Lock _lock_{item.FieldName} = new();";
                yield return $"    internal {item.TypeName} {item.FieldName} {{get {{ lock(_lock_{item.FieldName}) {{ return {getterFrom} {item.InitializeString}; }} }} }}";
            }
            else
            {
                yield return $"    internal {item.TypeName} {item.FieldName} {{get => {getterFrom} {item.InitializeString}; }}";
            }
        }
    }

    private static IEnumerable<string> MakeResolveInterfaces(ILookup<string, ResolverItem> lookup) => lookup.Select(item => $"IResolver<{item.Key}>");

    private static IEnumerable<string> MakeResolveFunc(ILookup<string, ResolverItem> lookup)
    {
        foreach (var item in lookup)
        {
            var itemarray = item.ToArray();
            yield return $"        {item.Key} IResolver<{item.Key}>.Resolve(FieldStore localStore, ServiceKey key) => key switch {{";
            foreach (var i in itemarray)
            {
                var fld = $"ServiceKey.{(i.Key is null ? "None" : i.Key)}";
                yield return $"            {fld} => ({item.Key})localStore.{i.FieldName},";
            }
            yield return "            _ => throw new UnreachableException()";
            yield return "        };";
            yield return "";

            yield return $"        IEnumerable<{item.Key}> IResolver<{item.Key}>.ResolveAll(FieldStore localStore) => ";
            yield return "        [";
            foreach (var i in itemarray)
            {
                yield return $"            ({item.Key})(localStore.{i.FieldName}),";
            }
            yield return "        ];";
            yield return "";
        }
    }
    private static IEnumerable<string> MakeExtendResolveFunc(ImmutableArray<int> counts)
    {
        foreach (var i in counts.Where(i => i > 1))
        {
            var typeArgs = string.Join(",", Enumerable.Range(1, i).Select(ar => $"T{ar}"));
            var retArgs = string.Join(",", Enumerable.Range(1, i).Select(ar => $"res{ar}"));
            var typeEnumArgs = string.Join(",", Enumerable.Range(1, i).Select(ar => $"IEnumerable<T{ar}>"));
            var keyArgs = string.Join(",", Enumerable.Range(1, i).Select(ar => $"ServiceKey key{ar} = ServiceKey.None"));

            yield return "[System.CodeDom.Compiler.GeneratedCode(\"tsr-d\", null)]";
            yield return $"public static ({typeArgs}) Resolve<{typeArgs}>({keyArgs}) {{";
            yield return "    var localStore = new FieldStore();";
            for (var num = 1; num <= i; num++)
            {
                yield return $"    var res{num} = ((IResolver<T{num}>)inner).Resolve(localStore, key{num});";
            }
            yield return $"    return ({retArgs});";
            yield return "}";

            yield return "[System.CodeDom.Compiler.GeneratedCode(\"tsr-d\", null)]";
            yield return $"public static ({typeEnumArgs}) ResolveAll<{typeArgs}>() {{";
            yield return "    var localStore = new FieldStore();";
            for (var num = 1; num <= i; num++)
            {
                yield return $"    var res{num} = ((IResolver<T{num}>)inner).ResolveAll(localStore);";
            }
            yield return $"    return ({retArgs});";
            yield return "}";
        }
    }


    private static IEnumerable<string> MakeDelegatesLines(ImmutableArray<DelegateItem> items)
    {
        foreach (var item in items.Where(i => i.Create))
        {
            var actualList = item.ArgList.Select((tp, num) => $"{tp} p{num}");
            yield return $"public delegate {item.ReturnType} {item.Name} ({string.Join(",", actualList)});";
        }
    }
    private static IEnumerable<string> MakeExtensionLines(ImmutableArray<DelegateItem> items)
    {
        foreach (var item in items.Where(i => i.ArgList.Length > 0))
        {
            var isAction = item.ReturnType == "void";
            var baseTypeName = isAction ? "Action" : "Func";

            var fullArgList = item.ArgList.Select((type, index) => new { Type = type, Name = $"p{index}" }).ToArray();

            for (var i = 1; i <= fullArgList.Length; i++)
            {
                var boundArgs = fullArgList.Take(i).ToArray();
                var remainingArgs = fullArgList.Skip(i).ToArray();

                var genericTypes = remainingArgs.Select(x => x.Type).ToList();
                if (!isAction)
                {
                    genericTypes.Add(item.ReturnType);
                }

                var typeSignature = genericTypes.Count > 0 ? $"{baseTypeName}<{string.Join(", ", genericTypes)}>" : baseTypeName;

                var bindParams = string.Join(", ", boundArgs.Select(x => $"{x.Type} {x.Name}"));
                var lambdaParams = string.Join(", ", remainingArgs.Select(x => x.Name));
                var callParams = string.Join(", ", fullArgList.Select(x => x.Name));

                yield return $"public static {typeSignature} Bind(this {item.Name} func, {bindParams}) => ({lambdaParams}) => func({callParams});";
            }
        }
    }
}

