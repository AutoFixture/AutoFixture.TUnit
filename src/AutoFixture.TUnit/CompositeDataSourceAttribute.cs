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
    public override async IAsyncEnumerable<Func<Task<object?[]?>>> GetData(DataGeneratorMetadata dataGeneratorMetadata)
    {
        if (dataGeneratorMetadata is null)
        {
            throw new ArgumentNullException(nameof(dataGeneratorMetadata));
        }

        var results = await Task.WhenAll(this.attributes
            .Select(async attr =>
            {
                var rows = new List<object?[]>();
                await foreach (var rowFunc in attr.GetDataRowsAsync(dataGeneratorMetadata))
                {
                    var row = await rowFunc()
                        ?? throw new InvalidOperationException("The source member yielded a null test data.");
                    rows.Add(row);
                }

                return (IEnumerable<object?[]>)rows;
            }));

        var theoryRows = results
            .Zip(dataSets => dataSets.Collapse().ToArray());

        foreach (var row in theoryRows)
        {
            yield return () => Task.FromResult<object?[]?>(row);
        }
    }
}