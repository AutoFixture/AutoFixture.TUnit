using AutoFixture.TUnit.Internal;

namespace AutoFixture.TUnit.Tests.TestTypes;

public class DelegatingDataSource : AutoDataSourceAttribute, IDataSource
{
    public IEnumerable<object[]> TestData { get; set; } = Array.Empty<object[]>();

    public override IEnumerable<object[]> GetData(DataGeneratorMetadata dataGeneratorMetadata)
    {
        return this.TestData;
    }
}