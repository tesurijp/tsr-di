using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace tsr_di;
using static tsr_di.MapperUtil;

internal class ResolverFunctionMapper
{
    internal static IncrementalValueProvider<SvcProviderMappingResult> ToSvcResolverName(CollectedTypeSymbols svcResolverClass) =>
        svcResolverClass.Select((x, _) => ToSvcResolverName_Internal(x).ToImmutableArray());
    internal static IncrementalValueProvider<ResolverMappingResult> ToResolveItems(IncrementalValueProvider<ImmutableArray<ActualResolverFuncInfo>> needResolveItems, CollectedTypeSymbols ItemList, CollectedMethodSymbols MethodList, IncrementalValueProvider<SymbolSet> symbols) =>
        needResolveItems.Combine(ItemList).Combine(MethodList).Combine(symbols).Select((x, _) => ToResolveItems_Internal(x.Left.Left.Left, x.Left.Left.Right, x.Left.Right, x.Right.SvcResolverAttr, x.Right.SvcClassAttr, x.Right.SvcFuncAttr).ToImmutableArray());
    private static IEnumerable<ResultOrError<(string nmspc, string clsnm)>> ToSvcResolverName_Internal(TypeSymbols candidate)
    {
        var items = candidate.ToArray();
        switch (items.Length)
        {
            case 0:
                yield return new ErrorItem(DiagnosticDescriptors.NothingServiceResolver, "", Location.None);
                break;
            case 1:
                break;
            default:
                foreach (var i in items)
                {
                    yield return new ErrorItem(DiagnosticDescriptors.MultipleServiceResolver, i);
                }
                break;
        }
        foreach (var tp in items)
        {
            if (IsPartial(tp))
            {
                yield return (tp.ContainingNamespace.ToString(), tp.Name.ToString());
            }
            else
            {
                yield return new ErrorItem(DiagnosticDescriptors.MustBePartial, tp);
            }
        }
    }

    private static IEnumerable<ResultOrError<(INamedTypeSymbol tp, string arg, Location funcLoc)>> UsedArgByType(IEnumerable<ActualResolverFuncInfo> calls)
    {
        foreach (var (_, typeList, argList, _, funcLoc) in calls)
        {
            if (typeList.Length < argList.Length)
            {
                yield return new ErrorItem(DiagnosticDescriptors.ArgCountMissmatch, "", funcLoc);
            }
            else
            {
                for (var i = 0; i < argList.Length && i < typeList.Length; i++)
                {
                    var tp = typeList[i];
                    var arg = argList[i];
                    if (tp is not null)
                    {
                        yield return (tp, arg, funcLoc);
                    }
                }
            }
        }
    }

    private static IEnumerable<ResultOrError<ResolverItem>> ToResolveItems_Internal(ImmutableArray<ActualResolverFuncInfo> targets, TypeSymbols items, MethodSymbols mitems, INamedTypeSymbol svcResolverAttr, INamedTypeSymbol svcClsAttr, INamedTypeSymbol svcFuncAttr)
    {
        var lookup = CreateTypeLookup(items, svcClsAttr);
        var lookupMethod = CreateFuncLookup(mitems, svcFuncAttr);

        var flatlist = targets.SelectMany(i => i.typeList, (i, cls) => (i.pcls, cls, i.typeArgsLoc));

        foreach (var (pcls, cls, loc) in flatlist)
        {
            if (pcls is null || cls is null || !Collector.HasAttribute(pcls, svcResolverAttr))
            {
                continue;
            }
            if (cls is INamedTypeSymbol resTarget)
            {
                var resolvitems = ToResolveItem(resTarget, lookup, lookupMethod).ToArray();
                if (resolvitems.Length == 0)
                {
                    yield return new ErrorItem(DiagnosticDescriptors.NotRegisterd, resTarget.ToString(), loc);
                    continue;
                }

                foreach (var group in resolvitems.ToLookup(i => i.Key))
                {
                    if (group.Count() > 1)
                    {
                        var (descriptor, typename) = group.Key is null ? (DiagnosticDescriptors.ConflictType, resTarget.ToString()) : (DiagnosticDescriptors.ConflictKey, $"{resTarget}[{group.Key}]");
                        var names = group.Select(i => i.FieldName).ToArray();

                        foreach (var errItem in items.Where(i => names.Contains(ToTidyName(i))))
                        {
                            yield return new ErrorItem(descriptor, typename, errItem.Locations.FirstOrDefault(i => i.IsInSource) ?? Location.None);
                        }
                    }
                }
            }
        }

        var argsByTypeAndError = UsedArgByType(targets).ToArray();
        foreach (var err in argsByTypeAndError.Where(i => i.HasError).Select(i => i.Error))
        {
            yield return err!;
        }

        var usedKey = argsByTypeAndError.Where(i => !i.HasError).Select(i => i.Result).ToLookup(i => i.tp!, i => (i.arg, i.funcLoc), SymbolEqualityComparer.Default);

        foreach (var i in usedKey)
        {
            var registerdkeys = lookup[i.Key].Select(t => t.tag.name ?? "None").Concat(lookupMethod[i.Key!.ToString()].Select(i => i.tagname ?? "None"));
            foreach (var (arg, funcLoc) in i.ToArray())
            {
                if (!registerdkeys.Contains(arg))
                {
                    yield return new ErrorItem(DiagnosticDescriptors.NotRegisterdKeyOrMultipleKey, $"{i.Key} [{arg}]", funcLoc);
                }
            }
        }

        foreach (var resTarget in flatlist.Select(c => c.cls).Where(c => c is not null).Distinct(SymbolEqualityComparer.Default))
        {
            var keys = usedKey[resTarget].Select(i => i.arg);
            foreach (var item in ToResolveItem((resTarget as INamedTypeSymbol)!, lookup, lookupMethod))
            {
                item.NeedKeyResolve = keys.Contains(item.Key ?? "None");
                yield return item;
            }
        }
    }

    private static bool IsPartial(INamedTypeSymbol tp) =>
            !tp.DeclaringSyntaxReferences.IsEmpty &&
            tp.DeclaringSyntaxReferences.Select(syntaxRef => syntaxRef.GetSyntax()).OfType<TypeDeclarationSyntax>().Any(typeDecl => typeDecl.Modifiers.Any(SyntaxKind.PartialKeyword));
}

internal class ResolverPropertyMapper
{
    internal static IncrementalValueProvider<ImmutableArray<ResolverItem>> ToResolveItems(CollectedTypeSymbols ItemList, CollectedMethodSymbols MethodList, IncrementalValueProvider<SymbolSet> symbols) =>
        ItemList.Combine(MethodList).Combine(symbols).Select((x, _) => ToResolveItems_Internal(x.Left.Left, x.Left.Right, x.Right.SvcClassAttr, x.Right.SvcFuncAttr).ToImmutableArray());

    private static IEnumerable<ResolverItem> ToResolveItems_Internal(TypeSymbols items, MethodSymbols mitems, INamedTypeSymbol svcClsAttr, INamedTypeSymbol svcFuncAttr)
    {
        var lookup = CreateTypeLookup(items, svcClsAttr);
        var lookupMethod = CreateFuncLookup(mitems, svcFuncAttr);
        var typeList = lookup.SelectMany(i => ToResolveItemClasses((INamedTypeSymbol)i.Key!, lookup));
        var mtdList = lookupMethod.SelectMany(i => ToResolveItemFunctions(i.Key, lookupMethod));
        return typeList.Concat(mtdList);
    }
}
