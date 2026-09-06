using System.Reflection;
using TUnit.Core.Enums;
using TUnit.Core.Extensions;

namespace AutoFixture.TUnit.Internal;

internal static class DataGeneratorMetadataExtensions
{
    public static MethodBase GetMethod(this DataGeneratorMetadata dataGeneratorMetadata)
    {
        if (dataGeneratorMetadata.TestInformation is null)
        {
            throw new InvalidOperationException("Not a test method");
        }

        if (dataGeneratorMetadata.Type == DataGeneratorType.ClassParameters)
        {
            var constructors = dataGeneratorMetadata.TestInformation.Class.Type.GetConstructors();
            var members = dataGeneratorMetadata.MembersToGenerate;

            return constructors.FirstOrDefault(constructor => MatchesMembers(constructor, members))
                ?? throw new InvalidOperationException(
                    "Could not find a constructor matching the class parameters to generate.");
        }

        return dataGeneratorMetadata.TestInformation.GetReflectionInfo();
    }

    private static bool MatchesMembers(ConstructorInfo constructor, IMemberMetadata[] members)
    {
        var parameters = constructor.GetParameters();
        if (parameters.Length != members.Length)
        {
            return false;
        }

        for (var i = 0; i < parameters.Length; i++)
        {
            if (members[i] is not ParameterMetadata parameterMetadata)
            {
                return false;
            }

            if (parameters[i].ParameterType != parameterMetadata.Type
                || !string.Equals(parameters[i].Name, parameterMetadata.Name, StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }
}
