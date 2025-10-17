using System;

namespace InSite.Domain.Organizations
{
    public class MissingAssetNumberException : Exception
    {
        public MissingAssetNumberException(Guid organization, int asset)
            : base($"Asset number {asset} not found for Organization {organization}")
        {

        }
    }
}