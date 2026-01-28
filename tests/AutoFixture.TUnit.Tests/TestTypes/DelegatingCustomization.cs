namespace AutoFixture.TUnit.Tests.TestTypes;

internal class DelegatingCustomization : ICustomization
{
    internal DelegatingCustomization()
    {
        this.OnCustomize = _ => { };
    }

    public void Customize(IFixture fixture)
    {
        this.OnCustomize(fixture);
    }

    internal Action<IFixture> OnCustomize { get; set; }
}