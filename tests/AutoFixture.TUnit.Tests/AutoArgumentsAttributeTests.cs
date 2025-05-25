using System.Reflection;
using AutoFixture.TUnit.Tests.TestTypes;
using TestTypeFoundation;

namespace AutoFixture.TUnit.Tests;

public class AutoArgumentsAttributeTests
{
    [Test]
    public async Task SutIsDataAttribute()
    {
        // Arrange & Act
        var sut = new AutoArgumentsAttribute();

        // Assert
        await Assert.That(sut).IsAssignableTo<BaseDataSourceAttribute>();
    }

    [Test]
    public async Task ValuesWillBeEmptyWhenSutIsCreatedWithDefaultConstructor()
    {
        // Arrange
        var sut = new AutoArgumentsAttribute();
        var expected = Enumerable.Empty<object>();

        // Act
        var result = sut.Values;

        // Assert
        await Assert.That(result).IsEquivalentTo(expected);
    }

    [Test]
    public async Task ValuesWillNotBeEmptyWhenSutIsCreatedWithConstructorArguments()
    {
        // Arrange
        var expectedValues = new[] { new object(), new object(), new object() };
        var sut = new AutoArgumentsAttribute(expectedValues);

        // Act
        var result = sut.Values;

        // Assert
        await Assert.That(result).IsEquivalentTo(expectedValues);
    }

    [Test]
    public async Task ValuesAreCorrectWhenConstructedWithExplicitAutoDataAttribute()
    {
        // Arrange
        var expectedValues = new[] { new object(), new object(), new object() };
        var sut = new DerivedAutoArgumentsAttribute(() => new DelegatingFixture(), expectedValues);

        // Act
        var result = sut.Values;

        // Assert
        await Assert.That(result).IsEqualTo(expectedValues);
    }

    [Test]
    public async Task DoesntActivateFixtureImmediately()
    {
        // Arrange
        var wasInvoked = false;

        // Act
        _ = new DerivedAutoArgumentsAttribute(() =>
        {
            wasInvoked = true;
            return new DelegatingFixture();
        });

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
        var customizationLog = new List<ICustomization>();
        var fixture = new DelegatingFixture
        {
            OnCustomize = c => customizationLog.Add(c)
        };
        var sut = new DerivedAutoArgumentsAttribute(() => fixture);

        // Act
        _ = sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(typeof(TypeWithCustomizationAttributes), methodName))
            .Select(x => x())
            .ToArray();

        // Assert
        await Assert.That(customizationLog[0]).IsAssignableTo<CompositeCustomization>();

        var composite = (CompositeCustomization)customizationLog[0];

        await Assert.That(composite.Customizations.First()).IsNotTypeOf<FreezeOnMatchCustomization>();
        await Assert.That(composite.Customizations.Last()).IsAssignableTo<FreezeOnMatchCustomization>();
    }

    [Test]
    [MethodDataSource(typeof(InlinePrimitiveValuesTestData), nameof(InlinePrimitiveValuesTestData.GetData))]
    [MethodDataSource(typeof(InlineFrozenValuesTestData), nameof(InlineFrozenValuesTestData.GetData))]
    public async Task ReturnsSingleTestDataWithExpectedValues(BaseDataSourceAttribute attribute, MethodInfo testMethod,
        object[] expected)
    {
        // Act
        var actual = attribute.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod.DeclaringType, testMethod.Name)).ToArray();

        // Assert
        await Assert.That(actual).HasSingleItem();
        await Assert.That(actual[0]).IsEquivalentTo(expected);
    }

    [Test]
    [AutoArguments]
    public async Task GeneratesRandomData(int a, float b, string c, decimal d)
    {
        await Assert.That(a).IsNotEqualTo(0);
        await Assert.That(b).IsNotEqualTo(0);
        await Assert.That(c).IsNotNull();
        await Assert.That(d).IsNotEqualTo(0);
    }

    [Test]
    [AutoArguments(12, 32.1f, "hello", 71.231d)]
    public async Task InlinesAllData(int a, float b, string c, decimal d)
    {
        await Assert.That(a).IsEqualTo(12);
        await Assert.That(b).IsEqualTo(32.1f);
        await Assert.That(c).IsEqualTo("hello");
        await Assert.That(d).IsEqualTo(71.231m);
    }

    [Test]
    [AutoArguments(0)]
    [AutoArguments(5)]
    [AutoArguments(-12)]
    [AutoArguments(21.3f)]
    [AutoArguments(18.7d)]
    [AutoArguments(EnumType.First)]
    [AutoArguments("Hello World")]
    [AutoArguments("\t\r\n")]
    [AutoArguments(" ")]
    [AutoArguments("")]
    [AutoArguments([null!])]
    public async Task InjectsInlineValues([Frozen] object a,
        [Frozen] PropertyHolder<object> value,
        PropertyHolder<object> frozen)
    {
        await Assert.That(value.Property).IsEqualTo(a);
        await Assert.That(value).IsSameReferenceAs(frozen);
    }
}