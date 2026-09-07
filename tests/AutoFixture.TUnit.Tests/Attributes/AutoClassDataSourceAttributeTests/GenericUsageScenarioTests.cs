using System.Collections;
using AutoFixture.TUnit.Tests.TestTypes;
using TestTypeFoundation;

namespace AutoFixture.TUnit.Tests.Attributes.AutoClassDataSourceAttributeTests;

/// <summary>
/// End-to-end [AutoClassDataSource&lt;T&gt;] usage parallel to the non-generic scenarios.
/// </summary>
public class GenericUsageScenarioTests
{
    [Test, AutoClassDataSource<StringDataClass>]
    public async Task WhenGenericClassSuppliesValues_UsesSuppliedValues(string s1, string s2, string s3)
    {
        await Assert.That<string[]>(["foo", "dim"]).Contains(s1);
        await Assert.That(s2).IsNotEmpty();
        await Assert.That(s3).IsNotEmpty();
    }

    [Test, AutoClassDataSource<StringDataClass>]
    public async Task WhenGenericClassValuesPartial_SuppliesRemainingSpecimens(
        string s1, string s2, string s3, MyClass myClass)
    {
        await Assert.That(s1).IsNotEmpty();
        await Assert.That(s2).IsNotEmpty();
        await Assert.That(s3).IsNotEmpty();
        await Assert.That(myClass).IsNotNull();
    }

    [Test, AutoClassDataSource<ParameterizedDataClass>(28, "bar", 93.102)]
    public async Task WhenGenericClassDataParameterized_ReceivesExpectedData(
        int p1, string p2, double p3, RecordType<double> p4)
    {
        object[] actual = [p1, p2, p3];
        object[] expected = [28, "bar", 93.102];

        await Assert.That(actual).IsEquivalentTo(expected);
        await Assert.That(p4).IsNotNull();
    }

    [Test, MyCustomAutoClassDataSource<IntDataClass>]
    public async Task WhenGenericCustomClassAttribute_SuppliesExtraValues(int x, int y, int z)
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

    public class MyCustomAutoClassDataSourceAttribute<T> : AutoClassDataSourceAttribute<T>
    {
        public MyCustomAutoClassDataSourceAttribute(params object[] parameters)
            : base(() => new Fixture().Customize(new TheAnswer()), parameters)
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
