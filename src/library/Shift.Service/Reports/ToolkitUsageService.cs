using Shift.Common;
using Shift.Contract;
using Shift.Service.Metadata;

namespace Shift.Service.Reports;

public class ToolkitUsageService(ToolkitUsageReader usageReader, ToolkitUsageWriter usageWriter, ToolkitVisitWriter visitWriter, TActionReader actionReader)
{
    private Dictionary<string, Guid>? _actions;

    private async Task<Guid?> GetActionIdAsync(string url)
    {
        if (_actions == null)
        {
            var actions = await actionReader.CollectAsync(new ActionCriteria
            {
                Filter = new QueryFilter { Page = -1 }
            });
            _actions = actions
                .Where(x => x.ActionUrl != null)
                .ToDictionary(x => x.ActionUrl!, x => x.ActionIdentifier, StringComparer.OrdinalIgnoreCase);
        }

        if (url.StartsWith('/'))
            url = url.Substring(1);

        return _actions.TryGetValue(url, out var id) ? id : null;
    }

    public async Task<bool> SaveVisitAsync(Guid organizationId, Guid userId, Guid tokenId, string actionUrl, DateTimeOffset? visited)
    {
        var actionId = await GetActionIdAsync(actionUrl);
        if (actionId == null)
            return false;

        return await visitWriter.SaveVisitAsync(organizationId, userId, tokenId, actionId.Value, visited ?? DateTimeOffset.UtcNow);
    }

    public async Task<ToolkitUsageModel[]> CollectAsync(Guid organizationId, Guid userId)
    {
        var entities = await usageReader.CollectAsync(organizationId, userId);

        return entities
            .Select(x => new ToolkitUsageModel
            {
                ToolkitName = x.ToolkitName.ToEnum<ToolkitName>(),
                UsageScore = x.UsageScore
            })
            .ToArray();
    }

    public async Task<int> CalculateAsync()
    {
        return await usageWriter.CalculateAsync();
    }
}