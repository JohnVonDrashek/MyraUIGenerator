# Examples

Complete working examples demonstrating Myra UI Generator usage.

## Simple Button Example

A minimal example with a single button.

### XML File: `Content/UI/SimpleButton.xml`

```xml
<Project>
  <VerticalStackPanel>
    <Label Id="MessageLabel" Text="Click the button!" />
    <TextButton Id="ClickMeButton" Text="Click Me" />
  </VerticalStackPanel>
</Project>
```

### Project Configuration: `.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="Myra" Version="1.4.0" />
    <PackageReference Include="MyraUIGenerator" Version="1.0.9" />
  </ItemGroup>
  
  <ItemGroup>
    <AdditionalFiles Include="Content/UI/*.xml" />
  </ItemGroup>
</Project>
```

### Configuration: `.editorconfig`

```ini
[*.cs]
myra_ui_generator.namespace = MyGame.UI.Generated
myra_ui_generator.xml_directory = Content/UI
```

### C# Usage Code

```csharp
using MyGame.UI.Generated;
using Myra.Graphics2D.UI;
using Myra.Graphics2D;

public class SimpleButtonExample
{
    private SimpleButtonUI _ui;
    
    public void Load()
    {
        // Load the XML
        var project = UiLoader.Load("SimpleButton.xml");
        
        // Create and initialize the generated class
        _ui = new SimpleButtonUI();
        _ui.Initialize(project.Root);
        
        // Set up event handler
        _ui.ClickMeButton.Click += OnButtonClicked;
    }
    
    private void OnButtonClicked(object sender, EventArgs e)
    {
        // Update the label when button is clicked
        _ui.MessageLabel.Text = "Button was clicked!";
    }
    
    public void Draw()
    {
        // Render the UI (implementation depends on your setup)
        // Typically: _desktop.Render();
    }
}
```

### Generated Code (Automatic)

After building, `SimpleButtonUI.g.cs` is generated:

```csharp
namespace MyGame.UI.Generated;

public partial class SimpleButtonUI
{
    public Label MessageLabel { get; private set; }
    public TextButton ClickMeButton { get; private set; }
    
    public void Initialize(Widget root)
    {
        MessageLabel = root.FindChildById("MessageLabel") as Label;
        ClickMeButton = root.FindChildById("ClickMeButton") as TextButton;
    }
}
```

## Complex UI Example

A more realistic example with multiple widget types and nested layouts.

### XML File: `Content/UI/GameHUD.xml`

```xml
<Project>
  <Grid>
    <!-- Top Bar -->
    <Label Id="ScoreLabel" GridRow="0" GridColumn="0" Text="Score: 0" />
    <Label Id="LevelLabel" GridRow="0" GridColumn="1" Text="Level: 1" />
    <ProgressBar Id="HealthBar" GridRow="0" GridColumn="2" Maximum="100" Value="100" />
    
    <!-- Center Area -->
    <Panel Id="GameArea" GridRow="1" GridColumn="0" GridColumnSpan="3">
      <!-- Game content rendered here -->
    </Panel>
    
    <!-- Bottom Controls -->
    <HorizontalStackPanel GridRow="2" GridColumn="0" GridColumnSpan="3">
      <TextButton Id="PauseButton" Text="Pause" />
      <TextButton Id="MenuButton" Text="Menu" />
      <Label Id="AmmoLabel" Text="Ammo: 30" />
    </HorizontalStackPanel>
  </Grid>
</Project>
```

### C# Usage Code

```csharp
using MyGame.UI.Generated;
using Myra.Graphics2D.UI;
using Myra.Graphics2D;

public class GameHUD
{
    private GameHUDUI _ui;
    private int _score = 0;
    private int _health = 100;
    private int _ammo = 30;
    
    public void Load()
    {
        var project = UiLoader.Load("GameHUD.xml");
        _ui = new GameHUDUI();
        _ui.Initialize(project.Root);
        
        // Set initial values
        UpdateUI();
        
        // Set up event handlers
        _ui.PauseButton.Click += OnPauseClicked;
        _ui.MenuButton.Click += OnMenuClicked;
    }
    
    public void UpdateScore(int points)
    {
        _score += points;
        _ui.ScoreLabel.Text = $"Score: {_score}";
    }
    
    public void UpdateHealth(int health)
    {
        _health = Math.Max(0, Math.Min(100, health));
        _ui.HealthBar.Value = _health;
    }
    
    public void UpdateAmmo(int ammo)
    {
        _ammo = ammo;
        _ui.AmmoLabel.Text = $"Ammo: {_ammo}";
    }
    
    public void UpdateLevel(int level)
    {
        _ui.LevelLabel.Text = $"Level: {level}";
    }
    
    private void UpdateUI()
    {
        _ui.ScoreLabel.Text = $"Score: {_score}";
        _ui.HealthBar.Value = _health;
        _ui.AmmoLabel.Text = $"Ammo: {_ammo}";
        _ui.LevelLabel.Text = "Level: 1";
    }
    
    private void OnPauseClicked(object sender, EventArgs e)
    {
        // Handle pause
    }
    
    private void OnMenuClicked(object sender, EventArgs e)
    {
        // Return to main menu
    }
    
    public Widget Root => _ui.HealthBar.Parent; // Get root for rendering
}
```

## Multiple Screens Example

Managing multiple UI screens in a game.

### XML Files

**`Content/UI/MainMenu.xml`**:
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

**`Content/UI/Settings.xml`**:
```xml
<Project>
  <VerticalStackPanel>
    <Label Id="TitleLabel" Text="Settings" />
    <CheckBox Id="FullscreenCheckBox" Text="Fullscreen" />
    <CheckBox Id="SoundCheckBox" Text="Sound Enabled" />
    <Slider Id="VolumeSlider" Minimum="0" Maximum="100" Value="50" />
    <TextButton Id="BackButton" Text="Back" />
  </VerticalStackPanel>
