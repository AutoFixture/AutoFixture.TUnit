using System.ComponentModel;
using Nuke.Common.Tooling;

[TypeConverter(typeof(TypeConverter<Configuration>))]
public class Configuration : Enumeration
{
    public static readonly Configuration Debug = new() { Value = nameof(Debug) };
    public static readonly Configuration Release = new() { Value = nameof(Release) };
    public static readonly Configuration Verify = new() { Value = nameof(Verify) };

    public static implicit operator string(Configuration configuration)
    {
        return configuration.Value;
    }
}
