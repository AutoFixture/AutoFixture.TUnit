using System.Collections;
using AutoFixture.Kernel;
using AutoFixture.TUnit.Tests.Internal.DataGeneratorMetadataSupport;
using AutoFixture.TUnit.Tests.TestTypes;
using TestTypeFoundation;
using ConcreteType = TestTypeFoundation.ConcreteType;

namespace AutoFixture.TUnit.Tests.Attributes.AutoClassDataSourceAttributeTests;

public class ClassAutoDataSourceAttributeTests
{
    [Test]
    public void Constructor_WhenCreated_CanCreateInstance()
    {
        // Act & Assert
        _ = new AutoClassDataSourceAttribute(typeof(MixedTypeClassData));
    }

    [Test]
    public async Task Constructor_WhenCreated_IsBaseDataSourceAttribute()
    {
        // Arrange & Act
        var sut = new AutoClassDataSourceAttribute(typeof(MixedTypeClassData));

        // Assert
        await Assert.That(sut).IsAssignableTo<BaseDataSourceAttribute>();
    }

    [Test]
    public async Task Constructor_WhenSourceTypeIsNull_Throws()
    {
        // Act & Assert
        await Assert.That(() => new AutoClassDataSourceAttribute(null)).ThrowsExactly<ArgumentNullException>();
    }

    [Test]
    public async Task Constructor_WhenParameterValueIsNull_TreatsAsArrayWithNull()
    {
        // Arrange & Act
        var sut = new AutoClassDataSourceAttribute(typeof(MixedTypeClassData), null);

        // Assert
        await Assert.That(sut.Parameters).HasSingleItem()
            .And
            .IsNotNull();
    }

    [Test]
    public async Task Constructor_WhenFixtureFactoryIsNull_Throws()
    {
        // Act & Assert
        await Assert.That(() => new DerivedAutoClassDataSourceAttribute(
            fixtureFactory: null, typeof(MixedTypeClassData))).ThrowsExactly<ArgumentNullException>();
    }

