using System.Diagnostics.CodeAnalysis;

namespace AutoFixture.TUnit;

/// <summary>
/// Generic form of <see cref="AutoArgumentsAttribute"/> that supplies a single strongly typed inline value,
/// with remaining parameters filled by AutoFixture.
/// </summary>
/// <typeparam name="T">The type of the inline value.</typeparam>
/// <remarks>
/// Requires C# 11 or later to use the <c>[AutoArguments&lt;T&gt;]</c> attribute syntax.
/// </remarks>
[SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes",
    Justification = "This attribute is the root of a potential attribute hierarchy.")]
public class AutoArgumentsAttribute<T> : AutoArgumentsAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutoArgumentsAttribute{T}"/> class.
    /// </summary>
    /// <param name="value">The typed value to pass as the first theory argument.</param>
    public AutoArgumentsAttribute(T value)
        : base([value])
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoArgumentsAttribute{T}"/> class
    /// with a custom fixture factory.
    /// </summary>
    /// <param name="fixtureFactory">The fixture factory.</param>
    /// <param name="value">The typed value to pass as the first theory argument.</param>
    protected AutoArgumentsAttribute(Func<IFixture> fixtureFactory, T value)
        : base(fixtureFactory, [value])
    {
    }
}
