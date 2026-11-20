using Shift.Common;

namespace Shift.Test.Common
{
    public class UrlHelperTests
    {
        private const string Domain = "cmds.app";
        private const string OrganizationCode = "orlen";
        private const EnvironmentName Environment = EnvironmentName.Development;

        // The subdomain prefix depends on the convention active for the deployment, so the
        // expected host is derived rather than hardcoded. What is under test here is the
        // path joining and the already-absolute guard, not the prefix table.
        private static string ExpectedHost()
        {
            var prefix = SubdomainConvention.GetPrefix(Environment);
            return $"https://{prefix}{OrganizationCode}.{Domain}";
        }

        [Fact]
        public void GetAbsoluteUrl_RootRelativePath_PrefixesOrganizationHost()
        {
            var url = UrlHelper.GetAbsoluteUrl(Domain, Environment, "/files/images/logo/orlen.png", OrganizationCode);
            Assert.Equal($"{ExpectedHost()}/files/images/logo/orlen.png", url);
        }

        [Fact]
        public void GetAbsoluteUrl_ApplicationRelativePath_PrefixesOrganizationHost()
        {
            var url = UrlHelper.GetAbsoluteUrl(Domain, Environment, "~/files/images/logo/orlen.png", OrganizationCode);
            Assert.Equal($"{ExpectedHost()}/files/images/logo/orlen.png", url);
        }

        [Fact]
        public void GetAbsoluteUrl_AlreadyAbsoluteUrl_ReturnsInputUnchanged()
        {
            var input = "https://assets.cmds.app/img/logos/partners/orlen.png";
            var url = UrlHelper.GetAbsoluteUrl(Domain, Environment, input, OrganizationCode);
            Assert.Equal(input, url);
        }

        [Fact]
        public void GetAbsoluteUrl_ProtocolRelativeUrl_ReturnsInputUnchanged()
        {
            var input = "//assets.cmds.app/img/logos/partners/orlen.png";
            var url = UrlHelper.GetAbsoluteUrl(Domain, Environment, input, OrganizationCode);
            Assert.Equal(input, url);
        }

        [Fact]
        public void GetAbsoluteUrl_MixedCaseScheme_ReturnsInputUnchanged()
        {
            var input = "HTTPS://assets.cmds.app/img/logos/partners/orlen.png";
            var url = UrlHelper.GetAbsoluteUrl(Domain, Environment, input, OrganizationCode);
            Assert.Equal(input, url);
        }

        [Fact]
        public void GetAbsoluteUrl_NullPath_ReturnsNull()
        {
            var url = UrlHelper.GetAbsoluteUrl(Domain, Environment, null, OrganizationCode);
            Assert.Null(url);
        }

        [Fact]
        public void GetAbsoluteUrl_ServerUrlOverload_AlreadyAbsoluteUrl_ReturnsInputUnchanged()
        {
            var input = "https://assets.cmds.app/img/logos/partners/orlen.png";
            var url = UrlHelper.GetAbsoluteUrl("https://dev-orlen.cmds.app/anything", "/", input);
            Assert.Equal(input, url);
        }

        [Fact]
        public void GetAbsoluteUrl_ServerUrlOverload_RootRelativePath_PrefixesServerHost()
        {
            var url = UrlHelper.GetAbsoluteUrl("https://dev-orlen.cmds.app/anything", "/", "/files/images/logo/orlen.png");
            Assert.Equal("https://dev-orlen.cmds.app/files/images/logo/orlen.png", url);
        }

        [Fact]
        public void IsAbsoluteUrl_DataUri_ReturnsTrue()
        {
            var absolute = UrlHelper.IsAbsoluteUrl("data:image/png;base64,AAAA");
            Assert.True(absolute);
        }

        [Fact]
        public void IsAbsoluteUrl_RelativePath_ReturnsFalse()
        {
            var absolute = UrlHelper.IsAbsoluteUrl("/files/images/logo/orlen.png");
            Assert.False(absolute);
        }
    }
}
