using Shift.Constant;

namespace Shift.Test.Security.Organizations
{
    [Collection(TestFixtures.Default)]
    [Trait("Category", "Unit")]
    public class OrganizationIdentifiersTest
    {
        [Fact]
        public void TestIds_Success()
        {
            Assert.Equal(Guid.Parse("BD8275CD-5035-4020-BB25-B26989C9EB7F"), OrganizationIdentifiers.Archive);
            Assert.Equal(Guid.Parse("4D22C740-88ED-4DDE-9DE1-AC3A011805CF"), OrganizationIdentifiers.Demo);
            Assert.Equal(Guid.Parse("0C071B03-6FE1-400F-82F4-78FF6F751AE7"), OrganizationIdentifiers.Global);
        }
    }
}
