using System.Collections;
using System.Runtime.CompilerServices;
using TestTypeFoundation;

namespace AutoFixture.TUnit.Tests.TestTypes.DataSourceFixtures;

public class ObjectArrayClassData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return ["hello", 1, new RecordType<string>("world")];
        yield return ["foo", 2, new RecordType<string>("bar")];
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}

public class StringSequenceClassData : IEnumerable<string>
{
    public IEnumerator<string> GetEnumerator()
    {
        yield return "alpha";
        yield return "beta";
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}

public class TupleSequenceClassData : IEnumerable<(string, int)>
{
    public IEnumerator<(string, int)> GetEnumerator()
    {
        yield return ("a", 1);
        yield return ("b", 2);
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}

public class AsyncObjectArrayClassData : IAsyncEnumerable<object[]>
{
    public IAsyncEnumerator<object[]> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        EnumerateAsync(cancellationToken).GetAsyncEnumerator(cancellationToken);

    private static async IAsyncEnumerable<object[]> EnumerateAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        yield return ["hello", 1, new RecordType<string>("world")];
        yield return ["foo", 2, new RecordType<string>("bar")];
        await Task.CompletedTask;
    }
}

public class AsyncStringSequenceClassData : IAsyncEnumerable<string>
{
    public IAsyncEnumerator<string> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        EnumerateAsync(cancellationToken).GetAsyncEnumerator(cancellationToken);

    private static async IAsyncEnumerable<string> EnumerateAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        yield return "alpha";
        yield return "beta";
        await Task.CompletedTask;
    }
}

public class AsyncTupleSequenceClassData : IAsyncEnumerable<(string, int)>
{
    public IAsyncEnumerator<(string, int)> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        EnumerateAsync(cancellationToken).GetAsyncEnumerator(cancellationToken);

    private static async IAsyncEnumerable<(string, int)> EnumerateAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        yield return ("a", 1);
        yield return ("b", 2);
        await Task.CompletedTask;
    }
}

public class EmptyAsyncObjectArrayClassData : IAsyncEnumerable<object[]>
{
    public IAsyncEnumerator<object[]> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        EnumerateAsync(cancellationToken).GetAsyncEnumerator(cancellationToken);

    private static async IAsyncEnumerable<object[]> EnumerateAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        yield break;
    }
}

/// <summary>
/// Non-enumerable source type: the constructed instance becomes a single-cell row.
/// </summary>
public class StringScalarClassData
{
    public string Value { get; } = "seed";
}

/// <summary>
/// One row with no columns so AutoClassDataSource fills every constructor parameter.
/// </summary>
public class OneEmptyRowClassData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return [];
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}

public class IntSequenceClassData : IEnumerable<int>
{
    public IEnumerator<int> GetEnumerator()
    {
        yield return 11;
        yield return 22;
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}
