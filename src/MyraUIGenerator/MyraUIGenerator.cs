using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace MyraUIGenerator;

[Generator]
public class MyraUIGenerator : ISourceGenerator
{
    private const string DefaultNamespace = "GeneratedUI";
    private const string DefaultXmlDirectory = "Content/UI";
    private const string ConfigKeyNamespace = "myra_ui_generator.namespace";
    private const string ConfigKeyXmlDirectory = "myra_ui_generator.xml_directory";

    /// <summary>
    /// Initializes the source generator. This method is called once before generation starts.
    /// </summary>
    /// <param name="context">The generator initialization context.</param>
    public void Initialize(GeneratorInitializationContext context)
    {
        // Register for syntax receiver if needed, but for this generator we don't need it
        // The generator will process AdditionalFiles in Execute
    }

    /// <summary>
    /// Executes the source generator, processing XML files and generating C# UI accessor classes.
    /// Scans AdditionalFiles for Myra XML files and generates strongly-typed wrapper classes.
    /// </summary>
    /// <param name="context">The generator execution context containing source files and configuration.</param>
    public void Execute(GeneratorExecutionContext context)
    {
        try
        {
            // Get configuration from analyzer config options
        var namespaceName = GetConfigurationValue(context, ConfigKeyNamespace, DefaultNamespace);
        var xmlDirectory = GetConfigurationValue(context, ConfigKeyXmlDirectory, DefaultXmlDirectory);

        // Report diagnostic for debugging (use Warning so it shows up)
        var diagnostic = Diagnostic.Create(
            new DiagnosticDescriptor(
                "MYRA002",
                "Generator executing",
                "MyraUIGenerator executing. Namespace: {0}, Directory: {1}, AdditionalFiles count: {2}",
                "MyraUI",
                DiagnosticSeverity.Warning,
                true),
            Location.None,
            namespaceName,
            xmlDirectory,
            context.AdditionalFiles.Count().ToString());
        context.ReportDiagnostic(diagnostic);

        // Find all XML files - check all AdditionalFiles that end with .xml
        // The path matching is flexible to handle both relative and absolute paths
        var xmlFiles = context.AdditionalFiles
            .Where(f => 
            {
                if (!f.Path.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                    return false;
                
                var path = f.Path.Replace('\\', '/'); // Normalize path separators
                var normalizedDir = xmlDirectory.Replace('\\', '/');
                
                // Match if path contains the directory pattern (case-insensitive)
                return path.IndexOf(normalizedDir, StringComparison.OrdinalIgnoreCase) >= 0;
            })
            .ToList();

        // Report how many XML files were found
        var foundDiagnostic = Diagnostic.Create(
            new DiagnosticDescriptor(
                "MYRA003",
                "XML files found",
                "Found {0} XML files matching directory '{1}'",
                "MyraUI",
                DiagnosticSeverity.Warning,
                true),
            Location.None,
            xmlFiles.Count.ToString(),
            xmlDirectory);
        context.ReportDiagnostic(foundDiagnostic);

        foreach (var xmlFile in xmlFiles)
        {
            try
            {
                var xmlContent = xmlFile.GetText()?.ToString();
                if (string.IsNullOrEmpty(xmlContent)) continue;

                var xmlDoc = XDocument.Parse(xmlContent);
                var fileName = System.IO.Path.GetFileNameWithoutExtension(xmlFile.Path);
                
                // Extract widgets with Id attributes
                var widgets = ExtractWidgets(xmlDoc);
                
                if (widgets.Any())
                {
                    var generatedCode = GenerateUIClass(fileName, widgets, namespaceName);
                    var sourceText = SourceText.From(generatedCode, Encoding.UTF8);
                    context.AddSource($"{fileName}UI.g.cs", sourceText);
                    
                    // Report successful generation
                    var successDiagnostic = Diagnostic.Create(
                        new DiagnosticDescriptor(
                            "MYRA004",
                            "Generated UI class",
                            "Generated {0}UI with {1} widgets",
                            "MyraUI",
                            DiagnosticSeverity.Warning,
                            true),
                        Location.None,
                        fileName,
                        widgets.Count.ToString());
                    context.ReportDiagnostic(successDiagnostic);
                }
                else
                {
                    // Report if no widgets found
                    var noWidgetsDiagnostic = Diagnostic.Create(
                        new DiagnosticDescriptor(
                            "MYRA005",
                            "No widgets found",
                            "No widgets with Id found in {0}.xml",
                            "MyraUI",
                            DiagnosticSeverity.Warning,
                            true),
                        Location.None,
                        fileName);
                    context.ReportDiagnostic(noWidgetsDiagnostic);
                }
            }
            catch (System.Exception ex)
            {
                // Log error but don't fail the build
                var descriptor = new DiagnosticDescriptor(
                    "MYRA001",
                    "Error generating UI code",
                    "Error processing {0}: {1}",
                    "MyraUI",
                    DiagnosticSeverity.Warning,
                    true);
                
                var errorDiagnostic = Diagnostic.Create(
                    descriptor,
                    Location.None,
                    xmlFile.Path,
                    ex.Message);
                context.ReportDiagnostic(errorDiagnostic);
            }
        }
        }
        catch (Exception ex)
        {
            // Report any exceptions during generation
            var errorDiagnostic = Diagnostic.Create(
                new DiagnosticDescriptor(
                    "MYRA999",
                    "Generator exception",
                    "MyraUIGenerator threw an exception: {0}",
                    "MyraUI",
                    DiagnosticSeverity.Error,
                    true),
                Location.None,
                ex.ToString());
            context.ReportDiagnostic(errorDiagnostic);
        }
    }

    /// <summary>
    /// Retrieves a configuration value from analyzer config options, MSBuild properties, or returns the default.
    /// Checks multiple sources: global analyzer options, MSBuild properties, and per-file options.
    /// </summary>
    /// <param name="context">The generator execution context containing configuration options.</param>
    /// <param name="key">The configuration key to retrieve (e.g., "myra_ui_generator.namespace").</param>
    /// <param name="defaultValue">The default value to return if the key is not found.</param>
    /// <returns>The configuration value if found, otherwise the default value.</returns>
    private string GetConfigurationValue(GeneratorExecutionContext context, string key, string defaultValue)
    {
        // Try to get from analyzer config options (from .editorconfig or MSBuild)
        // Check GlobalOptions first
        if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue(key, out var globalValue) && !string.IsNullOrWhiteSpace(globalValue))
        {
            return globalValue;
        }

        // Also try MSBuild properties (prefixed with build_property.)
        var msbuildKey = $"build_property.{key.Replace("myra_ui_generator.", "MyraUIGenerator_")}";
        if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue(msbuildKey, out var msbuildValue) && !string.IsNullOrWhiteSpace(msbuildValue))
        {
            return msbuildValue;
        }

