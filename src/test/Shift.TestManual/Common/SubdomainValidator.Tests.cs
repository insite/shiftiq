using Shift.Common;

namespace Shift.Test.Common;

public class SubdomainValidatorTests
{
    private EnvironmentModel Production => Environments.Production;
    private EnvironmentModel Sandbox => Environments.Sandbox;
    private EnvironmentModel Development => Environments.Development;
    private EnvironmentModel Local => Environments.Local;

    [Fact]
    public void Validate_ProductionWithValidHost_ReturnsTrue()
    {
        var validator = new SubdomainValidator();

        Assert.True(validator.Validate(Production, "abc.example.com"));
    }

    [Fact]
    public void Validate_ProductionWithInvalidHost_ReturnsFalse()
    {
        var validator = new SubdomainValidator();

        Assert.False(validator.Validate(Production, "local-abc.example.com"));
        Assert.False(validator.Validate(Production, "dev-abc.example.com"));
        Assert.False(validator.Validate(Production, "sandbox-abc.example.com"));
    }

    [Fact]
    public void Validate_SandboxWithValidHost_ReturnsTrue()
    {
        var validator = new SubdomainValidator();

        Assert.True(validator.Validate(Sandbox, "sandbox-abc.example.com"));
    }

    [Fact]
    public void Validate_SandboxWithInvalidHost_ReturnsFalse()
    {
        var validator = new SubdomainValidator();

        Assert.False(validator.Validate(Sandbox, "local-abc.example.com"));
        Assert.False(validator.Validate(Sandbox, "dev-abc.example.com"));
        Assert.False(validator.Validate(Sandbox, "abc.example.com"));
    }

    [Fact]
    public void Validate_DevelopmentWithValidHost_ReturnsTrue()
    {
        var validator = new SubdomainValidator();

        Assert.True(validator.Validate(Development, "dev-abc.example.com"));
    }

    [Fact]
    public void Validate_DevelopmentWithInvalidHost_ReturnsFalse()
    {
        var validator = new SubdomainValidator();

        Assert.False(validator.Validate(Development, "local-abc.example.com"));
        Assert.False(validator.Validate(Development, "sandbox-abc.example.com"));
        Assert.False(validator.Validate(Development, "abc.example.com"));
    }

    [Fact]
    public void Validate_LocalWithValidHost_ReturnsTrue()
    {
        var validator = new SubdomainValidator();

        Assert.True(validator.Validate(Local, "local-abc.example.com"));
    }

    [Fact]
    public void Validate_LocalWithInvalidHost_ReturnsFalse()
    {
        var validator = new SubdomainValidator();

        Assert.False(validator.Validate(Local, "dev-abc.example.com"));
        Assert.False(validator.Validate(Local, "sandbox-abc.example.com"));
        Assert.False(validator.Validate(Local, "abc.example.com"));
    }

    [Fact]
    public void Validate_ProductionWithUnusualHost_ReturnsTrue()
    {
        var validator = new SubdomainValidator();

        // "devon" is an example of a subdomain for a Production environment that starts with "dev" but is NOT an
        // environment prefix.

        Assert.True(validator.Validate(Production, "devon.example.com"));
        Assert.False(validator.Validate(Development, "devon.example.com"));
    }
}