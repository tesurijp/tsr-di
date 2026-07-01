using System.Collections.Immutable;
using System.Collections;

namespace tsr_di;

internal record class ResolverItem(string IdentName, string? Key,  string FieldName);
internal record class FieldItem(string TypeName , string FieldName, LifeTime LifeTime, string InitializeString);
internal record class DelegateItem(string ReturnType, string Name, ImmutableArray<string> ArgList, bool Create = true) 
{
    public bool AreSameSigunature(DelegateItem other) =>
        ReturnType == other.ReturnType &&
        StructuralComparisons.StructuralEqualityComparer.Equals(ArgList, other.ArgList);
}

internal class ResultOrError<T>
{
    private ResultOrError() { }
    public T? Result { get; init; }
    public ErrorItem? Error { get; init; }
    public bool HasError => Error is not null;

    public static implicit operator ResultOrError<T>(T item) => new() { Result = item };
    public static implicit operator ResultOrError<T>(ErrorItem item) => new() { Error = item };
}

