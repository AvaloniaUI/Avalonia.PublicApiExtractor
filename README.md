# Avalonia.PublicApiExtractor

Generates a C# skeleton of the public API of a project from its `.csproj`, using Roslyn.

## Usage

```sh
dotnet run --project src/Avalonia.PublicApiExtractor -- [--no-docs] [--no-properties-first] [csprojPath] [outputPath]
```

- `csprojPath`: defaults to the only `*.csproj` in the current directory.
- `outputPath`: defaults to `public-api-skeleton.cs` in the nearest `artifacts/` folder above the csproj, or next to the csproj if there is none.
- `--no-docs`: omit XML doc comments, which are included by default.
- `--no-properties-first`: sort members by signature. By default, properties come first and the remaining members follow in alphabetical order by name.

Run the tests:

```sh
dotnet test --solution Avalonia.PublicApiExtractor.slnx
```

To install as a global tool:

```sh
dotnet pack src/Avalonia.PublicApiExtractor -o artifacts/nupkg
dotnet tool install -g Avalonia.PublicApiExtractor --add-source artifacts/nupkg
avalonia-public-api-extractor path/to/Project.csproj
```
