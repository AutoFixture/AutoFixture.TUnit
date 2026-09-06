using AutoFixture.TUnit.Tests.TestTypes;

namespace AutoFixture.TUnit.Tests.Attributes.AutoArgumentsAttributeTests;

public class ScenarioTests
{
    [Test]
    [AutoArguments("foo")]
    [AutoArguments("foo", "bar")]
    public async Task WhenInlineValuesProvided_UsesSuppliedValues(string s1, string s2)
    {
        await Assert.That(s1).IsEqualTo("foo");
        await Assert.That(s2).IsNotNull();
    }

    [Test]
    [AutoArguments("foo")]
    [AutoArguments("foo", "bar")]
    public async Task WhenInlineValuesPartial_SuppliesRemainingSpecimens(string s1, string s2, MyClass myClass)
    {
        await Assert.That(s1).IsEqualTo("foo");
        await Assert.That(s2).IsNotNull();
        await Assert.That(myClass).IsNotNull();
    }

    [Test]
    [AutoArguments("foo")]
    [AutoArguments("foo", "bar")]
    public async Task WhenInlineValuesPartial_DoesNotOverwriteSuppliedValues(string s1, string s2, string s3)
    {
        await Assert.That(s1).IsEqualTo("foo");
        await Assert.That(s2).IsNotNull();
        await Assert.That(s3).IsNotEqualTo("foo");
        await Assert.That(s3).IsNotEqualTo("bar");
    }

    [Test]
    [MyCustomAutoArguments(1337)]
    [MyCustomAutoArguments(1337, 7)]
    [MyCustomAutoArguments(1337, 7, 42)]
    public async Task WhenCustomInlineAttribute_SuppliesExtraValues(int x, int y, int z)
    {
        await Assert.That(x).IsEqualTo(1337);
        await Assert.That(z).IsEqualTo(42);
    }

    public class MyCustomAutoArgumentsAttribute : AutoArgumentsAttribute
    {
        public MyCustomAutoArgumentsAttribute(params object[] values)
            : base(() => new Fixture().Customize(new TheAnswer()), values)
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
