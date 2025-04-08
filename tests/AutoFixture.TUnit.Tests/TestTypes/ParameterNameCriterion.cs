using System.Reflection;
using AutoFixture.Kernel;

namespace AutoFixture.TUnit.Tests.TestTypes;

internal class ParameterNameCriterion(IEquatable<string> nameCriterion) : IEquatable<ParameterInfo>
{
    public ParameterNameCriterion(string name)
        : this(new Criterion<string>(name, StringComparer.Ordinal))
    {
    }

    /// <summary>
    /// The name criterion originally passed in via the class' constructor.
    /// </summary>
    public IEquatable<string> NameCriterion { get; } = nameCriterion ?? throw new ArgumentNullException(nameof(nameCriterion));

    public bool Equals(ParameterInfo other)
    {
        return this.NameCriterion.Equals(other.Name);
    }
}