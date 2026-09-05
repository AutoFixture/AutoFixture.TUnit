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
    public AutoDataSource(Func<IFixture> createFixture, IDataSource? source = null)
    {
        this.CreateFixture = createFixture ?? throw new ArgumentNullException(nameof(createFixture));
        this.Source = source;
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
    public override IAsyncEnumerable<Func<Task<object?[]?>>> GetData(DataGeneratorMetadata dataGeneratorMetadata)
    {
        return this.Source is null
            ? this.GenerateValues(dataGeneratorMetadata)
            : this.CombineValues(dataGeneratorMetadata, this.Source);
    }

    private IAsyncEnumerable<Func<Task<object?[]?>>> GenerateValues(DataGeneratorMetadata metadata)
    {
        var parameters = Array.ConvertAll(metadata.GetMethod().GetParameters(), TestParameter.From);
        var fixture = this.CreateFixture();
        return new[] { Array.ConvertAll(parameters, parameter => (object?)GenerateAutoValue(parameter, fixture)) }.ToAsyncDataSource();
    }

    private async IAsyncEnumerable<Func<Task<object?[]?>>> CombineValues(DataGeneratorMetadata metadata, IDataSource source)
    {
        var method = metadata.GetMethod();

        var parameters = Array.ConvertAll(method.GetParameters(), TestParameter.From);

        await foreach (var testDataFunc in source.GetData(metadata))
        {
            var testData = await testDataFunc();

            var customizations = parameters.Take(testData!.Length)
                .Zip(testData, (parameter, value) => new Argument(parameter, value))
                .Select(argument => argument.GetCustomization())
                .Where(x => x is not NullCustomization);

            var fixture = this.CreateFixture();

            foreach (var customization in customizations)
            {
                fixture.Customize(customization);
            }

            var missingValues = parameters.Skip(testData.Length)
                .Select(parameter => GenerateAutoValue(parameter, fixture))
                .ToArray();

            var combined = testData.Concat(missingValues).ToArray();
            yield return () => Task.FromResult<object?[]?>(combined);
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