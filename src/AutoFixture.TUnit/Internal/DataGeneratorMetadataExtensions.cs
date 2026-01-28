using System.Reflection;
using TUnit.Core.Enums;
using TUnit.Core.Extensions;

namespace AutoFixture.TUnit.Internal;

internal static class DataGeneratorMetadataExtensions
{
    public static MethodBase GetMethod(this DataGeneratorMetadata dataGeneratorMetadata)
    {
        if (dataGeneratorMetadata.Type == DataGeneratorType.ClassParameters)
        {
            return dataGeneratorMetadata.TestInformation.Class.Type.GetConstructors().First();
        }

        return dataGeneratorMetadata.TestInformation.GetReflectionInfo();
    }
}