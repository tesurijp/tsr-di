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
        Dictionary<string, DelegateItem> dupCheck = [];
        foreach (var item in items)
        {
            var (type, name, _) = GetServiceFunctionAttribute(sset.SvcFuncAttr, item);
            var (retType, args) = GetSignature(type?.DelegateInvokeMethod ?? item);
            var delegatename = type?.Name.ToString() ?? $"I{name ?? item.Name}";
            var delegateItem = new DelegateItem(retType, delegatename, [.. args], type is null);
            if (dupCheck.TryGetValue(delegatename, out var preRegist))
            {
                if (!preRegist.AreSameSigunature(delegateItem))
                {
                    yield return new ErrorItem(DiagnosticDescriptors.ConflictType, item);
                }
            }
            else
            {
                dupCheck[delegatename] = delegateItem;
                yield return delegateItem;
            }
        }
    }
}
