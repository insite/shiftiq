using System;
using System.Threading.Tasks;

using InSite.Domain.Banks;

namespace Shift.Contract
{
    public interface IWorkshopModifyOptionService
    {
        Task<string> ModifyAsync(BankState bank, Guid bankId, Guid questionId, int optionNumber, WorkshopQuestionOptionField field, int? columnIndex, string value);
    }
}
