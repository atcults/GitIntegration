# GitIntegration

GitIntegration is a small .NET Core sample demonstrating how to gather Git repository information from C# using command line tooling. It was originally created for BuildMaster integration tests.

## Prerequisites

 - [.NET SDK 8](https://dotnet.microsoft.com/download) or later
- Git command line installed and available on your `PATH`

## Building the project

Build the solution:

```bash
dotnet build
```

## Running

You can execute the console application from the `src/GitIntegration` directory:

```bash
dotnet run --project src/GitIntegration
```

The program enumerates commits in the target repository and writes the output to the console using the built-in logging framework.

## Tests

Integration tests reside under `test/IntegrationTest` and use xUnit. To run them:

```bash
dotnet test
```

## Repository Structure

- `src/GitIntegration` – application source
- `test/IntegrationTest` – xUnit tests

## License

This project is provided for demonstration purposes and does not include an explicit license.
