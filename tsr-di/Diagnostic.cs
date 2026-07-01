using Microsoft.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace tsr_di;

internal record ErrorItem(DiagnosticDescriptor Error, string TypeName, Location PrimaryLocation)
{
    internal ErrorItem(DiagnosticDescriptor error, ISymbol symbol) :
        this(error, symbol.ToString(), symbol.Locations.FirstOrDefault(loc => loc.IsInSource) ?? Location.None) { }

    public static implicit operator Diagnostic(ErrorItem item) => Diagnostic.Create(item.Error, item.PrimaryLocation, item.TypeName);
}
internal static class DiagnosticDescriptors
{
    public const string ID = "TD";
    public const string Category = "tsr-di";

    private static DiagnosticDescriptor CreateDiagnosticDescriptor(string id, string title, string message) =>
        new(id: id, title: title, messageFormat: message, category: Category, defaultSeverity: DiagnosticSeverity.Error, isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor CsVersionError
        = CreateDiagnosticDescriptor($"{ID}9001", "C# Version error", "Language Version must be C# 14 or higher");

    // ServiceResolver
    public static readonly DiagnosticDescriptor NothingServiceResolver
        = CreateDiagnosticDescriptor($"{ID}0001", "Missing ServiceResolver", "The ServiceResolver class is not defined");
    public static readonly DiagnosticDescriptor MultipleServiceResolver
        = CreateDiagnosticDescriptor($"{ID}0002", "Duplicate ServiceResolver", "Multiple ServiceResolver classes are defined: '{0}'");
    public static readonly DiagnosticDescriptor MustBePartial
        = CreateDiagnosticDescriptor($"{ID}0003", "ServiceResolver must be partial", "The ServiceResolver class '{0}' must be declared as partial");

    // Resolving interface 
    public static readonly DiagnosticDescriptor NotRegisterd
        = CreateDiagnosticDescriptor($"{ID}0011", "Unregistered service", "Cannot resolve unregistered interface '{0}'");
    public static readonly DiagnosticDescriptor NotRegisterdKeyOrMultipleKey
        = CreateDiagnosticDescriptor($"{ID}0012", "Unregistered keyed service", "Cannot resolve unregistered interface '{0}' with the specified key");
    public static readonly DiagnosticDescriptor ConflictType
        = CreateDiagnosticDescriptor($"{ID}0013", "Duplicate registration", "Multiple implementations are registered for the same interface '{0}'");
    public static readonly DiagnosticDescriptor ConflictKey
        = CreateDiagnosticDescriptor($"{ID}0014", "Duplicate keyed registration", "Multiple implementations are registered for the same interface '{0}' using the same key");

    // Register ServiceClass
    public static readonly DiagnosticDescriptor ServiceClassMustBePublic
        = CreateDiagnosticDescriptor($"{ID}0021", "Non-public service class", "The service class '{0}' must be public");
    public static readonly DiagnosticDescriptor NeedPublicInterface
        = CreateDiagnosticDescriptor($"{ID}0022", "Missing interfaces", "The service class '{0}' must implements public interface");
    public static readonly DiagnosticDescriptor SinglePublicConstructorRequired
        = CreateDiagnosticDescriptor($"{ID}0023", "Invalid constructor", "The service class '{0}' must have exactly one public constructor");
    public static readonly DiagnosticDescriptor InvalidServiceClassKey
        = CreateDiagnosticDescriptor($"{ID}0024", "Invalid service class key", "The service class key '{0}' is not a valid identifier");

    // Register ServiceFunction
    public static readonly DiagnosticDescriptor ServiceFunctionMustBePublic
        = CreateDiagnosticDescriptor($"{ID}0031", "Non-public service function", "The service function '{0}' must be public");
    public static readonly DiagnosticDescriptor MethodIsNotIdentify
        = CreateDiagnosticDescriptor($"{ID}0032", "Invalid service function", "The service function '{0}' must be static, or its containing class must be decorated with [ServiceClass]");
    public static readonly DiagnosticDescriptor ParentSharingModeIsIsolate
        = CreateDiagnosticDescriptor($"{ID}0033", "Invalid sharing mode", "The service function '{0}' cannot be defined in a class with Isolate sharing mode");
    public static readonly DiagnosticDescriptor InvalidServiceFunctionKey
        = CreateDiagnosticDescriptor($"{ID}0034", "Invalid service function key", "The service function key '{0}' is not a valid identifier");
    public static readonly DiagnosticDescriptor InvalidServiceFunctionName
        = CreateDiagnosticDescriptor($"{ID}0035", "Invalid service function name", "The service function name '{0}' is not a valid identifier");
    public static readonly DiagnosticDescriptor InvalidServiceFunctionType
        = CreateDiagnosticDescriptor($"{ID}0036", "Invalid service function type", "The service function type '{0}' is not compatible type");
}

