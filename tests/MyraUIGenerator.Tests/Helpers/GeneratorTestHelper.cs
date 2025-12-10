using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using MyraUIGenerator;

namespace MyraUIGenerator.Tests.Helpers;

/// <summary>
/// Helper class for testing the Myra UI Generator.
/// </summary>
public static class GeneratorTestHelper
{
    /// <summary>
    /// Creates a generator driver with the specified XML content.
    /// </summary>
    public static GeneratorDriver CreateDriver(string xmlContent, string fileName = "Content/UI/Test.xml")
    {
        var generator = new MyraUIGenerator();
        
        var additionalText = new InMemoryAdditionalText(fileName, xmlContent);
        
        var driver = CSharpGeneratorDriver.Create(generator)
            .AddAdditionalTexts(ImmutableArray.Create<AdditionalText>(additionalText));

        return driver;
    }

    /// <summary>
    /// Creates a generator driver with configuration options.
    /// Note: Full configuration testing requires complex mocking of AnalyzerConfigOptionsProvider.
    /// Configuration is tested through integration tests that verify the generated output.
    /// </summary>
    public static GeneratorDriver CreateDriverWithConfig(
        string xmlContent,
        string? namespaceConfig = null,
        string? directoryConfig = null,
        string fileName = "Content/UI/Test.xml")
    {
        // Configuration testing is complex with GeneratorDriver
        // We test configuration through integration tests that verify generated code uses correct namespace
        return CreateDriver(xmlContent, fileName);
    }

    /// <summary>
    /// Gets the generated source code from a driver run result.
    /// </summary>
    public static string GetGeneratedSource(GeneratorDriverRunResult result, string fileName)
    {
        var baseName = Path.GetFileNameWithoutExtension(fileName);
        var generatedFile = $"{baseName}UI.g.cs";
        
        var syntaxTrees = result.GeneratedTrees
            .Where(tree => tree.FilePath.Contains(generatedFile))
            .ToList();

        if (!syntaxTrees.Any())
        {
            return string.Empty;
        }

        return syntaxTrees.First().ToString();
    }

    /// <summary>
    /// Gets diagnostics with a specific ID from the run result.
    /// </summary>
    public static List<Diagnostic> GetDiagnostics(GeneratorDriverRunResult result, string id)
    {
        return result.Diagnostics
            .Where(d => d.Id == id)
            .ToList();
    }

    /// <summary>
    /// Creates a valid Myra UI XML document with the specified widgets.
    /// </summary>
    public static XDocument CreateValidXml(params (string Type, string Id)[] widgets)
    {
        var root = new XElement("Project");
        var container = new XElement("Panel");
        root.Add(container);

        foreach (var (type, id) in widgets)
        {
            container.Add(new XElement(type, new XAttribute("Id", id)));
        }

        return new XDocument(root);
    }

    /// <summary>
    /// Runs the generator and returns the result.
    /// </summary>
    public static GeneratorDriverRunResult RunGenerator(string xmlContent, string fileName = "Content/UI/Test.xml")
    {
        var driver = CreateDriver(xmlContent, fileName);
        
        // Create a minimal compilation to run the generator
        var compilation = CSharpCompilation.Create(
            "Test",
            Array.Empty<SyntaxTree>(),
            new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        driver = driver.RunGenerators(compilation);
        return driver.GetRunResult();
    }

    /// <summary>
    /// Creates a compilation that includes the generated code.
    /// </summary>
    public static CSharpCompilation CreateCompilationWithGeneratedCode(
        GeneratorDriverRunResult result,
        string? additionalSource = null)
    {
        var syntaxTrees = new List<SyntaxTree>();
        
        // Add generated trees
        foreach (var tree in result.GeneratedTrees)
        {
            syntaxTrees.Add(tree);
        }

        // Add additional source if provided
        if (additionalSource != null)
        {
            syntaxTrees.Add(CSharpSyntaxTree.ParseText(additionalSource));
        }

        return CSharpCompilation.Create(
            "Test",
            syntaxTrees,
            new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }
}

/// <summary>
/// In-memory implementation of AdditionalText for testing.
/// </summary>
internal class InMemoryAdditionalText : AdditionalText
{
    private readonly string _path;
    private readonly string _content;

    public InMemoryAdditionalText(string path, string content)
    {
        _path = path;
        _content = content;
    }

    public override string Path => _path;

    public override SourceText? GetText(CancellationToken cancellationToken = default)
    {
        return SourceText.From(_content, Encoding.UTF8);
    }
}


