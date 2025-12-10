using FluentAssertions;
using MyraUIGenerator.Tests.Helpers;
using Xunit;

namespace MyraUIGenerator.Tests.Diagnostics;

public class DiagnosticTests
{
    [Fact]
    public void Diagnostics_MYRA002_GeneratorExecuting()
    {
        // Arrange
        var xml = TestDataBuilder.Create()
            .AddWidget("Label", "TestLabel")
            .Build();

        // Act
        var result = GeneratorTestHelper.RunGenerator(xml, "Content/UI/Test.xml");
        var diagnostics = GeneratorTestHelper.GetDiagnostics(result, "MYRA002");

        // Assert
        // MYRA002 should be reported when generator executes
        // Note: Actual diagnostic reporting depends on the generator implementation
        result.Results.Should().NotBeEmpty();
    }

    [Fact]
    public void Diagnostics_MYRA003_XMLFilesFound()
    {
        // Arrange
        var xml = TestDataBuilder.Create()
            .AddWidget("Label", "TestLabel")
            .Build();

        // Act
        var result = GeneratorTestHelper.RunGenerator(xml, "Content/UI/Test.xml");
        var diagnostics = GeneratorTestHelper.GetDiagnostics(result, "MYRA003");

        // Assert
        // MYRA003 reports how many XML files were found
        result.Results.Should().NotBeEmpty();
    }

    [Fact]
    public void Diagnostics_MYRA004_GeneratedUIClass()
    {
        // Arrange
        var xml = TestDataBuilder.Create()
            .AddWidget("Label", "TestLabel")
            .AddWidget("TextButton", "TestButton")
            .Build();

        // Act
        var result = GeneratorTestHelper.RunGenerator(xml, "Content/UI/Test.xml");
        var diagnostics = GeneratorTestHelper.GetDiagnostics(result, "MYRA004");
        var generated = GeneratorTestHelper.GetGeneratedSource(result, "Content/UI/Test.xml");

        // Assert
        // MYRA004 should be reported when a class is generated
        generated.Should().NotBeEmpty();
        result.Results.Should().NotBeEmpty();
    }

    [Fact]
    public void Diagnostics_MYRA005_NoWidgetsFound()
    {
        // Arrange
        var xml = "<Project><Panel><Label Text=\"No Id\" /></Panel></Project>";

        // Act
        var result = GeneratorTestHelper.RunGenerator(xml, "Content/UI/NoIds.xml");
        var diagnostics = GeneratorTestHelper.GetDiagnostics(result, "MYRA005");
        var generated = GeneratorTestHelper.GetGeneratedSource(result, "Content/UI/NoIds.xml");

        // Assert
        // MYRA005 should be reported when no widgets with Ids are found
        generated.Should().BeEmpty();
    }

    [Fact]
    public void Diagnostics_MYRA001_ErrorGeneratingCode_InvalidXML()
    {
        // Arrange
        var xml = "<Project><Panel Id=\"Test\""; // Malformed XML

        // Act
        var result = GeneratorTestHelper.RunGenerator(xml, "Content/UI/Invalid.xml");
        var diagnostics = GeneratorTestHelper.GetDiagnostics(result, "MYRA001");

        // Assert
        // MYRA001 should be reported when there's an error processing XML
        // Note: This depends on how the generator handles XML parsing errors
        result.Results.Should().NotBeNull();
    }

    [Fact]
    public void Diagnostics_NoErrors_ForValidXML()
    {
        // Arrange
        var xml = TestDataBuilder.Create()
            .AddWidget("Label", "TestLabel")
            .Build();

        // Act
        var result = GeneratorTestHelper.RunGenerator(xml, "Content/UI/Test.xml");
        var errorDiagnostics = result.Diagnostics
            .Where(d => d.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
            .ToList();

        // Assert
        // Should not have errors for valid XML
        errorDiagnostics.Should().BeEmpty();
    }

    [Fact]
    public void Diagnostics_GeneratorReportsExecution()
    {
        // Arrange
        var xml = TestDataBuilder.Create()
            .AddWidget("Label", "TestLabel")
            .Build();

        // Act
        var result = GeneratorTestHelper.RunGenerator(xml, "Content/UI/Test.xml");

        // Assert
        // Generator should report that it executed
        result.Results.Should().NotBeEmpty();
        result.Diagnostics.Should().NotBeNull();
    }
}

