using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class OrganizationList : List<OrganizationState>
    {
        public bool Contains(Guid organizationId)
        {
            return this.Any(x => x.OrganizationIdentifier == organizationId);
        }

        public string OrganizationIdentifierList
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var item in this)
                {
                    if (sb.Length > 0) sb.Append(",");
                    sb.Append(item.OrganizationIdentifier);
                }
                return sb.ToString();
            }
        }
    }
}
