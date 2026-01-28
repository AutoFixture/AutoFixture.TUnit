using AutoFixture.TUnit.Internal;

namespace AutoFixture.TUnit;

/// <summary>
/// Base class for data sources that provide AutoFixture test data for TUnit data driven tests.
/// </summary>
public abstract class BaseDataSourceAttribute : UntypedDataSourceGeneratorAttribute, IDataSource
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
    public abstract IEnumerable<object?[]?> GetData(DataGeneratorMetadata dataGeneratorMetadata);

    /// <inheritdoc />
    protected override IEnumerable<Func<object?[]>> GenerateDataSources(DataGeneratorMetadata dataGeneratorMetadata)
    {
        if (dataGeneratorMetadata is null) throw new ArgumentNullException(nameof(dataGeneratorMetadata));

        return GetTestDataEnumerable();

        IEnumerable<Func<object?[]>> GetTestDataEnumerable()
        {
            var parameters = dataGeneratorMetadata.MembersToGenerate;
            if (parameters.Length == 0)
            {
                // If the method has no parameters, a single test run is enough.
                yield return () => [];
                yield break;
            }

            var enumerable = this.GetData(dataGeneratorMetadata)
                ?? throw new InvalidOperationException("The source member yielded no test data.");

            foreach (var testData in enumerable)
            {
                if (testData is null) throw new InvalidOperationException("The source member yielded a null test data.");
                if (testData.Length > parameters.Length) throw new InvalidOperationException("The number of arguments provided exceeds the number of parameters.");

                yield return () => testData;
            }
        }
    }

    public IEnumerable<Func<object?[]>> GetDataSources(DataGeneratorMetadata dataGeneratorMetadata)
    {
        return this.GenerateDataSources(dataGeneratorMetadata);
    }
}
