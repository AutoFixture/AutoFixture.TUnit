using System.Collections;

namespace AutoFixture.TUnit.Tests.TestTypes;

public class ClassWithNullTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return null;
        yield return null;
        yield return null;
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}