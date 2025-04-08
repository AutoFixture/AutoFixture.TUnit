﻿using AutoFixture.Kernel;
using TestTypeFoundation;
using TUnit.Assertions.AssertConditions.Throws;

namespace AutoFixture.TUnit.Tests;

public class FavorArraysAttributeTest
{
    [Test]
    public async Task SutIsAttribute()
    {
        // Arrange
        // Act
        var sut = new FavorArraysAttribute();
        // Assert
        await Assert.That(sut).IsAssignableTo<CustomizeAttribute>();
    }

    [Test]
    public async Task GetCustomizationFromNullParameterThrows()
    {
        // Arrange
        var sut = new FavorArraysAttribute();
        // Act & assert
        await Assert.That(() =>
            sut.GetCustomization(null)).ThrowsExactly<ArgumentNullException>();
    }

    [Test]
    public async Task GetCustomizationReturnsCorrectResult()
    {
        // Arrange
        var sut = new FavorArraysAttribute();
        var parameter = typeof(TypeWithOverloadedMembers)
            .GetMethod(nameof(TypeWithOverloadedMembers.DoSomething), [typeof(object)])!
            .GetParameters().Single();
        // Act
        var result = sut.GetCustomization(parameter);
        // Assert
        var invoker = await Assert.That(result).IsAssignableTo<ConstructorCustomization>();
        await Assert.That(invoker.TargetType).IsEqualTo(parameter.ParameterType);
        await Assert.That(invoker.Query).IsAssignableTo<ArrayFavoringConstructorQuery>();
    }
}