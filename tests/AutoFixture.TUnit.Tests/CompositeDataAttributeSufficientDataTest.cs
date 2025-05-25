using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AutoFixture.TUnit.Tests.TestTypes;
using TestTypeFoundation;
using TUnit.Assertions.Equality;

namespace AutoFixture.TUnit.Tests;

[SuppressMessage("Usage", "TUnit0014:Method should have a `Test` attribute or be made `private` or `protected`")]
[SuppressMessage("Usage", "TUnit0046:Return a `Func<T>` rather than a `<T>`")]
public class CompositeDataAttributeSufficientDataTest
{
    private readonly MethodInfo method = typeof(TypeWithOverloadedMembers)
        .GetMethod(nameof(TypeWithOverloadedMembers.DoSomething),
            [typeof(object), typeof(object), typeof(object)]);

    [Test]
    [MethodDataSource(typeof(CompositeDataAttributeSufficientDataTest), nameof(GetEnumerator))]
    public async Task GetDataReturnsCorrectResult(
        IEnumerable<BaseDataSourceAttribute> attributes,
        IEnumerable<object[]> expectedResult)
    {
        // Arrange
        var attribute = new CompositeDataSourceAttribute(attributes.ToArray());
        var dataGeneratorMetadata = DataGeneratorMetadataHelper
            .CreateDataGeneratorMetadata(this.method);
        var comparer = new CollectionEquivalentToEqualityComparer<object[]>();

        // Act
        var result = attribute.GenerateDataSources(dataGeneratorMetadata)
            .Select(x => x()).ToArray();

        // Assert
        await Assert.That(result)
            .IsEquivalentTo(expectedResult, comparer);
    }

    public IEnumerable<(
            IEnumerable<BaseDataSourceAttribute> Attributes,
            IEnumerable<object[]> ExpectedResult)>
        GetEnumerator()
    {
        yield return CreateTestData(
            data:
            [
                new FakeDataAttribute(this.method, [[1, 2, 3]])
            ],
            expected:
            [
                [1, 2, 3]
            ]);

        yield return CreateTestData(
            data:
            [
                new FakeDataAttribute(this.method, [[1, 2, 3]]),
                new FakeDataAttribute(this.method, [[4, 5, 6]])
            ],
            expected:
            [
                [1, 2, 3]
            ]);

        yield return CreateTestData(
            data:
            [
                new FakeDataAttribute(this.method, [[1]]),
                new FakeDataAttribute(this.method, [[2, 3, 4]])
            ],
            expected:
            [
                [1, 3, 4]
            ]);

        yield return CreateTestData(
            data:
            [
                new FakeDataAttribute(this.method, [[1, 2]]),
                new FakeDataAttribute(this.method, [[3, 4, 5]])
            ],
            expected:
            [
                [1, 2, 5]
            ]);

        yield return CreateTestData(
            data:
            [
                new FakeDataAttribute(this.method, [[1, 2, 3], [4, 5, 6]])
            ],
            expected:
            [
                [1, 2, 3],
                [4, 5, 6]
            ]);

        yield return CreateTestData(
            data:
            [
                new FakeDataAttribute(this.method, [[1, 2, 3], [4, 5, 6]]),
                new FakeDataAttribute(this.method,
                    [[7, 8], [9, 10], [11, 12]])
            ],
            expected:
            [
                [1, 2, 3],
                [4, 5, 6]
            ]);

        yield return CreateTestData(
            data:
            [
                new FakeDataAttribute(this.method,
                    [[1, 2], [3, 4], [5, 6]]),
                new FakeDataAttribute(this.method,
                    [[7, 8, 9], [10, 11, 12], [13, 14, 15]])
            ],
            expected:
            [
                [1, 2, 9],
                [3, 4, 12],
                [5, 6, 15]
            ]);

        // Second attribute restricts
        yield return CreateTestData(
            data:
            [
                new FakeDataAttribute(this.method, [[1, 2, 3], [4, 5, 6]]),
                new FakeDataAttribute(this.method, [[7, 8, 9]])
            ],
            expected:
            [
                [1, 2, 3]
            ]);

        // Shortest data provider is limiting factor
        yield return CreateTestData(
            data:
            [
                new FakeDataAttribute(this.method, [[1, 2, 3]]),
                new FakeDataAttribute(this.method, [[4, 5, 6], [7, 8, 9]])
            ],
            expected:
            [
                [1, 2, 3]
            ]);

        // Test incorrect number of parameters - should just return what it's given
        // and let xUnit deal with counting parameters
        yield return CreateTestData(
            data:
            [
                new FakeDataAttribute(this.method, [[1, 2]]),
                new FakeDataAttribute(this.method, [[3, 4]])
            ],
            expected:
            [
                [1, 2]
            ]);

        yield return CreateTestData(
            data:
            [
                new FakeDataAttribute(this.method, [[1]]),
                new FakeDataAttribute(this.method, [[2, 3]])
            ],
            expected:
            [
                [1, 3]
            ]);

        yield return CreateTestData(
            data:
            [
                new FakeDataAttribute(this.method, [[1]]),
                new FakeDataAttribute(this.method, [[]]),
                new FakeDataAttribute(this.method, [[2, 3]])
            ],
            expected:
            [
                [1, 3]
            ]);

        yield return CreateTestData(
            data:
            [
                new FakeDataAttribute(this.method, [[1]]),
                new FakeDataAttribute(this.method, [[2]]),
                new FakeDataAttribute(this.method, [[3]])
            ],
            expected:
            [
                [1]
            ]);
    }

    private static (
        IEnumerable<BaseDataSourceAttribute> Attributes,
        IEnumerable<object[]> ExpectedResult)
        CreateTestData(BaseDataSourceAttribute[] data, object[][] expected)
    {
        return (data, expected);
    }
}