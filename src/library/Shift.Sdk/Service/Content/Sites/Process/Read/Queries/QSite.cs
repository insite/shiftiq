using System;
using System.Collections.Generic;

using InSite.Application.Organizations.Read;

using Shift.Common;

namespace InSite.Application.Sites.Read
{
    public class QSite
    {
        public Guid OrganizationIdentifier { get; set; }
        public Guid SiteIdentifier { get; set; }

        public string SiteDomain { get; set; }
        public string SiteTitle { get; set; }

        public DateTimeOffset? LastChangeTime { get; set; }
        public string LastChangeType { get; set; }
        public string LastChangeUser { get; set; }

        public bool SiteIsPortal
        {
            get
            {
                if (!string.IsNullOrEmpty(SiteDomain))
                    if (StringHelper.Split(SiteDomain, '.').Length == 2)
                        return false;

                return true;
            }
        }


        public virtual QOrganization Organization { get; set; }

        public virtual ICollection<QPage> Pages { get; set; } = new HashSet<QPage>();
    }
}
