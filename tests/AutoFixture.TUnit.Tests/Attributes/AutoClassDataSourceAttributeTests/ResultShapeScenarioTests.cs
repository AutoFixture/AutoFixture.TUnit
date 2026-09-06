using AutoFixture.TUnit.Tests.TestTypes;
using AutoFixture.TUnit.Tests.TestTypes.DataSourceFixtures;
using TestTypeFoundation;

namespace AutoFixture.TUnit.Tests.Attributes.AutoClassDataSourceAttributeTests;

/// <summary>
/// End-to-end [AutoClassDataSource] coverage for README result shapes.
/// </summary>
public class ResultShapeScenarioTests
{
    [Test]
    [AutoClassDataSource(typeof(StringSequenceClassData))]
    public async Task WhenClassYieldsStrings_ReceivesValueAndFillsRemaining(string value, MyClass leftover)
    {
        await Assert.That(new[] { "alpha", "beta" }).Contains(value);
        await Assert.That(leftover).IsNotNull();
    }

    [Test]
    [AutoClassDataSource(typeof(TupleSequenceClassData))]
    public async Task WhenClassYieldsTuples_ExpandsColumnsAndFillsRemaining(
        string left, int right, MyClass leftover)
    {
        await Assert.That(new[] { ("a", 1), ("b", 2) }).Contains((left, right));
        await Assert.That(leftover).IsNotNull();
    }

    [Test]
    [AutoClassDataSource(typeof(ObjectArrayClassData))]
    public async Task WhenClassYieldsObjectArrays_UsesSuppliedValues(
        string a, int b, RecordType<string> c)
    {
        await Assert.That(new[]
        {
            ("hello", 1, "world"),
            ("foo", 2, "bar")
        }).Contains((a, b, c.Value));
    }

    [Test]
    [AutoClassDataSource(typeof(AsyncObjectArrayClassData))]
    public async Task WhenClassYieldsAsyncObjectArrays_UsesSuppliedValues(
        string a, int b, RecordType<string> c)
    {
        await Assert.That(new[]
        {
            ("hello", 1, "world"),
            ("foo", 2, "bar")
        }).Contains((a, b, c.Value));
    }

    [Test]
    [AutoClassDataSource(typeof(AsyncStringSequenceClassData))]
    public async Task WhenClassYieldsAsyncStrings_ReceivesValueAndFillsRemaining(string value, MyClass leftover)
    {
        await Assert.That(new[] { "alpha", "beta" }).Contains(value);
        await Assert.That(leftover).IsNotNull();
    }

    [Test]
    [AutoClassDataSource(typeof(AsyncTupleSequenceClassData))]
    public async Task WhenClassYieldsAsyncTuples_ExpandsColumnsAndFillsRemaining(
        string left, int right, MyClass leftover)
    {
        await Assert.That(new[] { ("a", 1), ("b", 2) }).Contains((left, right));
        await Assert.That(leftover).IsNotNull();
    }

    [Test]
    [AutoClassDataSource(typeof(StringScalarClassData))]
    public async Task WhenClassIsScalar_UsesInstanceAsCellAndFillsRemaining(
        StringScalarClassData seed, MyClass leftover)
    {
        await Assert.That(seed).IsNotNull();
        await Assert.That(seed.Value).IsEqualTo("seed");
        await Assert.That(leftover).IsNotNull();
    }
}
