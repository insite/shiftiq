using System;
using System.Threading.Tasks;

using Shift.Common;

namespace Shift.Contract
{
    public interface IWorkshopImageListService
    {
        Task<WorkshopImage[]> CollectImagesAsync(IPrincipal principal, Guid bankId);
    }
}
