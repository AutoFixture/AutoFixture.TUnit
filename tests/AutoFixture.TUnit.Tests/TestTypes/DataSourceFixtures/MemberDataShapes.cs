using TestTypeFoundation;

namespace AutoFixture.TUnit.Tests.TestTypes.DataSourceFixtures;

/// <summary>
/// Shared member data shapes for public AutoMemberDataSourceAttribute coverage.
/// </summary>
public static class MemberDataShapes
{
    public static object? NullValue() => null;

    public static string StringScalar() => "hello";

    public static object ObjectScalar() => new object();

    public static (string, int) SingleTuple() => ("a", 1);

    public static IEnumerable<object[]> EmptyObjectArrays() => [];

    public static IEnumerable<object[]> ObjectArrays()
    {
        yield return ["hello", 1, new RecordType<string>("world")];
        yield return ["foo", 2, new RecordType<string>("bar")];
    }

    public static IEnumerable<string> Strings()
    {
        yield return "one";
        yield return "two";
    }

    public static IEnumerable<(string, int)> Tuples()
    {
        yield return ("a", 1);
        yield return ("b", 2);
    }

    public static Task<IEnumerable<object[]>> TaskOfObjectArrays() =>
        Task.FromResult(ObjectArrays());

    public static Task<IEnumerable<(string, int)>> TaskOfTuples() =>
        Task.FromResult(Tuples());

    public static async IAsyncEnumerable<object[]> AsyncObjectArrays()
    {
        foreach (var row in ObjectArrays())
        {
            yield return row;
        }

        await Task.CompletedTask;
    }

    public static async IAsyncEnumerable<string> AsyncStrings()
    {
        foreach (var value in Strings())
        {
            yield return value;
        }

        await Task.CompletedTask;
    }

    public static async IAsyncEnumerable<(string, int)> AsyncTuples()
    {
        foreach (var value in Tuples())
        {
            yield return value;
        }

        await Task.CompletedTask;
    }

    public static async IAsyncEnumerable<object[]> EmptyAsyncObjectArrays()
    {
        await Task.CompletedTask;
        yield break;
    }

    // Field / property mirrors for resolve-member coverage
    public static readonly IEnumerable<(string, int)> TupleField = [("a", 1), ("b", 2)];

    public static IEnumerable<(string, int)> TupleProperty => [("a", 1), ("b", 2)];
}
