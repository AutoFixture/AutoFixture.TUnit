using System.Collections;
using AutoFixture.TUnit.Tests.TestTypes;
using TestTypeFoundation;

namespace AutoFixture.TUnit.Tests.Attributes.AutoClassDataSourceAttributeTests;

/// <summary>
/// End-to-end [AutoClassDataSource] usage moved from the old Scenarios bucket.
/// </summary>
public class UsageScenarioTests
{
    [Test, AutoClassDataSource(typeof(StringDataClass))]
    public async Task WhenClassSuppliesValues_UsesSuppliedValues(string s1, string s2, string s3)
    {
        await Assert.That(new[] { "foo", "dim" }).Contains(s1);
        await Assert.That(s2).IsNotEmpty();
        await Assert.That(s3).IsNotEmpty();
    }

    [Test, AutoClassDataSource(typeof(StringDataClass))]
    public async Task WhenClassValuesPartial_SuppliesRemainingSpecimens(string s1, string s2, string s3, MyClass myClass)
    {
        await Assert.That(s1).IsNotEmpty();
        await Assert.That(s2).IsNotEmpty();
        await Assert.That(s3).IsNotEmpty();
        await Assert.That(myClass).IsNotNull();
    }

    [Test, AutoClassDataSource(typeof(MixedDataClass))]
    public async Task WhenMixedTypes_SuppliesExpectedData(int p1, string p2, PropertyHolder<string> p3, MyClass myClass)
    {
        await Assert.That(p1).IsNotEqualTo(0);
        await Assert.That(p2).IsNotEmpty();
        await Assert.That(p3).IsNotNull();
        await Assert.That(p3.Property).IsNotEmpty();
        await Assert.That(myClass).IsNotNull();
    }

    [Test, AutoClassDataSource(typeof(ParameterizedDataClass), 28, "bar", 93.102)]
    public async Task WhenClassDataParameterized_ReceivesExpectedData(int p1, string p2, double p3, RecordType<double> p4)
    {
        var actual = new object[] { p1, p2, p3 };
        var expected = new object[] { 28, "bar", 93.102 };

        await Assert.That(actual).IsEquivalentTo(expected);
        await Assert.That(p4).IsNotNull();
    }

    [Test, MyCustomAutoClassDataSource(typeof(IntDataClass))]
    public async Task WhenCustomClassAttribute_SuppliesExtraValues(int x, int y, int z)
    {
        await Assert.That(x).IsEqualTo(1337);
        await Assert.That(y).IsNotEqualTo(0);
        await Assert.That(z).IsEqualTo(42);
    }

    public class StringDataClass : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return ["foo", "bar", "foobar"];
            yield return ["dim", "sum", "dimsum"];
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

    public class MixedDataClass : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return [1];
            yield return [4, "testValue"];
            yield return [20, "otherValue", new PropertyHolder<string> { Property = "testValue1" }];
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

    public class ParameterizedDataClass : IEnumerable<object[]>
    {
        private readonly int p1;
        private readonly string p2;
        private readonly double p3;

        public ParameterizedDataClass(int p1, string p2, double p3)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
        }

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return [this.p1, this.p2, this.p3];
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

    public class IntDataClass : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return [1337];
            yield return [1337, 7];
            yield return [1337, 7, 42];
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

    public class MyCustomAutoClassDataSourceAttribute : AutoClassDataSourceAttribute
    {
        public MyCustomAutoClassDataSourceAttribute(Type sourceType, params object[] parameters)
            : base(() => new Fixture().Customize(new TheAnswer()), sourceType, parameters)
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
