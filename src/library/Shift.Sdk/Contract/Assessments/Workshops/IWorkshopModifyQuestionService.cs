using System;
using System.Threading.Tasks;

using InSite.Domain.Banks;

namespace Shift.Contract
{
    public interface IWorkshopModifyQuestionService
    {
        Task<Guid> AddQuestionAsync(Guid organizationId, Guid bankId, Guid setId, Guid? standardId, WorkshopNewQuestionCommand command);
        Task ReplaceQuestionAsync(BankState bank, Guid bankId, Guid fieldId, WorkshopReplaceCommand command);
        Task<string> ModifyAsync(BankState bank, Guid bankId, Guid questionId, WorkshopQuestionField field, int? columnIndex, string value);
    }
}
