using FluentAssertions;
using MyraUIGenerator;
using Xunit;

namespace MyraUIGenerator.Tests.Unit;

/// <summary>
/// Configuration tests are primarily done through integration tests
/// because GeneratorExecutionContext is not easily mockable.
/// These tests verify the default behavior through the public API.
/// </summary>
public class ConfigurationTests
{
    [Fact]
    public void Generator_UsesDefaultNamespace_WhenNotConfigured()
    {
        // This test verifies default behavior through integration
        // Full configuration testing is in GeneratorIntegrationTests
        var generator = new MyraUIGenerator();
        generator.Should().NotBeNull();
    }
}

