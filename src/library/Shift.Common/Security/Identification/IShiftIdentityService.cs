using System;

namespace Shift.Common
{
    public interface IShiftIdentityService
    {
        IShiftPrincipal GetPrincipal();
        Guid OrganizationId { get; }
        TimeZoneInfo GetTimeZone();
    }
}
