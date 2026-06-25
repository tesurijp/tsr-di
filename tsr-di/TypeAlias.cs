global using FieldMappingResult = System.Collections.Immutable.ImmutableArray<tsr_di.ResultOrError<tsr_di.FieldItem>>;
global using ResolverMappingResult = System.Collections.Immutable.ImmutableArray<tsr_di.ResultOrError<tsr_di.ResolverItem>>;
global using SvcProviderMappingResult = System.Collections.Immutable.ImmutableArray<tsr_di.ResultOrError<(string nmspc, string clsnm)>>;
global using DelegateMappingResult = System.Collections.Immutable.ImmutableArray<tsr_di.ResultOrError<tsr_di.DelegateItem>>;

global using CollectedTypeSymbols = Microsoft.CodeAnalysis.IncrementalValueProvider<System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.INamedTypeSymbol>>;
global using CollectedMethodSymbols = Microsoft.CodeAnalysis.IncrementalValueProvider<System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.IMethodSymbol>>;
global using TypeSymbols = System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.INamedTypeSymbol>;
global using MethodSymbols = System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.IMethodSymbol>;
