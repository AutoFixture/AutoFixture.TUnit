using AutoFixture.Kernel;

namespace AutoFixture.TUnit.Tests.TestTypes;

internal class DelegatingSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        return this.OnCreate(request, context);
    }

    internal Func<object, ISpecimenContext, object> OnCreate { get; set; } = (_, _) => new object();
}