# Myra UI Generator

![NuGet](https://img.shields.io/nuget/v/MyraUIGenerator)
![License](https://img.shields.io/github/license/JohnVonDrashek/MyraUIGenerator)
![.NET](https://img.shields.io/badge/.NET-Standard%202.0-blue)

![Myra UI Generator icon](icon.png)

A source generator for Myra UI XML files that automatically creates strongly-typed C# accessor classes.

## Features

- 🔧 **Automatic Code Generation**: Scans Myra XML files and generates strongly-typed C# classes
- 🎯 **Type Safety**: Compile-time access to UI widgets with IntelliSense support
- ⚙️ **Configurable**: Customize namespace and XML directory paths
- 🚀 **Zero Runtime Overhead**: Pure compile-time code generation
- 📦 **NuGet Package**: Easy to add to any MonoGame + Myra project

## Installation

```bash
dotnet add package MyraUIGenerator
```

Or add to your `.csproj`:

```xml
<ItemGroup>
  <PackageReference Include="MyraUIGenerator" Version="1.0.0" />
</ItemGroup>
```

## Usage

### 1. Add XML Files to AdditionalFiles

In your `.csproj`, include your Myra XML files:

```xml
<ItemGroup>
  <AdditionalFiles Include="Content/UI/*.xml" />
</ItemGroup>
```

### 2. Configure (Optional)

Create or update `.editorconfig` in your project root:

```ini
[*.cs]
# Namespace for generated UI classes
myra_ui_generator.namespace = YourNamespace.UI.Generated

# Directory where XML files are located (relative to project root)
myra_ui_generator.xml_directory = Content/UI
```

If not configured, defaults are:
- Namespace: `GeneratedUI`
- Directory: `Content/UI`

### 3. Use Generated Classes

For a file named `TitleScreen.xml`:

```csharp
using YourNamespace.UI.Generated;
using Myra.Graphics2D.UI;

// Load the UI
var project = UiLoader.Load("TitleScreen.xml");

// Create and initialize the generated class
var ui = new TitleScreenUI();
ui.Initialize(project.Root);

// Use strongly-typed access
ui.StartButton.Click += OnStartClicked;
ui.TitleLabel.Text = "Welcome!";
ui.SettingsButton.IsEnabled = true;
```

## Generated Code

For `TitleScreen.xml` with widgets:
- `Id="StartButton"` (TextButton)
- `Id="TitleLabel"` (Label)
- `Id="SettingsButton"` (Button)

The generator creates `TitleScreenUI.g.cs`:

```csharp
namespace YourNamespace.UI.Generated;

public partial class TitleScreenUI
{
    public TextButton StartButton { get; private set; }
    public Label TitleLabel { get; private set; }
    public Button SettingsButton { get; private set; }
    
    public void Initialize(Widget root)
    {
        StartButton = root.FindChildById("StartButton") as TextButton;
        TitleLabel = root.FindChildById("TitleLabel") as Label;
        SettingsButton = root.FindChildById("SettingsButton") as Button;
    }
}
```

## Supported Widget Types

The generator recognizes these Myra XML elements:

- `Label` → `Label`
- `TextButton` → `TextButton`
- `Button` → `Button`
- `CheckBox` → `CheckBox`
- `TextBox` → `TextBox`
- `Panel` → `Panel`
- `VerticalStackPanel` → `VerticalStackPanel`
- `HorizontalStackPanel` → `HorizontalStackPanel`
- `Grid` → `Grid`
- `ScrollViewer` → `ScrollViewer`
- `Slider` → `Slider`
- `ProgressBar` → `ProgressBar`
- `ListBox` → `ListBox`
- `ComboBox` → `ComboBox`
- `Image` → `Image`
- `TextBlock` → `TextBlock`
- Unknown types → `Widget` (fallback)

## Property Names

Property names in the generated classes match the `Id` attribute values from your XML files exactly. For example:
- Widget with `Id="StartButton"` → Property named `StartButton`
- Widget with `Id="TitleLabel"` → Property named `TitleLabel`
- Widget with `Id="HealthBar"` → Property named `HealthBar`

## Requirements

- .NET Standard 2.0 or later
- Myra UI library
- MonoGame (or compatible framework)

## Documentation

📚 **Complete documentation is available in the [GitHub Wiki](https://github.com/JohnVonDrashek/MyraUIGenerator/wiki)**

The wiki includes:
- Getting started guides
- Configuration options
- Usage examples and patterns
- API reference
- Troubleshooting guide
- Contributing guidelines

Wiki documentation is automatically synced from the `wiki/` directory in this repository.

## License

MIT License - see LICENSE file for details.

## Contributing

Contributions welcome! Please open an issue or pull request on GitHub.

## Changelog

### 1.0.0
- Initial release
- Configurable namespace and directory
- Support for common Myra widget types
- Property names match XML `Id` attributes exactly


