using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace AutoFixture.TUnit.Internal;

/// <summary>
/// Encapsulates access to a property that provides test data.
/// </summary>
[SuppressMessage("Design", "CA1010:Generic interface should also be implemented",
    Justification = "Type is not a collection.")]
public class PropertyDataSource : DataSource
{
    /// <summary>
    /// Creates an instance of type <see cref="PropertyDataSource"/>.
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public PropertyDataSource(PropertyInfo propertyInfo)
    {
        PropertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
    }

    /// <summary>
    /// Gets the source property.
    /// </summary>
    public PropertyInfo PropertyInfo { get; }

    /// <inheritdoc/>
    public override IEnumerable<object?[]> GetData(DataGeneratorMetadata dataGeneratorMetadata)
    {
        var value = PropertyInfo.GetValue(null);

        if (value is not IEnumerable<object?[]> enumerable)
        {
            throw new InvalidCastException("Member does not return an enumerable value.");
        }

        return enumerable;
    }
}