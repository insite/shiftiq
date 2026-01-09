using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Workflow;

public class CaseService : IEntityService
{
    private readonly CaseReader _reader;
    private readonly CaseWriter _writer;
    private readonly CaseAdapter _adapter = new CaseAdapter();

    public CaseService(CaseReader reader, CaseWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid issue, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(issue, cancellation);
    }

    public async Task<IEnumerable<CaseModel>> CollectAsync(ICaseCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(ICaseCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    }

    public async Task<bool> CreateAsync(CreateCase create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid issue, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(issue, cancellation);
    }

    public async IAsyncEnumerable<CaseModel> DownloadAsync(ICaseCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyCase modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.CaseIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<CaseModel?> RetrieveAsync(Guid issue, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(issue, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<CaseMatch>> SearchAsync(ICaseCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}