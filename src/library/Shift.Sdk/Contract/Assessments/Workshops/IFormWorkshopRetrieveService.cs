using System;
using System.Threading.Tasks;

using InSite.Domain.Banks;

namespace Shift.Contract
{
    public interface IFormWorkshopRetrieveService
    {
        Task<FormWorkshop> RetrieveAsync(
            Guid bankId,
            Guid formId,
            Guid? sectionId,
            Guid? questionId,
            string publicationStatus,
            bool enableQuestionSubCompetencySelection,
            string timeZone
        );
        Task<FormWorkshop.Section> RetrieveSectionAsync(Guid bankId, Guid sectionId, string timeZone);
        Task<FormWorkshop.Questions> RetrieveVerifiedQuestionsAsync(Guid bankId, Guid formId);
        Task<WorkshopQuestionComments> RetrieveQuestionCommentsAsync(Guid bankId, Guid fieldId, string timeZone);
    }
}
