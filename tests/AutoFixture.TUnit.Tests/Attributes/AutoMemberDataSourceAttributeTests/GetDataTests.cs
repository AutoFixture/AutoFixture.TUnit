using System.Diagnostics.CodeAnalysis;
using AutoFixture.TUnit.Tests.Internal.DataGeneratorMetadataSupport;
using AutoFixture.TUnit.Tests.TestTypes;
using TestTypeFoundation;
using ConcreteType = TestTypeFoundation.ConcreteType;

namespace AutoFixture.TUnit.Tests.Attributes.AutoMemberDataSourceAttributeTests;

[SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local",
    Justification = "Using parameter for precondition checks is acceptable in assertions.")]
public class GetDataTests
{
    [Test]
    public void GetData_WhenTestMethodNull_Throws()
    {
        // Arrange
        var sut = new AutoMemberDataSourceAttribute("memberName");

        // Act & Assert
        Assert.Throws<Exception>(
            () => _ = sut.GenerateDataSources(null).Select(x => x()).ToArray());
    }

    [Test]
    public async Task GetData_WhenMemberReturnsScalar_ReturnsData()
    {
        // Arrange
        const string memberName = nameof(TestTypeWithMethodData.NonEnumerableMethod);
        var sut = new AutoMemberDataSourceAttribute(memberName);
        var method = TestTypeWithMethodData.GetMultipleValueTestMethodInfo();
        var dataGeneratorMetadata = DataGeneratorMetadataHelper
            .CreateDataGeneratorMetadata(method);

        // Act
        var result = sut.GenerateDataSources(dataGeneratorMetadata).Select(x => x()).ToArray();

        // Assert — scalar becomes one row; remaining parameters are auto-generated
        await Assert.That(result.Length).IsEqualTo(1);
        await Assert.That(result[0].Length).IsEqualTo(method.GetParameters().Length);
    }

    [Test]
    public async Task GetData_WhenMemberNotStatic_Throws()
    {
        // Arrange
        const string memberName = nameof(TestTypeWithMethodData.NonStaticSource);
        var sut = new AutoMemberDataSourceAttribute(memberName);
        var method = TestTypeWithMethodData.GetMultipleValueTestMethodInfo();
        var dataGeneratorMetadata = DataGeneratorMetadataHelper
            .CreateDataGeneratorMetadata(method);

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(
            () => sut.GenerateDataSources(dataGeneratorMetadata).ToArray());

        await Assert.That(ex.Message).Contains(memberName);
    }

    [Test]
    public async Task GetData_WhenMemberDoesNotExist_Throws()
    {
        // Arrange
        var memberName = Guid.NewGuid().ToString();
        var sut = new AutoMemberDataSourceAttribute(typeof(TestTypeWithMethodData), memberName);
        var method = TestTypeWithMethodData.GetMultipleValueTestMethodInfo();
        var dataGeneratorMetadata = DataGeneratorMetadataHelper
            .CreateDataGeneratorMetadata(method);

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(
            () => sut.GenerateDataSources(dataGeneratorMetadata).Select(x => x()).ToArray());
        await Assert.That(ex.Message).Contains(memberName);
    }

    [Test]
    public async Task Constructor_WhenCreated_DoesNotActivateFixtureImmediately()
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
        var dataGeneratorMetadata = DataGeneratorMetadataHelper
            .CreateDataGeneratorMetadata(method);
        var sut = new DerivedAutoMemberDataSourceAttribute(
            () => fixture,
            typeof(TestTypeWithMethodData),
            nameof(TestTypeWithMethodData.TestDataWithNoValues));

        // Act
        _ = sut.GenerateDataSources(dataGeneratorMetadata)
            .Select(x => x()).ToArray();

