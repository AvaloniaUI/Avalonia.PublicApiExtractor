namespace Avalonia.PublicApiExtractor.Tests;

public class ProgramTests
{
    private static readonly string FixtureProject = Path.Combine(AppContext.BaseDirectory, "Fixture", "Fixture.csproj");

    private static async Task<string> GenerateAsync()
    {
        var output = Path.Combine(Path.GetTempPath(), $"public-api-{Guid.NewGuid():N}.cs");
        try
        {
            Assert.Equal(0, await Program.Main([FixtureProject, output]));
            return await File.ReadAllTextAsync(output, TestContext.Current.CancellationToken);
        }
        finally
        {
            File.Delete(output);
        }
    }

    [Fact]
    public async Task Emits_public_surface()
    {
        var api = await GenerateAsync();

        Assert.Contains("namespace Fixture", api);
        Assert.Contains("public enum Color", api);
        Assert.Contains("public partial interface IShape", api);
        Assert.Contains("public partial class Widget : IShape, IDisposable", api);
        Assert.Contains("public const string Name = @\"widget\";", api);
        Assert.Contains("public Widget(int size);", api);
        Assert.Contains("public int Size { get; set; }", api);
        Assert.Contains("public virtual void Resize(int width, int height = 10);", api);
        Assert.Contains("public static Widget operator +(Widget a, Widget b);", api);
        Assert.Contains("int IShape.Area { get; }", api);
        Assert.Contains("void IDisposable.Dispose();", api);
    }

    [Fact]
    public async Task Renders_accessors_by_visibility()
    {
        var api = await GenerateAsync();

        Assert.Contains("public int PrivateSetter { get; }", api);
        Assert.Contains("public int ProtectedSetter { get; protected set; }", api);
        Assert.Contains("public int InitOnly { get; init; }", api);
    }

    [Fact]
    public async Task Indents_delegates()
    {
        var api = await GenerateAsync();

        Assert.Contains("\n    public delegate T Factory<T>(int x) where T : class;\n", api);
        Assert.Contains("\n        public delegate void Callback();\n", api);
    }

    [Fact]
    public async Task Skips_non_public_and_removed_code()
    {
        var api = await GenerateAsync();

        Assert.DoesNotContain("InternalMethod", api);
        Assert.DoesNotContain("PrivateMethod", api);
        Assert.DoesNotContain("InternalType", api);
        Assert.DoesNotContain("ExcludedType", api);
        Assert.DoesNotContain("ExcludedNestedType", api);
    }

    [Fact]
    public async Task Returns_usage_error_for_missing_project()
    {
        Assert.Equal(2, await Program.Main([Path.Combine(AppContext.BaseDirectory, "missing.csproj")]));
    }
}
