using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Workflow;

public class CaseGroupService : IEntityService
{
    private readonly CaseGroupReader _reader;
    private readonly CaseGroupWriter _writer;
    private readonly CaseGroupAdapter _adapter = new CaseGroupAdapter();

    public CaseGroupService(CaseGroupReader reader, CaseGroupWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid join, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(join, cancellation);
    }

    public async Task<IEnumerable<CaseGroupModel>> CollectAsync(ICaseGroupCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(ICaseGroupCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    } 

    public async Task<bool> CreateAsync(CreateCaseGroup create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid join, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(join, cancellation);
    }

    public async IAsyncEnumerable<CaseGroupModel> DownloadAsync(ICaseGroupCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyCaseGroup modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.JoinIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<CaseGroupModel?> RetrieveAsync(Guid join, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(join, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<CaseGroupMatch>> SearchAsync(ICaseGroupCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}