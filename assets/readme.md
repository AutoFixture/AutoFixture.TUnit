# AutoFixture.TUnit

AutoFixture.TUnit integrates [AutoFixture](https://github.com/AutoFixture/AutoFixture) with [TUnit](https://github.com/thomhurst/TUnit). Use data-source attributes to generate arguments declaratively so tests stay focused on behavior, not setup.

Full documentation, data-shape reference, and contributing guide: [github.com/AutoFixture/AutoFixture.TUnit](https://github.com/AutoFixture/AutoFixture.TUnit).

## Install

```bash
dotnet add package AutoFixture.TUnit
```

## Quick start

`[AutoDataSource]` fills every parameter with AutoFixture-generated values:

```csharp
using TUnit;
using TUnit.Assertions;
using AutoFixture.TUnit;

public class Calculator
{
    public int Add(int a, int b) => a + b;
}

public class CalculatorTests
{
    [Test, AutoDataSource]
    public async Task Add_SimpleValues_ReturnsCorrectResult(
        Calculator calculator, int a, int b)
    {
        int result = calculator.Add(a, b);

        await Assert.That(result).IsEqualTo(a + b);
    }
}
```

## Inline values

`[AutoArguments]` supplies some arguments; AutoFixture fills the rest:

```csharp
[Test]
[AutoArguments(5, 8)]
public async Task Add_SpecificValues_ReturnsCorrectResult(
    int a, int b, Calculator calculator)
{
    await Assert.That(calculator.Add(a, b)).IsEqualTo(13);
}
```

Prefer `[AutoArguments<T>]` (C# 11+) when you need a strongly typed single value, especially arrays that must stay one parameter:

```csharp
[Test, AutoArguments<int[]>(new int[] { 1, 2 })]
public async Task Add_GenericArrayArgument_KeepsArrayAsOneParameter(
    int[] values, Calculator calculator)
{
    await Assert.That(values).IsEquivalentTo([1, 2]);
    await Assert.That(calculator).IsNotNull();
}
```

## Freeze shared instances

Use `[Frozen]` when several parameters should receive the same instance:

```csharp
public class Dependency { }

public class Consumer
{
    public Dependency Dependency { get; }
    public Consumer(Dependency dependency) => Dependency = dependency;
}

public class ConsumerTests
{
    [Test, AutoDataSource]
    public async Task Consumer_UsesSameDependency(
        [Frozen] Dependency dependency, Consumer consumer)
    {
        await Assert.That(consumer.Dependency).IsSameReferenceAs(dependency);
    }
}
```

## Main attributes

| Attribute | Role |
| --- | --- |
| `[AutoDataSource]` | Generate all parameters |
| `[AutoArguments]` / `[AutoArguments<T>]` | Mix inline values with generated data |
| `[AutoMemberDataSource]` / `[AutoMemberDataSource<T>]` | Rows from a static member; remaining parameters are generated |
| `[AutoClassDataSource]` / `[AutoClassDataSource<T>]` | Rows from a provider type; remaining parameters are generated |
| `[CompositeDataSource]` | Compose multiple data-source attributes |
| `[Frozen]`, `[Greedy]`, `[Modest]`, `[NoAutoProperties]`, `[FavorArrays]`, `[FavorEnumerables]`, `[FavorLists]` | Parameter / construction customizations |

Member and class sources accept the same result shapes as TUnit method data sources (`object[]` rows, tuples, scalars, `Task<T>`, `IAsyncEnumerable<T>`, and more). See the [repository README](https://github.com/AutoFixture/AutoFixture.TUnit#member-and-class-data-sources) for the full shape table and examples.

## License

AutoFixture.TUnit is released under the [MIT license](https://github.com/AutoFixture/AutoFixture.TUnit/blob/master/LICENCE.txt).
