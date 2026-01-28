using System.Diagnostics.CodeAnalysis;
using AutoFixture.TUnit.Internal;

namespace AutoFixture.TUnit;

/// <summary>
/// An implementation of DataAttribute that composes other DataAttribute instances.
/// </summary>
[SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes",
    Justification = "This attribute is the root of a potential attribute hierarchy.")]
public class CompositeDataSourceAttribute : BaseDataSourceAttribute
{
    private readonly BaseDataSourceAttribute[] attributes;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeDataSourceAttribute"/> class.
    /// </summary>
    /// <param name="attributes">The attributes representing a data source for a data theory.</param>
    public CompositeDataSourceAttribute(IEnumerable<BaseDataSourceAttribute> attributes)
        : this(attributes as BaseDataSourceAttribute[] ?? attributes.ToArray())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeDataSourceAttribute"/> class.
    /// </summary>
    /// <param name="attributes">The attributes representing a data source for a data theory.</param>
    public CompositeDataSourceAttribute(params BaseDataSourceAttribute[] attributes)
    {
        this.attributes = attributes ?? throw new ArgumentNullException(nameof(attributes));
    }

    /// <summary>
    /// Gets the attributes supplied through one of the constructors.
    /// </summary>
    public IReadOnlyList<BaseDataSourceAttribute> Attributes => Array.AsReadOnly(this.attributes);

    /// <inheritdoc />
    public override IEnumerable<object?[]> GetData(DataGeneratorMetadata dataGeneratorMetadata)
    {
        if (dataGeneratorMetadata is null)
        {
            throw new ArgumentNullException(nameof(dataGeneratorMetadata));
        }

        var results = this.attributes
            .Select(attr => attr.GetDataSources(dataGeneratorMetadata))
            .ToArray();

        var theoryRows = results
            .Select(x => x.Select(y => y()))
            .Zip(dataSets => dataSets.Collapse().ToArray())
            .ToArray();

        return theoryRows;
    }
}