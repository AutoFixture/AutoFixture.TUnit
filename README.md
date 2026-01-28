# AutoFixture.TUnit

[![License](https://img.shields.io/badge/license-MIT-green)](https://raw.githubusercontent.com/AutoFixture/AutoFixture.TUnit/master/LICENCE.txt)
[![NuGet version](https://img.shields.io/nuget/v/AutoFixture.TUnit?logo=nuget)](https://www.nuget.org/packages/AutoFixture.TUnit)

[AutoFixture.TUnit](https://github.com/AutoFixture/AutoFixture.TUnit) is a .NET library that integrates [AutoFixture](https://github.com/AutoFixture/AutoFixture) with [TUnit](https://github.com/tunitframework/tunit), allowing you to effortlessly generate test data for your unit tests.
By leveraging the data generators feature of TUnit, this extension turns AutoFixture into a declarative framework for writing unit tests. In many ways it becomes a unit testing DSL (Domain Specific Language).

## Table of Contents

- [Installation](#installation)
- [Getting Started](#getting-started)
- [Features](#features)
- [Contributing](#contributing)
- [License](#license)

## Installation

AutoFixture packages are distributed via NuGet. 
To install the packages you can use the integrated package manager of your IDE, the .NET CLI, or reference the package directly in your project file.

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
- **`[AutoArguments]`** - Combines inline values with auto-generated data
- **`[AutoMemberDataSource]`** - Uses static members (properties, fields, methods) as data sources
- **`[AutoClassDataSource]`** - Uses classes implementing `IEnumerable<object[]>` as data sources
- **`[CompositeDataSource]`** - Composes multiple data source attributes together

### Parameter Customization Attributes

- **`[Frozen]`** - Freezes a parameter value so the same instance is reused (supports various matching criteria)
- **`[Greedy]`** - Uses the constructor with the most parameters
- **`[Modest]`** - Uses the constructor with the fewest parameters
- **`[NoAutoProperties]`** - Prevents auto-population of properties
- **`[FavorArrays]`** - Prefers constructors that take array parameters
- **`[FavorEnumerables]`** - Prefers constructors that take `IEnumerable<T>` parameters
- **`[FavorLists]`** - Prefers constructors that take `IList<T>` parameters

All customization attributes can be combined and support custom fixture factories for advanced scenarios.

## Contributing

Contributions are welcome! 
If you would like to contribute, please review our contributing guidelines and open an issue or pull request.

## License

AutoFixture is Open Source software and is released under the [MIT license](LICENCE.txt). 
The license allows the use of AutoFixture libraries in free and commercial applications and libraries without restrictions.
