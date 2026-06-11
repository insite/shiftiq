using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shift.Contract
{
    public interface IContentModifyService
    {
        Task<Dictionary<int, Guid>> ModifyPageContentAsync(PageModel pageModel, PageContentModifyModel modifyModel, string userFullName);
    }
}
