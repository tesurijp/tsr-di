using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace tsr_di;

internal static class Emitter
{
    internal static void OutputErrors(SourceProductionContext context, ImmutableArray<ErrorItem> errors)
    {
        foreach (var err in errors)
        {
            var diagnostic = Diagnostic.Create(err.Error, err.PrimaryLocation, err.TypeName);
            context.ReportDiagnostic(diagnostic);
        }
    }
    internal static void WriteAttribute(IncrementalGeneratorInitializationContext context) =>
        context.RegisterPostInitializationOutput((ctx) => ctx.AddSource("Attribute.g.cs", TemplateReader.AttributeCS));

    internal static void WriteSource(SourceProductionContext context, ((((ImmutableArray<(string nmspc, string clsnm)> , ImmutableArray<FieldItem>), ImmutableArray<ResolverItem>) , ImmutableArray<DelegateItem>), ImmutableArray<int>) param)
    {
        var ident = param.Item1.Item1.Item1.Item1;
        var fieldItems = param.Item1.Item1.Item1.Item2;
        var resolveItems = param.Item1.Item1.Item2;
        var delegateItems = param.Item1.Item2;
        var count = param.Item2;

        if (ident.Any())
        {
            var (nmspc, clsnm) = ident.First();
            var fieldsLine = MakeFieldsLines(fieldItems);
            var storeCsCode = string.Format(TemplateReader.StoreFieldsCS, nmspc, clsnm, string.Join("\r\n", fieldsLine));

            var lookup = resolveItems.ToLookup(i => i.IdentName, i => i);
            var resoleveLine = MakeResolveLines(lookup);
            var extendResolveLine = MakeExtendResolveFunc(count);
            var resolveCsCode = string.Format(TemplateReader.ResolveMethodCS, nmspc, clsnm, string.Join("\r\n", resoleveLine), string.Join("\r\n", extendResolveLine));

            var list = resolveItems.Where(i => i.Key is not null).Select(i => i.Key!).Distinct();
            var enumCS = string.Format(TemplateReader.ServiceTypeEnumCS, nmspc, clsnm, string.Join(",\r\n", list));

            var delegateLines = MakeDelegatesLines(delegateItems);
            var extensionLines = MakeExtensionLines(delegateItems);
            var delegatesCsCode = string.Format(TemplateReader.DelegatesCS, nmspc, clsnm, string.Join("\r\n", delegateLines), string.Join("\r\n", extensionLines));

            context.AddSource($"Properties.g.cs", storeCsCode);
            context.AddSource($"Resolve.g.cs", resolveCsCode);
            context.AddSource($"TypedEnum.g.cs", enumCS);
            context.AddSource($"Delegates.g.cs", delegatesCsCode);
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
                yield return $"    private static object? _{item.FieldName};";
                yield return $"    private static Lock _lock_{item.FieldName} = new();";
                yield return $"    internal object {item.FieldName} {{get {{ lock(_lock_{item.FieldName}) {{ return {getterFrom} {item.InitializeString}; }} }} }}";
            }
            else
            {
                yield return $"    internal object {item.FieldName} {{get => {getterFrom} {item.InitializeString}; }}";
            }
        }
    }
    private static IEnumerable<string> MakeResolveLines(ILookup<string, ResolverItem> lookup)
    {
        foreach (var item in lookup)
        {
            var itemarray = item.ToArray();
            yield return $"    else if ( tp == typeof({item.Key})) {{";
            yield return "        Resolve = (localStore, key) => key switch {";
            foreach (var i in itemarray)
            {
                var fld = $"ServiceKey.{(i.Key is null ? "None" : i.Key)}";
                yield return $"            {fld} => (T)localStore.{i.FieldName},";
            }
            yield return "            _ => throw new UnreachableException()";
            yield return "        };";

            yield return "        ResolveAll = (localStore) => [ ";
            foreach (var i in itemarray)
            {
                yield return $"            (T)(localStore.{i.FieldName}),";
            }
            yield return "        ];";
            yield return "    }";
        }
    }
    private static IEnumerable<string> MakeExtendResolveFunc(ImmutableArray<int> counts)
    {
        foreach(var i in counts)
        {
            if(i > 1)
            {
                var typeArgs = string.Join(",", Enumerable.Range(1, i).Select(ar => $"T{ar}"));
                var retArgs = string.Join(",", Enumerable.Range(1, i).Select(ar => $"res{ar}"));
                var typeEnumArgs = string.Join(",", Enumerable.Range(1, i).Select(ar => $"IEnumerable<T{ar}>"));
                var keyArgs = string.Join(",", Enumerable.Range(1, i).Select(ar => $"ServiceKey key{ar} = ServiceKey.None"));

                yield return "[MethodImpl(MethodImplOptions.AggressiveInlining)] [System.CodeDom.Compiler.GeneratedCode(\"tsr-d\", null)]";
                yield return $"public static ({typeArgs}) Resolve<{ typeArgs}>({keyArgs}) {{";
                yield return "    var localStore = new FieldStore();";
                for(var num =1; num <= i; num++)
                {
                    yield return $"    var res{num} = InnerResolver<T{num}>.Resolve(localStore, key{num});";
                }
                yield return $"    return ({retArgs});";
                yield return "}";

                yield return "[MethodImpl(MethodImplOptions.AggressiveInlining)] [System.CodeDom.Compiler.GeneratedCode(\"tsr-d\", null)]";
                yield return $"public static ({typeEnumArgs}) ResolveAll<{ typeArgs}>() {{";
                yield return "    var localStore = new FieldStore();";
                for(var num =1; num <= i; num++)
                {
                    yield return $"    var res{num} = InnerResolver<T{num}>.ResolveAll(localStore);";
                }
                yield return $"    return ({retArgs});";
                yield return "}";
            }
        }
    }


    private static IEnumerable<string> MakeDelegatesLines(ImmutableArray<DelegateItem> items)
    {
        foreach (var item in items)
        {
            if (item.Create)
            {
                var actualList = item.ArgList.Select((tp, num) => $"{tp} p{num}");
                yield return $"public delegate {item.ReturnType} {item.Name} ({string.Join(",", actualList)});";
            }
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

