using Newtonsoft.Json;

using Shift.Common;

namespace Shift.Test.Security.Permissions
{
    [Trait("Category", "shift/utility/security")]
    public class AccessTests
    {
        [Fact]
        public void Deserialize()
        {
            var json = "{\"Basic\":\"All\",\"Data\":\"All\",\"Http\":\"All\",\"Authority\":\"All\"}";

            var access = JsonConvert.DeserializeObject<Access>(json);

            Assert.NotNull(access);

            Assert.Equal(BasicAccess.All, access.Basic);

            Assert.Equal(DataAccess.All, access.Data);

            Assert.Equal(HttpAccess.All, access.Http);

            Assert.Equal(AuthorityAccess.All, access.Authority);
        }
    }
}
