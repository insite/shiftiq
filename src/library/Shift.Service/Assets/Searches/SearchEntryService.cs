using System.Text;
using System.Text.RegularExpressions;

using Shift.Service.Content.Searches.Dtos;

namespace Shift.Service.Content.Searches;

public class SearchEntryService(SearchEntryRepository searchRepository)
{
    public int PageSize => 20;

    public async Task<SearchEntryViewDto[]> SearchAsync(Guid organizationId, Guid userId, bool isOperator, string searchText, int pageIndex, CancellationToken cancellation)
    {
        var ftsSearchText = BuildFTSSearchText(searchText);

        if (string.IsNullOrEmpty(ftsSearchText))
            return [];

        var items = await searchRepository.SearchAsync(organizationId, userId, isOperator, ftsSearchText, PageSize, pageIndex * PageSize, cancellation);

        return items.Select(SearchEntryViewDtoMapper.Map).ToArray();
    }

    public async Task RefreshAsync(bool isCmds, CancellationToken cancellation)
    {
        await searchRepository.CreateSearchAsync(isCmds, cancellation);
    }

    public async Task<int> ProcessSearchQueueAsync(CancellationToken cancellation)
    {
        return await searchRepository.ProcessSearchQueueObjectAsync(cancellation)
            + await searchRepository.ProcessSearchQueuePermissionAsync(cancellation);
    }

    private static readonly Regex TermRegex = new(
        pattern: @"[\p{L}\p{Nd}]+",
        options:
            RegexOptions.Compiled |
            RegexOptions.CultureInvariant
    );

    private static string? BuildFTSSearchText(string? searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return null;

        var normalized = searchText
            .Trim()
            .Normalize(NormalizationForm.FormKC);

        var terms = TermRegex
            .Matches(normalized)
            .Select(match => match.Value)
            .Where(term => term.Length >= 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (terms.Length == 0)
            return null;

        return string.Join(
            " AND ",
            terms.Select(term => $"\"{term}*\"")
        );
    }
}
