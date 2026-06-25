using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Linq;

namespace tsr_di;

internal partial class Mapper
{

    internal static IncrementalValueProvider<FieldMappingResult> ToFieldItems(CollectedTypeSymbols ItemList, CollectedMethodSymbols FuncItemList, IncrementalValueProvider<SymbolSet> symbols) =>
        ItemList.Combine(FuncItemList).Combine(symbols).Select((x, _) => ToFieldItems_Internal(x.Left.Left, x.Left.Right, x.Right).ToImmutableArray());

    internal static IncrementalValueProvider<FieldMappingResult> ToFunctionField(CollectedMethodSymbols ItemList, IncrementalValueProvider<SymbolSet> symbols) =>
        ItemList.Combine(symbols).Select((x, _) => ToFunctionField_Internal(x.Left, x.Right).ToImmutableArray());

    internal static IncrementalValueProvider<ResolverMappingResult> ToResolveItems(IncrementalValueProvider<ImmutableArray<(INamedTypeSymbol?, ImmutableArray<INamedTypeSymbol?>, Location)>> needResolveItems, CollectedTypeSymbols ItemList, CollectedMethodSymbols MethodList, IncrementalValueProvider<SymbolSet> symbols) =>
        needResolveItems.Combine(ItemList).Combine(MethodList).Combine(symbols).Select((x, _) => ToResolveItems_Internal(x.Left.Left.Left, x.Left.Left.Right, x.Left.Right, x.Right.SvcResolverAttr, x.Right.SvcClassAttr, x.Right.SvcFuncAttr).ToImmutableArray());

    internal static IncrementalValueProvider<ImmutableArray<int>> ToResolveMethodArgs(IncrementalValueProvider<ImmutableArray<(INamedTypeSymbol?, ImmutableArray<INamedTypeSymbol?> items, Location)>> needResolveItems) =>
        needResolveItems.Select((x, _) => x.Select(i => i.items.Length).Distinct().ToImmutableArray());

    internal static IncrementalValueProvider<SvcProviderMappingResult> ToSvcResolverName(CollectedTypeSymbols svcResolverClass) =>
        svcResolverClass.Select((x, _) => ToSvcResolverName_Internal(x).ToImmutableArray());

    internal static IncrementalValueProvider<DelegateMappingResult> ToDelegateItem(CollectedMethodSymbols ItemList, IncrementalValueProvider<SymbolSet> symbols) =>
        ItemList.Combine(symbols).Select((x, _) => ToDelegateItem_Internal(x.Left, x.Right).ToImmutableArray());
}

