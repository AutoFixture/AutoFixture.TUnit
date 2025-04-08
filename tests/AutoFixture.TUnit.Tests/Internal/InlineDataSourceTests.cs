﻿using AutoFixture.TUnit.Internal;
using AutoFixture.TUnit.Tests.TestTypes;
using TUnit.Assertions.AssertConditions.Throws;

namespace AutoFixture.TUnit.Tests.Internal;

public class InlineDataSourceTests
{
    [Test]
    public async Task SutIsTestDataSource()
    {
        // Arrange
        // Act
        var sut = new InlineDataSource(Array.Empty<object>());
        // Assert
        await Assert.That(sut).IsAssignableTo<IDataSource>();
    }

    [Test]
    public async Task InitializeWithNullValuesThrows()
    {
        // Arrange
        // Act & Assert
        await Assert.That(() =>
            new InlineDataSource(null)).ThrowsExactly<ArgumentNullException>();
    }

    [Test]
    public async Task ValuesIsCorrect()
    {
        // Arrange
        var expectedValues = Array.Empty<object>();
        var sut = new InlineDataSource(expectedValues);
        // Act
        var result = sut.Values;
        // Assert
        await Assert.That(result).IsEquivalentTo(expectedValues);
    }

    [Test]
    public async Task GetTestDataWithNullMethodThrows()
    {
        // Arrange
        var sut = new InlineDataSource(Array.Empty<object>());
        // Act & Assert
        await Assert.That(() =>
            sut.GenerateDataSources(null)).ThrowsExactly<ArgumentNullException>();
    }

    [Test]
    public async Task SourceThrowsWhenArgumentCountExceedParameterCount()
    {
        // Arrange
        var values = new object[] { "aloha", 42, 12.3d, "extra" };
        var sut = new InlineDataSource(values);
        var testMethod = typeof(SampleTestType)
            .GetMethod(nameof(SampleTestType.TestMethodWithMultipleParameters));

        // Act & Assert
        await Assert.That(() =>
            sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod))
                .Select(x => x()).ToArray())
        .ThrowsExactly<InvalidOperationException>();
    }

    [Test]
    public async Task ReturnsTestDataWhenArgumentCountMatchesParameterCount()
    {
        // Arrange
        var values = new object[] { "aloha", 42, 12.3d };
        var sut = new InlineDataSource(values);
        var testMethod = typeof(SampleTestType)
            .GetMethod(nameof(SampleTestType.TestMethodWithMultipleParameters));

        // Act
        var result = sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod));

        // Assert
        var testData = await Assert.That(result).HasSingleItem();
        await Assert.That(testData).IsEqualTo(values);
    }

    [Test]
    public async Task ReturnsAllArgumentsWhenArgumentCountLessThanParameterCount()
    {
        // Arrange
        var values = new object[] { "aloha", 42 };
        var sut = new InlineDataSource(values);
        var testMethod = typeof(SampleTestType)
            .GetMethod(nameof(SampleTestType.TestMethodWithMultipleParameters));

        // Act
        var result = sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod));

        // Assert
        var testData = await Assert.That(result).HasSingleItem();
        await Assert.That(testData).IsEqualTo(values);
    }
}