using AutoFixture.TUnit.Internal;
using AutoFixture.TUnit.Tests.Internal.DataGeneratorMetadataSupport;
using AutoFixture.TUnit.Tests.TestTypes;
using PropertyDataSource = AutoFixture.TUnit.Internal.PropertyDataSource;

namespace AutoFixture.TUnit.Tests.Internal.DataSources.MemberDataSourceTests;

public class MemberDataSourceTests
{
    public static object NonTestDataField = new();
    public static object NonTestDataProperty => new();
    public static object NonTestDataMethod() => new();
    public static IEnumerable<object[]> EmptyTestDataField = Array.Empty<object[]>();
    public static IEnumerable<object[]> EmptyTestData => Array.Empty<object[]>();
    public static IEnumerable<object[]> GetEmptyTestData() => Array.Empty<object[]>();

    [Test]
    public async Task Constructor_WhenCreated_IsDataSource()
    {
        // Arrange & Act
        var sut = new MemberDataSource(
            typeof(MemberDataSourceTests),
            nameof(GetEmptyTestData));

        // Assert
        await Assert.That(sut).IsAssignableTo<IDataSource>();
    }

    [Test]
    public async Task Constructor_WhenTypeIsNull_Throws()
    {
        // Arrange
        var method = nameof(GetEmptyTestData);

        // Act & Assert
        await Assert.That(() => new MemberDataSource(null, method)).ThrowsExactly<ArgumentNullException>();
    }

    [Test]
    public async Task Constructor_WhenNameIsNull_Throws()
    {
        // Arrange
        var type = typeof(MemberDataSourceTests);

        // Act & Assert
        await Assert.That(() => new MemberDataSource(type, null)).ThrowsExactly<ArgumentNullException>();
    }

    [Test]
    public async Task Constructor_WhenArgumentsIsNull_Throws()
    {
        // Arrange
        var type = typeof(MemberDataSourceTests);
        var method = nameof(GetEmptyTestData);

        // Act & Assert
        await Assert.That(() => new MemberDataSource(type, method, null)).ThrowsExactly<ArgumentNullException>();
    }

    [Test]
    public async Task Type_WhenRead_ReturnsConstructorArgument()
    {
        // Arrange
        var type = typeof(MemberDataSourceTests);
        var method = nameof(GetEmptyTestData);

        // Act
        var sut = new MemberDataSource(type, method);

        // Assert
        await Assert.That(sut.Type).IsEqualTo(type);
        await Assert.That(sut.Name).IsEqualTo(method);
        await Assert.That(sut.Arguments).IsEmpty();
    }

    [Test]
    [Arguments(nameof(EmptyTestDataField), typeof(FieldDataSource))]
    [Arguments(nameof(EmptyTestData), typeof(PropertyDataSource))]
    [Arguments(nameof(GetEmptyTestData), typeof(MethodDataSource))]
    public async Task Source_WhenRead_ReturnsResolvedMember(string memberName, Type expectedInnerSourceType)
    {
        // Arrange
        var type = typeof(MemberDataSourceTests);

        // Act
        var sut = new DelegatingMemberDataSource(type, memberName);

        // Assert
        await Assert.That(sut.GetSource().GetType()).IsEqualTo(expectedInnerSourceType);
    }

    [Test]
    public async Task GetData_WhenSourceDoesNotExist_Throws()
    {
        // Arrange
        var type = typeof(MemberDataSourceTests);

        // Act & Assert
        await Assert.That(() => _ = new DelegatingMemberDataSource(type, "NonExistentMember")).ThrowsExactly<ArgumentException>();
    }

    [Test]
    [Arguments(nameof(NonTestDataField))]
    [Arguments(nameof(NonTestDataProperty))]
    [Arguments(nameof(NonTestDataMethod))]
    public async Task GetData_WhenSourceReturnsScalar_ReturnsSingleRow(string memberName)
    {
        // Arrange
        var type = typeof(MemberDataSourceTests);
        var sut = new MemberDataSource(type, memberName);
        var testMethod = typeof(SampleTestType)
            .GetMethod(nameof(SampleTestType.TestMethodWithReferenceTypeParameter));
        var metadata = DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod!);

        // Act
        var result = new List<object?[]?>();
        await foreach (var rowFunc in sut.GetData(metadata))
        {
            result.Add(await rowFunc());
        }

        // Assert
        await Assert.That(result.Count).IsEqualTo(1);
        await Assert.That(result[0]!.Length).IsEqualTo(1);
    }
}
