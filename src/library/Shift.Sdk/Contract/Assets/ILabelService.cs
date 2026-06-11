using System;
using System.Threading.Tasks;

namespace Shift.Contract
{
    public interface ILabelService
    {
        Task RefreshAsync();
        string GetTranslation(string name, string language, Guid organization, bool allowNull, bool allowDefault);
        Task SaveTranslationAsync(string language, string label, string source, string target);
    }
}
