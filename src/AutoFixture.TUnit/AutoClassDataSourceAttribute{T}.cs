using System.Diagnostics.CodeAnalysis;

namespace AutoFixture.TUnit;

/// <summary>
/// Generic form of <see cref="AutoClassDataSourceAttribute"/> that takes the data provider type
/// as a type argument instead of <see cref="Type"/>.
/// </summary>
/// <typeparam name="T">
/// The data provider type that yields test rows (for example an
/// <see cref="IEnumerable{T}"/> of object arrays).
/// This is not the same as TUnit's <c>ClassDataSource&lt;T&gt;</c>, which injects an instance of
/// <typeparamref name="T"/> as a dependency.
/// </typeparam>
/// <remarks>
/// Requires C# 11 or later to use the <c>[AutoClassDataSource&lt;T&gt;]</c> attribute syntax.
/// </remarks>
[SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes",
    Justification = "This attribute is the root of a potential attribute hierarchy.")]
public class AutoClassDataSourceAttribute<T> : AutoClassDataSourceAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutoClassDataSourceAttribute{T}"/> class.
    /// </summary>
    /// <param name="parameters">The parameters passed to the data provider class constructor.</param>
    public AutoClassDataSourceAttribute(params object?[] parameters)
        : base(typeof(T), parameters)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoClassDataSourceAttribute{T}"/> class
    /// with a custom fixture factory.
    /// </summary>
    /// <param name="fixtureFactory">The fixture factory that provides missing data from <typeparamref name="T"/>.</param>
    /// <param name="parameters">The parameters passed to the data provider class constructor.</param>
    protected AutoClassDataSourceAttribute(Func<IFixture> fixtureFactory, params object?[] parameters)
        : base(fixtureFactory, typeof(T), parameters)
    {
    }
}
