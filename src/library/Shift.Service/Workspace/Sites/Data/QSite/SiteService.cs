using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Workspace;

public class SiteService : IEntityService
{
    private readonly SiteReader _reader;
    private readonly SiteWriter _writer;
    private readonly SiteAdapter _adapter = new SiteAdapter();

    public SiteService(SiteReader reader, SiteWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid site, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(site, cancellation);
    }

    public async Task<IEnumerable<SiteModel>> CollectAsync(ISiteCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(ISiteCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    }

    public async Task<bool> CreateAsync(CreateSite create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid site, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(site, cancellation);
    }

    public async IAsyncEnumerable<SiteModel> DownloadAsync(ISiteCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifySite modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.SiteIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<SiteModel?> RetrieveAsync(Guid site, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(site, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<SiteMatch>> SearchAsync(ISiteCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}