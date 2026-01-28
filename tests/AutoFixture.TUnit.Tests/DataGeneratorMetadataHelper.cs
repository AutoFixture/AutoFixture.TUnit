using System.Reflection;
using TUnit.Core;
using TUnit.Core.Enums;
using TUnit.Core.Extensions;

namespace AutoFixture.TUnit.Tests;

public static class DataGeneratorMetadataHelper
{
    public static DataGeneratorMetadata CreateDataGeneratorMetadata(Type type, string methodName)
    {
        return CreateDataGeneratorMetadata(type.GetMethod(methodName));
    }

    public static DataGeneratorMetadata CreateDataGeneratorMetadata(MethodInfo methodInfo)
    {
        if (methodInfo is null)
        {
            throw new ArgumentNullException(nameof(methodInfo));
        }

        var parameters = methodInfo.GetParameters();
        var declaringType = methodInfo.ReflectedType ?? methodInfo.DeclaringType!;
        var methodName = methodInfo.Name;
        var attributes = methodInfo.GetCustomAttributes().ToArray();

        // Use reflection to find and create the required TUnit types
        var assembly = typeof(DataGeneratorMetadata).Assembly;
        
        // Find ParameterMetadata type (likely implements IMemberMetadata)
        var parameterMetadataType = assembly.GetTypes()
            .FirstOrDefault(t => (t.Name == "ParameterMetadata" || t.Name.Contains("Parameter")) && 
                                 typeof(IMemberMetadata).IsAssignableFrom(t) &&
                                 t.Namespace?.StartsWith("TUnit") == true);

        // Find MethodMetadata type
        var methodMetadataType = assembly.GetTypes()
            .FirstOrDefault(t => t.Name == "MethodMetadata" && 
                                 typeof(MethodMetadata).IsAssignableFrom(t) &&
                                 t.Namespace?.StartsWith("TUnit") == true);

        // Find ClassMetadata type
        var classMetadataType = assembly.GetTypes()
            .FirstOrDefault(t => (t.Name == "ClassMetadata" || t.Name.Contains("Class")) && 
                                 t.Namespace?.StartsWith("TUnit") == true);

        if (parameterMetadataType == null || methodMetadataType == null || classMetadataType == null)
        {
            // Fallback: try to find any type that implements IMemberMetadata
            parameterMetadataType = assembly.GetTypes()
                .FirstOrDefault(t => typeof(IMemberMetadata).IsAssignableFrom(t) && 
                                     !t.IsInterface && 
                                     !t.IsAbstract &&
                                     t.Namespace?.StartsWith("TUnit") == true);
            
            if (parameterMetadataType == null)
            {
                throw new InvalidOperationException(
                    "Could not find required TUnit types (ParameterMetadata/MethodMetadata). " +
                    "This may indicate a breaking change in TUnit 1.12 API. " +
                    "Consider updating tests to use TUnit's actual DataGeneratorMetadata instances.");
            }
        }

        // Create parameter metadata objects
        var memberMetadatas = parameters
            .Select(p => CreateParameterMetadata(parameterMetadataType, p))
            .Cast<IMemberMetadata>()
            .ToArray();

        // Create class metadata
        var classMetadata = CreateClassMetadata(classMetadataType, declaringType);

        // Create method metadata
        var methodMetadata = CreateMethodMetadata(methodMetadataType, declaringType, methodName, attributes, classMetadata, memberMetadatas);

        // Create DataGeneratorMetadata
        var metadata = new DataGeneratorMetadata
        {
            Type = DataGeneratorType.TestParameters,
            TestBuilderContext = null!,
            TestSessionId = null!,
            MembersToGenerate = memberMetadatas,
            TestInformation = methodMetadata,
            TestClassInstance = null!,
            ClassInstanceArguments = []
        };

        return metadata;
    }

    private static object CreateParameterMetadata(Type parameterMetadataType, ParameterInfo parameterInfo)
    {
        // Try constructor with ParameterType
        var constructor = parameterMetadataType.GetConstructors()
            .FirstOrDefault(c => c.GetParameters().Length == 1 && 
                                 c.GetParameters()[0].ParameterType == typeof(Type));
        
        if (constructor != null)
        {
            var instance = Activator.CreateInstance(parameterMetadataType, parameterInfo.ParameterType)!;
            SetProperty(instance, "Name", parameterInfo.Name!);
            SetProperty(instance, "Attributes", parameterInfo.GetCustomAttributes().ToArray());
            return instance;
        }

        // Fallback: try parameterless constructor
        var instance2 = Activator.CreateInstance(parameterMetadataType)!;
        SetProperty(instance2, "Type", parameterInfo.ParameterType);
        SetProperty(instance2, "Name", parameterInfo.Name!);
        SetProperty(instance2, "Attributes", parameterInfo.GetCustomAttributes().ToArray());
        return instance2;
    }

    private static object CreateClassMetadata(Type classMetadataType, Type declaringType)
    {
        var instance = Activator.CreateInstance(classMetadataType)!;
        SetProperty(instance, "Type", declaringType);
        SetProperty(instance, "Assembly", declaringType.Assembly);
        SetProperty(instance, "Attributes", declaringType.GetCustomAttributes().ToArray());
        SetProperty(instance, "Name", declaringType.Name);
        SetProperty(instance, "Namespace", declaringType.Namespace);
        return instance;
    }

    private static MethodMetadata CreateMethodMetadata(Type methodMetadataType, Type declaringType, string methodName, 
        object[] attributes, object classMetadata, IMemberMetadata[] parameters)
    {
        var instance = Activator.CreateInstance(methodMetadataType)!;
        SetProperty(instance, "Type", declaringType);
        SetProperty(instance, "Name", methodName);
        SetProperty(instance, "Attributes", attributes);
        SetProperty(instance, "Class", classMetadata);
        SetProperty(instance, "Parameters", parameters);
        SetProperty(instance, "ReturnType", typeof(void));
        return (MethodMetadata)instance;
    }

    private static void SetProperty(object obj, string propertyName, object? value)
    {
        var property = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
        if (property != null && property.CanWrite)
        {
            property.SetValue(obj, value);
        }
    }
}