    [Test]
    public async Task GetData_WhenSourceTypeIsScalar_ReturnsSingleRow()
    {
        // Arrange
        var sut = new AutoClassDataSourceAttribute(typeof(MyClass));
        var testMethod = typeof(ExampleTestClass)
            .GetMethod(nameof(ExampleTestClass.TestMethod));

        // Act
        var data = sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod))
            .Select(x => x()).ToArray();

        // Assert — scalar class instance is one supplied argument; remaining are auto-generated
        await Assert.That(data.Length).IsEqualTo(1);
        await Assert.That(data[0].Length).IsEqualTo(testMethod.GetParameters().Length);
        await Assert.That(data[0][0]).IsAssignableTo<MyClass>();
    }

    [Test]
    public async Task GetData_WhenParametersDoNotMatchConstructor_Throws()
    {
        // Arrange
        var sut = new AutoClassDataSourceAttribute(typeof(MyClass), "myString", 33, null);
        var testMethod = typeof(ExampleTestClass).GetMethod(nameof(ExampleTestClass.TestMethod));

        // Act & Assert
        await Assert.That(() => sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod))
            .Select(x => x()).ToArray()).ThrowsException();
    }

    [Test]
    public async Task GetData_WhenSourceYieldsNoResults_DoesNotThrow()
    {
        // Arrange
        var sut = new AutoClassDataSourceAttribute(typeof(EmptyClassData));
        var testMethod = typeof(ExampleTestClass).GetMethod(nameof(ExampleTestClass.TestMethod));

        // Act
        var data = sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod))
            ;

        // Assert
        await Assert.That(data).IsEmpty();
    }

    [Test]
    public async Task GetData_WhenSourceYieldsNullResults_Throws()
    {
        // Arrange
        var sut = new AutoClassDataSourceAttribute(typeof(ClassWithNullTestData));
        var testMethod = typeof(ExampleTestClass).GetMethod(nameof(ExampleTestClass.TestMethod));

        // Act & assert
        await Assert.That(() => sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod))
            .Select(x => x()).ToArray()).ThrowsException();
    }

    [Test]
    public void GetData_WhenCalled_DoesNotThrow()
    {
        // Arrange
        var sut = new AutoClassDataSourceAttribute(typeof(MixedTypeClassData));
        var testMethod = typeof(ExampleTestClass).GetMethod(nameof(ExampleTestClass.TestMethod));

        // Act & Assert
        sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod));
    }

    [Test]
    public async Task GetData_WhenCalled_ReturnsEnumerable()
    {
        // Arrange
        var sut = new AutoClassDataSourceAttribute(typeof(MixedTypeClassData));
        var testMethod = typeof(ExampleTestClass).GetMethod(nameof(ExampleTestClass.TestMethod));

        // Act
        var actual = sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod));

        // Assert
        await Assert.That(actual).IsNotNull();
    }

    [Test]
    public async Task GetData_WhenCalled_ReturnsNonEmptyEnumerable()
    {
        // Arrange
        var sut = new AutoClassDataSourceAttribute(typeof(MixedTypeClassData));
        var testMethod = typeof(ExampleTestClass).GetMethod(nameof(ExampleTestClass.TestMethod));

        // Act
        var actual = sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod));

        // Assert
        await Assert.That(actual).IsNotEmpty();
    }

    [Test]
    public async Task GetData_WhenCalled_ReturnsExpectedRowCount()
    {
        // Arrange
        var sut = new AutoClassDataSourceAttribute(typeof(MixedTypeClassData));
        var testMethod = typeof(ExampleTestClass).GetMethod(nameof(ExampleTestClass.TestMethod));

        // Act
        var actual = sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod));

        // Assert
        await Assert.That(actual).Count().IsEqualTo(5);
    }

    [Test]
    public async Task GetData_WhenDataSourceNotEnumerable_Throws()
    {
        // Arrange
        var sut = new AutoClassDataSourceAttribute(typeof(GuardedConstructorHost<object>));
        var testMethod = typeof(ExampleTestClass).GetMethod(nameof(ExampleTestClass.TestMethod));

        // Act & Assert
        await Assert.That(() => sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod))
            .Select(x => x()).ToArray()).ThrowsException();
    }

    [Test]
    public async Task GetData_WhenConstructorTypesDoNotMatch_Throws()
    {
        // Arrange
        var sut = new AutoClassDataSourceAttribute(typeof(DelegatingTestData), "myString", 33, null);
        var testMethod = typeof(ExampleTestClass).GetMethod(nameof(ExampleTestClass.TestMethod));

        // Act & Assert
        await Assert.That(() => sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod))
            .Select(x => x()).ToArray()).ThrowsException();
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
    public async Task GetData_WhenCustomizationsPresent_OrdersThem(string methodName)
    {
        // Arrange
        var method = typeof(TypeWithCustomizationAttributes)
            .GetMethod(methodName, [typeof(ConcreteType)]);
        var customizationLog = new List<ICustomization>();
        var fixture = new DelegatingFixture
        {
            OnCustomize = c => customizationLog.Add(c)
        };

        var sut = new DerivedAutoClassDataSourceAttribute(() => fixture, typeof(ClassWithEmptyTestData));

        // Act
        _ = sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(method))
            .Select(x => x())
            .ToArray();

        // Assert
        await Assert.That(customizationLog[0]).IsAssignableTo<CompositeCustomization>();

        var composite = (CompositeCustomization)customizationLog[0];

        await Assert.That(composite.Customizations.First()).IsNotTypeOf<FreezeOnMatchCustomization>();
        await Assert.That(composite.Customizations.Last()).IsAssignableTo<FreezeOnMatchCustomization>();
    }

    [Test]
    public async Task GetData_WhenCalled_ReturnsExpectedTestData()
    {
        var builder = new CompositeSpecimenBuilder(
            new FixedParameterBuilder<int>("a", 1),
            new FixedParameterBuilder<string>("b", "value"),
            new FixedParameterBuilder<EnumType>("c", EnumType.First),
            new FixedParameterBuilder<Tuple<string, int>>("d", new Tuple<string, int>("value", 1)));
        var sut = new DerivedAutoClassDataSourceAttribute(
            () => new DelegatingFixture { OnCreate = (r, c) => builder.Create(r, c) },
            typeof(MixedTypeClassData));
        var testMethod = typeof(ExampleTestClass).GetMethod(nameof(ExampleTestClass.TestMethod));
        object[][] expected =
        [
            [1, "value", EnumType.First, new Tuple<string, int>("value", 1)],
            [9, "value", EnumType.First, new Tuple<string, int>("value", 1)],
            [12, "test-12", EnumType.First, new Tuple<string, int>("value", 1)],
            [223, "test-17", EnumType.Third, new Tuple<string, int>("value", 1)],
            [-95, "test-92", EnumType.Second, new Tuple<string, int>("myValue", 5)]
        ];

        var actual = sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod))
            .Select(x => x())
            .ToArray();

        await Assert.That(actual).IsEquivalentTo(expected);
    }

    [Test]
    public async Task GetData_WhenParameterizedSource_ReturnsExpectedTestData()
    {
        var builder = new CompositeSpecimenBuilder(
            new FixedParameterBuilder<int>("a", 1),
            new FixedParameterBuilder<string>("b", "value"),
            new FixedParameterBuilder<Tuple<string, int>>("d", new Tuple<string, int>("value", 1)));
        var sut = new DerivedAutoClassDataSourceAttribute(
            () => new DelegatingFixture { OnCreate = (r, c) => builder.Create(r, c) },
            typeof(ParameterizedClassData),
            29, "myValue", EnumType.Third);
        var testMethod = typeof(ExampleTestClass).GetMethod(nameof(ExampleTestClass.TestMethod));
        object[][] expected =
        [
            [29, "myValue", EnumType.Third, new Tuple<string, int>("value", 1)],
            [29, "myValue", EnumType.Third, new Tuple<string, int>("value", 1)]
        ];

        var actual = sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod))
            .Select(x => x())
            .ToArray();

        await Assert.That(actual).IsEquivalentTo(expected);
    }

    [Test]
    public async Task WhenNullParameters_Passes()
    {
        // Arrange
        var sut = new AutoClassDataSourceAttribute(typeof(TestDataWithNullValues));
        var testMethod = typeof(ExampleTestClass<string, string, string[], RecordType<string>>)
            .GetMethod(nameof(ExampleTestClass<string, string, string[], RecordType<string>>.TestMethod));
        object[][] expected = [
            [ null, null, null, null ],
            [string.Empty, null, null, null],
            [null, "  ", null, null],
        ];

        // Act
        var actual = sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod))
            .Select(x => x())
            .ToArray();

        // Assert
        await Assert.That(actual).IsEquivalentTo(expected);
    }

    public class TestDataWithNullValues : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return [null, null, null, null];
            yield return [string.Empty, null, null, null];
            yield return [null, "  ", null, null];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
