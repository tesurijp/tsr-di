using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace tsr_di;

using FuncLookup = ILookup<string, (IMethodSymbol tp, string? tagname)>;
using TypeLookup = ILookup<ISymbol?, (INamedTypeSymbol tp, (string? name, SharingMode mode) tag)>;

internal partial class Mapper
{
    private static IEnumerable<ResultOrError<string>> ConstructorArguments(INamedTypeSymbol item, TypeLookup lookup, FuncLookup funcLookup, SymbolSet sset)
    {
        var constructors = item.InstanceConstructors.Where(i => i.DeclaredAccessibility == Accessibility.Public).ToArray();
        if(constructors.Length != 1)
        {
            yield return new ErrorItem(DiagnosticDescriptors.SinglePublicConstructorRequired, item);

        }
        var constructor = constructors.FirstOrDefault();
        if (constructor != null)
        {

            foreach (var param in constructor.Parameters)
            {
                var (type, isCollection, isLazy) = param.Type switch
                {
                    IArrayTypeSymbol array => (array.ElementType as INamedTypeSymbol, true, false),
                    INamedTypeSymbol named when SymbolEqualityComparer.Default.Equals(named.OriginalDefinition, sset.IEnumerable) => (named.TypeArguments.FirstOrDefault() as INamedTypeSymbol, true, false),
                    INamedTypeSymbol named when SymbolEqualityComparer.Default.Equals(named.OriginalDefinition, sset.List) => (named.TypeArguments.FirstOrDefault() as INamedTypeSymbol, true, false),
                    INamedTypeSymbol named when SymbolEqualityComparer.Default.Equals(named.OriginalDefinition, sset.Lazy) => (named.TypeArguments.FirstOrDefault() as INamedTypeSymbol, false, true),
                    _ => (param.Type as INamedTypeSymbol, false, false)
                };
                if (type is null)
                {
                    yield return "default";
                    continue;
                }

                var items = ToResolveItem(type, lookup, funcLookup).ToArray();

                if (isCollection)
                {
                    if (items.Length > 0)
                    {
                        yield return $"[{string.Join(",", items.Select(i => $"({type}){i.FieldName}"))}]";
                    }
                    else
                    {
                        yield return "default";
                        yield return new ErrorItem(DiagnosticDescriptors.NotRegisterd, param.ToString(), GetParameterFullLocation(param));
                    }
                }
                else
                {
                    var nameKey = param.GetAttributes().FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, sset.FromNameAttr))?.ConstructorArguments.FirstOrDefault().Value?.ToString();
                    var matches = items.Where(i => i.Key == nameKey).ToArray();

                    if (matches.Length == 1)
                    {
                        if (isLazy)
                        {
                            yield return $"new (() => ({type}){matches[0].FieldName})";
                        }
                        else
                        {
                            yield return $"({type}){matches[0].FieldName}";
                        }
                    }
                    else
                    {
                        yield return "default";
                        var (descriptor, typename) = items.Length > 0 ? (DiagnosticDescriptors.NotRegisterdKeyOrMultipleKey, $"{param.Name} [{nameKey}]") : (DiagnosticDescriptors.NotRegisterd, param.Name);
                        yield return new ErrorItem(descriptor, typename, GetParameterFullLocation(param));
                    }
                }
            }
        }
    }

    internal static IEnumerable<ResultOrError<(string nmspc, string clsnm)>> ToSvcResolverName_Internal(TypeSymbols candidate)
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


    internal static IEnumerable<ResultOrError<DelegateItem>> ToDelegateItem_Internal(MethodSymbols items, SymbolSet sset)
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

    internal static IEnumerable<ResultOrError<FieldItem>> ToFunctionField_Internal(MethodSymbols items, SymbolSet sset)
    {
        foreach (var item in items)
        {
            var parentCls = item.ContainingType;
            var (type, name, key) = GetServiceFunctionAttribute(sset.SvcFuncAttr, item);
            var delName = DelegateName(type, name, item);

            if(IsInvalideName(name))
            {
                yield return new ErrorItem(DiagnosticDescriptors.InvalidServiceFunctionName, item.ToString(), GetAttributeFullLocation(item, sset.SvcFuncAttr));
            }
            if(IsInvalideName(key))
            {
                yield return new ErrorItem(DiagnosticDescriptors.InvalidServiceFunctionKey, item.ToString(), GetAttributeFullLocation(item, sset.SvcFuncAttr));
            }

            if (item.DeclaredAccessibility != Accessibility.Public)
            {
                yield return new ErrorItem(DiagnosticDescriptors.ServiceFunctionMustBePublic, item);
            }
            else if (item.IsStatic)
            {
                yield return new FieldItem(ToTidyName(item), LifeTime.Transient, $"({delName}){parentCls}.{item.Name}");
            }
            else if (Collector.HasAttribute(parentCls, sset.SvcClassAttr))
            {
                if (GetServiceClassAttribute(sset.SvcClassAttr, parentCls) is { shared: SharingMode.Shared })
                {
                    yield return new FieldItem(ToTidyName(item), LifeTime.Transient, $"({delName})(({parentCls}){ToTidyName(parentCls)}).{item.Name}");
                }
                else
                {
                    yield return new ErrorItem(DiagnosticDescriptors.ParentSharingModeIsIsolate, parentCls.ToString(), GetAttributeFullLocation(parentCls, sset.SvcClassAttr));
                }
            }
            else
            {
                yield return new ErrorItem(DiagnosticDescriptors.MethodIsNotIdentify, item);
            }
        }
    }
    internal static IEnumerable<ResultOrError<FieldItem>> ToFieldItems_Internal(TypeSymbols items, MethodSymbols mitems, SymbolSet sset)
    {
        var lookup = CreateTypeLookup(items, sset.SvcClassAttr);
        var lookupMethod = CreateFuncLookup(mitems, sset.SvcFuncAttr);

        foreach (var item in items)
        {
            if( item.DeclaredAccessibility != Accessibility.Public)
            {
                yield return new ErrorItem(DiagnosticDescriptors.ServiceClassMustBePublic, item);
            }
            if(! item.AllInterfaces.Where(i => i.DeclaredAccessibility == Accessibility.Public).Any())
            {
                yield return new ErrorItem(DiagnosticDescriptors.NeedPublicInterface, item);
            }
            var (name, lifetime, shared) = GetServiceClassAttribute(sset.SvcClassAttr, item);
            if(IsInvalideName(name))
            {
                yield return new ErrorItem(DiagnosticDescriptors.InvalidServiceClassKey, item.ToString(), GetAttributeFullLocation(item,sset.SvcClassAttr));
            }
            var errorOrArgs = ConstructorArguments(item, lookup, lookupMethod, sset);
            foreach (var err in errorOrArgs.Where(i => i.HasError))
            {
                yield return err.Error!;
            }
            ImmutableArray<string> args = [.. errorOrArgs.Where(i => !i.HasError).Select(i => i.Result!)];
            if (shared == SharingMode.IsolatePerService)
            {
                foreach (var intf in item.AllInterfaces)
                {
                    yield return new FieldItem($"{ToTidyName(intf)}_{ToTidyName(item)}", lifetime, $"new {item}({string.Join(", ", args)})");
                }
            }
            else
            {
                yield return new FieldItem(ToTidyName(item), lifetime, $"new {item}({string.Join(", ", args)})");
            }
        }
    }

    internal static IEnumerable<ResultOrError<ResolverItem>> ToResolveItems_Internal(ImmutableArray<(INamedTypeSymbol? pcls,INamedTypeSymbol? cls, Location loc)> targets, TypeSymbols items, MethodSymbols mitems, INamedTypeSymbol svcResolverAttr, INamedTypeSymbol svcClsAttr, INamedTypeSymbol svcFuncAttr)
    {
        var lookup = CreateTypeLookup(items, svcClsAttr);
        var lookupMethod = CreateFuncLookup(mitems, svcFuncAttr);

        foreach (var (pcls, cls, loc) in targets)
        {
            if(pcls is null || cls is null || !Collector.HasAttribute(pcls, svcResolverAttr))
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
                            yield return new ErrorItem(descriptor, typename, errItem.Locations.FirstOrDefault(i=>i.IsInSource) ?? Location.None);
                        }
                    }
                }
            }
        }

        foreach (var resTarget in targets.Select(c => c.cls).Where(c=>c is not null).Distinct(SymbolEqualityComparer.Default))
        {
            foreach (var item in ToResolveItem((resTarget as INamedTypeSymbol)!, lookup, lookupMethod))
            {
                yield return item;
            }
        }
    }
    private static IEnumerable<ResolverItem> ToResolveItem(INamedTypeSymbol target, TypeLookup lookup, FuncLookup funcLookup)
    {
        var candidate = lookup[target];

        if (candidate != null && candidate.Count() > 0)
        {
            foreach (var (tp, tag) in candidate)
            {
                if (tag.mode == SharingMode.IsolatePerService)
                {
                    yield return new(target.ToString(), tag.name, $"{ToTidyName(target)}_{ToTidyName(tp)}");
                }
                else
                {
                    yield return new(target.ToString(), tag.name, ToTidyName(tp));
                }
            }
        }

        var candidateFunc = funcLookup[target.ToString()];
        if (candidateFunc != null && candidateFunc.Count() > 0)
        {
            foreach (var (tp, tag) in candidateFunc)
            {
                yield return new(target.ToString(), tag, ToTidyName(tp));
            }
        }
    }
}

