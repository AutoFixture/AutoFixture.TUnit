using System.Collections;
using TestTypeFoundation;

namespace AutoFixture.TUnit.Tests.TestTypes;

public class ParameterizedClassData(int p1, string p2, EnumType p3) : IEnumerable<object?[]>
{
    public IEnumerator<object?[]> GetEnumerator()
    {
        yield return [p1, p2, p3];
        yield return [p1, p2, p3];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}