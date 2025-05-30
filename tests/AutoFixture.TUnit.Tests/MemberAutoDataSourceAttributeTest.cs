﻿using System.Diagnostics.CodeAnalysis;
using AutoFixture.TUnit.Tests.TestTypes;
using TestTypeFoundation;
using TUnit.Assertions.AssertConditions.Throws;

namespace AutoFixture.TUnit.Tests;

[SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local",
    Justification = "Using parameter for precondition checks is acceptable in assertions.")]
public class MemberAutoDataSourceAttributeTest
{
    [Test]
    public async Task SutIsDataAttribute()
    {
        // Arrange
        var memberName = Guid.NewGuid().ToString();

        // Act
        var sut = new AutoMemberDataSourceAttribute(memberName);

        // Assert
        await Assert.That(sut).IsAssignableTo<BaseDataSourceAttribute>();
    }

    [Test]
    public async Task InitializedWithMemberNameAndParameters()
    {
        // Arrange
        var memberName = Guid.NewGuid().ToString();
        var parameters = new object[] { "value-one", 3, 12.2f };

        // Act
        var sut = new AutoMemberDataSourceAttribute(memberName, parameters);

        // Assert
        await Assert.That(sut.MemberName).IsEqualTo(memberName);
        await Assert.That(sut.Parameters).IsEqualTo(parameters);
        await Assert.That(sut.MemberType).IsNull();
        await Assert.That(sut.FixtureFactory).IsNotNull();
    }

    [Test]
    public async Task InitializedWithTypeMemberNameAndParameters()
    {
        // Arrange
        var memberName = Guid.NewGuid().ToString();
        var parameters = new object[] { "value-one", 3, 12.2f };
        var testType = typeof(MemberAutoDataSourceAttributeTest);

        // Act
        var sut = new AutoMemberDataSourceAttribute(testType, memberName, parameters);

        // Assert
        await Assert.That(sut.MemberName).IsEqualTo(memberName);
        await Assert.That(sut.Parameters).IsEqualTo(parameters);
        await Assert.That(sut.MemberType).IsEqualTo(testType);
        await Assert.That(sut.FixtureFactory).IsNotNull();
    }

    [Test]
    public async Task ThrowsWhenInitializedWithNullMemberName()
    {
        // Act & Assert
        await Assert.That(() => new AutoMemberDataSourceAttribute(null!))
            .ThrowsExactly<ArgumentNullException>();
    }

    [Test]
    public async Task TreatsNullParametersAsArrayWithNullValue()
    {
        // Arrange
        var memberName = Guid.NewGuid().ToString();

        // Act
        var actual = new AutoMemberDataSourceAttribute(memberName, null!);

        // Act & Assert
        var value = await Assert.That(actual.Parameters).HasSingleItem();
        await Assert.That(value).IsNull();
    }

    [Test]
    public void DoesNotThrowWhenInitializedWithNullType()
    {
        // Arrange
        var memberName = Guid.NewGuid().ToString();

        // Act & Assert
        _ = new AutoMemberDataSourceAttribute(null!, memberName);
    }

    [Test]
    public void ThrowsWhenTestMethodNull()
    {
        // Arrange
        var sut = new AutoMemberDataSourceAttribute("memberName");

        // Act & Assert
        Assert.Throws<Exception>(
            () => _ = sut.GenerateDataSources(null!).Select(x => x()).ToArray());
    }

    [Test]
    public async Task ThrowsWhenMemberNotEnumerable()
    {
        // Arrange
        const string memberName = nameof(TestTypeWithMethodData.NonEnumerableMethod);
        var sut = new AutoMemberDataSourceAttribute(memberName);
        var method = TestTypeWithMethodData.GetNonEnumerableMethodInfo();
        var dataGeneratorMetadata = DataGeneratorMetadataHelper
            .CreateDataGeneratorMetadata(method!);

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(
            () => sut.GetData(dataGeneratorMetadata).ToArray());

        await Assert.That(ex.Message).Contains(memberName);
    }

    [Test]
    public async Task ThrowsWhenMemberNotStatic()
    {
        // Arrange
        const string memberName = nameof(TestTypeWithMethodData.NonStaticSource);
        var sut = new AutoMemberDataSourceAttribute(memberName);
        var method = TestTypeWithMethodData.GetNonStaticSourceMethodInfo();
        var dataGeneratorMetadata = DataGeneratorMetadataHelper
            .CreateDataGeneratorMetadata(method!);

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(
            () => sut.GetData(dataGeneratorMetadata).ToArray());

        await Assert.That(ex.Message).Contains(memberName);
    }

    [Test]
    public async Task ThrowsWhenMemberDoesNotExist()
    {
        // Arrange
        var memberName = Guid.NewGuid().ToString();
        var sut = new AutoMemberDataSourceAttribute(typeof(TestTypeWithMethodData), memberName);
        var method = TestTypeWithMethodData.GetMultipleValueTestMethodInfo();
        var dataGeneratorMetadata = DataGeneratorMetadataHelper
            .CreateDataGeneratorMetadata(method!);

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(
            () => sut.GenerateDataSources(dataGeneratorMetadata).Select(x => x()).ToArray());
        await Assert.That(ex.Message).Contains(memberName);
    }

