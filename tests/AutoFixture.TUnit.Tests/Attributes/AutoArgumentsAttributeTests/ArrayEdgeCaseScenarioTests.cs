using AutoFixture.TUnit.Tests.TestTypes;

namespace AutoFixture.TUnit.Tests.Attributes.AutoArgumentsAttributeTests;

/// <summary>
/// Documents the <c>params object?[]</c> pitfall: a non-generic
/// <c>[AutoArguments(new object[] { ... })]</c> expands into separate cells,
/// while <c>[AutoArguments&lt;T[]&gt;(...)]</c> keeps the array as one argument.
/// </summary>
public class ArrayEdgeCaseScenarioTests
{
    [Test]
    [AutoArguments(new object[] { 1, 2 })]
    public async Task WhenNonGenericObjectArray_ExpandsIntoSeparateArguments(int a, int b)
    {
        await Assert.That(a).IsEqualTo(1);
        await Assert.That(b).IsEqualTo(2);
    }

    [Test]
    [AutoArguments<object[]>(new object[] { 1, 2 })]
    public async Task WhenGenericObjectArray_KeepsArrayAsSingleArgument(object[] values, MyClass leftover)
    {
        await Assert.That(values).IsEquivalentTo(new object[] { 1, 2 });
        await Assert.That(leftover).IsNotNull();
    }

    [Test]
    [AutoArguments<int[]>(new int[] { 1, 2 })]
    public async Task WhenGenericIntArray_KeepsArrayAsSingleArgument(int[] values, MyClass leftover)
    {
        await Assert.That(values).IsEquivalentTo(new[] { 1, 2 });
        await Assert.That(leftover).IsNotNull();
    }

    [Test]
    [AutoArguments<string[]>(new string[] { "a", "b" })]
    public async Task WhenGenericStringArray_KeepsArrayAsSingleArgument(string[] values, MyClass leftover)
    {
        await Assert.That(values).IsEquivalentTo(new[] { "a", "b" });
        await Assert.That(leftover).IsNotNull();
    }
}
