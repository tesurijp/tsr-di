using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
namespace tsr_di;

internal record class SymbolSet(INamedTypeSymbol SvcResolverAttr, INamedTypeSymbol SvcClassAttr,INamedTypeSymbol SvcFuncAttr, INamedTypeSymbol FromNameAttr, INamedTypeSymbol IEnumerable, INamedTypeSymbol Lazy, INamedTypeSymbol List);

internal static class Collector
{
    private const string ServiceResolverAttributeName = "tsr_di.ServiceResolverAttribute";
    private const string ServiceClassAttributeName = "tsr_di.ServiceClassAttribute";
    private const string ServiceFunctionAttributeName = "tsr_di.ServiceFunctionAttribute";
    private const string NamedAttribute = "tsr_di.FromNamedAttribute";
    private const string SystemLazy = "System.Lazy`1";
    private const string GenericList = "System.Collections.Generic.List`1";

    internal static IncrementalValueProvider<SymbolSet> ConstSymbols(IncrementalGeneratorInitializationContext context) => context.CompilationProvider.Select((c, _) => new SymbolSet(
        c.GetTypeByMetadataName(ServiceResolverAttributeName)!,
        c.GetTypeByMetadataName(ServiceClassAttributeName)!,
        c.GetTypeByMetadataName(ServiceFunctionAttributeName)!,
        c.GetTypeByMetadataName(NamedAttribute)!,
        c.GetSpecialType(SpecialType.System_Collections_Generic_IEnumerable_T),
        c.GetTypeByMetadataName(SystemLazy)!,
        c.GetTypeByMetadataName(GenericList)!
    ));

    internal static IncrementalValuesProvider<(INamedTypeSymbol?, INamedTypeSymbol?, Location)> FindResolveFunc(IncrementalGeneratorInitializationContext context) =>
        context.SyntaxProvider.CreateSyntaxProvider(
            (node, _) => node is InvocationExpressionSyntax caller && PreCheckResolveFunction(caller),
            (context, _) =>
            {
                var mb = ((context.Node as InvocationExpressionSyntax)!.Expression as MemberAccessExpressionSyntax)!;
                if (mb.Name is GenericNameSyntax generic && generic.TypeArgumentList.Arguments.Count == 1)
                {
                    var tp = context.SemanticModel.GetTypeInfo(mb.Expression).Type as INamedTypeSymbol;
                    var argSyntax = generic.TypeArgumentList.Arguments[0];
                    return (tp,  context.SemanticModel.GetTypeInfo(argSyntax).Type as INamedTypeSymbol, mb.GetLocation());
                }
                return (null, null, Location.None);
            });

    internal static IncrementalValuesProvider<INamedTypeSymbol> FindServiceResolver(IncrementalGeneratorInitializationContext context) =>
            context.SyntaxProvider.ForAttributeWithMetadataName(ServiceResolverAttributeName,
                static (node, _) => node is TypeDeclarationSyntax,
                static (context, _) => (INamedTypeSymbol)context.TargetSymbol);

    internal static IncrementalValuesProvider<IMethodSymbol> FindLocalServiceFunctions(IncrementalGeneratorInitializationContext context) =>
            context.SyntaxProvider.ForAttributeWithMetadataName(ServiceFunctionAttributeName,
                static (node, _) => node is MethodDeclarationSyntax,
                static (context, _) => (IMethodSymbol)context.TargetSymbol);
    internal static IncrementalValuesProvider<INamedTypeSymbol> FindLocalServiceClasses(IncrementalGeneratorInitializationContext context) =>
            context.SyntaxProvider.ForAttributeWithMetadataName(ServiceClassAttributeName,
                static (node, _) => node is TypeDeclarationSyntax,
                static (context, _) => (INamedTypeSymbol)context.TargetSymbol);

    internal static IncrementalValuesProvider<INamedTypeSymbol> FindReferServiceClasses(IncrementalGeneratorInitializationContext context) =>
        context.CompilationProvider.SelectMany(static (compilation, _) => EnumerateAll<INamedTypeSymbol>(compilation, t => [t], ServiceClassAttributeName));
    internal static IncrementalValuesProvider<IMethodSymbol> FindReferServiceFunctions(IncrementalGeneratorInitializationContext context) =>
        context.CompilationProvider.SelectMany(static (compilation, _) => EnumerateAll(compilation, t => t.GetMembers().OfType<IMethodSymbol>(), ServiceFunctionAttributeName));


    static readonly string[] sysArray = ["mscorlib", "System", "netstandard", "Accessibility", "WindowsBase"];
    private static IEnumerable<T> EnumerateAll<T>(Compilation c, Func<INamedTypeSymbol, IEnumerable<T>> getFunc, string attribute) where T : ISymbol
    {
        var regAttr = c.GetTypeByMetadataName(attribute)!;

        foreach (var assembly in c.SourceModule.ReferencedAssemblySymbols)
        {
            if (IsSystemAssembly(assembly))
            {
                continue;
            }
            foreach (var tp in EnumerateAll_Recursive(assembly.GlobalNamespace, getFunc).Where(tp => HasAttribute(tp, regAttr)))
            {
                yield return tp;
            }
        }
        static bool IsSystemAssembly(IAssemblySymbol assembly)
        {
            var name = assembly.Name;
            return name.StartsWith("System.") || name.StartsWith("Microsoft.") || name.StartsWith("FSharp.") || sysArray.Contains(name);
        }
    }
    private static IEnumerable<T> EnumerateAll_Recursive<T>(INamespaceOrTypeSymbol nsortype, Func<INamedTypeSymbol, IEnumerable<T>> getFunc)
    {
        foreach (var type in nsortype.GetTypeMembers())
        {
            foreach (var item in getFunc(type))
            {
                yield return item;
            }

            foreach (var nested in EnumerateAll_Recursive(type, getFunc))
            {
                yield return nested;
            }
        }

        if (nsortype is INamespaceSymbol ns)
        {
            foreach (var child in ns.GetNamespaceMembers())
            {
                foreach (var item in EnumerateAll_Recursive(child, getFunc))
                {
                    yield return item;
                }
            }
        }
    }
    internal static bool HasAttribute(ISymbol tp, INamedTypeSymbol regAttr) =>
            tp.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, regAttr));

    private static bool PreCheckResolveFunction(InvocationExpressionSyntax node) =>
        node.Expression is MemberAccessExpressionSyntax { Name.Identifier.Text: "Resolve" } ||
        node.Expression is MemberAccessExpressionSyntax { Name.Identifier.Text: "ResolveAll" };

    private static bool PreCheckBindFunction(InvocationExpressionSyntax node) =>
        node.Expression is MemberAccessExpressionSyntax { Name.Identifier.Text: "Bind" };
}
