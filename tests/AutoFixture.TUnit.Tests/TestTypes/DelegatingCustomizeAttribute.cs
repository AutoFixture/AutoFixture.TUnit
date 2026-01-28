using System.Reflection;

namespace AutoFixture.TUnit.Tests.TestTypes;

internal class DelegatingCustomizeAttribute : CustomizeAttribute
{
    public override ICustomization GetCustomization(ParameterInfo parameter)
    {
        return this.OnGetCustomization(parameter);
    }

    public Func<ParameterInfo, ICustomization> OnGetCustomization { get; set; } = p => new DelegatingCustomization();
}