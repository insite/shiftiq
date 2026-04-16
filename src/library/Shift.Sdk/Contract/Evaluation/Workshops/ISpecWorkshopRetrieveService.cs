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
            TimeZoneInfo timeZone
        );

        Task<SpecWorkshop.Set> RetrieveSpecSetAsync(Guid bankId, Guid specificationId, Guid setId, TimeZoneInfo timeZone);

        Task<WorkshopQuestion> RetrieveQuestionWithoutCommentsAsync(Guid bankId, Guid questionId);

        Task<WorkshopQuestionComments> RetrieveQuestionCommentsAsync(Guid bankId, Guid questionId, TimeZoneInfo timeZone);
    }
}