        // Also check per-file options (sometimes .editorconfig values are file-specific)
        foreach (var file in context.AdditionalFiles)
        {
            var options = context.AnalyzerConfigOptions.GetOptions(file);
            if (options.TryGetValue(key, out var fileValue) && !string.IsNullOrWhiteSpace(fileValue))
            {
                return fileValue;
            }
        }

        // Fallback to default
        return defaultValue;
    }

    /// <summary>
    /// Extracts all widgets with Id attributes from a Myra UI XML document.
    /// Recursively searches all descendant elements to find any widget that has an Id attribute.
    /// </summary>
    /// <param name="xml">The XML document to parse for widgets.</param>
    /// <returns>A list of widget information containing Id, type, and property name.</returns>
    private List<WidgetInfo> ExtractWidgets(XDocument xml)
    {
        var widgets = new List<WidgetInfo>();
        
        // Find all elements with Id attribute
        var elementsWithId = xml.Descendants()
            .Where(e => e.Attribute("Id") != null)
            .ToList();

        foreach (var element in elementsWithId)
        {
            var id = element.Attribute("Id")?.Value;
            var elementName = element.Name.LocalName;
            
            if (!string.IsNullOrEmpty(id))
            {
                widgets.Add(new WidgetInfo
                {
                    Id = id!,
                    Type = elementName,
                    PropertyName = id!
                });
            }
        }
        
        return widgets;
    }

    /// <summary>
    /// Generates the C# source code for a UI accessor class.
    /// Creates a partial class with properties for each widget and an Initialize method to bind them.
    /// </summary>
    /// <param name="fileName">The base name of the XML file (without extension), used to create the class name.</param>
    /// <param name="widgets">The list of widgets to generate properties for.</param>
    /// <param name="namespaceName">The namespace to place the generated class in.</param>
    /// <returns>The complete C# source code as a string.</returns>
    private string GenerateUIClass(string fileName, List<WidgetInfo> widgets, string namespaceName)
    {
        var className = $"{fileName}UI";
        var sb = new StringBuilder();
        
        sb.AppendLine("using Myra.Graphics2D.UI;");
        sb.AppendLine("using System;");
        sb.AppendLine();
        sb.AppendLine($"namespace {namespaceName};");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine($"/// Auto-generated UI accessor for {fileName}.xml");
        sb.AppendLine("/// This file is automatically generated by MyraUIGenerator - do not edit manually.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine($"public partial class {className}");
        sb.AppendLine("{");
        
        // Generate properties
        foreach (var widget in widgets)
        {
            sb.AppendLine($"    public {widget.Type} {widget.PropertyName} {{ get; private set; }}");
        }
        
        sb.AppendLine();
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Initialize widgets from the loaded UI root.");
        sb.AppendLine("    /// Call this after loading the XML via UiLoader.Load().");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public void Initialize(Widget root)");
        sb.AppendLine("    {");
        
        foreach (var widget in widgets)
        {
            sb.AppendLine($"        {widget.PropertyName} = root.FindChildById(\"{widget.Id}\") as {widget.Type};");
        }
        
        sb.AppendLine("    }");
        sb.AppendLine("}");
        
        return sb.ToString();
    }

    private class WidgetInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = "Widget";
        public string PropertyName { get; set; } = string.Empty;
    }
}

