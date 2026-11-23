namespace Avalonia.PublicApiExtractor.Tests;

public class ProgramTests
{
    private static readonly string FixtureProject = Path.Combine(AppContext.BaseDirectory, "Fixture", "Fixture.csproj");

    private static async Task<string> GenerateAsync(params string[] options)
    {
        var output = Path.Combine(Path.GetTempPath(), $"public-api-{Guid.NewGuid():N}.cs");
        try
        {
            Assert.Equal(0, await Program.Main([.. options, FixtureProject, output]));
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
        Assert.Contains("public void Resolve(string @ref);", api);
    }

    [Fact]
    public async Task Includes_xml_docs_by_default()
    {
        var api = await GenerateAsync();

        Assert.Contains("    /// <summary>\n    /// A widget.\n    /// </summary>\n    public partial class Widget", api);
        Assert.Contains("        /// <summary>Resizes the widget.</summary>\n        /// <param name=\"width\">New width.</param>\n        /// <param name=\"height\">New height.</param>\n        public virtual void Resize(", api);
        Assert.Contains("        /// <summary>The red one.</summary>\n        Red,", api);
    }

    [Fact]
    public async Task Omits_xml_docs_with_no_docs()
    {
        var api = await GenerateAsync("--no-docs");

        Assert.DoesNotContain("///", api);
        Assert.Contains("public virtual void Resize(int width, int height = 10);", api);
    }

    [Fact]
    public async Task Orders_properties_first_then_by_name()
    {
        var api = await GenerateAsync("--no-docs");

        AssertInOrder(api,
            "public int InitOnly { get; init; }",
            "public int PrivateSetter { get; }",
            "public int ProtectedSetter { get; protected set; }",
            "public int Size { get; set; }",
            "public Widget(int size);",
            "public const string Name = @\"widget\";",
            "public static Widget operator +(Widget a, Widget b);",
            "public virtual void Resize(int width, int height = 10);",
            "public void Resolve(string @ref);");
    }

    [Fact]
    public async Task Orders_by_signature_with_no_properties_first()
    {
        var api = await GenerateAsync("--no-docs", "--no-properties-first");

        AssertInOrder(api,
            "public Widget(int size);",
            "public const string Name = @\"widget\";",
            "public int InitOnly { get; init; }",
            "public static Widget operator +(Widget a, Widget b);",
            "public virtual void Resize(int width, int height = 10);",
            "public void Resolve(string @ref);");
    }

    private static void AssertInOrder(string text, params string[] expected)
    {
        var positions = expected.Select(e => text.IndexOf(e, StringComparison.Ordinal)).ToArray();
        Assert.All(positions, p => Assert.NotEqual(-1, p));
        Assert.Equal(positions.Order(), positions);
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
    public async Task Renders_extensions()
    {
        var api = await GenerateAsync();

        Assert.Contains("public static int Classic(this Widget widget);", api);
        Assert.Contains("        extension(Widget widget)\n        {\n            public int Doubled { get; }\n            public static Widget Create();\n            public void Grow(int by);\n        }", api);
        Assert.Contains("        extension<T>(IEnumerable<T> items)\n        where T : class\n", api);
    }

    [Fact]
    public async Task Renders_global_types_enum_defaults_and_usings()
    {
        var api = await GenerateAsync();

        Assert.DoesNotContain("<global namespace>", api);
        Assert.Contains("\npublic partial class GlobalType\n", api);
        Assert.Contains("public void Paint(Color color = Color.Green, Color fallback = (Color)7);", api);
        Assert.Contains("using System;", api);
        Assert.Contains("using System.Collections.Generic;", api);
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
