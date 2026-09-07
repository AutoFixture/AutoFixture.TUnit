using AutoFixture.TUnit.Tests.TestTypes;

namespace AutoFixture.TUnit.Tests.Attributes.CompositeDataSourceAttributeTests;

/// <summary>
/// End-to-end [CompositeDataSource] usage via the public attribute API.
/// Partial child sources are expressed as BaseDataSourceAttribute derivatives —
/// the supported extension point for custom data sources.
/// </summary>
public class UsageScenarioTests
{
    [Test, LeadingAndTrailingComposite]
    public async Task WhenCompositeZipsPartialRows_MergesByCollapse(string a, int b, string c)
    {
        await Assert.That<(string, int, string)[]>([
            ("alpha", 1, "tail-a"),
            ("beta", 2, "tail-b")
        ]).Contains((a, b, c));
    }

    [Test, ArgumentsThenMemberComposite]
    public async Task WhenCompositeCombinesAutoSources_RunsWithFirstSourceValues(
        int id, string name, MyClass leftover)
    {
        await Assert.That(id).IsEqualTo(100);
        await Assert.That(name).IsEqualTo("fixed-name");
        await Assert.That(leftover).IsNotNull();
    }

    public class LeadingAndTrailingCompositeAttribute : CompositeDataSourceAttribute
    {
        public LeadingAndTrailingCompositeAttribute()
            : base(
                new FixedRowsAttribute(["alpha", 1], ["beta", 2]),
                new FixedRowsAttribute(["x", 0, "tail-a"], ["y", 0, "tail-b"]))
        {
        }
    }

    public class ArgumentsThenMemberCompositeAttribute : CompositeDataSourceAttribute
    {
        public ArgumentsThenMemberCompositeAttribute()
            : base(
                new AutoArgumentsAttribute(100, "fixed-name"),
                new AutoMemberDataSourceAttribute(nameof(SecondaryRows)))
        {
        }
    }

    public static IEnumerable<object[]> SecondaryRows
    {
        get
        {
            yield return ["ignored-when-first-source-fills-all"];
            yield return ["also-ignored"];
        }
    }

    /// <summary>
    /// Yields fixed rows without AutoFixture fill — same public extension model users get
    /// by deriving <see cref="BaseDataSourceAttribute"/>.
    /// </summary>
    public class FixedRowsAttribute : BaseDataSourceAttribute
    {
        private readonly object?[][] rows;

        public FixedRowsAttribute(params object?[][] rows)
        {
            this.rows = rows;
        }

#pragma warning disable CS1998
        public override async IAsyncEnumerable<Func<Task<object?[]?>>> GetData(
            DataGeneratorMetadata dataGeneratorMetadata)
        {
            foreach (var row in this.rows)
            {
                var captured = row;
                yield return () => Task.FromResult<object?[]?>(captured);
            }
        }
#pragma warning restore CS1998
    }
}
