using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;
using System.Linq;

namespace tsr_di;

static file class IncrementalValueProviderExtensions
{
    internal static (IncrementalValueProvider<ImmutableArray<T>>, IncrementalValueProvider<ImmutableArray<ErrorItem>>) Split<T>(this IncrementalValueProvider<ImmutableArray<ResultOrError<T>>> resultOrErrors)
        => (resultOrErrors.Select((x, _) => x.Where(i => !i.HasError).Select(i => i.Result!).ToImmutableArray()), resultOrErrors.Select((x, _) => x.Where(i => i.HasError).Select(i => i.Error!).ToImmutableArray()));

    internal static IncrementalValueProvider<ImmutableArray<T>> Append<T>(this IncrementalValueProvider<ImmutableArray<T>> left, IncrementalValueProvider<ImmutableArray<T>> right) 
        => left.Combine(right).Select((x, _) => x.Left.AddRange(x.Right));
}


[Generator(LanguageNames.CSharp)]
public class Generator : IIncrementalGenerator
{
    private static void CompilationCheck(IncrementalGeneratorInitializationContext context)
    {
        var verCheck = context.CompilationProvider.Select((c, _) => (c is CSharpCompilation { LanguageVersion: >= LanguageVersion.CSharp14 }) ? null : new ErrorItem(DiagnosticDescriptors.CsVersionError, "", Location.None));
        context.RegisterSourceOutput(verCheck, (ctx, err) =>
        {
            if (err != null)
            {
                var diagnostic = Diagnostic.Create(err.Error, err.PrimaryLocation, err.TypeName);
                ctx.ReportDiagnostic(diagnostic);
            }
        });
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // PreOutput
        Emitter.WriteAttribute(context);

        CompilationCheck(context);

        // Collect info
        var constSymbol = Collector.ConstSymbols(context);
        var localServiceClasses = Collector.FindLocalServiceClasses(context).Collect();
        var referServiceClasses = Collector.FindReferServiceClasses(context).Collect();
        var localServiceFunctions = Collector.FindLocalServiceFunctions(context).Collect();
        var referServiceFunctions = Collector.FindReferServiceFunctions(context).Collect();
        var serviceResolverClass = Collector.FindServiceResolver(context).Collect();
        var directUsedTypes = Collector.FindResolveFunc(context).Collect();
        var serviceClass = localServiceClasses.Append(referServiceClasses);
        var serviceFunctions = localServiceFunctions.Append(referServiceFunctions);

        // Convert and Check
        var fieldsItemsOrError = Mapper.ToFieldItems(serviceClass, serviceFunctions, constSymbol);
        var funcFieldsItemsOrError = Mapper.ToFunctionField(serviceFunctions, constSymbol);
        var resolveItemOrError = Mapper.ToResolveItems(directUsedTypes, serviceClass, serviceFunctions, constSymbol);
        var delegateItemOrError = Mapper.ToDelegateItem(serviceFunctions, constSymbol);
        var svcResolverItemOrError = Mapper.ToSvcResolverName(serviceResolverClass);
        var typeArgsCount = Mapper.ToResolveMethodArgs(directUsedTypes);

        // Split data and items
        var (fieldsItems, fieldErrors) = fieldsItemsOrError.Split();
        var (funcfieldsItems, funcfieldErrors) = funcFieldsItemsOrError.Split();
        var (resolveItem, resolveErrors) = resolveItemOrError.Split();
        var (svcResolverItem, svcResolverErrors) = svcResolverItemOrError.Split();
        var (delegateItem, svcDelegateErrors) = delegateItemOrError.Split();
        var fieldsItemsAll = fieldsItems.Append(funcfieldsItems);

        // Post output
        context.RegisterSourceOutput(svcResolverItem.Combine(fieldsItemsAll), Emitter.WriteFieldItems);
        context.RegisterSourceOutput(svcResolverItem.Combine(delegateItem), Emitter.WriteDelegates);
        context.RegisterSourceOutput(svcResolverItem.Combine(typeArgsCount), Emitter.WriteResolveFunc);
        context.RegisterSourceOutput(svcResolverItem.Combine(resolveItem), Emitter.WriteTypedEnum);
        context.RegisterSourceOutput(svcResolverItem.Combine(resolveItem), Emitter.WriteInnerResolve);
        context.RegisterSourceOutput(fieldErrors, Emitter.OutputErrors);
        context.RegisterSourceOutput(funcfieldErrors, Emitter.OutputErrors);
        context.RegisterSourceOutput(resolveErrors, Emitter.OutputErrors);
        context.RegisterSourceOutput(svcResolverErrors, Emitter.OutputErrors);
        context.RegisterSourceOutput(svcDelegateErrors, Emitter.OutputErrors);
    }
}

