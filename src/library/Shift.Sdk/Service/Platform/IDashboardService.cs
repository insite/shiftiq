using System;
using System.Threading.Tasks;

using Shift.Sdk.Service.Platform.DashboardNotifications;

namespace Shift.Sdk.Service.Platform
{
    public interface IDashboardService
    {
        Task<DashboardData> CreateAsync(Guid organizationId);
    }
}