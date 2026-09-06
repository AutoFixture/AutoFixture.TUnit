using System.Diagnostics.CodeAnalysis;

namespace AutoFixture.TUnit;

/// <summary>
/// Generic form of <see cref="AutoMemberDataSourceAttribute"/> that takes the member-declaring type
/// as a type argument instead of <see cref="Type"/>.
/// </summary>
/// <typeparam name="T">The type that declares the static member providing test data.</typeparam>
/// <remarks>
/// Requires C# 11 or later to use the <c>[AutoMemberDataSource&lt;T&gt;]</c> attribute syntax.
/// </remarks>
[SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes",
    Justification = "This attribute is the root of a potential attribute hierarchy.")]
public class AutoMemberDataSourceAttribute<T> : AutoMemberDataSourceAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutoMemberDataSourceAttribute{T}"/> class.
    /// </summary>
    /// <param name="memberName">The name of the public static member on <typeparamref name="T"/> that will provide the test data.</param>
    /// <param name="parameters">The parameters for the member (only supported for methods; ignored for everything else).</param>
    public AutoMemberDataSourceAttribute(string memberName, params object?[] parameters)
        : base(typeof(T), memberName, parameters)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoMemberDataSourceAttribute{T}"/> class
    /// with a custom fixture factory.
    /// </summary>
    /// <param name="fixtureFactory">The fixture factory delegate.</param>
    /// <param name="memberName">The name of the public static member on <typeparamref name="T"/> that will provide the test data.</param>
    /// <param name="parameters">The parameters for the member (only supported for methods; ignored for everything else).</param>
    protected AutoMemberDataSourceAttribute(Func<IFixture> fixtureFactory, string memberName, params object?[] parameters)
        : base(fixtureFactory, typeof(T), memberName, parameters)
    {
    }
}
