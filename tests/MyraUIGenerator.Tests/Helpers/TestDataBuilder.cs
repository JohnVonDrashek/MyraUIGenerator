using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MyraUIGenerator.Tests.Helpers;

/// <summary>
/// Fluent builder for creating test XML data.
/// </summary>
public class TestDataBuilder
{
    private readonly List<WidgetInfo> _widgets = new();
    private readonly Dictionary<string, List<WidgetInfo>> _nestedWidgets = new();

    private class WidgetInfo
    {
        public string Type { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string? ParentId { get; set; }
    }

    /// <summary>
    /// Adds a widget to the root container.
    /// </summary>
    public TestDataBuilder AddWidget(string type, string id)
    {
        _widgets.Add(new WidgetInfo { Type = type, Id = id });
        return this;
    }

    /// <summary>
    /// Adds a nested widget under the specified parent.
    /// </summary>
    public TestDataBuilder AddNestedWidget(string parentId, string type, string id)
    {
        if (!_nestedWidgets.ContainsKey(parentId))
        {
            _nestedWidgets[parentId] = new List<WidgetInfo>();
        }
        _nestedWidgets[parentId].Add(new WidgetInfo { Type = type, Id = id, ParentId = parentId });
        return this;
    }

    /// <summary>
    /// Builds the XML document string.
    /// </summary>
    public string Build()
    {
        var root = new XElement("Project");
        var container = new XElement("Panel");
        root.Add(container);

        // Add root-level widgets
        foreach (var widget in _widgets)
        {
            container.Add(new XElement(widget.Type, new XAttribute("Id", widget.Id)));
        }

        // Add nested widgets
        foreach (var (parentId, children) in _nestedWidgets)
        {
            // Find parent element
            var parent = container.Descendants()
                .FirstOrDefault(e => e.Attribute("Id")?.Value == parentId);
            
            if (parent != null)
            {
                foreach (var child in children)
                {
                    parent.Add(new XElement(child.Type, new XAttribute("Id", child.Id)));
                }
            }
        }

        return new XDocument(root).ToString();
    }

    /// <summary>
    /// Creates a new builder instance.
    /// </summary>
    public static TestDataBuilder Create()
    {
        return new TestDataBuilder();
    }
}

