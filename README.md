# PublicApiGenerator

Generates a C# skeleton of the public API of a project from its `.csproj`, using Roslyn. Extracted from `tools/Avalonia.DBus.PublicApiExtractor` in [Avalonia.DBus](https://github.com/AvaloniaUI/Avalonia.DBus).

## Usage

```sh
dotnet run --project src/PublicApiGenerator -- [csprojPath] [outputPath]
```

- `csprojPath`: defaults to the only `*.csproj` in the current directory.
- `outputPath`: defaults to `public-api-skeleton.cs` in the nearest `artifacts/` folder above the csproj, or next to the csproj if there is none.

To install as a global tool:

```sh
dotnet pack src/PublicApiGenerator -o artifacts/nupkg
dotnet tool install -g PublicApiGenerator --add-source artifacts/nupkg
public-api-generator path/to/Project.csproj
```
