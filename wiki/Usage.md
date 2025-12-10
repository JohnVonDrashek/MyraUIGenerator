# Usage Guide

This section covers how to use Myra UI Generator in your projects.

## Basic Usage

### Adding XML Files to Your Project

The generator processes XML files marked as `AdditionalFiles` in your `.csproj`:

```xml
<ItemGroup>
  <AdditionalFiles Include="Content/UI/*.xml" />
</ItemGroup>
```

You can include:
- Individual files: `Include="Content/UI/MainMenu.xml"`
- Wildcards: `Include="Content/UI/*.xml"`
- Recursive: `Include="Content/UI/**/*.xml"`
- Multiple patterns: Multiple `<AdditionalFiles>` entries

### Understanding Generated Classes

For each XML file, the generator creates a partial class:

- **File**: `TitleScreen.xml` → **Class**: `TitleScreenUI`
- **Location**: Generated in `obj/` directory (hidden by default in IDEs)
- **Namespace**: As configured (default: `GeneratedUI`)

### The Initialize Method

Every generated class has an `Initialize(Widget root)` method that binds widgets:

```csharp
public void Initialize(Widget root)
{
    StartButton = root.FindChildById("StartButton") as TextButton;
    TitleLabel = root.FindChildById("TitleLabel") as Label;
    // ... more widgets
}
```

**Important**: You must call `Initialize()` after loading the XML and before using the properties.

### Accessing Widgets

After initialization, access widgets through strongly-typed properties:

```csharp
var ui = new TitleScreenUI();
ui.Initialize(project.Root);

// All properties are strongly-typed
ui.StartButton.Click += OnStartClicked;
ui.TitleLabel.Text = "Welcome!";
ui.ExitButton.IsEnabled = false;
```

## Step-by-Step Tutorial

Let's create a complete working example from scratch.

### Step 1: Create the XML File

Create `Content/UI/MainMenu.xml`:

```xml
<Project>
  <VerticalStackPanel>
    <Label Id="TitleLabel" Text="My Awesome Game" />
    <TextButton Id="NewGameButton" Text="New Game" />
    <TextButton Id="LoadGameButton" Text="Load Game" />
    <TextButton Id="SettingsButton" Text="Settings" />
    <TextButton Id="ExitButton" Text="Exit" />
  </VerticalStackPanel>
</Project>
```

### Step 2: Configure Your Project

**Option A: Add to .csproj**

```xml
<ItemGroup>
  <AdditionalFiles Include="Content/UI/*.xml" />
</ItemGroup>
```

**Option B: Use .editorconfig** (create in project root)

```ini
[*.cs]
myra_ui_generator.namespace = MyGame.UI.Generated
myra_ui_generator.xml_directory = Content/UI
```

### Step 3: Build Your Project

Build the project. The generator will:
1. Find `MainMenu.xml`
2. Extract widgets with `Id` attributes
3. Generate `MainMenuUI.g.cs`

### Step 4: Use the Generated Class

```csharp
using MyGame.UI.Generated;
using Myra.Graphics2D.UI;
using Myra.Graphics2D;

public class GameScreen
{
    private MainMenuUI _ui;
    
    public void Load()
    {
        // Load the XML
        var project = UiLoader.Load("MainMenu.xml");
        
        // Create and initialize the generated class
        _ui = new MainMenuUI();
        _ui.Initialize(project.Root);
        
        // Set up event handlers
        _ui.NewGameButton.Click += OnNewGameClicked;
        _ui.LoadGameButton.Click += OnLoadGameClicked;
        _ui.SettingsButton.Click += OnSettingsClicked;
        _ui.ExitButton.Click += OnExitClicked;
        
        // Customize UI
        _ui.TitleLabel.TextColor = Color.White;
    }
    
    private void OnNewGameClicked(object sender, EventArgs e)
    {
        // Start new game
    }
    
    // ... other event handlers
}
```

### Step 5: Render the UI

```csharp
public void Draw()
{
    // Render the UI root (from project.Root)
    // This depends on your rendering setup
}
```

## Common Patterns

### Pattern 1: Screen Management

Organize UI classes by screen:

```csharp
public class ScreenManager
{
    private MainMenuUI _mainMenu;
    private SettingsUI _settings;
    private GameHUDUI _hud;
    
    public void ShowMainMenu()
    {
        var project = UiLoader.Load("MainMenu.xml");
        _mainMenu = new MainMenuUI();
        _mainMenu.Initialize(project.Root);
        // ... setup
    }
    
    public void ShowSettings()
    {
        var project = UiLoader.Load("Settings.xml");
        _settings = new SettingsUI();
        _settings.Initialize(project.Root);
        // ... setup
    }
}
```

