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
    private static void CompilationCheck(IncrementalGeneratorInitializationContext context, IncrementalValueProvider<bool> isEnabled)
    {
        var verCheck = context.CompilationProvider.Select((c, _) => (c is CSharpCompilation { LanguageVersion: >= LanguageVersion.CSharp14 }) ? null : new ErrorItem(DiagnosticDescriptors.CsVersionError, "", Location.None));
        context.RegisterSourceOutputWhenEnabled(verCheck, isEnabled, (ctx, err) =>
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
        var isEnabled = DirectPackageReferenceGate.IsEnabled(context, "TSR_DI_GENERATOR_ENABLED");

        // PreOutput
        Emitter.WriteAttribute(context);

        CompilationCheck(context, isEnabled);

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
        var fieldsItemsOrError = FieldStoreMapper.ToFieldItems(serviceClass, serviceFunctions, constSymbol);
        var funcFieldsItemsOrError = FieldStoreMapper.ToFunctionField(serviceFunctions, constSymbol);
        var resolveItemOrError = ResolverFunctionMapper.ToResolveItems(directUsedTypes, serviceClass, serviceFunctions, constSymbol);
        var delegateItemOrError = DeclarationMapper.ToDelegateItem(serviceFunctions, constSymbol);
        var svcResolverItemOrError = ResolverFunctionMapper.ToSvcResolverName(serviceResolverClass);
        var typeArgsCount = DeclarationMapper.ToResolveMethodArgs(directUsedTypes);
        var resolvPropList = ResolverPropertyMapper.ToResolveItems(serviceClass, serviceFunctions, constSymbol);

        // Split data and items
        var (fieldsItems, fieldErrors) = fieldsItemsOrError.Split();
        var (funcfieldsItems, funcfieldErrors) = funcFieldsItemsOrError.Split();
        var (resolveItem, resolveErrors) = resolveItemOrError.Split();
        var (svcResolverItem, svcResolverErrors) = svcResolverItemOrError.Split();
        var (delegateItem, svcDelegateErrors) = delegateItemOrError.Split();
        var fieldsItemsAll = fieldsItems.Append(funcfieldsItems);

        // Post output
        context.RegisterSourceOutputWhenEnabled(svcResolverItem.Combine(fieldsItemsAll), isEnabled, Emitter.WriteFieldItems);
        context.RegisterSourceOutputWhenEnabled(svcResolverItem.Combine(delegateItem), isEnabled, Emitter.WriteDelegates);
        context.RegisterSourceOutputWhenEnabled(svcResolverItem.Combine(typeArgsCount), isEnabled, Emitter.WriteResolveFunc);
        context.RegisterSourceOutputWhenEnabled(svcResolverItem.Combine(resolveItem), isEnabled, Emitter.WriteTypedEnum);
        context.RegisterSourceOutputWhenEnabled(svcResolverItem.Combine(resolveItem), isEnabled, Emitter.WriteInnerResolve);
        context.RegisterSourceOutputWhenEnabled(svcResolverItem.Combine(resolvPropList), isEnabled, Emitter.WriteResolverProp);
        context.RegisterSourceOutputWhenEnabled(fieldErrors, isEnabled, Emitter.OutputErrors);
        context.RegisterSourceOutputWhenEnabled(funcfieldErrors, isEnabled, Emitter.OutputErrors);
        context.RegisterSourceOutputWhenEnabled(resolveErrors, isEnabled, Emitter.OutputErrors);
        context.RegisterSourceOutputWhenEnabled(svcResolverErrors, isEnabled, Emitter.OutputErrors);
        context.RegisterSourceOutputWhenEnabled(svcDelegateErrors, isEnabled, Emitter.OutputErrors);
    }
}

