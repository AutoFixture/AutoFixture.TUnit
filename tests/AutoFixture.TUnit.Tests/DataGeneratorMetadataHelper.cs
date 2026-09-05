using System.Reflection;
using TUnit.Core.Enums;

namespace AutoFixture.TUnit.Tests;

public static class DataGeneratorMetadataHelper
{
    public static DataGeneratorMetadata CreateDataGeneratorMetadata(Type type, string methodName)
    {
        return CreateDataGeneratorMetadata(type.GetMethod(methodName));
    }

    public static DataGeneratorMetadata CreateDataGeneratorMetadata(MethodInfo methodInfo)
    {
        var parameters = methodInfo.GetParameters();
        var type = methodInfo.ReflectedType ?? methodInfo.DeclaringType!;
        var methodName = methodInfo.Name;

        var parameterMetadatas = parameters
            .Select(CreateParameter).ToArray();

        return new DataGeneratorMetadata
        {
            Type = DataGeneratorType.TestParameters,
            TestBuilderContext = null!,
            TestSessionId = null!,
            MembersToGenerate = parameterMetadatas
                .Cast<IMemberMetadata>().ToArray(),
            TestInformation = new MethodMetadata
            {
                Type = type,
                TypeInfo = new global::TUnit.Core.ConcreteType(type),
                Name = methodName,
                GenericTypeCount = 0,
                ReturnTypeInfo = new global::TUnit.Core.ConcreteType(typeof(void)),
                Class = new ClassMetadata
                {
                    Type = type,
                    TypeInfo = new global::TUnit.Core.ConcreteType(type),
                    Assembly = null!,
                    Name = type.Name,
                    Namespace = null!,
                    Parameters = [],
                    Properties = [],
                    Parent = null!
                },
                Parameters = parameterMetadatas,
                ReturnType = typeof(void),
            },
            TestClassInstance = null!,
            ClassInstanceArguments = []
        };
    }

    private static ParameterMetadata CreateParameter(ParameterInfo parameterInfo)
    {
        return new ParameterMetadata(parameterInfo.ParameterType)
        {
            Name = parameterInfo.Name!,
            TypeInfo = new global::TUnit.Core.ConcreteType(parameterInfo.ParameterType),
            ReflectionInfo = parameterInfo
        };
    }
}
