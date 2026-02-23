using System.Threading.Tasks;

namespace Shift.Contract
{
    public interface IContentModifyService
    {
        Task ModifyPageContentAsync(PageModel pageModel, PageContentModifyModel modifyModel, string userFullName);
    }
}
