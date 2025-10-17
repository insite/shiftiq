namespace Shift.Service.Site;

using Shift.Contract;

public class PageService : IPageService
{
    private readonly QPageReader _reader;

    private readonly QPageAdapter _adapter = new QPageAdapter();

    public PageService(QPageReader reader)
    {
        _reader = reader;
    }

    public async Task<bool> AssertAsync(Guid page, CancellationToken cancellation = default)
        => await _reader.AssertAsync(page, cancellation);

    public async Task<PageModel?> RetrieveAsync(Guid page, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(page, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<int> CountAsync(IPageCriteria criteria, CancellationToken cancellation = default)
        => await _reader.CountAsync(criteria, cancellation);

    public async Task<IEnumerable<PageModel>> CollectAsync(IPageCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<IEnumerable<PageMatch>> SearchAsync(IPageCriteria criteria, CancellationToken cancellation = default)
        => await _reader.SearchAsync(criteria, cancellation);
}