        // Assert
        await Assert.That(customizationLog[0]).IsAssignableTo<CompositeCustomization>();
        var composite = (CompositeCustomization)customizationLog[0];
        await Assert.That(composite.Customizations.First()).IsNotTypeOf<FreezeOnMatchCustomization>();
        await Assert.That(composite.Customizations.Last()).IsAssignableTo<FreezeOnMatchCustomization>();
    }

    [Test]
    public async Task GetData_WhenParameterlessMethod_GeneratesTests()
    {
        // Arrange
        const string memberName = nameof(TestTypeWithMethodData.GetSingleStringValueTestData);
        var sut = new AutoMemberDataSourceAttribute(memberName);
        var testMethod = TestTypeWithMethodData.GetSingleStringValueTestMethodInfo();
        var dataGeneratorMetadata = DataGeneratorMetadataHelper
            .CreateDataGeneratorMetadata(testMethod);
        object[][] expected = [
            [ "value-one" ],
            ["value-two"],
            ["value-three"]
        ];

        // Act
        var testData = sut.GenerateDataSources(dataGeneratorMetadata)
            .Select(x => x())
            .ToArray();

        // Assert
        await Assert.That(testData).IsEquivalentTo(expected);
    }

    [Test]
    public async Task GetData_WhenMethodWithParameter_GeneratesTests()
    {
        // Arrange
        const string memberName = nameof(TestTypeWithMethodData.GetStringTestsFromArgument);
        var sut = new AutoMemberDataSourceAttribute(memberName, "value");
        var testMethod = TestTypeWithMethodData.GetStringTestsFromArgumentMethodInfo();
        var dataGeneratorMetadata = DataGeneratorMetadataHelper
            .CreateDataGeneratorMetadata(testMethod);
        object[][] expected = [
            [ "value-one" ],
            ["value-two"],
            ["value-three"]
        ];

        // Act
        var testData = sut.GenerateDataSources(dataGeneratorMetadata)
            .Select(x => x()).ToArray();

        await Assert.That(testData).IsEquivalentTo(expected);
    }

    [Test]
    public async Task GetData_WhenMultipleParameters_GeneratesTestData()
    {
        // Arrange
        const string memberName = nameof(TestTypeWithMethodData.GetMultipleValueTestData);
        var sut = new AutoMemberDataSourceAttribute(memberName);
        var testMethod = TestTypeWithMethodData.GetMultipleValueTestMethodInfo();
        var dataGeneratorMetadata = DataGeneratorMetadataHelper
            .CreateDataGeneratorMetadata(testMethod);
        object[][] expected = [
            [ "value-one", 12, 23.3m ],
            ["value-two", 38, 12.7m],
            ["value-three", 94, 52.21m]
        ];

        // Act
        var testData = sut.GenerateDataSources(dataGeneratorMetadata)
            .Select(x => x()).ToArray();

        // Assert
        await Assert.That(testData).IsEquivalentTo(expected);
    }

    [Test]
    public async Task GetData_WhenMultipleParametersPartial_GeneratesMissingData()
    {
        // Arrange
        const string memberName = nameof(TestTypeWithMethodData.GetSingleStringValueTestData);
        var sut = new AutoMemberDataSourceAttribute(memberName);
        var testMethod = TestTypeWithMethodData.GetMultipleValueTestMethodInfo();
        var dataGeneratorMetadata = DataGeneratorMetadataHelper
            .CreateDataGeneratorMetadata(testMethod);

        // Act
        var testData = sut.GenerateDataSources(dataGeneratorMetadata)
            .Select(x => x()).ToArray();

        var arguments1 = testData[0];
        var arguments2 = testData[1];
        var arguments3 = testData[2];

        // Assert
        await Assert.That(arguments1.Length).IsEqualTo(3);
        await Assert.That((string)arguments1[0]).IsEqualTo("value-one");
        await Assert.That((int)arguments1[1]).IsNotEqualTo(0);
        await Assert.That((decimal)arguments1[2]).IsNotEqualTo(0m);

        await Assert.That(arguments2.Length).IsEqualTo(3);
        await Assert.That((string)arguments2[0]).IsEqualTo("value-two");
        await Assert.That((int)arguments2[1]).IsNotEqualTo(0);
        await Assert.That((decimal)arguments2[2]).IsNotEqualTo(0m);

        await Assert.That(arguments3.Length).IsEqualTo(3);
        await Assert.That((string)arguments3[0]).IsEqualTo("value-three");
        await Assert.That((int)arguments3[1]).IsNotEqualTo(0);
        await Assert.That((decimal)arguments3[2]).IsNotEqualTo(0m);
    }

    [Test]
    public async Task GetData_WhenInjectedParameters_GeneratesTestData()
    {
        // Arrange
        const string memberName = nameof(TestTypeWithMethodData.GetDataForTestWithFrozenParameter);
        var sut = new AutoMemberDataSourceAttribute(memberName);
        var testMethod = TestTypeWithMethodData.GetTestWithFrozenParameter();
        var dataGeneratorMetadata = DataGeneratorMetadataHelper
            .CreateDataGeneratorMetadata(testMethod);
        object[][] expected = [
            [ "value-one", "value-two", "value-two" ],
            ["value-two", "value-three", "value-three"],
            ["value-three", "value-one", "value-one"]
        ];

        // Act
        var testData = sut.GenerateDataSources(dataGeneratorMetadata)
            .Select(x => x()).ToArray();

        // Assert
        await Assert.That(testData).IsEquivalentTo(expected);
    }

    [Test]
    public async Task GetData_WhenFrozenParameters_AutoGeneratesValues()
    {
        // Arrange
        const string memberName = nameof(TestTypeWithMethodData.GetSingleStringValueTestData);
        var sut = new AutoMemberDataSourceAttribute(memberName);
        var testMethod = TestTypeWithMethodData.GetTestWithFrozenParameter();
        var dataGeneratorMetadata = DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod);

        // Act
        var testData = sut.GenerateDataSources(dataGeneratorMetadata)
            .Select(x => x()).ToArray();

        var arguments1 = testData[0];
        var arguments2 = testData[1];
        var arguments3 = testData[2];

        // Assert
        await Assert.That(arguments1.Length).IsEqualTo(3);
        await Assert.That((string)arguments1[0]).IsEqualTo("value-one");
        await Assert.That(arguments1[1].ToString()).IsNotEmpty();
        await Assert.That(arguments1[2]).IsSameReferenceAs(arguments1[1]);

        await Assert.That(arguments2.Length).IsEqualTo(3);
        await Assert.That((string)arguments2[0]).IsEqualTo("value-two");
        await Assert.That(arguments2[1].ToString()).IsNotEmpty();
        await Assert.That(arguments2[2]).IsSameReferenceAs(arguments2[1]);

        await Assert.That(arguments3.Length).IsEqualTo(3);
        await Assert.That((string)arguments3[0]).IsEqualTo("value-three");
        await Assert.That(arguments3[1].ToString()).IsNotEmpty();
        await Assert.That(arguments3[2]).IsSameReferenceAs(arguments3[1]);
    }

    [Test]
    public async Task GetData_WhenMemberInherited_SupportsMember()
    {
        // Arrange
        const string memberName = nameof(TestTypeWithMethodData.GetMultipleValueTestData);
        var sut = new AutoMemberDataSourceAttribute(memberName);
        var testMethod = ChildTestTypeMethodData.GetMultipleValueTestMethodInfo();
        object[][] expected = [
            [ "value-one", 12, 23.3m ],
            ["value-two", 38, 12.7m],
            ["value-three", 94, 52.21m]
        ];
        var dataGeneratorMetadata = DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod);

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
            yield return [null, null];
            yield return [string.Empty, null];
            yield return [" ", null];
        }
    }

    [Test]
    [AutoMemberDataSource(nameof(TestDataWithNullValues))]
    public async Task WhenMemberReturnsNullRows_ReceivesNullValues(string a, string b, PropertyHolder<string> c)
    {
        await Assert.That(string.IsNullOrWhiteSpace(a)).IsTrue();
        await Assert.That(b).IsNull();
        await Assert.That(c).IsNotNull();
    }
}
