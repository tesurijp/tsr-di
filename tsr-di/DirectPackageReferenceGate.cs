using Microsoft.CodeAnalysis;
using System;

namespace tsr_di;

internal static class DirectPackageReferenceGate
{
    internal static IncrementalValueProvider<bool> IsEnabled(
        IncrementalGeneratorInitializationContext context,
        string buildPropertyName)
    {
        var analyzerConfigPropertyName = $"build_property.{buildPropertyName}";
        return context.AnalyzerConfigOptionsProvider.Select((options, _) =>
            options.GlobalOptions.TryGetValue(analyzerConfigPropertyName, out var value)
            && bool.TryParse(value, out var enabled)
            && enabled);
    }

    internal static void RegisterSourceOutputWhenEnabled<T>(
        this IncrementalGeneratorInitializationContext context,
        IncrementalValueProvider<T> source,
        IncrementalValueProvider<bool> isEnabled,
        Action<SourceProductionContext, T> action)
        => context.RegisterSourceOutput(source.Combine(isEnabled), (productionContext, value) =>
        {
            if (value.Right)
            {
                action(productionContext, value.Left);
            }
        });
}
