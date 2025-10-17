using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Shift.Contract
{
    public interface IPageService
    {
        Task<bool> AssertAsync(Guid page, CancellationToken cancellation = default);
        Task<PageModel> RetrieveAsync(Guid page, CancellationToken cancellation = default);

        Task<int> CountAsync(IPageCriteria criteria, CancellationToken cancellation = default);
        Task<IEnumerable<PageModel>> CollectAsync(IPageCriteria criteria, CancellationToken cancellation = default);
        Task<IEnumerable<PageMatch>> SearchAsync(IPageCriteria criteria, CancellationToken cancellation = default);
    }
}
