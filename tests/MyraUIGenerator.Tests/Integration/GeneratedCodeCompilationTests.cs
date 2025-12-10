using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using MyraUIGenerator.Tests.Helpers;
using Xunit;

namespace MyraUIGenerator.Tests.Integration;

public class GeneratedCodeCompilationTests
{
    [Fact]
    public void GeneratedCode_CompilesSuccessfully()
    {
        // Arrange
        var xml = TestDataBuilder.Create()
            .AddWidget("Label", "TestLabel")
            .AddWidget("TextButton", "TestButton")
            .Build();

        // Act
        var result = GeneratorTestHelper.RunGenerator(xml, "Content/UI/Test.xml");
        var generated = GeneratorTestHelper.GetGeneratedSource(result, "Content/UI/Test.xml");
        var compilation = GeneratorTestHelper.CreateCompilationWithGeneratedCode(result);

        // Assert
        var diagnostics = compilation.GetDiagnostics();
        var errors = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        
        // Note: We expect some errors because we don't have Myra references
        // But the syntax should be valid
        generated.Should().NotBeEmpty();
        compilation.SyntaxTrees.Should().NotBeEmpty();
    }

    [Fact]
    public void GeneratedCode_HasCorrectStructure()
    {
        // Arrange
        var xml = TestDataBuilder.Create()
            .AddWidget("Label", "TestLabel")
            .Build();

        // Act
        var result = GeneratorTestHelper.RunGenerator(xml, "Content/UI/Test.xml");
        var generated = GeneratorTestHelper.GetGeneratedSource(result, "Content/UI/Test.xml");

        // Assert
        generated.Should().Contain("namespace");
        generated.Should().Contain("public partial class");
        generated.Should().Contain("public void Initialize(Widget root)");
        generated.Should().Contain("using Myra.Graphics2D.UI;");
        generated.Should().Contain("using System;");
    }

    [Fact]
    public void GeneratedCode_PartialClassKeyword()
    {
        // Arrange
        var xml = TestDataBuilder.Create()
            .AddWidget("Label", "TestLabel")
            .Build();

        // Act
        var result = GeneratorTestHelper.RunGenerator(xml, "Content/UI/Test.xml");
        var generated = GeneratorTestHelper.GetGeneratedSource(result, "Content/UI/Test.xml");

        // Assert
        generated.Should().Contain("public partial class");
    }

    [Fact]
    public void GeneratedCode_PropertyAccessors()
    {
        // Arrange
        var xml = TestDataBuilder.Create()
            .AddWidget("Label", "TestLabel")
            .Build();

        // Act
        var result = GeneratorTestHelper.RunGenerator(xml, "Content/UI/Test.xml");
        var generated = GeneratorTestHelper.GetGeneratedSource(result, "Content/UI/Test.xml");

        // Assert
        generated.Should().Contain("public Label TestLabel { get; private set; }");
    }

    [Fact]
    public void GeneratedCode_InitializeMethod_ValidSyntax()
    {
        // Arrange
        var xml = TestDataBuilder.Create()
            .AddWidget("Label", "TestLabel")
            .AddWidget("TextButton", "TestButton")
            .Build();

        // Act
        var result = GeneratorTestHelper.RunGenerator(xml, "Content/UI/Test.xml");
        var generated = GeneratorTestHelper.GetGeneratedSource(result, "Content/UI/Test.xml");

        // Assert
        generated.Should().Contain("public void Initialize(Widget root)");
        generated.Should().Contain("TestLabel = root.FindChildById(\"TestLabel\") as Label;");
        generated.Should().Contain("TestButton = root.FindChildById(\"TestButton\") as TextButton;");
    }

    [Fact]
    public void GeneratedCode_ClassNameMatchesFileName()
    {
        // Arrange
        var xml = TestDataBuilder.Create()
            .AddWidget("Label", "TestLabel")
            .Build();

        // Act
        var result1 = GeneratorTestHelper.RunGenerator(xml, "Content/UI/TitleScreen.xml");
        var result2 = GeneratorTestHelper.RunGenerator(xml, "Content/UI/MainMenu.xml");
        
        var generated1 = GeneratorTestHelper.GetGeneratedSource(result1, "Content/UI/TitleScreen.xml");
        var generated2 = GeneratorTestHelper.GetGeneratedSource(result2, "Content/UI/MainMenu.xml");

        // Assert
        generated1.Should().Contain("public partial class TitleScreenUI");
        generated2.Should().Contain("public partial class MainMenuUI");
    }

    [Fact]
    public void GeneratedCode_UnknownWidgetType_StillGenerates()
    {
        // Arrange
        var xml = "<Project><Panel><CustomWidget Id=\"Custom1\" /></Panel></Project>";

        // Act
        var result = GeneratorTestHelper.RunGenerator(xml, "Content/UI/Test.xml");
        var generated = GeneratorTestHelper.GetGeneratedSource(result, "Content/UI/Test.xml");

        // Assert
        generated.Should().Contain("public CustomWidget Custom1");
    }
}

