namespace Shift.Service.Site;

using Shift.Contract;

using Shift.Common;

public class SiteService : IEntityService
{
    private readonly QSiteReader _reader;
    private readonly QSiteAdapter _adapter = new QSiteAdapter();

    public SiteService(QSiteReader reader)
    {
        _reader = reader;
    }

    public async Task<bool> AssertAsync(Guid site, CancellationToken cancellation = default)
        => await _reader.AssertAsync(site, cancellation);

    public async Task<SiteModel?> RetrieveAsync(Guid site, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(site, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<int> CountAsync(ISiteCriteria criteria, CancellationToken cancellation = default)
        => await _reader.CountAsync(criteria, cancellation);

    public async Task<IEnumerable<SiteModel>> CollectAsync(ISiteCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<IEnumerable<SiteMatch>> SearchAsync(ISiteCriteria criteria, CancellationToken cancellation = default)
        => await _reader.SearchAsync(criteria, cancellation);
}