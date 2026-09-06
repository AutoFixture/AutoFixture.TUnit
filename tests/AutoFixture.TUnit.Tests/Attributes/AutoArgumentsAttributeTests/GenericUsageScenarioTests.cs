using AutoFixture.TUnit.Tests.TestTypes;

namespace AutoFixture.TUnit.Tests.Attributes.AutoArgumentsAttributeTests;

/// <summary>
/// End-to-end [AutoArguments&lt;T&gt;] usage parallel to the non-generic scenarios.
/// </summary>
public class GenericUsageScenarioTests
{
    [Test]
    [AutoArguments<string>("foo")]
    public async Task WhenGenericInlineValueProvided_UsesSuppliedValueAndFillsRemaining(string s1, string s2)
    {
        await Assert.That(s1).IsEqualTo("foo");
        await Assert.That(s2).IsNotNull();
    }

    [Test]
    [AutoArguments<string>("foo")]
    public async Task WhenGenericInlineValuePartial_SuppliesRemainingSpecimens(
        string s1, string s2, MyClass myClass)
    {
        await Assert.That(s1).IsEqualTo("foo");
        await Assert.That(s2).IsNotNull();
        await Assert.That(myClass).IsNotNull();
    }

    [Test]
    [AutoArguments<string>("foo")]
    public async Task WhenGenericInlineValuePartial_DoesNotOverwriteSuppliedValue(
        string s1, string s2, string s3)
    {
        await Assert.That(s1).IsEqualTo("foo");
        await Assert.That(s2).IsNotNull();
        await Assert.That(s3).IsNotEqualTo("foo");
    }

    [Test]
    [MyCustomAutoArguments<int>(1337)]
    public async Task WhenGenericCustomInlineAttribute_SuppliesExtraValues(int x, int y, int z)
    {
        await Assert.That(x).IsEqualTo(1337);
        await Assert.That(y).IsNotEqualTo(0);
        await Assert.That(z).IsEqualTo(42);
    }

    public class MyCustomAutoArgumentsAttribute<T> : AutoArgumentsAttribute<T>
    {
        public MyCustomAutoArgumentsAttribute(T value)
            : base(() => new Fixture().Customize(new TheAnswer()), value)
        {
        }
    }

    private class TheAnswer : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Inject(42);
        }
    }
}
