using System.Collections;

namespace AutoFixture.TUnit.Tests.TestTypes;

public class DelegatingTestData : IEnumerable<object[]>
{
    private readonly List<object[]> _data;

    public DelegatingTestData(params object[][] data)
    {
        this._data = data.ToList();
    }

    public DelegatingTestData(IEnumerable<object[]> data)
    {
        this._data = data as List<object[]> ?? data.ToList();
    }

    public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}