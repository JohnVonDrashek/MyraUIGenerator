# Contributing

Thank you for your interest in contributing to Myra UI Generator!

## Development Setup

### Prerequisites

- .NET SDK 6.0 or later
- Visual Studio 2019/2022, VS Code, or JetBrains Rider
- Git

### Getting the Source

1. **Fork the Repository**:
   - Go to https://github.com/JohnVonDrashek/MyraUIGenerator
   - Click "Fork" to create your own copy

2. **Clone Your Fork**:
   ```bash
   git clone https://github.com/YOUR_USERNAME/MyraUIGenerator.git
   cd MyraUIGenerator
   ```

3. **Add Upstream Remote**:
   ```bash
   git remote add upstream https://github.com/JohnVonDrashek/MyraUIGenerator.git
   ```

### Project Structure

```
MyraUIGenerator/
├── src/
│   └── MyraUIGenerator/
│       ├── MyraUIGenerator.cs          # Main generator code
│       └── MyraUIGenerator.csproj      # Project file
├── wiki/                               # Documentation
├── README.md                           # Main readme
└── LICENSE                             # MIT License
```

### Opening in IDE

**Visual Studio**:
- Open `MyraUIGenerator.sln` (if exists) or `src/MyraUIGenerator/MyraUIGenerator.csproj`

**VS Code**:
- Open the repository root
- Install C# extension
- Open `src/MyraUIGenerator/MyraUIGenerator.csproj`

**Rider**:
- Open the repository root
- Rider will detect the project automatically

### Restoring Dependencies

```bash
cd src/MyraUIGenerator
dotnet restore
```

## Building

### Build the Project

```bash
cd src/MyraUIGenerator
dotnet build
```

### Build in Release Mode

```bash
dotnet build --configuration Release
```

### Create NuGet Package

```bash
dotnet pack --configuration Release
```

Package will be created in:
```
bin/Release/MyraUIGenerator.{version}.nupkg
```

### Testing Locally

To test the generator locally:

1. **Create a Test Project**:
   ```bash
   mkdir ../TestProject
   cd ../TestProject
   dotnet new console
   ```

2. **Reference the Generator**:
   ```xml
   <ItemGroup>
     <ProjectReference Include="../MyraUIGenerator/MyraUIGenerator.csproj" />
   </ItemGroup>
   ```

3. **Or Use Local Package**:
   ```bash
   dotnet add package MyraUIGenerator --source ../MyraUIGenerator/bin/Release
   ```

4. **Test with XML Files**:
   - Create test XML files
   - Add as `AdditionalFiles`
   - Build and verify generated code

## Testing

### Manual Testing

1. Create test XML files with various widget types
2. Build the test project
3. Verify generated code matches expectations
4. Test edge cases (missing IDs, unknown widgets, etc.)

### Test Scenarios

**Basic Functionality**:
- Single widget with ID
- Multiple widgets
- Different widget types
- Nested widgets

**Configuration**:
- Custom namespace
- Custom directory
- MSBuild properties
- .editorconfig

**Edge Cases**:
- XML without IDs
- Unknown widget types
- Invalid XML (should handle gracefully)
- Missing XML files
- Empty XML files

### Testing Checklist

- [ ] Generator runs without errors
- [ ] Generated code compiles
- [ ] Properties match widget IDs
- [ ] Types are correct for each widget
- [ ] Namespace is configured correctly
- [ ] Diagnostic messages are helpful
- [ ] Error handling works for invalid input

## Development Workflow

### Making Changes

1. **Create a Branch**:
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Make Changes**:
   - Edit source files
   - Update documentation if needed
   - Test your changes

3. **Commit Changes**:
   ```bash
   git add .
   git commit -m "Description of changes"
   ```

4. **Push to Your Fork**:
   ```bash
   git push origin feature/your-feature-name
   ```

5. **Create Pull Request**:
   - Go to your fork on GitHub
   - Click "New Pull Request"
   - Select your branch
   - Fill out the PR template

### Code Style

- Follow C# coding conventions
- Use meaningful variable names
- Add XML documentation comments
- Keep methods focused and small
- Handle errors gracefully

### Commit Messages

Use clear, descriptive commit messages:

```
Add support for CustomWidget type

- Added CustomWidget to widget type mapping
- Updated generated code to use CustomWidget type
- Added test case for custom widgets
```

## Areas for Contribution

### New Widget Types

To add support for a new widget type:

1. **Update Widget Type Mapping**:
   In `MyraUIGenerator.cs`, the `ExtractWidgets` method determines widget types. The type is based on the XML element name, so most widgets are automatically supported if they match Myra's naming.

2. **Test the Widget**:
   - Create test XML with the new widget
   - Verify generated code uses correct type
   - Test with various scenarios

3. **Update Documentation**:
   - Add to [Supported Widgets](Reference#supported-widgets) table
   - Add example if needed

### Bug Fixes

1. **Identify the Bug**:
   - Reproduce the issue
   - Understand the root cause
   - Check existing issues

2. **Fix the Bug**:
   - Make minimal changes
   - Add tests if possible
   - Verify fix works

3. **Submit PR**:
   - Reference the issue number
   - Describe the fix
   - Include test cases

### Documentation

Documentation improvements are always welcome:

- Fix typos or errors
- Add missing information
- Improve examples
- Add new examples
- Clarify confusing sections

### Performance

If you identify performance improvements:

- Profile the code
- Make optimizations
- Verify no regressions
- Document improvements

## Pull Request Process

### Before Submitting

- [ ] Code builds without errors
- [ ] Code follows style guidelines
- [ ] Changes are tested
- [ ] Documentation is updated if needed
- [ ] Commit messages are clear

### PR Description

Include:
- What changes were made
- Why the changes were needed
- How to test the changes
- Any breaking changes

### Review Process

1. Maintainers will review your PR
2. Address any feedback
3. Make requested changes
4. PR will be merged when approved

## Reporting Issues

### Before Reporting

1. Check existing issues
2. Verify it's a real bug
3. Try to reproduce
4. Gather information

### Issue Template

When reporting issues, include:

- **Description**: What happened
- **Expected Behavior**: What should happen
- **Steps to Reproduce**: How to reproduce
- **Configuration**: `.editorconfig` or MSBuild settings
- **XML Example**: Sample XML file (if applicable)
- **Diagnostic Messages**: Any `MYRA` codes from build output
- **Environment**: .NET version, IDE, OS

### Feature Requests

For new features:

- Describe the feature
- Explain the use case
- Provide examples if possible
- Discuss implementation approach

## Code of Conduct

- Be respectful and inclusive
- Welcome newcomers
- Focus on constructive feedback
- Help others learn

## License

By contributing, you agree that your contributions will be licensed under the MIT License.

## Questions?

- Open an issue for questions
- Check existing documentation
- Review closed issues/PRs for examples

Thank you for contributing to Myra UI Generator!

