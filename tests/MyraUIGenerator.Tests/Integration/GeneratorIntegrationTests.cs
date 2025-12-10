using FluentAssertions;
using MyraUIGenerator.Tests.Helpers;
using Xunit;

namespace MyraUIGenerator.Tests.Integration;

public class GeneratorIntegrationTests
{
    [Fact]
    public void Generator_Executes_WithoutErrors()
    {
        // Arrange
        var xml = TestDataBuilder.Create()
            .AddWidget("Label", "TestLabel")
            .AddWidget("TextButton", "TestButton")
            .Build();

        // Act
        var result = GeneratorTestHelper.RunGenerator(xml);

        // Assert
        result.Results.Should().NotBeEmpty();
        // Generator reports informational warnings (MYRA002, MYRA003, MYRA004) which is expected
        var errors = result.Diagnostics.Where(d => d.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error).ToList();
        errors.Should().BeEmpty();
    }

    [Fact]
    public void Generator_GeneratesSource_ForValidXML()
    {
        // Arrange
        var xml = TestDataBuilder.Create()
            .AddWidget("Label", "TestLabel")
            .AddWidget("TextButton", "TestButton")
            .Build();

        // Act
        var result = GeneratorTestHelper.RunGenerator(xml, "Content/UI/TestScreen.xml");
        var generated = GeneratorTestHelper.GetGeneratedSource(result, "Content/UI/TestScreen.xml");

        // Assert
        generated.Should().NotBeEmpty();
        generated.Should().Contain("public partial class TestScreenUI");
        generated.Should().Contain("public Label TestLabel");
        generated.Should().Contain("public TextButton TestButton");
    }

    [Fact]
    public void Generator_ProcessesMultipleFiles()
    {
        // Arrange
        var xml1 = TestDataBuilder.Create()
            .AddWidget("Label", "Label1")
            .Build();
        var xml2 = TestDataBuilder.Create()
            .AddWidget("TextButton", "Button1")
            .Build();

        // Act
        // Note: GeneratorDriver processes one set of files at a time
        // For multiple files, we'd need to add multiple AdditionalTexts
        // This is a simplified test - full multi-file testing would require more setup
        var result1 = GeneratorTestHelper.RunGenerator(xml1, "Content/UI/File1.xml");
        var result2 = GeneratorTestHelper.RunGenerator(xml2, "Content/UI/File2.xml");

        // Assert
        var generated1 = GeneratorTestHelper.GetGeneratedSource(result1, "Content/UI/File1.xml");
        var generated2 = GeneratorTestHelper.GetGeneratedSource(result2, "Content/UI/File2.xml");

        generated1.Should().Contain("File1UI");
        generated2.Should().Contain("File2UI");
    }

    [Fact]
    public void Generator_HandlesEmptyXML()
    {
        // Arrange
        var xml = "";

        // Act
        var result = GeneratorTestHelper.RunGenerator(xml);

        // Assert
        // Empty XML should not crash - may or may not generate code depending on implementation
        result.Results.Should().NotBeNull();
    }

    [Fact]
    public void Generator_HandlesInvalidXML()
    {
        // Arrange
        var xml = "<Project><Panel Id=\"Test\""; // Malformed

        // Act
        var result = GeneratorTestHelper.RunGenerator(xml);

        // Assert
        // Should handle gracefully - may report diagnostic or not generate code
        result.Results.Should().NotBeNull();
    }

    [Fact]
    public void Generator_HandlesMissingIds()
    {
        // Arrange
        var xml = "<Project><Panel><Label Text=\"No Id\" /></Panel></Project>";

        // Act
        var result = GeneratorTestHelper.RunGenerator(xml, "Content/UI/NoIds.xml");
        var generated = GeneratorTestHelper.GetGeneratedSource(result, "Content/UI/NoIds.xml");

        // Assert
        // Should not generate class if no widgets have Ids
        generated.Should().BeEmpty();
    }

    [Fact]
    public void Generator_FileDiscovery_Works()
    {
        // Arrange
        var xml = TestDataBuilder.Create()
            .AddWidget("Label", "TestLabel")
            .Build();

        // Act - File in Content/UI directory should be found
        var result = GeneratorTestHelper.RunGenerator(xml, "Content/UI/Test.xml");
        var generated = GeneratorTestHelper.GetGeneratedSource(result, "Content/UI/Test.xml");

        // Assert
        generated.Should().NotBeEmpty();
    }

