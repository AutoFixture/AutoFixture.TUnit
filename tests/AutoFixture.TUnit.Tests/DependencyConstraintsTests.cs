using System.Reflection;

namespace AutoFixture.TUnit.Tests;

public class DependencyConstraintsTests
{
    [Test]
    [Arguments("FakeItEasy")]
    [Arguments("Foq")]
    [Arguments("FsCheck")]
    [Arguments("Moq")]
    [Arguments("NSubstitute")]
    [Arguments("nunit.framework")]
    [Arguments("Rhino.Mocks")]
    [Arguments("Unquote")]
    [Arguments("xunit")]
    [Arguments("xunit.extensions")]
    [Arguments("TUnit.Engine")]
    [Arguments("TUnit.Assertions")]
    public async Task WhenPackageNameGiven_DoesNotReferenceAssembly(string assemblyName)
    {
        // Arrange && Act
        var typeInfo = typeof(AutoDataSourceAttribute).GetTypeInfo();
        var referencedAssemblies = typeInfo.Assembly.GetReferencedAssemblies();
        // Assert
        await Assert.That(referencedAssemblies)
            .DoesNotContain(an => an.Name == assemblyName);
    }

    [Test]
    [Arguments("FakeItEasy")]
    [Arguments("Foq")]
    [Arguments("FsCheck")]
    [Arguments("Moq")]
    [Arguments("NSubstitute")]
    [Arguments("nunit.framework")]
    [Arguments("Rhino.Mocks")]
    [Arguments("Unquote")]
    [Arguments("xunit")]
    [Arguments("xunit.extensions")]
    public async Task WhenPackageNameGiven_UnitTestsAssemblyDoesNotReference(string assemblyName)
    {
        // Arrange && Act
        var typeInfo = this.GetType().GetTypeInfo();
        var referencedAssemblies = typeInfo.Assembly.GetReferencedAssemblies();

        // Assert
        await Assert.That(referencedAssemblies)
            .DoesNotContain(an => an.Name == assemblyName);
    }
}
