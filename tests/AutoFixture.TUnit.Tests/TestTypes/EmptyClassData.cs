using System.Collections;

namespace AutoFixture.TUnit.Tests.TestTypes;

public class EmptyClassData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield break;
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}