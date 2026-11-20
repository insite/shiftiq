using Shift.Common;

namespace Shift.Test.Common
{
    public class EmailAddressTests
    {
        private const string Whitelist = "example1.com, example2.com";
        private string Testers = string.Empty;

        [Fact]
        public void Constructor_ValidAddress_AddressIsParsed()
        {
            var email = new EmailAddress("Someone@Example.COM");
            Assert.Equal("Someone@Example.COM", email.Address);
            Assert.Equal("someone", email.Mailbox);
            Assert.Equal("example.com", email.Domain);
        }

        [Fact]
        public void Constructor_InvalidAddress_AddressIsInvalid()
        {
            var email = new EmailAddress("SomeoneExample");
            Assert.Equal("SomeoneExample", email.Address);
            Assert.Null(email.Mailbox);
            Assert.Null(email.Domain);
            Assert.False(email.IsValid);
        }

        [Fact]
        public void Constructor_ValidAddressAndDisplayName_Success()
        {
            var email = new EmailAddress("someone@example.com", "Someone Fictitious");
            Assert.Equal("someone@example.com", email.Address);
            Assert.Equal("Someone Fictitious", email.DisplayName);
            Assert.Equal("Someone Fictitious <someone@example.com>", email.ToString());
        }

        [Fact]
        public void Filter_EmailContainsWhitelistedDomains_ReturnOnlyWhitelisted()
        {
            var result = EmailAddress.Filter("someone@example1.com, someone@hotmail.com, someone@example2.com", Whitelist, Testers);
            Assert.Equal(new[] { "someone@example1.com", "someone@example2.com" }, result);
        }

        [Fact]
        public void Filter_EmailContainsNoWhitelistedDomains_ReturnEmpty()
        {
            var result = EmailAddress.Filter("someone@hotmail.com; someone@example.com", Whitelist, Testers);
            Assert.Empty(result);
        }

        [Fact]
        public void Filter_EmailContainsInvalidAddress_ReturnOnlyValidAddresses()
        {
            var result = EmailAddress.Filter("someone@example1.com; blah-blah@blah, someone@example2.com;", Whitelist, Testers);
            Assert.Equal(new[] { "someone@example1.com", "someone@example2.com" }, result);
        }

        [Fact]
        public void Filter_EmailIsEmpty_ReturnEmpty()
        {
            var result = EmailAddress.Filter("  ", Whitelist, Testers);
            Assert.Empty(result);
        }

        [Fact]
        public void Filter_EmailIsNull_ReturnEmpty()
        {
            var result = EmailAddress.Filter(null, Whitelist, Testers);
            Assert.Empty(result);
        }

        [Fact]
        public void Filter_WhitelistIsEmpty_ReturnEmail()
        {
            var result = EmailAddress.Filter("someone@example.com", "    ", "    ");
            Assert.Equal(new[] { "someone@example.com" }, result);
        }

        [Fact]
        public void Filter_WhitelistIsNull_ReturnEmail()
        {
            var result = EmailAddress.Filter("someone@example.com", null, null);
            Assert.Equal(new[] { "someone@example.com" }, result);
        }
    }
}
