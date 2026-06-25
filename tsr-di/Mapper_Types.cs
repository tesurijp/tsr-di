using System.Collections.Immutable;

namespace tsr_di;

internal record class ResolverItem(string IdentName, string? Key,  string FieldName);
internal record class FieldItem(string FieldName, LifeTime LifeTime, string InitializeString);
internal record class DelegateItem(string ReturnType, string Name, ImmutableArray<string> ArgList, bool Create = true);

internal class ResultOrError<T>
{
    private ResultOrError() { }
    public T? Result { get; init; }
    public ErrorItem? Error { get; init; }
    public bool HasError => Error is not null;

    public static implicit operator ResultOrError<T>(T item) => new() { Result = item };
    public static implicit operator ResultOrError<T>(ErrorItem item) => new() { Error = item };
}

