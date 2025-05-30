﻿using System.Collections;

namespace AutoFixture.TUnit.Tests.TestTypes;

public class DelegatingTestData : IEnumerable<object[]>
{
    private readonly List<object[]> data;

    public DelegatingTestData(params object[][] data)
    {
        this.data = data.ToList();
    }

    public DelegatingTestData(IEnumerable<object[]> data)
    {
        this.data = data as List<object[]> ?? data.ToList();
    }

    public IEnumerator<object[]> GetEnumerator() => this.data.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}