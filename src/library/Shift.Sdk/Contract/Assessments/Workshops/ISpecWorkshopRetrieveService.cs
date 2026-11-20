using System;
using System.Threading.Tasks;

namespace Shift.Contract
{
    public interface ISpecWorkshopRetrieveService
    {
        Task<SpecWorkshop> RetrieveAsync(
            Guid bankId,
            Guid specificationId,
            Guid? setId,
            Guid? questionId,
            string timeZone
        );

        Task<SpecWorkshop.Set> RetrieveSpecSetAsync(Guid bankId, Guid specificationId, Guid setId, string timeZone);

        Task<SpecWorkshopPartial> RetrievePartialAsync(Guid bankId, Guid specificationId);

        Task<WorkshopQuestionComments> RetrieveQuestionCommentsAsync(Guid bankId, Guid questionId, string timeZone);
    }
}
