using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace tsr_di;

using FuncLookup = ILookup<string, (IMethodSymbol tp, string? tagname)>;
using TypeLookup = ILookup<ISymbol?, (INamedTypeSymbol tp, (string? name, SharingMode mode) tag)>;

internal partial class Mapper
{
    internal static string DelegateName(INamedTypeSymbol? tp, string? typename, IMethodSymbol item) => tp?.ToString() ?? $"I{typename ?? item.Name}";
    private static TypeLookup CreateTypeLookup(TypeSymbols items, INamedTypeSymbol regAttr)
    {
        (string? name, SharingMode mode) _GetTag(INamedTypeSymbol tp)
        {
            var (name, _, shared) = GetServiceClassAttribute(regAttr, tp);
            return (name, shared);
        }
        return items.SelectMany(item => item.AllInterfaces.Where(i=>i.DeclaredAccessibility == Accessibility.Public), (item, iftp) => (iftp, item))
            .ToLookup(i => i.iftp, i => (tp: i.item, tag: _GetTag(i.item)), SymbolEqualityComparer.Default);
    }

    private static (string iftp, string? tag) GetInterfaceName(IMethodSymbol tp, INamedTypeSymbol regAttr)
    {
        var (type, name, key) = GetServiceFunctionAttribute(regAttr, tp);
        return (DelegateName(type, name, tp), key);
    }
    private static FuncLookup CreateFuncLookup(MethodSymbols items, INamedTypeSymbol regAttr) =>
        items.Select(i => (tag: GetInterfaceName(i, regAttr), item: i)).ToLookup(i => i.tag.iftp, i => (i.item, i.tag.tag));

    private static readonly SHA256 sha256 = SHA256.Create();
    private static readonly object shaLock = new ();
    private static byte[] MakeHash(byte[] strbuf)
    {
        lock (shaLock)
        {
            return sha256.ComputeHash(strbuf);
        }
    }
    private static string ToTidyName(ISymbol tp)
    {
        var hash = MakeHash(Encoding.UTF8.GetBytes(tp.ToString()));
        var value = BitConverter.ToUInt64(hash, 0);
        var hashStr = value.ToString("X16");
        return $"{(char.IsDigit(tp.Name[0]) ? "_" : "")}{tp.Name}_{hashStr}";
    }
    private static (string?, LifeTime lifetime, SharingMode shared) GetServiceClassAttribute(INamedTypeSymbol regClassAttr, INamedTypeSymbol tp)
    {
        var attr = tp.GetAttributes().Single(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, regClassAttr));
        var lifetime = GetAttributeParamValue(LifeTime.Transient, "LifeTime", attr.NamedArguments);
        var shared = GetAttributeParamValue(SharingMode.Shared, "SharingMode", attr.NamedArguments);
        var name = GetAttributeParamValue((string?)null, "Name", attr.NamedArguments);
        return (name, lifetime, shared);
    }
    private static (INamedTypeSymbol ? type, string? serviceName, string? name) GetServiceFunctionAttribute(INamedTypeSymbol regClassAttr, IMethodSymbol tp)
    {
        var attr = tp.GetAttributes().Single(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, regClassAttr));
        var type = GetAttributeParamValue((INamedTypeSymbol?)null, "ServiceType", attr.NamedArguments);
        var servicename = GetAttributeParamValue((string?)null, "ServiceName", attr.NamedArguments);
        var name = GetAttributeParamValue((string?)null, "Name", attr.NamedArguments);
        return (type, servicename, name);
    }
    private static T GetAttributeParamValue<T>(T defaultValue, string keyField, ImmutableArray<KeyValuePair<string, TypedConstant>> args) =>
            (args.FirstOrDefault(a => a.Key == keyField).Value.Value is object value) ? (T)value : defaultValue;
    private static bool IsPartial(INamedTypeSymbol tp) =>
            !tp.DeclaringSyntaxReferences.IsEmpty &&
            tp.DeclaringSyntaxReferences.Select(syntaxRef => syntaxRef.GetSyntax()).OfType<TypeDeclarationSyntax>().Any(typeDecl => typeDecl.Modifiers.Any(SyntaxKind.PartialKeyword));
    private static bool IsInvalideName(string? value) => value is not null && ! SyntaxFacts.IsValidIdentifier(value);
    private static Location GetParameterFullLocation(IParameterSymbol psymbol) =>
            (psymbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as ParameterSyntax)?.GetLocation() ?? psymbol.Locations.FirstOrDefault() ?? Location.None;

    private static Location GetAttributeFullLocation(ISymbol sym, INamedTypeSymbol attrType) =>
        sym.GetAttributes().Single(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attrType))
            .ApplicationSyntaxReference?.GetSyntax().GetLocation() ?? Location.None;
}

