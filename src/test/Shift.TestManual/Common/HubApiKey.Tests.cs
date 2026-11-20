using System.Net.Http;

using Shift.Common.Integration;

namespace Shift.Test.Common;

public class HubApiKeyTests
{
    [Theory]
    [InlineData("/partitions")]
    [InlineData("/api/partitions")]
    [InlineData("/partitions/")]
    [InlineData("/google/location/countries")]
    [InlineData("/google/location/countries/ca/provinces")]
    [InlineData("/google/translation/translate")]
    [InlineData("/google/contact/locations/countries")]
    [InlineData("/google/content/translations/translate")]
    [InlineData("/premailer/execute")]
    [InlineData("/imagemagick/info")]
    [InlineData("/imagemagick/adjust")]
    [InlineData("/Google/location/countries")]
    [InlineData("/google/error")]
    public void IsContractPath_ContractRoutes_RequireTheKey(string path)
    {
        Assert.True(HubApiKey.IsContractPath(path));
    }

    [Theory]
    [InlineData("/google/status")]
    [InlineData("/google/health")]
    [InlineData("/premailer/version")]
    [InlineData("/imagemagick/health/")]
    [InlineData("/status")]
    [InlineData("/health")]
    [InlineData("/version")]
    [InlineData("/deadend/index.html")]
    [InlineData("/google")]
    [InlineData("/googlex/location/countries")]
    [InlineData("/partitions/extra")]
    [InlineData("")]
    [InlineData(null)]
    public void IsContractPath_OtherRoutes_AreOpen(string? path)
    {
        Assert.False(HubApiKey.IsContractPath(path));
    }

    [Fact]
    public void Matches_TheSameKey_ReturnsTrue()
    {
        Assert.True(HubApiKey.Matches("hub-key", "hub-key"));
    }

    [Theory]
    [InlineData("hub-ke", "hub-key")]
    [InlineData("hub-key-", "hub-key")]
    [InlineData("HUB-KEY", "hub-key")]
    [InlineData("", "hub-key")]
    [InlineData(null, "hub-key")]
    [InlineData("anything", "")]
    [InlineData("", "")]
    public void Matches_AnythingElse_ReturnsFalse(string? presented, string expected)
    {
        Assert.False(HubApiKey.Matches(presented, expected));
    }

    [Theory]
    [InlineData("/status", "", "hub-key", HubApiKey.Check.NotRequired)]
    [InlineData("/google/health", "", "hub-key", HubApiKey.Check.NotRequired)]
    [InlineData("/partitions", "hub-key", "hub-key", HubApiKey.Check.Valid)]
    [InlineData("/premailer/execute", "", "hub-key", HubApiKey.Check.Missing)]
    [InlineData("/premailer/execute", null, "hub-key", HubApiKey.Check.Missing)]
    [InlineData("/imagemagick/info", "wrong", "hub-key", HubApiKey.Check.Invalid)]
    [InlineData("/google/location/countries", "", "", HubApiKey.Check.Unconfigured)]
    [InlineData("/google/location/countries", "anything", null, HubApiKey.Check.Unconfigured)]
    public void Evaluate_ClassifiesTheRequest(string path, string? presented, string? configured, HubApiKey.Check expected)
    {
        Assert.Equal(expected, HubApiKey.Evaluate(path, presented, configured));
    }

    [Fact]
    public void Apply_SetsTheHeader()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://hub.example/partitions");

        HubApiKey.Apply(request, "hub-key");

        Assert.Equal("hub-key", Assert.Single(request.Headers.GetValues(HubApiKey.HeaderName)));
    }

    [Fact]
    public void Apply_WithoutAKey_SendsNoHeader()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://hub.example/partitions");

        HubApiKey.Apply(request, null);

        Assert.False(request.Headers.Contains(HubApiKey.HeaderName));
    }
}