</Project>
```

**`Content/UI/PauseMenu.xml`**:
```xml
<Project>
  <VerticalStackPanel>
    <Label Id="TitleLabel" Text="Game Paused" />
    <TextButton Id="ResumeButton" Text="Resume" />
    <TextButton Id="SettingsButton" Text="Settings" />
    <TextButton Id="QuitButton" Text="Quit to Menu" />
  </VerticalStackPanel>
</Project>
```

### Screen Manager

```csharp
using MyGame.UI.Generated;
using Myra.Graphics2D.UI;
using Myra.Graphics2D;

public enum ScreenType
{
    MainMenu,
    Settings,
    PauseMenu,
    GameHUD
}

public class ScreenManager
{
    private MainMenuUI _mainMenu;
    private SettingsUI _settings;
    private PauseMenuUI _pauseMenu;
    private GameHUDUI _hud;
    
    private ScreenType _currentScreen = ScreenType.MainMenu;
    private Widget _currentRoot;
    
    public void LoadAllScreens()
    {
        LoadMainMenu();
        LoadSettings();
        LoadPauseMenu();
        LoadHUD();
    }
    
    private void LoadMainMenu()
    {
        var project = UiLoader.Load("MainMenu.xml");
        _mainMenu = new MainMenuUI();
        _mainMenu.Initialize(project.Root);
        
        _mainMenu.NewGameButton.Click += OnNewGame;
        _mainMenu.LoadGameButton.Click += OnLoadGame;
        _mainMenu.SettingsButton.Click += () => ShowScreen(ScreenType.Settings);
        _mainMenu.ExitButton.Click += OnExit;
    }
    
    private void LoadSettings()
    {
        var project = UiLoader.Load("Settings.xml");
        _settings = new SettingsUI();
        _settings.Initialize(project.Root);
        
        _settings.BackButton.Click += () => ShowScreen(ScreenType.MainMenu);
        _settings.FullscreenCheckBox.IsChecked = IsFullscreen();
        _settings.SoundCheckBox.IsChecked = IsSoundEnabled();
        _settings.VolumeSlider.Value = GetVolume();
        
        _settings.FullscreenCheckBox.Click += OnFullscreenToggled;
        _settings.SoundCheckBox.Click += OnSoundToggled;
        _settings.VolumeSlider.ValueChanged += OnVolumeChanged;
    }
    
    private void LoadPauseMenu()
    {
        var project = UiLoader.Load("PauseMenu.xml");
        _pauseMenu = new PauseMenuUI();
        _pauseMenu.Initialize(project.Root);
        
        _pauseMenu.ResumeButton.Click += OnResume;
        _pauseMenu.SettingsButton.Click += () => ShowScreen(ScreenType.Settings);
        _pauseMenu.QuitButton.Click += OnQuitToMenu;
    }
    
    private void LoadHUD()
    {
        var project = UiLoader.Load("GameHUD.xml");
        _hud = new GameHUDUI();
        _hud.Initialize(project.Root);
        
        // Set up HUD event handlers
        _hud.PauseButton.Click += () => ShowScreen(ScreenType.PauseMenu);
    }
    
    public void ShowScreen(ScreenType screen)
    {
        _currentScreen = screen;
        
        switch (screen)
        {
            case ScreenType.MainMenu:
                _currentRoot = _mainMenu.NewGameButton.Parent;
                break;
            case ScreenType.Settings:
                _currentRoot = _settings.BackButton.Parent;
                break;
            case ScreenType.PauseMenu:
                _currentRoot = _pauseMenu.ResumeButton.Parent;
                break;
            case ScreenType.GameHUD:
                _currentRoot = _hud.HealthBar.Parent;
                break;
        }
    }
    
    public Widget CurrentRoot => _currentRoot;
    public ScreenType CurrentScreen => _currentScreen;
    
    // Event handlers
    private void OnNewGame() { /* Start new game */ }
    private void OnLoadGame() { /* Load saved game */ }
    private void OnExit() { /* Exit application */ }
    private void OnResume() { ShowScreen(ScreenType.GameHUD); }
    private void OnQuitToMenu() { ShowScreen(ScreenType.MainMenu); }
    private void OnFullscreenToggled(object sender, EventArgs e) { /* Toggle fullscreen */ }
    private void OnSoundToggled(object sender, EventArgs e) { /* Toggle sound */ }
    private void OnVolumeChanged(object sender, EventArgs e) { /* Update volume */ }
    
    // Helper methods
    private bool IsFullscreen() { return false; }
    private bool IsSoundEnabled() { return true; }
    private int GetVolume() { return 50; }
}
```

### Usage in Game Class

```csharp
public class Game
{
    private ScreenManager _screenManager;
    
    public void LoadContent()
    {
        _screenManager = new ScreenManager();
        _screenManager.LoadAllScreens();
        _screenManager.ShowScreen(ScreenType.MainMenu);
    }
    
    public void Draw()
    {
        // Render current screen
        var root = _screenManager.CurrentRoot;
        if (root != null)
        {
            // Render using your Myra desktop renderer
            // _desktop.Render();
        }
    }
}
```

## Next Steps

- Learn about [Common Patterns](Usage#common-patterns)
- Read [Best Practices](Usage#best-practices)
- Check [Advanced Topics](Advanced) for more complex scenarios

