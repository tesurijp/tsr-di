using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace tsr_di;
using static tsr_di.MapperUtil;
internal static class FieldStoreMapper 
{
    internal static IncrementalValueProvider<FieldMappingResult> ToFieldItems(CollectedTypeSymbols ItemList, CollectedMethodSymbols FuncItemList, IncrementalValueProvider<SymbolSet> symbols) =>
        ItemList.Combine(FuncItemList).Combine(symbols).Select((x, _) => ToFieldItems_Internal(x.Left.Left, x.Left.Right, x.Right).ToImmutableArray());

    internal static IncrementalValueProvider<FieldMappingResult> ToFunctionField(CollectedMethodSymbols ItemList, IncrementalValueProvider<SymbolSet> symbols) =>
        ItemList.Combine(symbols).Select((x, _) => ToFunctionField_Internal(x.Left, x.Right).ToImmutableArray());

    private static IEnumerable<ResultOrError<FieldItem>> ToFieldItems_Internal(TypeSymbols items, MethodSymbols mitems, SymbolSet sset)
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
                    yield return new FieldItem(intf.ToString(), $"{ToTidyName(intf)}_{ToTidyName(item)}", lifetime, $"new {item}({string.Join(", ", args)})");
                }
            }
            else
            {
                yield return new FieldItem(item.ToString() , ToTidyName(item), lifetime, $"new {item}({string.Join(", ", args)})");
            }
        }
    }

    private static IEnumerable<ResultOrError<FieldItem>> ToFunctionField_Internal(MethodSymbols items, SymbolSet sset)
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
                yield return new FieldItem(delName, ToTidyName(item), LifeTime.Transient, $"{parentCls}.{item.Name}");
            }
            else if (Collector.HasAttribute(parentCls, sset.SvcClassAttr))
            {
                if (GetServiceClassAttribute(sset.SvcClassAttr, parentCls) is { shared: SharingMode.Shared })
                {
                    yield return new FieldItem(delName, ToTidyName(item), LifeTime.Transient, $"{ToTidyName(parentCls)}.{item.Name}");
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
                        yield return $"[{string.Join(",", items.Select(i => $"{i.FieldName}"))}]";
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
                            yield return $"new (() => {matches[0].FieldName})";
                        }
                        else
                        {
                            yield return $"{matches[0].FieldName}";
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
    private static Location GetParameterFullLocation(IParameterSymbol psymbol) =>
            (psymbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as ParameterSyntax)?.GetLocation() ?? psymbol.Locations.FirstOrDefault() ?? Location.None;
    private static Location GetAttributeFullLocation(ISymbol sym, INamedTypeSymbol attrType) =>
        sym.GetAttributes().Single(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attrType))
            .ApplicationSyntaxReference?.GetSyntax().GetLocation() ?? Location.None;
    private static bool IsInvalideName(string? value) => value is not null && ! SyntaxFacts.IsValidIdentifier(value);

}