    [Fact]
    public void Generator_FileDiscovery_FiltersNonXmlFiles()
    {
        // Arrange
        var xml = TestDataBuilder.Create()
            .AddWidget("Label", "TestLabel")
            .Build();

        // Act - File not in Content/UI should be filtered out
        var result = GeneratorTestHelper.RunGenerator(xml, "Other/Directory/Test.xml");
        var generated = GeneratorTestHelper.GetGeneratedSource(result, "Other/Directory/Test.xml");

        // Assert
        // Currently the generator filters by directory, so files outside Content/UI won't generate
        // This tests current behavior - may need adjustment based on requirements
        generated.Should().BeEmpty();
    }

    [Fact]
    public void Generator_SimpleButtonExample_Works()
    {
        // Arrange
        var xml = TestDataBuilder.Create()
            .AddWidget("Label", "MessageLabel")
            .AddWidget("TextButton", "ClickMeButton")
            .Build();

        // Act
        var result = GeneratorTestHelper.RunGenerator(xml, "Content/UI/SimpleButton.xml");
        var generated = GeneratorTestHelper.GetGeneratedSource(result, "Content/UI/SimpleButton.xml");

        // Assert
        generated.Should().Contain("public partial class SimpleButtonUI");
        generated.Should().Contain("public Label MessageLabel");
        generated.Should().Contain("public TextButton ClickMeButton");
        generated.Should().Contain("MessageLabel = root.FindChildById(\"MessageLabel\") as Label;");
        generated.Should().Contain("ClickMeButton = root.FindChildById(\"ClickMeButton\") as TextButton;");
    }

    [Fact]
    public void Generator_NestedWidgets_AllFound()
    {
        // Arrange
        var xml = @"
            <Project>
                <Panel Id=""RootPanel"">
                    <VerticalStackPanel Id=""Stack"">
                        <Label Id=""NestedLabel"" />
                        <TextButton Id=""NestedButton"" />
                    </VerticalStackPanel>
                </Panel>
            </Project>";

        // Act
        var result = GeneratorTestHelper.RunGenerator(xml, "Content/UI/Nested.xml");
        var generated = GeneratorTestHelper.GetGeneratedSource(result, "Content/UI/Nested.xml");

        // Assert
        generated.Should().Contain("RootPanel");
        generated.Should().Contain("Stack");
        generated.Should().Contain("NestedLabel");
        generated.Should().Contain("NestedButton");
    }

    [Fact]
    public void Generator_AllWidgetTypes_Supported()
    {
        // Arrange
        var xml = TestDataBuilder.Create()
            .AddWidget("Label", "Label1")
            .AddWidget("TextButton", "TextButton1")
            .AddWidget("Button", "Button1")
            .AddWidget("CheckBox", "CheckBox1")
            .AddWidget("TextBox", "TextBox1")
            .AddWidget("Panel", "Panel1")
            .AddWidget("VerticalStackPanel", "VStack1")
            .AddWidget("HorizontalStackPanel", "HStack1")
            .AddWidget("Grid", "Grid1")
            .AddWidget("ScrollViewer", "ScrollViewer1")
            .AddWidget("Slider", "Slider1")
            .AddWidget("ProgressBar", "ProgressBar1")
            .AddWidget("ListBox", "ListBox1")
            .AddWidget("ComboBox", "ComboBox1")
            .AddWidget("Image", "Image1")
            .AddWidget("TextBlock", "TextBlock1")
            .Build();

        // Act
        var result = GeneratorTestHelper.RunGenerator(xml, "Content/UI/AllTypes.xml");
        var generated = GeneratorTestHelper.GetGeneratedSource(result, "Content/UI/AllTypes.xml");

        // Assert
        generated.Should().Contain("Label Label1");
        generated.Should().Contain("TextButton TextButton1");
        generated.Should().Contain("Button Button1");
        generated.Should().Contain("CheckBox CheckBox1");
        generated.Should().Contain("TextBox TextBox1");
        generated.Should().Contain("Panel Panel1");
        generated.Should().Contain("VerticalStackPanel VStack1");
        generated.Should().Contain("HorizontalStackPanel HStack1");
        generated.Should().Contain("Grid Grid1");
        generated.Should().Contain("ScrollViewer ScrollViewer1");
        generated.Should().Contain("Slider Slider1");
        generated.Should().Contain("ProgressBar ProgressBar1");
        generated.Should().Contain("ListBox ListBox1");
        generated.Should().Contain("ComboBox ComboBox1");
        generated.Should().Contain("Image Image1");
        generated.Should().Contain("TextBlock TextBlock1");
    }
}

