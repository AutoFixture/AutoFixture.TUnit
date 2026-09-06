# AutoFixture.TUnit

[![License](https://img.shields.io/badge/license-MIT-green)](https://raw.githubusercontent.com/AutoFixture/AutoFixture.TUnit/master/LICENCE.txt)
[![NuGet version](https://img.shields.io/nuget/v/AutoFixture.TUnit?logo=nuget)](https://www.nuget.org/packages/AutoFixture.TUnit)

[AutoFixture.TUnit](https://github.com/AutoFixture/AutoFixture.TUnit) is a .NET library that integrates [AutoFixture](https://github.com/AutoFixture/AutoFixture) with [TUnit](https://github.com/thomhurst/TUnit), allowing you to effortlessly generate test data for your unit tests.
By leveraging the data generators feature of TUnit, this extension turns AutoFixture into a declarative framework for writing unit tests. In many ways it becomes a unit testing DSL (Domain Specific Language).

<p align="center">
  <a href="https://autofixture.com">
    <picture>
      <source media="(prefers-color-scheme: dark)" srcset="assets/lockup-dark.svg">
      <img src="assets/lockup-light.svg" height="40" alt="AutoFixture" />
    </picture>
  </a>
</p>

## Table of Contents

- [Installation](#installation)
- [Getting Started](#getting-started)
- [Features](#features)
- [Contributing](#contributing)
- [License](#license)

## Installation

AutoFixture.TUnit is distributed via NuGet.
To install the package you can use the integrated package manager of your IDE, the .NET CLI, or reference the package directly in your project file.

```cmd
dotnet add package AutoFixture.TUnit
```

## Getting Started

### Basic Usage

`AutoFixture.TUnit` provides an `[AutoDataSource]` attribute that automatically populates test method parameters with generated data.

For example, imagine you have a simple calculator class:

```csharp
public class Calculator
{
    public int Add(int a, int b) => a + b;
}
```

You can write a test using AutoFixture to provide the input values:

```csharp
using TUnit;
using TUnit.Assertions;
using AutoFixture.TUnit;

public class CalculatorTests
{
    [Test, AutoDataSource]
    public async Task Add_SimpleValues_ReturnsCorrectResult(
        Calculator calculator, int a, int b)
    {
        // Act
        int result = calculator.Add(a, b);

        // Assert
        await Assert.That(result).IsEqualTo(a + b);
    }
}
```

### Inline Auto-Data

You can also combine auto-generated data with inline arguments using the `[AutoArguments]` attribute.
This allows you to specify some parameters while still letting AutoFixture generate the rest.

```csharp
using TUnit;
using TUnit.Assertions;
using AutoFixture.TUnit;

public class CalculatorTests
{
    [Test]
    [AutoArguments(5, 8)]
    public async Task Add_SpecificValues_ReturnsCorrectResult(
        int a, int b, Calculator calculator)
    {
        // Act
        int result = calculator.Add(a, b);

        // Assert
        await Assert.That(result).IsEqualTo(13);
    }
}
```

### Member and Class Data Sources

Use `[AutoMemberDataSource]` when some values come from a static member (property, field, or method).
Use `[AutoClassDataSource]` when values come from a separate data provider type.
Any remaining test parameters are filled by AutoFixture.

Supported member/class result shapes (same ideas as TUnit MethodDataSource):

| Result shape | How it becomes test rows |
| --- | --- |
| `IEnumerable<object[]>` / `IEnumerable<object?[]>` | Each array is one multi-column row |
| `IEnumerable<T>` (for example `IEnumerable<string>`) | Each item is one single-cell row |
| `IEnumerable<(T1, T2, ...)>` | Each tuple expands into columns |
| `(T1, T2, ...)` | One row with expanded columns |
| Scalar (`string`, `int`, custom type, …) | One single-cell row (`string` is not enumerated as chars) |
| `null` | One row with a single `null` cell |
| `Task<T>` / nested `Task`s | Awaited, then the result is interpreted as above |
| `IAsyncEnumerable<T>` | Same row rules as the sync sequences above |
| Empty sequence | No rows |

```csharp
using System.Collections;
using System.Collections.Generic;
using TUnit;
using TUnit.Assertions;
using AutoFixture.TUnit;

public class CalculatorTests
{
    // Multi-column rows
    public static IEnumerable<object[]> ObjectArrayRows =>
    [
        [2, 3],
        [10, -4]
    ];

    // Single-cell rows from a typed sequence
    public static IEnumerable<string> StringRows => ["left", "right"];

    // Tuples expand to columns
    public static IEnumerable<(int Left, int Right)> TupleRows =>
    [
        (2, 3),
        (10, -4)
    ];

    // One tuple => one row
    public static (int Left, int Right) SingleTupleRow => (5, 8);

    // Scalar => one single-cell row
    public static string StringScalar => "seed";

    // Async method (for example loading rows from a file or database)
    public static async Task<IEnumerable<object[]>> TaskObjectArrayRows()
    {
        await Task.Yield(); // stand-in for real I/O
        return [[1, 1], [7, 8]];
    }

    // Streaming rows as they become available
    public static async IAsyncEnumerable<(int Left, int Right)> AsyncTupleRows()
    {
        yield return (2, 3);
        await Task.Yield();
        yield return (10, -4);
    }

    [Test, AutoMemberDataSource(nameof(ObjectArrayRows))]
    public async Task Add_ObjectArrayRows_ReturnsExpectedTotal(
        int a, int b, Calculator calculator)
    {
        await Assert.That(calculator.Add(a, b)).IsEqualTo(a + b);
    }

    [Test, AutoMemberDataSource(nameof(TupleRows))]
    public async Task Add_TupleRows_ReturnsExpectedTotal(
        int a, int b, Calculator calculator)
    {
        await Assert.That(calculator.Add(a, b)).IsEqualTo(a + b);
    }

    [Test, AutoClassDataSource(typeof(KnownSumRows))]
    public async Task Add_ClassData_ReturnsExpectedTotal(
        int a, int b, Calculator calculator)
    {
        await Assert.That(calculator.Add(a, b)).IsEqualTo(a + b);
    }
}

public class KnownSumRows : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return [1, 1];
        yield return [7, 8];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
```

Class data providers can return the same shapes (for example `IEnumerable<object[]>`, `IEnumerable<string>`, or `IEnumerable<(string, int)>`).

Generic attribute forms (C# 11+) avoid `typeof(...)` for the provider or member host type, and strongly type a single inline argument:

```csharp
[Test, AutoMemberDataSource<CalculatorTests>(nameof(ObjectArrayRows))]
public async Task Add_GenericMember_ReturnsExpectedTotal(int a, int b, Calculator calculator)
{
    await Assert.That(calculator.Add(a, b)).IsEqualTo(a + b);
}

[Test, AutoClassDataSource<KnownSumRows>]
public async Task Add_GenericClassData_ReturnsExpectedTotal(int a, int b, Calculator calculator)
{
    await Assert.That(calculator.Add(a, b)).IsEqualTo(a + b);
}

[Test, AutoArguments<int>(2)]
public async Task Add_GenericArguments_FillsRemaining(int a, int b, Calculator calculator)
{
    await Assert.That(a).IsEqualTo(2);
    await Assert.That(calculator.Add(a, b)).IsEqualTo(a + b);
}

// Prefer AutoArguments<T[]> when the value is an array: non-generic
// [AutoArguments(new object[] { 1, 2 })] expands into separate cells because of params object?[].
[Test, AutoArguments<int[]>(new int[] { 1, 2 })]
public async Task Add_GenericArrayArgument_KeepsArrayAsOneParameter(int[] values, Calculator calculator)
{
    await Assert.That(values).IsEquivalentTo(new[] { 1, 2 });
    await Assert.That(calculator).IsNotNull();
}
```

### Freezing Dependencies

AutoFixture's `[Frozen]` attribute can be used to ensure that the same instance of a dependency is injected into multiple parameters.

For example, if you have a consumer class that depends on a shared dependency:

```csharp
public class Dependency { }

public class Consumer
{
    public Dependency Dependency { get; }

    public Consumer(Dependency dependency)
    {
        Dependency = dependency;
    }
}
```

You can freeze the Dependency so that all requests for it within the test will return the same instance:

```csharp
using TUnit;
using TUnit.Assertions;
using AutoFixture.TUnit;

public class ConsumerTests
{
    [Test, AutoDataSource]
    public async Task Consumer_UsesSameDependency(
        [Frozen] Dependency dependency, Consumer consumer)
    {
        // Assert
        await Assert.That(consumer.Dependency).IsSameReferenceAs(dependency);
    }
}
```

## Features

### Data Source Attributes

- **`[AutoDataSource]`** - Automatically generates test data for all parameters
- **`[AutoArguments]`** / **`[AutoArguments<T>]`** - Combines inline values with auto-generated data (`T` is a single strongly typed inline value; C# 11+)
- **`[AutoMemberDataSource]`** / **`[AutoMemberDataSource<T>]`** - Uses static members (properties, fields, methods) as data sources; the generic form takes the member-declaring type as `T` (C# 11+)
- **`[AutoClassDataSource]`** / **`[AutoClassDataSource<T>]`** - Uses a provider type as a data source; the generic form takes the provider type as `T` (C# 11+). This is not the same as TUnit's `[ClassDataSource<T>]`, which injects an instance of `T`
- **`[CompositeDataSource]`** - Composes multiple data source attributes together

### Parameter Customization Attributes

- **`[Frozen]`** - Freezes a parameter value so the same instance is reused (supports various matching criteria via `Matching`)
- **`[Greedy]`** - Uses the constructor with the most parameters
- **`[Modest]`** - Uses the constructor with the fewest parameters
- **`[NoAutoProperties]`** - Prevents auto-population of properties
- **`[FavorArrays]`** - Prefers constructors that take array parameters
- **`[FavorEnumerables]`** - Prefers constructors that take `IEnumerable<T>` parameters
- **`[FavorLists]`** - Prefers constructors that take `IList<T>` parameters

Customization attributes can be combined on parameters.
Data source attributes also support custom fixture factories through derived attributes for advanced scenarios.

## Contributing

Contributions are welcome!
If you would like to contribute, please review our [contributing guidelines](CONTRIBUTING.md) and open an issue or pull request.

## License

AutoFixture.TUnit is Open Source software and is released under the [MIT license](LICENCE.txt).
The license allows the use of AutoFixture.TUnit libraries in free and commercial applications and libraries without restrictions.
