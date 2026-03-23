using AutoFixture.TUnit.Internal;

namespace AutoFixture.TUnit.Tests.TestTypes;

public class DelegatingDataSource : BaseDataSourceAttribute, IDataSource
{
    public IEnumerable<object[]> TestData { get; set; } = Array.Empty<object[]>();

    public override async IAsyncEnumerable<Func<Task<object?[]?>>> GetData(DataGeneratorMetadata dataGeneratorMetadata)
    {
        if (this.TestData is null)
        {
            throw new InvalidOperationException("The source member yielded no test data.");
        }

        var maxArgs = dataGeneratorMetadata.MembersToGenerate.Length;

        foreach (var data in this.TestData)
        {
            if (data.Length > maxArgs)
            {
                throw new InvalidOperationException(
                    "The number of arguments provided exceeds the number of parameters.");
            }

            yield return () => Task.FromResult<object?[]?>(data);
        }

        await Task.CompletedTask;
    }
}
