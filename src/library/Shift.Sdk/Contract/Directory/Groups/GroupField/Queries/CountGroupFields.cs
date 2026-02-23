using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountGroupFields : Query<int>, IGroupFieldCriteria
    {
        public Guid? GroupId { get; set; }
        public Guid? OrganizationId { get; set; }

        public string SettingName { get; set; }
        public string SettingValue { get; set; }
    }
}