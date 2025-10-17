using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchGroupFields : Query<IEnumerable<GroupFieldMatch>>, IGroupFieldCriteria
    {
        public Guid? GroupIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }

        public string SettingName { get; set; }
        public string SettingValue { get; set; }
    }
}