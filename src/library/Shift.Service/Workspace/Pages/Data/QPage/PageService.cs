using System.Runtime.CompilerServices;

using Shift.Contract;

namespace Shift.Service.Workspace;

public class PageService : IPageService
{
    private readonly PageReader _reader;
    private readonly PageWriter _writer;
    private readonly PageAdapter _adapter = new PageAdapter();

    public PageService(PageReader reader, PageWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid page, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(page, cancellation);
    }

    public async Task<IEnumerable<PageModel>> CollectAsync(IPageCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IPageCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    }

    public async Task<bool> CreateAsync(CreatePage create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid page, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(page, cancellation);
    }

    public async IAsyncEnumerable<PageModel> DownloadAsync(IPageCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyPage modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.PageIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<PageModel?> RetrieveAsync(Guid page, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(page, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<PageMatch>> SearchAsync(IPageCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}