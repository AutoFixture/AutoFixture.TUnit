using AutoFixture.TUnit.Tests.TestTypes;
using AutoFixture.TUnit.Tests.TestTypes.DataSourceFixtures;
using TestTypeFoundation;

namespace AutoFixture.TUnit.Tests.Attributes.AutoMemberDataSourceAttributeTests;

/// <summary>
/// End-to-end [AutoMemberDataSource] coverage for README result shapes.
/// </summary>
public class ResultShapeScenarioTests
{
    [Test]
    [AutoMemberDataSource(typeof(MemberDataShapes), nameof(MemberDataShapes.ObjectArrays))]
    public async Task WhenMemberReturnsObjectArrays_UsesSuppliedValues(
        string a, int b, RecordType<string> c)
    {
        await Assert.That<(string, int, string)[]>([
            ("hello", 1, "world"),
            ("foo", 2, "bar")
        ]).Contains((a, b, c.Value));
    }

    [Test]
    [AutoMemberDataSource(typeof(MemberDataShapes), nameof(MemberDataShapes.Strings))]
    public async Task WhenMemberYieldsStrings_ReceivesValueAndFillsRemaining(string value, MyClass leftover)
    {
        await Assert.That<string[]>(["one", "two"]).Contains(value);
        await Assert.That(leftover).IsNotNull();
    }

    [Test]
    [AutoMemberDataSource(typeof(MemberDataShapes), nameof(MemberDataShapes.StringScalar))]
    public async Task WhenMemberReturnsStringScalar_ReceivesValueAndFillsRemaining(string value, MyClass leftover)
    {
        await Assert.That(value).IsEqualTo("hello");
        await Assert.That(leftover).IsNotNull();
    }

    [Test]
    [AutoMemberDataSource(typeof(MemberDataShapes), nameof(MemberDataShapes.ObjectScalar))]
    public async Task WhenMemberReturnsObjectScalar_ReceivesValueAndFillsRemaining(object value, MyClass leftover)
    {
        await Assert.That(value).IsNotNull();
        await Assert.That(leftover).IsNotNull();
    }

    [Test]
    [AutoMemberDataSource(typeof(MemberDataShapes), nameof(MemberDataShapes.Tuples))]
    public async Task WhenMemberYieldsTuples_ExpandsColumnsAndFillsRemaining(
        string left, int right, MyClass leftover)
    {
        await Assert.That<(string, int)[]>([("a", 1), ("b", 2)]).Contains((left, right));
        await Assert.That(leftover).IsNotNull();
    }

    [Test]
    [AutoMemberDataSource(typeof(MemberDataShapes), nameof(MemberDataShapes.SingleTuple))]
    public async Task WhenMemberReturnsSingleTuple_ExpandsColumnsAndFillsRemaining(
        string left, int right, MyClass leftover)
    {
        await Assert.That(left).IsEqualTo("a");
        await Assert.That(right).IsEqualTo(1);
        await Assert.That(leftover).IsNotNull();
    }

    [Test]
    [AutoMemberDataSource(typeof(MemberDataShapes), nameof(MemberDataShapes.NullValue))]
    public async Task WhenMemberReturnsNull_ReceivesNullCellAndFillsRemaining(string value, MyClass leftover)
    {
        await Assert.That(value).IsNull();
        await Assert.That(leftover).IsNotNull();
    }

    [Test]
    [AutoMemberDataSource(typeof(MemberDataShapes), nameof(MemberDataShapes.TaskOfObjectArrays))]
    public async Task WhenMemberReturnsTaskOfObjectArrays_UsesSuppliedValues(
        string a, int b, RecordType<string> c)
    {
        await Assert.That<(string, int, string)[]>([
            ("hello", 1, "world"),
            ("foo", 2, "bar")
        ]).Contains((a, b, c.Value));
    }

    [Test]
    [AutoMemberDataSource(typeof(MemberDataShapes), nameof(MemberDataShapes.TaskOfTuples))]
    public async Task WhenMemberReturnsTaskOfTuples_ExpandsColumnsAndFillsRemaining(
        string left, int right, MyClass leftover)
    {
        await Assert.That<(string, int)[]>([("a", 1), ("b", 2)]).Contains((left, right));
        await Assert.That(leftover).IsNotNull();
    }

    [Test]
    [AutoMemberDataSource(typeof(MemberDataShapes), nameof(MemberDataShapes.AsyncObjectArrays))]
    public async Task WhenMemberReturnsAsyncObjectArrays_UsesSuppliedValues(
        string a, int b, RecordType<string> c)
    {
        await Assert.That<(string, int, string)[]>([
            ("hello", 1, "world"),
            ("foo", 2, "bar")
        ]).Contains((a, b, c.Value));
    }

    [Test]
    [AutoMemberDataSource(typeof(MemberDataShapes), nameof(MemberDataShapes.AsyncStrings))]
    public async Task WhenMemberReturnsAsyncStrings_ReceivesValueAndFillsRemaining(string value, MyClass leftover)
    {
        await Assert.That<string[]>(["one", "two"]).Contains(value);
        await Assert.That(leftover).IsNotNull();
    }

    [Test]
    [AutoMemberDataSource(typeof(MemberDataShapes), nameof(MemberDataShapes.AsyncTuples))]
    public async Task WhenMemberReturnsAsyncTuples_ExpandsColumnsAndFillsRemaining(
        string left, int right, MyClass leftover)
    {
        await Assert.That<(string, int)[]>([("a", 1), ("b", 2)]).Contains((left, right));
        await Assert.That(leftover).IsNotNull();
    }

    [Test]
    [AutoMemberDataSource(typeof(MemberDataShapes), nameof(MemberDataShapes.TupleField))]
    public async Task WhenMemberIsTupleField_ExpandsColumns(string left, int right)
    {
        await Assert.That<(string, int)[]>([("a", 1), ("b", 2)]).Contains((left, right));
    }

    [Test]
    [AutoMemberDataSource(typeof(MemberDataShapes), nameof(MemberDataShapes.TupleProperty))]
    public async Task WhenMemberIsTupleProperty_ExpandsColumns(string left, int right)
    {
        await Assert.That<(string, int)[]>([("a", 1), ("b", 2)]).Contains((left, right));
    }
}
