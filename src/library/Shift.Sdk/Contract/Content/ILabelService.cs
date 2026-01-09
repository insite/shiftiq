using System;
using System.Threading.Tasks;

namespace Shift.Contract
{
    public interface ILabelService
    {
        Task Refresh();
        string GetTranslation(string name, string language, Guid organization, bool allowNull, bool allowDefault);
        Task SaveTranslation(string language, string label, string source, string target);
    }
}
