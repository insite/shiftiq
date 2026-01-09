using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Shift.Contract
{
    public interface IActionService
    {
        Task RefreshAsync();
        List<ActionModel> Search(Func<ActionModel, bool> predicate);
        ActionModel Retrieve(Guid action);
        ActionModel Retrieve(string actionUrl);

        Task<ActionModel> RetrieveAsync(Guid action, CancellationToken cancellation = default);
        Task<ActionModel> RetrieveAsync(string actionUrl, CancellationToken cancellation = default);
        Task<bool> AssertAsync(Guid action, CancellationToken cancellation = default);
        Task<int> CountAsync(IActionCriteria criteria, CancellationToken cancellation = default);
        Task<IEnumerable<ActionModel>> CollectAsync(IActionCriteria criteria, CancellationToken cancellation = default);
        Task<IEnumerable<ActionMatch>> SearchAsync(IActionCriteria criteria, CancellationToken cancellation = default);
        Task<bool> CreateAsync(CreateAction create, CancellationToken cancellation);
        Task<bool> ModifyAsync(ModifyAction modify, CancellationToken cancellation = default);
        Task<bool> DeleteAsync(Guid action, CancellationToken cancellation = default);

        Task<IEnumerable<ActionModel>> DownloadAsync(IActionCriteria criteria, CancellationToken cancellation = default);
    }
}
