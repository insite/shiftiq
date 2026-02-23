using System.Threading.Tasks;

namespace Shift.Contract
{
    public interface IContentRetrieveService
    {
        Task<PageContentModel> RetrievePageContentAsync(PageModel pageModel);
    }
}
