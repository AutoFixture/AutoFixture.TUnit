using System.Collections;

namespace AutoFixture.TUnit.Tests.TestTypes;

/// <summary>
/// Yields rows with more values than a typical one-parameter test method needs.
/// </summary>
public class ClassWithExcessTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return ["one", "two", "three"];
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}
