using AutoFixture.TUnit.Extensions;

namespace AutoFixture.TUnit.Internal;

/// <summary>
/// Combines the values from a source with auto-generated values.
/// </summary>
public class AutoDataSource : DataSource
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutoDataSource"/> class.
    /// </summary>
    /// <param name="createFixture">The factory method for creating a fixture.</param>
    /// <param name="source">The source of test data to combine with auto-generated values.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="createFixture"/> is <see langword="null"/>.
    /// </exception>
    public AutoDataSource(Func<IFixture> createFixture, IDataSource? source = default)
    {
        CreateFixture = createFixture ?? throw new ArgumentNullException(nameof(createFixture));
        Source = source;
    }

    /// <summary>
    /// Gets the factory method for creating a fixture.
    /// </summary>
    public Func<IFixture> CreateFixture { get; }

    /// <summary>
    /// Gets the source of test data to combine with auto-generated values.
    /// </summary>
    public IDataSource? Source { get; }

    /// <summary>
    /// Returns the combined test data provided by the source and auto-generated values.
    /// </summary>
    /// <param name="dataGeneratorMetadata">The target method for which to provide the arguments.</param>
    /// <returns>Returns a sequence of argument collections.</returns>
    public override IEnumerable<object?[]> GetData(DataGeneratorMetadata dataGeneratorMetadata)
    {
        return Source is null
            ? GenerateValues(dataGeneratorMetadata)
            : CombineValues(dataGeneratorMetadata, Source);
    }

    private IEnumerable<object?[]> GenerateValues(DataGeneratorMetadata metadata)
    {
        var parameters = Array.ConvertAll(metadata.GetMethod().GetParameters(), TestParameter.From);
        var fixture = CreateFixture();
        yield return Array.ConvertAll(parameters, parameter => GenerateAutoValue(parameter, fixture));
    }

    private IEnumerable<object?[]> CombineValues(DataGeneratorMetadata metadata, IDataSource source)
    {
        var method = metadata.GetMethod();

        var parameters = Array.ConvertAll(method.GetParameters(), TestParameter.From);

        foreach (var testData in source.GetData(metadata))
        {
            var customizations = parameters.Take(testData!.Length)
                .Zip(testData, (parameter, value) => new Argument(parameter, value))
                .Select(argument => argument.GetCustomization())
                .Where(x => x is not NullCustomization);

            var fixture = CreateFixture();

            foreach (var customization in customizations)
            {
                fixture.Customize(customization);
            }

            var missingValues = parameters.Skip(testData.Length)
                .Select(parameter => GenerateAutoValue(parameter, fixture))
                .ToArray();

            yield return testData.Concat(missingValues).ToArray();
        }
    }

    private static object GenerateAutoValue(TestParameter parameter, IFixture fixture)
    {
        var customization = parameter.GetCustomization();

        if (customization is not NullCustomization)
        {
            fixture.Customize(customization);
        }

        return fixture.Resolve(parameter.ParameterInfo);
    }
}