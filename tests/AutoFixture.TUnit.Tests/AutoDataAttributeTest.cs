﻿using AutoFixture.Kernel;
using AutoFixture.TUnit.Tests.TestTypes;
using TestTypeFoundation;
using TUnit.Assertions.AssertConditions.Throws;

namespace AutoFixture.TUnit.Tests;

public class AutoDataAttributeTest
{
    [Test]
    public async Task SutIsDataAttribute()
    {
        // Arrange & Act
        var sut = new AutoDataAttribute();

        // Assert
        await Assert.That(sut).IsAssignableTo<AutoFixtureDataSourceAttribute>();
    }

    [Test]
    public async Task InitializedWithDefaultConstructorHasCorrectFixture()
    {
        // Arrange
        var sut = new AutoDataAttribute();

        // Act
        var result = sut.FixtureFactory();

        // Assert
        await Assert.That(result).IsAssignableTo<Fixture>();
    }

    [Test]
    public async Task InitializedWithFixtureFactoryConstructorHasCorrectFixture()
    {
        // Arrange
        var fixture = new Fixture();

        // Act
        var sut = new DerivedAutoDataAttribute(() => fixture);

        // Assert
        await Assert.That(sut.FixtureFactory()).IsSameReferenceAs(fixture);
    }

    [Test]
    public async Task InitializeWithNullFixtureFactoryThrows()
    {
        // Arrange
        // Act & Assert
        await Assert.That(() =>
            new DerivedAutoDataAttribute(null)).ThrowsExactly<ArgumentNullException>();
    }

    [Test]
    public async Task DoesntActivateFixtureImmediately()
    {
        // Arrange
        var wasInvoked = false;

        // Act
        _ = new DerivedAutoDataAttribute(() =>
        {
            wasInvoked = true;
            return null;
        });

        // Assert
        await Assert.That(wasInvoked).IsFalse();
    }

    [Test]
    public async Task GetDataWithNullMethodThrows()
    {
        // Arrange
        var sut = new AutoDataAttribute();

        // Act & assert
        await Assert.That(() => sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(null!, null!))).ThrowsException();
    }

    [Test]
    public async Task GetDataReturnsCorrectResult()
    {
        // Arrange
        var method = typeof(TypeWithOverloadedMembers)
            .GetMethod("DoSomething", [typeof(object)]);
        var parameters = method!.GetParameters();
        var expectedResult = new object();

        object actualParameter = null;
        ISpecimenContext actualContext = null;
        var builder = new DelegatingSpecimenBuilder
        {
            OnCreate = (r, c) =>
            {
                actualParameter = r;
                actualContext = c;
                return expectedResult;
            }
        };
        var composer = new DelegatingFixture { OnCreate = builder.OnCreate };
        var sut = new DerivedAutoDataAttribute(() => composer);

        // Act
        var result = sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(method))
            .Select(x => x())
            .ToArray();

        // Assert
        await Assert.That(actualContext).IsNotNull();
        await Assert.That(parameters).HasSingleItem();
        await Assert.That(actualParameter).IsEqualTo(parameters[0]);
        await Assert.That(result.Single()).IsEquivalentTo(new[] { expectedResult });
    }

    [Test]
    [Arguments("CreateWithFrozenAndFavorArrays")]
    [Arguments("CreateWithFavorArraysAndFrozen")]
    [Arguments("CreateWithFrozenAndFavorEnumerables")]
    [Arguments("CreateWithFavorEnumerablesAndFrozen")]
    [Arguments("CreateWithFrozenAndFavorLists")]
    [Arguments("CreateWithFavorListsAndFrozen")]
    [Arguments("CreateWithFrozenAndGreedy")]
    [Arguments("CreateWithGreedyAndFrozen")]
    [Arguments("CreateWithFrozenAndModest")]
    [Arguments("CreateWithModestAndFrozen")]
    [Arguments("CreateWithFrozenAndNoAutoProperties")]
    [Arguments("CreateWithNoAutoPropertiesAndFrozen")]
    public async Task GetDataOrdersCustomizationAttributes(string methodName)
    {
        // Arrange
        var method = typeof(TypeWithCustomizationAttributes)
            .GetMethod(methodName, [typeof(ConcreteType)]);
        var customizationLog = new List<ICustomization>();
        var fixture = new DelegatingFixture
        {
            OnCustomize = c => customizationLog.Add(c)
        };
        var sut = new DerivedAutoDataAttribute(() => fixture);

        // Act
        _ = sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(method.DeclaringType, method.Name))
            .Select(x => x())
            .ToArray();

        // Assert
        await Assert.That(customizationLog[0]).IsAssignableTo<CompositeCustomization>();

        var composite = (CompositeCustomization)customizationLog[0];

        await Assert.That(composite.Customizations.First()).IsNotTypeOf<FreezeOnMatchCustomization>();
        await Assert.That(composite.Customizations.Last()).IsAssignableTo<FreezeOnMatchCustomization>();
    }

    [Test]
    public async Task ShouldRecognizeAttributesImplementingIParameterCustomizationSource()
    {
        // Arrange
        var method = typeof(TypeWithIParameterCustomizationSourceUsage)
            .GetMethod(nameof(TypeWithIParameterCustomizationSourceUsage.DecoratedMethod));

        var customizationLog = new List<ICustomization>();
        var fixture = new DelegatingFixture
        {
            OnCustomize = c => customizationLog.Add(c)
        };
        var sut = new DerivedAutoDataAttribute(() => fixture);

        // Act
        _ = sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(method.DeclaringType, method.Name))
            .Select(x => x())
            .ToArray();

        // Assert
        await Assert.That(customizationLog[0]).IsAssignableTo<TypeWithIParameterCustomizationSourceUsage.Customization>();
    }
}