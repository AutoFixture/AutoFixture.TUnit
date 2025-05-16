using System.Reflection;

namespace AutoFixture.TUnit.Tests.TestTypes;

public class FakeDataAttribute : AutoFixtureDataSourceAttribute
{
    private readonly MethodInfo expectedMethod;
    private readonly IEnumerable<object[]> output;

    public FakeDataAttribute(MethodInfo expectedMethod, IEnumerable<object[]> output)
    {
        this.output = output;
        this.expectedMethod = expectedMethod;
    }

    public override IEnumerable<object[]> GetData(DataGeneratorMetadata dataGeneratorMetadata)
    {
        return this.output;
    }
}