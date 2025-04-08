﻿using System.Collections;
using TestTypeFoundation;

namespace AutoFixture.TUnit.Tests.TestTypes;

public class ParameterizedClassData : IEnumerable<object[]>
{
    private readonly int p1;
    private readonly string p2;
    private readonly EnumType p3;

    public ParameterizedClassData(int p1, string p2, EnumType p3)
    {
        this.p1 = p1;
        this.p2 = p2;
        this.p3 = p3;
    }

    public IEnumerator<object[]> GetEnumerator()
    {
        yield return [this.p1, this.p2, this.p3];
        yield return [this.p1, this.p2, this.p3];
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}