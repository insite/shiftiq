using System;

using Shift.Common;

namespace Shift.Sdk.UI
{
    public static class RecentLinksHelper
    {
        public static string CreateRecentLinksKey(EnvironmentName env, Guid organizationId, Guid userId)
        {
            var key = $"{env}-{organizationId}-{userId}";
            var bytes = EncryptionHelper.ComputeHashMd5(key);
            return Convert.ToBase64String(bytes).Substring(0, 22);
        }
    }
}
