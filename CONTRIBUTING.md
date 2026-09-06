# How to contribute to AutoFixture.TUnit

AutoFixture.TUnit is developed in modern C# on .NET using [TUnit](https://github.com/thomhurst/TUnit) as the unit testing framework.
The library targets `netstandard2.0`, `net8.0`, and `net10.0`. Development uses the .NET SDK version pinned in `global.json` (currently .NET 10 with latest-minor roll-forward).

So far, development has been done with a strong focus on automated tests, and the aim is to keep it that way.

## Code of Conduct

This project follows the [AutoFixture Code of Conduct](https://github.com/AutoFixture/AutoFixture/blob/master/CODE_OF_CONDUCT.md). It applies to all interactions within the project.

## Issues

Use [Issues](https://github.com/AutoFixture/AutoFixture.TUnit/issues) for confirmed bugs, tasks, and vulnerability reports.
For questions and feature ideas, prefer starting a discussion with maintainers before a large pull request.

When opening a new issue, follow the instructions in the issue template. Do not alter the template structure.

## Build

AutoFixture.TUnit uses [NUKE](https://nuke.build/) as a build engine. From the repository root, run:

```sh
./build.cmd
```

On Windows PowerShell you can also run `.\build.ps1`.

The repository state (last tag and commits since that tag) determines the build version via [GitVersion](https://gitversion.net/).

Use `./build.cmd --help` for supported parameters and targets.

## Verification

The Verify configuration treats analyzer warnings as errors for the product library.
Test projects do not use StyleCop; prefer `.editorconfig` and light analyzers there.

Before opening a PR, run the following from the repository root.
This verifies coding rules, runs the full test suite, and writes an HTML coverage report under `./artifacts/reports/`:

```sh
./build.cmd Verify Cover --no-logo --configuration Release
```

## Pull requests

When contributing, follow the coding style already present in the repository.
Keep line lengths under 120 characters when practical so reviews stay readable on GitHub.

Please follow common [Open Source Contribution Etiquette](http://tirania.org/blog/archive/2010/Dec-31.html): fork the repo, keep pull requests focused on one change, and be ready to iterate on review feedback.

For larger ideas, open an issue first so maintainers can confirm direction.

## Versioning

AutoFixture.TUnit follows [Semantic Versioning 2.0.0](https://semver.org/spec/v2.0.0.html) for public releases on [nuget.org](https://www.nuget.org/).

## Continuous Integration

CI runs on [GitHub Actions](https://github.com/AutoFixture/AutoFixture.TUnit/actions) for pull requests and releases.
The Nuke build typically:

1. Compiles the solution
2. Runs static analysis (Verify)
3. Runs tests with coverage (Cover)
4. Packs NuGet packages (and publishes on tagged releases)