### Pattern 2: Event Handler Organization

Keep event handlers organized:

```csharp
public class MainMenuScreen
{
    private MainMenuUI _ui;
    
    public void Initialize()
    {
        var project = UiLoader.Load("MainMenu.xml");
        _ui = new MainMenuUI();
        _ui.Initialize(project.Root);
        
        SetupEventHandlers();
    }
    
    private void SetupEventHandlers()
    {
        _ui.NewGameButton.Click += OnNewGame;
        _ui.LoadGameButton.Click += OnLoadGame;
        _ui.SettingsButton.Click += OnSettings;
        _ui.ExitButton.Click += OnExit;
    }
    
    private void OnNewGame(object sender, EventArgs e) { /* ... */ }
    private void OnLoadGame(object sender, EventArgs e) { /* ... */ }
    private void OnSettings(object sender, EventArgs e) { /* ... */ }
    private void OnExit(object sender, EventArgs e) { /* ... */ }
}
```

### Pattern 3: Widget Initialization

Initialize widgets with default values:

```csharp
public void InitializeUI()
{
    var project = UiLoader.Load("GameHUD.xml");
    _hud = new GameHUDUI();
    _hud.Initialize(project.Root);
    
    // Set initial values
    _hud.HealthBar.Value = 100;
    _hud.ScoreLabel.Text = "0";
    _hud.AmmoLabel.Text = "30";
}
```

### Pattern 4: Multiple UI Files

Handle multiple UI files in one class:

```csharp
public class GameUI
{
    private MainMenuUI _mainMenu;
    private PauseMenuUI _pauseMenu;
    private GameHUDUI _hud;
    
    public void LoadAll()
    {
        LoadMainMenu();
        LoadPauseMenu();
        LoadHUD();
    }
    
    private void LoadMainMenu()
    {
        var project = UiLoader.Load("MainMenu.xml");
        _mainMenu = new MainMenuUI();
        _mainMenu.Initialize(project.Root);
    }
    
    // ... similar for other UIs
}
```

### Pattern 5: Extending Generated Classes

Use partial classes to extend generated code:

```csharp
// In your own file: MainMenuUI.cs
public partial class MainMenuUI
{
    public void Show()
    {
        // Custom logic
    }
    
    public void Hide()
    {
        // Custom logic
    }
}
```

## Best Practices

### Naming Conventions

1. **XML Files**: Use PascalCase (e.g., `MainMenu.xml`, `SettingsScreen.xml`)
2. **Widget IDs**: Use PascalCase (e.g., `StartButton`, `HealthBar`, `ScoreLabel`)
3. **Namespaces**: Use your project's namespace convention

### File Organization

```
Content/
└── UI/
    ├── Menus/
    │   ├── MainMenu.xml
    │   └── PauseMenu.xml
    ├── Screens/
    │   ├── TitleScreen.xml
    │   └── GameOverScreen.xml
    └── HUD/
        └── GameHUD.xml
```

### When to Use IDs

Only add `Id` attributes to widgets you need to access from code:

- ✅ Buttons you'll attach click handlers to
- ✅ Labels you'll update dynamically
- ✅ Input fields you'll read values from
- ✅ Progress bars you'll update
- ❌ Static decorative elements
- ❌ Layout containers you don't access directly

### Error Handling

Always check for null after initialization:

```csharp
_ui.Initialize(project.Root);

if (_ui.StartButton == null)
{
    throw new InvalidOperationException("StartButton not found in UI");
}
```

Or use null-conditional operators:

```csharp
_ui.StartButton?.Click += OnStartClicked;
```

### Performance Tips

1. **Load Once**: Load XML files once and reuse the `Project` object
2. **Cache Instances**: Reuse UI class instances when possible
3. **Lazy Initialization**: Only initialize UI when needed
4. **Batch Updates**: Update multiple widgets together when possible

### Code Organization

```csharp
// Good: Organized by responsibility
public class MainMenuScreen
{
    private MainMenuUI _ui;
    
    public void Load() { /* ... */ }
    public void Update() { /* ... */ }
    public void Draw() { /* ... */ }
    private void SetupHandlers() { /* ... */ }
}

// Avoid: Everything in one method
public void DoEverything()
{
    // 200 lines of mixed initialization, handlers, and logic
}
```

## Next Steps

- See [Examples](Examples) for complete working code
- Read the [API Reference](Reference#api-reference)
- Check [Supported Widgets](Reference#supported-widgets)
- Learn [Advanced Topics](Advanced)

