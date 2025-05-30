﻿namespace AutoFixture.TUnit.Internal;

internal sealed class CustomizeAttributeComparer : Comparer<IParameterCustomizationSource>
{
    public override int Compare(IParameterCustomizationSource? x, IParameterCustomizationSource? y)
    {
        return (x is FrozenAttribute, y is FrozenAttribute) switch
        {
            (true, false) => 1,
            (false, true) => -1,
            _ => 0
        };
    }
}