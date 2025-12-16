# Troubleshooting

Common issues and solutions when using Myra UI Generator.

## Common Issues

### No Generated Files

**Symptoms**:
- No `.g.cs` files appear after building
- IntelliSense doesn't show generated classes
- Compilation errors about missing types

**Possible Causes**:
1. XML files not included as `AdditionalFiles`
2. XML files not in configured directory
3. No widgets with `Id` attributes in XML
4. Generator not running (check diagnostics)

**Solutions**:

1. **Verify AdditionalFiles**:
   ```xml
   <ItemGroup>
     <AdditionalFiles Include="Content/UI/*.xml" />
   </ItemGroup>
   ```

2. **Check Directory Configuration**:
   ```ini
   # .editorconfig
   myra_ui_generator.xml_directory = Content/UI
   ```
   Ensure XML files are actually in this directory.

3. **Add Id Attributes**:
   ```xml
   <!-- Widgets need Id to be generated -->
   <Button Id="MyButton" Content="Click Me" />
   ```

4. **Check Build Output**:
   Look for `MYRA002` and `MYRA003` diagnostic messages to see if generator is running.

5. **Rebuild Project**:
   Clean and rebuild: `dotnet clean && dotnet build`

### Wrong Namespace

**Symptoms**:
- Generated classes in unexpected namespace
- `using` statements don't work
- Namespace doesn't match configuration

**Solutions**:

1. **Check .editorconfig**:
   ```ini
   [*.cs]
   myra_ui_generator.namespace = YourNamespace.UI.Generated
   ```

2. **Verify File Location**:
   `.editorconfig` must be in project root (same directory as `.csproj`)

3. **Restart IDE**:
   Some IDEs cache configuration - restart after changing `.editorconfig`

4. **Check MSBuild Properties**:
   If using MSBuild properties, they take precedence over `.editorconfig`

5. **Rebuild**:
   Configuration is read at build time - rebuild after changes

### Widgets Not Found

**Symptoms**:
- Properties are `null` after `Initialize()`
- Runtime errors when accessing widgets
- Widgets exist in XML but not accessible

**Possible Causes**:
1. Widget doesn't have `Id` attribute
2. `Id` value doesn't match property name
3. Widget type mismatch
4. Widget not in root hierarchy

**Solutions**:

1. **Add Id Attribute**:
   ```xml
   <!-- Correct -->
   <Button Id="StartButton" Content="Start" />

   <!-- Missing Id - won't be generated -->
   <Button Content="Start" />
   ```

2. **Check Property Names**:
   Property names match `Id` exactly (case-sensitive):
   ```xml
   <Button Id="StartButton" />  <!-- Property: StartButton -->
   ```

