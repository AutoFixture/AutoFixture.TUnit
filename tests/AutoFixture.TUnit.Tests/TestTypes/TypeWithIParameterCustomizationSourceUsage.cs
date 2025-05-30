﻿using System.Reflection;

namespace AutoFixture.TUnit.Tests.TestTypes;

public class TypeWithIParameterCustomizationSourceUsage
{
    public void DecoratedMethod([CustomizationSource] int arg)
    {
    }

    [AttributeUsage(AttributeTargets.All)]
    public class CustomizationSourceAttribute : Attribute, IParameterCustomizationSource
    {
        public ICustomization GetCustomization(ParameterInfo parameter)
        {
            return new Customization();
        }
    }

    public class Customization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
        }
    }
}