    [Test]
    public async Task DoesntActivateFixtureImmediately()
    {
        // Arrange
        var memberName = Guid.NewGuid().ToString();
        var wasInvoked = false;

        // Act
        _ = new DerivedAutoMemberDataSourceAttribute(() =>
        {
            wasInvoked = true;
            return new DelegatingFixture();
        }, memberName);

        // Assert
        await Assert.That(wasInvoked).IsFalse();
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
            .GetMethod(methodName, [typeof(ConcreteType)])!;
        var customizationLog = new List<ICustomization>();
        var fixture = new DelegatingFixture
        {
            OnCustomize = c => customizationLog.Add(c)
        };
        var dataGeneratorMetadata = DataGeneratorMetadataHelper
            .CreateDataGeneratorMetadata(method!);
        var sut = new DerivedAutoMemberDataSourceAttribute(
            () => fixture,
            typeof(TestTypeWithMethodData),
            nameof(TestTypeWithMethodData.TestDataWithNoValues));

        // Act
        _ = sut.GenerateDataSources(dataGeneratorMetadata)
            .Select(x => x()).ToArray();

        // Assert
        var composite = await Assert.That(customizationLog[0]).IsAssignableTo<CompositeCustomization>();
        await Assert.That(composite.Customizations.First()).IsNotTypeOf<FreezeOnMatchCustomization>();
        await Assert.That(composite.Customizations.Last()).IsAssignableTo<FreezeOnMatchCustomization>();
    }

    [Test]
    public async Task GeneratesTestsFromParameterlessMethod()
    {
        // Arrange
        const string memberName = nameof(TestTypeWithMethodData.GetSingleStringValueTestData);
        var sut = new AutoMemberDataSourceAttribute(memberName);
        var testMethod = TestTypeWithMethodData.GetSingleStringValueTestMethodInfo();
        var dataGeneratorMetadata = DataGeneratorMetadataHelper
            .CreateDataGeneratorMetadata(testMethod!);
        var expected = new[]
        {
            new object[] { "value-one" },
            new object[] { "value-two" },
            new object[] { "value-three" }
        };

        // Act
        var testData = sut.GenerateDataSources(dataGeneratorMetadata)
            .Select(x => x())
            .ToArray();

        // Assert
        await Assert.That(testData).IsEquivalentTo(expected);
    }

    [Test]
    public async Task GeneratesTestsFromMethodWithParameter()
    {
        // Arrange
        const string memberName = nameof(TestTypeWithMethodData.GetStringTestsFromArgument);
        var sut = new AutoMemberDataSourceAttribute(memberName, "value");
        var testMethod = TestTypeWithMethodData.GetStringTestsFromArgumentMethodInfo();
        var dataGeneratorMetadata = DataGeneratorMetadataHelper
            .CreateDataGeneratorMetadata(testMethod!);
        var expected = new[]
        {
            new object[] { "value-one" },
            new object[] { "value-two" },
            new object[] { "value-three" }
        };

        // Act
        var testData = sut.GenerateDataSources(dataGeneratorMetadata)
            .Select(x => x()).ToArray();

        await Assert.That(testData).IsEquivalentTo(expected);
    }

    [Test]
    public async Task GeneratesTestDataForTestsWithMultipleParameters()
    {
        // Arrange
        const string memberName = nameof(TestTypeWithMethodData.GetMultipleValueTestData);
        var sut = new AutoMemberDataSourceAttribute(memberName);
        var testMethod = TestTypeWithMethodData.GetMultipleValueTestMethodInfo();
        var dataGeneratorMetadata = DataGeneratorMetadataHelper
            .CreateDataGeneratorMetadata(testMethod!);
        var expected = new[]
        {
            new object[] { "value-one", 12, 23.3m },
            new object[] { "value-two", 38, 12.7m },
            new object[] { "value-three", 94, 52.21m }
        };

        // Act
        var testData = sut.GenerateDataSources(dataGeneratorMetadata)
            .Select(x => x()).ToArray();

        // Assert
        await Assert.That(testData).IsEquivalentTo(expected);
    }