3. **Verify Widget Type**:
   Ensure XML element name matches supported widget type (see [Supported Widgets](Reference#supported-widgets))

4. **Check XML Structure**:
   Widgets must be descendants of the root `<Project>` element

5. **Null Checks**:
   Always check for null:
   ```csharp
   _ui.Initialize(project.Root);
   if (_ui.StartButton == null)
   {
       // Widget not found
   }
   ```

### Build Errors

**Symptoms**:
- Compilation errors related to generated code
- Type not found errors
- Missing assembly references

**Solutions**:

1. **Check Myra Reference**:
   Ensure Myra package is referenced:
   ```xml
   <PackageReference Include="Myra" Version="1.4.0" />
   ```

2. **Verify Target Framework**:
   Project must target .NET Standard 2.0 or later

3. **Check Generated Code**:
   Look at generated files in `obj/` directory to see what was generated

4. **Clean and Rebuild**:
   ```bash
   dotnet clean
   dotnet build
   ```

### IntelliSense Not Working

**Symptoms**:
- No autocomplete for generated classes
- Red squiggles on valid code
- "Type not found" errors in IDE

**Solutions**:

1. **Rebuild Project**:
   Generated code must be built before IntelliSense works

2. **Restart IDE**:
   Some IDEs need restart to recognize new generated files

3. **Check Generated Files**:
   Verify files exist in `obj/Debug/{framework}/generated/`

4. **Verify Namespace**:
   Ensure `using` statement matches configured namespace

5. **Reload Project**:
   In Visual Studio: Right-click project → Reload Project

## Debugging

### Enabling Detailed Diagnostics

**Visual Studio**:
1. Tools → Options → Projects and Solutions → Build and Run
2. Set "MSBuild project build output verbosity" to "Detailed" or "Diagnostic"

**Command Line**:
```bash
dotnet build --verbosity detailed
```

### Reading Diagnostic Messages

Look for messages starting with `MYRA`:

- **MYRA002**: Generator is running (shows configuration)
- **MYRA003**: XML files found (shows count)
- **MYRA004**: Class generated successfully
- **MYRA005**: No widgets found in XML
- **MYRA001**: Error processing a file
- **MYRA999**: Generator exception

### Checking Generated Files

**Location**:
```
obj/Debug/{TargetFramework}/generated/MyraUIGenerator/MyraUIGenerator.MyraUIGenerator/
```

**To View**:
1. Show all files in Solution Explorer
2. Navigate to `obj` folder
3. Expand to find generated files

**Or Use "Go to Definition"**:
Right-click on generated class name → Go to Definition

### Verifying Configuration

**Check .editorconfig**:
```ini
[*.cs]
myra_ui_generator.namespace = YourNamespace.UI.Generated
myra_ui_generator.xml_directory = Content/UI
```

**Verify in Build Output**:
Look for `MYRA002` message - it shows the actual values being used.

### Testing XML Files

**Validate XML Syntax**:
- Use an XML validator
- Check for well-formed XML
- Ensure proper encoding (UTF-8)

**Check Myra Format**:
- Verify XML matches Myra UI format
- Check widget element names
- Ensure proper nesting

## Frequently Asked Questions

### Can I edit generated files?

**No**. Generated files are automatically created and will be overwritten on each build. If you need to extend functionality, use partial classes:

```csharp
public partial class MainMenuUI
{
    // Your custom code here
}
```

### How do I handle custom widget types?

Unknown widget types are generated as `Widget`. You can cast to your specific type:

```csharp
var custom = _ui.CustomWidget as MyCustomWidgetType;
```

See [Custom Widget Types](Advanced#custom-widget-types) for more details.

### Can I use this with other UI frameworks?

No. Myra UI Generator is specifically designed for Myra UI and requires Myra's widget types and XML format.

### Why aren't my XML files being found?

Check:
1. Files are included as `AdditionalFiles` in `.csproj`
2. File paths match `xml_directory` configuration
3. Files have `.xml` extension
4. Look for `MYRA003` diagnostic message

### How do I change the generated class name?

The class name is based on the XML filename:
- `MainMenu.xml` → `MainMenuUI`
- `Settings.xml` → `SettingsUI`

Rename the XML file to change the class name.

### Can I generate multiple classes from one XML?

No. Each XML file generates one class. If you need multiple classes, split into multiple XML files.

### Does this work with .NET Framework?

Yes, if targeting .NET Standard 2.0 or later. The generator itself targets .NET Standard 2.0.

### How do I update the generator?

Update the NuGet package:

```bash
dotnet add package MyraUIGenerator --version {new-version}
```

Or update in `.csproj`:
```xml
<PackageReference Include="MyraUIGenerator" Version="1.0.9" />
```

### Can I use this in a library project?

Yes. Generated classes will be in the library's namespace and can be used by projects that reference the library.

### Why is my property null?

Possible reasons:
1. Widget not found (check `Id` attribute)
2. Widget type mismatch
3. `Initialize()` not called
4. Widget not in XML hierarchy

Always check for null:
```csharp
if (_ui.MyButton == null)
{
    // Handle missing widget
}
```

### How do I debug the generator itself?

1. Check diagnostic messages (`MYRA` codes)
2. Look at generated files in `obj/` directory
3. Verify XML files are valid
4. Check configuration values in build output
5. Report issues on GitHub with diagnostic messages

## Getting Help

If you're still experiencing issues:

1. **Check Documentation**: Review [Reference](Reference) and [Examples](Examples)
2. **Search Issues**: Check [GitHub Issues](https://github.com/JohnVonDrashek/MyraUIGenerator/issues)
3. **Report Bug**: Open a new issue with:
   - Diagnostic messages (`MYRA` codes)
   - XML file examples
   - Configuration files
   - Project structure
   - Expected vs actual behavior

## Next Steps

- Review [Getting Started](Getting-Started) if you're new
- Check [Examples](Examples) for working code
- Read [Advanced Topics](Advanced) for complex scenarios

