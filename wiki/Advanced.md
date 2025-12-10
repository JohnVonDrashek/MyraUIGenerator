# Advanced Topics

Advanced usage patterns and techniques for Myra UI Generator.

## Custom Widget Types

The generator has built-in support for common Myra widget types. When it encounters an unknown widget type, it falls back to the base `Widget` type.

### Handling Unknown Widgets

If you use a custom widget or a widget type not yet supported:

**Generated Code**:
```csharp
public Widget CustomWidget { get; private set; }
```

**Usage with Type Casting**:
```csharp
_ui.Initialize(project.Root);

// Cast to your specific type
var custom = _ui.CustomWidget as MyCustomWidgetType;
if (custom != null)
{
    custom.CustomProperty = value;
}
```

### Extending Generated Classes

Since generated classes are `partial`, you can add helper methods:

```csharp
// Your code: CustomWidgetUI.cs
public partial class CustomWidgetUI
{
    public MyCustomWidgetType GetCustomWidget()
    {
        return CustomWidget as MyCustomWidgetType;
    }
    
    public bool IsCustomWidgetValid()
    {
        return CustomWidget is MyCustomWidgetType;
    }
}
```

### Requesting New Widget Types

If you need support for additional widget types:

1. Check if the widget is already supported (see [Supported Widgets](Reference#supported-widgets))
2. If not, you can:
   - Use the `Widget` fallback and cast manually
   - Request support by opening an issue on GitHub
   - Contribute support by submitting a pull request

## Namespace Organization

Organizing generated code with proper namespaces is important for large projects.

### Single Project Organization

For a single project, use a clear namespace hierarchy:

```ini
[*.cs]
myra_ui_generator.namespace = MyGame.UI.Generated
```

**Structure**:
```
MyGame.UI.Generated
├── MainMenuUI
├── SettingsUI
├── GameHUDUI
└── PauseMenuUI
```

### Multi-Project Solution

For solutions with multiple projects, each can have its own namespace:

**Project 1: MyGame.Client**
```ini
[*.cs]
myra_ui_generator.namespace = MyGame.Client.UI.Generated
```

**Project 2: MyGame.Editor**
```ini
[*.cs]
myra_ui_generator.namespace = MyGame.Editor.UI.Generated
```

### Shared UI Library

If you have a shared UI library project:

**Shared.UI Project**:
```ini
[*.cs]
myra_ui_generator.namespace = MyGame.Shared.UI.Generated
```

**Other Projects**:
Reference the shared project and use:
```csharp
using MyGame.Shared.UI.Generated;
```

### Namespace Best Practices

1. **Be Consistent**: Use the same namespace pattern across your project
2. **Include "Generated"**: Makes it clear these are auto-generated files
3. **Match Project Structure**: Align with your project's namespace conventions
4. **Avoid Conflicts**: Ensure namespaces don't conflict with your own code

## Performance Considerations

Myra UI Generator is designed for zero runtime overhead. All code generation happens at compile time.

### Compile-Time Generation

**What Happens**:
- Generator runs during compilation
- XML files are parsed
- C# code is generated
- Generated code is compiled with your project

**Runtime Impact**: **None** - Generated code is just regular C# classes

### Build Time Impact

**Typical Impact**:
- Small projects (< 10 XML files): Negligible (< 1 second)
- Medium projects (10-50 XML files): < 5 seconds
- Large projects (50+ XML files): < 10 seconds

**Optimization Tips**:
1. Only include XML files you actually use
2. Use wildcards efficiently (avoid overly broad patterns)
3. Keep XML files organized in specific directories

### Memory Usage

**During Build**:
- Generator loads XML files into memory
- Memory usage is proportional to XML file sizes
- Typically very small (< 1MB for most projects)

**At Runtime**:
- No generator code runs
- Only your generated classes exist (same as hand-written code)

### Large Projects

For projects with many UI files:

1. **Organize by Feature**:
   ```
   Content/UI/
   ├── Menus/
   ├── HUD/
   ├── Dialogs/
   └── Screens/
   ```

2. **Use Specific Patterns**:
   ```xml
   <!-- Good: Specific -->
   <AdditionalFiles Include="Content/UI/Menus/*.xml" />
   
   <!-- Avoid: Too broad -->
   <AdditionalFiles Include="**/*.xml" />
   ```

3. **Consider Separate Projects**: Split UI into separate projects if it becomes unwieldy

### Generated Code Size

Generated classes are minimal:

- **Per Widget**: ~3 lines (property declaration + initialization)
- **Per File**: ~10-20 lines base + 3 lines per widget
- **Example**: 10 widgets = ~40-50 lines of generated code

This is negligible compared to typical project sizes.

## Integration Patterns

### With Dependency Injection

```csharp
public class UIService
{
    private readonly IServiceProvider _services;
    
    public MainMenuUI CreateMainMenu()
    {
        var project = UiLoader.Load("MainMenu.xml");
        var ui = new MainMenuUI();
        ui.Initialize(project.Root);
        
        // Inject dependencies if needed
        // ui.SetDependencies(_services);
        
        return ui;
    }
}
```

### With State Management

```csharp
public class GameState
{
    public int Score { get; set; }
    public int Health { get; set; }
    // ... other state
}

public class GameHUD
{
    private GameHUDUI _ui;
    private GameState _state;
    
    public void UpdateFromState(GameState state)
    {
        _state = state;
        _ui.ScoreLabel.Text = $"Score: {state.Score}";
        _ui.HealthBar.Value = state.Health;
    }
}
```

### With MVVM Pattern

```csharp
public class MainMenuViewModel
{
    public string Title { get; set; } = "My Game";
    public bool CanStartNewGame { get; set; } = true;
    // ... other properties
}

public class MainMenuView
{
    private MainMenuUI _ui;
    private MainMenuViewModel _viewModel;
    
    public void Bind(MainMenuViewModel viewModel)
    {
        _viewModel = viewModel;
        UpdateUI();
    }
    
    private void UpdateUI()
    {
        _ui.TitleLabel.Text = _viewModel.Title;
        _ui.NewGameButton.IsEnabled = _viewModel.CanStartNewGame;
    }
}
```

## Debugging Generated Code

### Viewing Generated Files

Generated files are in the `obj/` directory:

```
obj/
└── Debug/
    └── net6.0/
        └── generated/
            └── MyraUIGenerator/
                └── MyraUIGenerator.MyraUIGenerator/
                    ├── MainMenuUI.g.cs
                    └── SettingsUI.g.cs
```

**To View**:
1. Show all files in Solution Explorer (Visual Studio)
2. Navigate to `obj/Debug/{framework}/generated/`
3. Or use "Go to Definition" on a generated class

### Debugging Tips

1. **Check Diagnostics**: Look for `MYRA` diagnostic codes in build output
2. **Verify XML**: Ensure XML files are valid and have `Id` attributes
3. **Check Configuration**: Verify namespace and directory settings
4. **Inspect Generated Code**: Look at actual generated files if behavior is unexpected
5. **Null Checks**: Always check for null after `Initialize()`

### Common Debugging Scenarios

**Widget is null after Initialize**:
- Widget not found in XML (check `Id` attribute)
- Widget type mismatch (check XML element name)
- Widget is nested too deeply (ensure `FindChildById` can find it)

**Wrong namespace in generated code**:
- Check `.editorconfig` configuration
- Verify configuration is in project root
- Rebuild project after changing configuration

**XML file not being processed**:
- Check `AdditionalFiles` in `.csproj`
- Verify file path matches `xml_directory` setting
- Check file extension is `.xml`
- Look for `MYRA003` diagnostic message

## Next Steps

- Review [Troubleshooting](Troubleshooting) for common issues
- Check [Reference](Reference) for complete API documentation
- See [Examples](Examples) for more patterns

