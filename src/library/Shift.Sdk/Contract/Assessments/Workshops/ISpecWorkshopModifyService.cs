using System;
using System.Threading.Tasks;

namespace Shift.Contract
{
    public interface ISpecWorkshopModifyService
    {
        Task<bool> ModifyAsync(Guid bankId, Guid specificationId, SpecWorkshop.Input input);
    }
}
