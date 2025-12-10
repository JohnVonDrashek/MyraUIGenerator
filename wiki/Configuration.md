# Configuration

Myra UI Generator can be configured to customize the namespace and XML directory paths for generated code.

## Overview

The generator supports two configuration methods:

1. **.editorconfig** (Recommended) - Project-wide configuration
2. **MSBuild Properties** - Build-time configuration

Both methods support the same configuration keys:

- `myra_ui_generator.namespace` - Namespace for generated classes
- `myra_ui_generator.xml_directory` - Directory where XML files are located

### Default Values

If no configuration is provided, the generator uses:

- **Namespace**: `GeneratedUI`
- **Directory**: `Content/UI`

## EditorConfig Setup

The recommended way to configure the generator is using `.editorconfig` files. This provides a simple, version-controlled configuration.

### Creating .editorconfig

Create or update `.editorconfig` in your project root:

```ini
[*.cs]
# Namespace for generated UI classes
myra_ui_generator.namespace = YourNamespace.UI.Generated

# Directory where XML files are located (relative to project root)
myra_ui_generator.xml_directory = Content/UI
```

### Configuration Examples

#### Simple Project Structure

```ini
[*.cs]
myra_ui_generator.namespace = MyGame.UI
myra_ui_generator.xml_directory = Content/UI
```

#### Nested Namespace

```ini
[*.cs]
myra_ui_generator.namespace = MyGame.UserInterface.Generated
myra_ui_generator.xml_directory = Assets/UserInterface
```

#### Multiple Projects

If you have multiple projects with different configurations, you can use project-specific `.editorconfig` files in each project directory.

### Path Matching

The `xml_directory` setting uses case-insensitive path matching. The generator will find XML files whose paths contain the specified directory string.

Examples:
- Setting: `Content/UI`
- Matches: `Content/UI/Menu.xml`
- Matches: `C:/Projects/MyGame/Content/UI/Settings.xml`
- Matches: `Content\UI\TitleScreen.xml` (Windows paths normalized)

### Troubleshooting EditorConfig

**Issue**: Configuration not being read

**Solutions**:
1. Ensure `.editorconfig` is in the project root (same directory as `.csproj`)
2. Restart your IDE after creating/modifying `.editorconfig`
3. Rebuild the project (configuration is read at build time)
4. Check for syntax errors in `.editorconfig` (use proper INI format)

## MSBuild Properties

You can also configure the generator using MSBuild properties in your `.csproj` file.

### Setting Properties in .csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    
    <!-- Generator Configuration -->
    <MyraUIGenerator_namespace>MyGame.UI.Generated</MyraUIGenerator_namespace>
    <MyraUIGenerator_xml_directory>Content/UI</MyraUIGenerator_xml_directory>
  </PropertyGroup>
  
  <!-- Rest of your project file -->
</Project>
```

### Setting Properties via Command Line

```bash
dotnet build /p:MyraUIGenerator_namespace=MyGame.UI.Generated /p:MyraUIGenerator_xml_directory=Content/UI
```

### Property Name Format

MSBuild properties use this format:
- `MyraUIGenerator_namespace` (replaces dots with underscores)
- `MyraUIGenerator_xml_directory`

### When to Use MSBuild

Use MSBuild properties when:
- You need different configurations for different build configurations (Debug/Release)
- You're using conditional compilation
- You need to set properties dynamically
- You prefer project file configuration over `.editorconfig`

## Configuration Precedence

The generator checks configuration in this order:

1. **MSBuild Properties** (highest priority)
2. **.editorconfig Global Options**
3. **.editorconfig Per-File Options**
4. **Default Values** (lowest priority)

## Verification

To verify your configuration is working:

1. Build your project
2. Check the build output for diagnostic messages:
   - `MYRA002`: Shows the namespace and directory being used
   - `MYRA003`: Shows how many XML files were found
3. Check generated files in `obj/Debug/net6.0/generated/` (path varies by framework)
4. Verify the namespace in generated code matches your configuration

## Examples

### Example 1: Simple Game Project

**Project Structure**:
```
MyGame/
├── MyGame.csproj
├── .editorconfig
└── Content/
    └── UI/
        ├── MainMenu.xml
        └── Settings.xml
```

**.editorconfig**:
```ini
[*.cs]
myra_ui_generator.namespace = MyGame.UI
myra_ui_generator.xml_directory = Content/UI
```

**Result**: Generated classes in `MyGame.UI` namespace for XML files in `Content/UI/`.

### Example 2: Multi-Project Solution

**Solution Structure**:
```
MyGameSolution/
├── MyGame.Core/
│   ├── MyGame.Core.csproj
│   └── .editorconfig (namespace: MyGame.Core.UI)
├── MyGame.Client/
│   ├── MyGame.Client.csproj
│   └── .editorconfig (namespace: MyGame.Client.UI)
└── Shared/
    └── UI/
        ├── SharedMenu.xml
        └── SharedSettings.xml
```

Each project can have its own `.editorconfig` with different namespaces.

## Next Steps

- Learn about [Basic Usage](Usage#basic-usage)
- See [Common Patterns](Usage#common-patterns)
- Check [Troubleshooting](Troubleshooting#common-issues) if configuration isn't working

