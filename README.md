# GitIntegration

GitIntegration is a small .NET Core sample demonstrating how to gather Git repository information from C# using command line tooling. It was originally created for BuildMaster integration tests.

## Prerequisites

- [.NET Core SDK 1.1](https://dotnet.microsoft.com/download/dotnet-core/1.1) (version specified in `global.json`)
- Git command line installed and available on your `PATH`

## Building the project

Restore NuGet packages and build the solution:

```bash
cd src/GitIntegration
 dotnet restore
 dotnet build
```

## Running

You can execute the console application from the `src/GitIntegration` directory:

```bash
 dotnet run
```

The program enumerates commits in the target repository and writes the output to both the console and log files under `c:/temp/Logs`.

## Tests

Integration tests reside under `test/IntegrationTest` and use xUnit. To run them:

```bash
cd test/IntegrationTest
 dotnet restore
 dotnet test
```

## Repository Structure

- `src/GitIntegration` – application source
- `test/IntegrationTest` – xUnit tests
- `global.json` – specifies the expected .NET Core SDK version

## License

This project is provided for demonstration purposes and does not include an explicit license.
