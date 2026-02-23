using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IGroupFieldCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        Guid? GroupId { get; set; }

        string SettingName { get; set; }
        string SettingValue { get; set; }
    }
}