# MyraUIGenerator Tests

This test suite provides comprehensive testing for the Myra UI Generator source generator.

## Status

The test suite is currently being implemented. Some unit tests that directly access internal methods may need adjustment based on InternalsVisibleTo configuration.

## Test Structure

- **Unit Tests**: Test individual methods and components
- **Integration Tests**: Test the full generator execution through GeneratorDriver
- **Diagnostics Tests**: Verify diagnostic reporting

## Running Tests

```bash
dotnet test
```

## Test Categories

### Unit Tests
- Configuration tests (basic setup)
- Widget extraction tests (via integration - ExtractWidgets is internal)
- Code generation tests (via integration - GenerateUIClass is internal)

### Integration Tests
- Full generator execution
- Generated code compilation
- Multiple file processing
- Error handling

### Diagnostics Tests
- Verify all diagnostic codes are reported correctly

## Known Issues

Some unit tests may need to be refactored to use integration test approach if InternalsVisibleTo access doesn't work as expected. The internal methods are exposed for testing, but there may be assembly loading issues that need to be resolved.

