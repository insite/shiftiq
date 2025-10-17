using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IGroupFieldCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? GroupIdentifier { get; set; }
        Guid? OrganizationIdentifier { get; set; }

        string SettingName { get; set; }
        string SettingValue { get; set; }
    }
}