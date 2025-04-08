using System.Collections;

namespace AutoFixture.TUnit.Tests.TestTypes;

public class ClassWithEmptyTestData : IEnumerable<object?[]>
{
    public IEnumerator<object?[]> GetEnumerator()
    {
        yield return [];
        yield return [];
        yield return [];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}