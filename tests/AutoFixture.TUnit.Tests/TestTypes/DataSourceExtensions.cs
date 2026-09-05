namespace AutoFixture.TUnit.Tests.TestTypes;

internal static class DataSourceExtensions
{
    /// <summary>
    /// Bridges the old sync GenerateDataSources API to the new async GetDataRowsAsync API for test compatibility.
    /// </summary>
    public static IEnumerable<Func<object?[]>> GenerateDataSources(
        this IDataSourceAttribute source,
        DataGeneratorMetadata dataGeneratorMetadata)
    {
        var asyncEnumerable = source.GetDataRowsAsync(dataGeneratorMetadata);
        var enumerator = asyncEnumerable.GetAsyncEnumerator();
        try
        {
            while (enumerator.MoveNextAsync().AsTask().GetAwaiter().GetResult())
            {
                var current = enumerator.Current;
                yield return () => current().GetAwaiter().GetResult()!;
            }
        }
        finally
        {
            enumerator.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }
    }
}
