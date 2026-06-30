using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace tsr_di;
using static tsr_di.MapperUtil;
internal static class DeclarationMapper
{
    internal static IncrementalValueProvider<DelegateMappingResult> ToDelegateItem(CollectedMethodSymbols ItemList, IncrementalValueProvider<SymbolSet> symbols) =>
        ItemList.Combine(symbols).Select((x, _) => ToDelegateItem_Internal(x.Left, x.Right).ToImmutableArray());
    internal static IncrementalValueProvider<ImmutableArray<int>> ToResolveMethodArgs(IncrementalValueProvider<ImmutableArray<(INamedTypeSymbol?, ImmutableArray<INamedTypeSymbol?> items, Location)>> needResolveItems) =>
        needResolveItems.Select((x, _) => x.Select(i => i.items.Length).Distinct().ToImmutableArray());

    private static IEnumerable<ResultOrError<DelegateItem>> ToDelegateItem_Internal(MethodSymbols items, SymbolSet sset)
    {
        List<string> dupCheck = [];
        foreach (var item in items)
        {
            var (type, name, _) = GetServiceFunctionAttribute(sset.SvcFuncAttr, item);
            if (type is null)
            {
                var delegatename = $"I{name ?? item.Name}";
                if (dupCheck.Contains(delegatename))
                {
                    yield return new ErrorItem(DiagnosticDescriptors.ConflictType, item);
                }
                else
                {
                    dupCheck.Add(delegatename);
                    var (retType, args) = GetSignature(item);
                    yield return new DelegateItem(retType, delegatename, [.. args]);
                }
            }
            else
            {
                var delegatename = type.Name.ToString();
                if (dupCheck.Contains(delegatename))
                {
                    continue;
                }
                dupCheck.Add(delegatename);
                var invoke = type.DelegateInvokeMethod;
                if (invoke is not null)
                {
                    var (retType, args) = GetSignature(invoke);
                    yield return new DelegateItem(retType, delegatename, [.. args], false);
                }
            }
        }
        static (string retType, IEnumerable<string> args) GetSignature(IMethodSymbol method) => (method.ReturnType.ToString(), method.Parameters.Select(t => t.Type.ToString()));
    }
}
