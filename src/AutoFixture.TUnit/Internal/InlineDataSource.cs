using System.Diagnostics.CodeAnalysis;

namespace AutoFixture.TUnit.Internal;

/// <summary>
/// Provides test data from a predefined collection of values.
/// </summary>
[SuppressMessage("Design", "CA1010:Generic interface should also be implemented",
    Justification = "Type is not a collection.")]
public sealed class InlineDataSource : AutoDataSourceAttribute
{
    private readonly object?[] values;

    /// <summary>
    /// Creates an instance of type <see cref="InlineDataSource" />.
    /// </summary>
    /// <param name="values">The collection of inline values.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the values collection is <see langword="null" />.
    /// </exception>
    public InlineDataSource(object?[] values)
    {
        this.values = values ?? throw new ArgumentNullException(nameof(values));
    }

    /// <summary>
    /// The collection of inline values.
    /// </summary>
    public IReadOnlyList<object?> Values => Array.AsReadOnly(this.values);

    /// <inheritdoc />
    public override IEnumerable<object?[]> GetData(DataGeneratorMetadata dataGeneratorMetadata)
    {
        if (dataGeneratorMetadata is null)
        {
            throw new ArgumentNullException(nameof(dataGeneratorMetadata));
        }

        var membersToGenerate = dataGeneratorMetadata.MembersToGenerate;
        if (this.values.Length > membersToGenerate.Length)
        {
            throw new InvalidOperationException(
                "The number of arguments provided exceeds the number of parameters.");
        }

        yield return this.values;
    }
}