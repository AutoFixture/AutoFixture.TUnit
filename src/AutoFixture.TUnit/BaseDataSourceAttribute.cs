using AutoFixture.TUnit.Internal;

namespace AutoFixture.TUnit;

/// <summary>
/// Base class for data sources that provide AutoFixture test data for TUnit data driven tests.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public abstract class BaseDataSourceAttribute : Attribute, IDataSourceAttribute, IDataSource
{
    /// <summary>
    /// Returns the test data provided by the source.
    /// </summary>
    /// <param name="dataGeneratorMetadata">
    /// The metadata for the target method for which to provide the arguments.
    /// </param>
    /// <returns>
    /// Returns a sequence of argument collections, where each collection
    /// is an array of objects representing the arguments for a test method.
    /// </returns>
    public abstract IAsyncEnumerable<Func<Task<object?[]?>>> GetData(DataGeneratorMetadata dataGeneratorMetadata);

    /// <inheritdoc />
    public bool SkipIfEmpty { get; set; }

    /// <inheritdoc />
    public bool DeferEnumeration { get; set; }

    /// <inheritdoc />
    public async IAsyncEnumerable<Func<Task<object?[]?>>> GetDataRowsAsync(DataGeneratorMetadata dataGeneratorMetadata)
    {
        if (dataGeneratorMetadata is null) throw new ArgumentNullException(nameof(dataGeneratorMetadata));

        var parameters = dataGeneratorMetadata.MembersToGenerate;
        if (parameters.Length == 0)
        {
            // If the method has no parameters, a single test run is enough.
            yield return () => Task.FromResult<object?[]?>(Array.Empty<object?>());
            yield break;
        }

        var enumerable = this.GetData(dataGeneratorMetadata)
            ?? throw new InvalidOperationException("The source member yielded no test data.");

        await foreach (var testDataFunc in enumerable)
        {
            if (testDataFunc is null) throw new InvalidOperationException("The source member yielded a null test data.");

            yield return testDataFunc;
        }
    }
}
