# Getting Started

This section will help you get up and running with Myra UI Generator quickly.

## Installation

### Prerequisites

Before installing Myra UI Generator, ensure you have:

- .NET Standard 2.0 or later
- Myra UI library installed in your project
- MonoGame (or compatible framework)
- A .NET IDE (Visual Studio, VS Code, or JetBrains Rider)

### Install via NuGet

#### Package Manager Console

```powershell
Install-Package MyraUIGenerator
```

#### .NET CLI

```bash
dotnet add package MyraUIGenerator
```

#### PackageReference (Manual)

Add to your `.csproj` file:

```xml
<ItemGroup>
  <PackageReference Include="MyraUIGenerator" Version="1.0.9" />
</ItemGroup>
```

### Verify Installation

After installation, rebuild your project. The generator will run automatically during compilation. You should see generated files in your `obj/` directory (they're typically hidden in the IDE).

Check the build output for diagnostic messages starting with `MYRA` to confirm the generator is running.

## Quick Start

Get your first UI class generated in 5 minutes:

### Step 1: Create a Myra XML File

Create a file `Content/UI/TitleScreen.xml`:

```xml
<Project>
  <Label Id="TitleLabel" Text="Welcome to My Game" />
  <Button Id="StartButton" Content="Start Game" />
  <Button Id="ExitButton" Content="Exit" />
</Project>
```

### Step 2: Add XML to Project

In your `.csproj`, add:

```xml
<ItemGroup>
  <AdditionalFiles Include="Content/UI/*.xml" />
</ItemGroup>
```

### Step 3: Configure (Optional)

Create or update `.editorconfig` in your project root:

```ini
[*.cs]
myra_ui_generator.namespace = MyGame.UI.Generated
myra_ui_generator.xml_directory = Content/UI
```

### Step 4: Build and Use

Build your project, then use the generated class:

```csharp
using MyGame.UI.Generated;
using Myra.Graphics2D.UI;

// Load the UI
var project = UiLoader.Load("TitleScreen.xml");

// Create and initialize the generated class
var ui = new TitleScreenUI();
ui.Initialize(project.Root);

// Use strongly-typed access
ui.StartButton.Click += OnStartClicked;
ui.TitleLabel.Text = "Welcome!";
ui.ExitButton.IsEnabled = true;
```

That's it! The generator creates `TitleScreenUI` with properties for each widget.

## Requirements

### .NET Framework

- **Minimum**: .NET Standard 2.0
- **Recommended**: .NET 6.0 or later

### Dependencies

- **Myra UI**: Any version compatible with your MonoGame setup
- **MonoGame**: 3.8 or later (or compatible framework)

### IDE Support

The generator works with any IDE that supports .NET source generators:

- ✅ Visual Studio 2019 16.9+ / Visual Studio 2022
- ✅ Visual Studio Code (with C# extension)
- ✅ JetBrains Rider 2021.1+
- ✅ Command line builds (`dotnet build`)

### Build Tools

- MSBuild 16.9+ (comes with Visual Studio)
- .NET SDK 5.0+ (for command line builds)

## Next Steps

- Learn about [Configuration](Configuration) options
- See [Basic Usage](Usage#basic-usage) patterns
- Check out [Examples](Examples) for complete working code
- Read the [API Reference](Reference#api-reference)

