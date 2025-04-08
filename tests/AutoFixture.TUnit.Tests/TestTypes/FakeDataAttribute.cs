using System.Reflection;

namespace AutoFixture.TUnit.Tests.TestTypes;

public class FakeDataAttribute(MethodInfo expectedMethod, IEnumerable<object[]> output) : AutoFixtureDataSourceAttribute
{
    private readonly MethodInfo _expectedMethod = expectedMethod;

    public override IEnumerable<object[]> GetData(DataGeneratorMetadata dataGeneratorMetadata)
    {
        return output;
    }
}