    [Test]
    public async Task GeneratesMissingDataForTestsWithMultipleParameters()
    {
        // Arrange
        const string memberName = nameof(TestTypeWithMethodData.GetSingleStringValueTestData);
        var sut = new AutoMemberDataSourceAttribute(memberName);
        var testMethod = TestTypeWithMethodData.GetMultipleValueTestMethodInfo();
        var dataGeneratorMetadata = DataGeneratorMetadataHelper
            .CreateDataGeneratorMetadata(testMethod!);

        // Act
        var testData = sut.GenerateDataSources(dataGeneratorMetadata)
            .Select(x => x()).ToArray();

        var arguments1 = testData[0];
        var arguments2 = testData[1];
        var arguments3 = testData[2];

        // Assert
        await Assert.That(arguments1.Length).IsEqualTo(3);
        await Assert.That(arguments1[0]).IsEqualTo("value-one");
        await Assert.That(arguments1[1]).IsNotEqualTo(0);
        await Assert.That(arguments1[2]).IsNotEqualTo(0);

        await Assert.That(arguments2.Length).IsEqualTo(3);
        await Assert.That(arguments2[0]).IsEqualTo("value-two");
        await Assert.That(arguments2[1]).IsNotEqualTo(0);
        await Assert.That(arguments2[2]).IsNotEqualTo(0);

        await Assert.That(arguments3.Length).IsEqualTo(3);
        await Assert.That(arguments3[0]).IsEqualTo("value-three");
        await Assert.That(arguments3[1]).IsNotEqualTo(0);
        await Assert.That(arguments3[2]).IsNotEqualTo(0);
    }

    [Test]
    public async Task GeneratesTestDataWithInjectedParameters()
    {
        // Arrange
        const string memberName = nameof(TestTypeWithMethodData.GetDataForTestWithFrozenParameter);
        var sut = new AutoMemberDataSourceAttribute(memberName);
        var testMethod = TestTypeWithMethodData.GetTestWithFrozenParameter();
        var dataGeneratorMetadata = DataGeneratorMetadataHelper
            .CreateDataGeneratorMetadata(testMethod!);
        var expected = new[]
        {
            new object[] { "value-one", "value-two", "value-two" },
            new object[] { "value-two", "value-three", "value-three" },
            new object[] { "value-three", "value-one", "value-one" }
        };

        // Act
        var testData = sut.GenerateDataSources(dataGeneratorMetadata)
            .Select(x => x()).ToArray();

        // Assert
        await Assert.That(testData).IsEquivalentTo(expected);
    }

    [Test]
    public async Task AutoGeneratesValuesForFrozenParameters()
    {
        // Arrange
        const string memberName = nameof(TestTypeWithMethodData.GetSingleStringValueTestData);
        var sut = new AutoMemberDataSourceAttribute(memberName);
        var testMethod = TestTypeWithMethodData.GetTestWithFrozenParameter();
        var dataGeneratorMetadata = DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod!);

        // Act
        var testData = sut.GenerateDataSources(dataGeneratorMetadata)
            .Select(x => x()).ToArray();

        var arguments1 = testData[0];
        var arguments2 = testData[1];
        var arguments3 = testData[2];

        // Assert
        await Assert.That(arguments1.Length).IsEqualTo(3);
        await Assert.That(arguments1[0]).IsEqualTo("value-one");
        await Assert.That(arguments1[1].ToString()!).IsNotEmpty();
        await Assert.That(arguments1[2]).IsEqualTo(arguments1[1]);

        await Assert.That(arguments2.Length).IsEqualTo(3);
        await Assert.That(arguments2[0]).IsEqualTo("value-two");
        await Assert.That(arguments2[1].ToString()!).IsNotEmpty();
        await Assert.That(arguments2[2]).IsEqualTo(arguments2[1]);

        await Assert.That(arguments3.Length).IsEqualTo(3);
        await Assert.That(arguments3[0]).IsEqualTo("value-three");
        await Assert.That(arguments3[1].ToString()!).IsNotEmpty();
        await Assert.That(arguments3[2]).IsEqualTo(arguments3[1]);
    }

    [Test]
    public async Task SupportsInheritedTestDataMembers()
    {
        // Arrange
        const string memberName = nameof(TestTypeWithMethodData.GetMultipleValueTestData);
        var sut = new AutoMemberDataSourceAttribute(memberName);
        var testMethod = ChildTestTypeMethodData.GetMultipleValueTestMethodInfo();
        var expected = new[]
        {
            new object[] { "value-one", 12, 23.3m },
            new object[] { "value-two", 38, 12.7m },
            new object[] { "value-three", 94, 52.21m }
        };
        var dataGeneratorMetadata = DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod!);

        // Act
        var testData = sut.GenerateDataSources(dataGeneratorMetadata)
            .Select(x => x()).ToArray();

        // Assert
        await Assert.That(testData).IsEquivalentTo(expected);
    }

    public static IEnumerable<object[]> TestDataWithNullValues
    {
        get
        {
            yield return [null!, null!];
            yield return [string.Empty, null!];
            yield return [" ", null!];
        }
    }

    [Test]
    [AutoMemberDataSource(nameof(TestDataWithNullValues))]
    public async Task NullTestDataReturned(string a, string b, PropertyHolder<string> c)
    {
        await Assert.That(string.IsNullOrWhiteSpace(a)).IsTrue();
        await Assert.That(b).IsNull();
        await Assert.That(c).IsNotNull();
    }
}