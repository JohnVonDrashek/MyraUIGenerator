using FluentAssertions;
using System.Xml.Linq;
using MyraUIGenerator;
using MyraUIGenerator.Tests.Helpers;
using Xunit;

namespace MyraUIGenerator.Tests.Unit;

public class WidgetExtractionTests
{
    [Fact]
    public void ExtractWidgets_ReturnsEmptyList_WhenNoIds()
    {
        // Arrange
        var generator = new MyraUIGenerator();
        var xml = XDocument.Parse("<Project><Panel><Label Text=\"No Id\" /></Panel></Project>");

        // Act
        var result = generator.ExtractWidgets(xml);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void ExtractWidgets_FindsSingleWidget()
    {
        // Arrange
        var generator = new MyraUIGenerator();
        var xml = GeneratorTestHelper.CreateValidXml(("Label", "TestLabel"));

        // Act
        var result = generator.ExtractWidgets(xml);

        // Assert
        result.Should().HaveCount(1);
        result[0].Id.Should().Be("TestLabel");
        result[0].Type.Should().Be("Label");
        result[0].PropertyName.Should().Be("TestLabel");
    }

    [Fact]
    public void ExtractWidgets_FindsMultipleWidgets()
    {
        // Arrange
        var generator = new MyraUIGenerator();
        var xml = GeneratorTestHelper.CreateValidXml(
            ("Label", "Label1"),
            ("TextButton", "Button1"),
            ("Button", "Button2"));

        // Act
        var result = generator.ExtractWidgets(xml);

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(w => w.Id == "Label1" && w.Type == "Label");
        result.Should().Contain(w => w.Id == "Button1" && w.Type == "TextButton");
        result.Should().Contain(w => w.Id == "Button2" && w.Type == "Button");
    }

    [Fact]
    public void ExtractWidgets_FindsNestedWidgets()
    {
        // Arrange
        var generator = new MyraUIGenerator();
        var xml = XDocument.Parse(@"
            <Project>
                <Panel Id=""RootPanel"">
                    <VerticalStackPanel Id=""Stack"">
                        <Label Id=""NestedLabel"" />
                    </VerticalStackPanel>
                </Panel>
            </Project>");

        // Act
        var result = generator.ExtractWidgets(xml);

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(w => w.Id == "RootPanel");
        result.Should().Contain(w => w.Id == "Stack");
        result.Should().Contain(w => w.Id == "NestedLabel");
    }

    [Fact]
    public void ExtractWidgets_ExtractsAllWidgetTypes()
    {
        // Arrange
        var generator = new MyraUIGenerator();
        var xml = GeneratorTestHelper.CreateValidXml(
            ("Label", "Label1"),
            ("TextButton", "TextButton1"),
            ("Button", "Button1"),
            ("CheckBox", "CheckBox1"),
            ("TextBox", "TextBox1"),
            ("Panel", "Panel1"),
            ("VerticalStackPanel", "VStack1"),
            ("HorizontalStackPanel", "HStack1"),
            ("Grid", "Grid1"),
            ("ScrollViewer", "ScrollViewer1"),
            ("Slider", "Slider1"),
            ("ProgressBar", "ProgressBar1"),
            ("ListBox", "ListBox1"),
            ("ComboBox", "ComboBox1"),
            ("Image", "Image1"),
            ("TextBlock", "TextBlock1"));

        // Act
        var result = generator.ExtractWidgets(xml);

        // Assert
        result.Should().HaveCount(16);
        result.Should().Contain(w => w.Type == "Label");
        result.Should().Contain(w => w.Type == "TextButton");
        result.Should().Contain(w => w.Type == "Button");
        result.Should().Contain(w => w.Type == "CheckBox");
        result.Should().Contain(w => w.Type == "TextBox");
        result.Should().Contain(w => w.Type == "Panel");
        result.Should().Contain(w => w.Type == "VerticalStackPanel");
        result.Should().Contain(w => w.Type == "HorizontalStackPanel");
        result.Should().Contain(w => w.Type == "Grid");
        result.Should().Contain(w => w.Type == "ScrollViewer");
        result.Should().Contain(w => w.Type == "Slider");
        result.Should().Contain(w => w.Type == "ProgressBar");
        result.Should().Contain(w => w.Type == "ListBox");
        result.Should().Contain(w => w.Type == "ComboBox");
        result.Should().Contain(w => w.Type == "Image");
        result.Should().Contain(w => w.Type == "TextBlock");
    }

    [Fact]
    public void ExtractWidgets_HandlesUnknownWidgetType()
    {
        // Arrange
        var generator = new MyraUIGenerator();
        var xml = XDocument.Parse("<Project><Panel><CustomWidget Id=\"Custom1\" /></Panel></Project>");

        // Act
        var result = generator.ExtractWidgets(xml);

        // Assert
        result.Should().HaveCount(1);
        result[0].Id.Should().Be("Custom1");
        result[0].Type.Should().Be("CustomWidget");
    }

    [Fact]
    public void ExtractWidgets_IgnoresEmptyId()
    {
        // Arrange
        var generator = new MyraUIGenerator();
        var xml = XDocument.Parse("<Project><Panel><Label Id=\"\" /><Label Id=\"ValidId\" /></Panel></Project>");

        // Act
        var result = generator.ExtractWidgets(xml);

        // Assert
        result.Should().HaveCount(1);
        result[0].Id.Should().Be("ValidId");
    }

    [Fact]
    public void ExtractWidgets_HandlesSpecialCharactersInId()
    {
        // Arrange
        var generator = new MyraUIGenerator();
        var xml = GeneratorTestHelper.CreateValidXml(
            ("Label", "Widget_1"),
            ("Label", "Widget_2"),
            ("Label", "MyWidget123"),
            ("Label", "WidgetWith_Underscores"));

        // Act
        var result = generator.ExtractWidgets(xml);

        // Assert
        result.Should().HaveCount(4);
        result.Should().Contain(w => w.Id == "Widget_1");
        result.Should().Contain(w => w.Id == "Widget_2");
        result.Should().Contain(w => w.Id == "MyWidget123");
        result.Should().Contain(w => w.Id == "WidgetWith_Underscores");
    }

    [Fact]
    public void ExtractWidgets_PreservesCaseSensitiveIds()
    {
        // Arrange
        var generator = new MyraUIGenerator();
        var xml = GeneratorTestHelper.CreateValidXml(
            ("Label", "LabelA"),
            ("Label", "labela"),
            ("Label", "LABELA"));

        // Act
        var result = generator.ExtractWidgets(xml);

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(w => w.Id == "LabelA");
        result.Should().Contain(w => w.Id == "labela");
        result.Should().Contain(w => w.Id == "LABELA");
    }

    [Fact]
    public void ExtractWidgets_DuplicateIds_AllIncluded()
    {
        // Arrange
        var generator = new MyraUIGenerator();
        var xml = XDocument.Parse("<Project><Panel><Label Id=\"Duplicate\" /><Label Id=\"Duplicate\" /></Panel></Project>");

        // Act
        var result = generator.ExtractWidgets(xml);

        // Assert
        result.Should().HaveCount(2);
        result.All(w => w.Id == "Duplicate").Should().BeTrue();
    }
}

