using AutoFixture.TUnit.Internal;

namespace AutoFixture.TUnit.Tests.TestTypes;

public class DelegatingMemberDataSource(Type type, string name, params object?[] arguments)
    : MemberDataSource(type, name, arguments)
{
    public DataSource GetSource() => Source;
}