using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Workflow;

public class CaseUserService : IEntityService
{
    private readonly CaseUserReader _reader;
    private readonly CaseUserWriter _writer;
    private readonly CaseUserAdapter _adapter = new CaseUserAdapter();

    public CaseUserService(CaseUserReader reader, CaseUserWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid join, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(join, cancellation);
    }

    public async Task<IEnumerable<CaseUserModel>> CollectAsync(ICaseUserCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(ICaseUserCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    } 

    public async Task<bool> CreateAsync(CreateCaseUser create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid join, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(join, cancellation);
    }

    public async IAsyncEnumerable<CaseUserModel> DownloadAsync(ICaseUserCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyCaseUser modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.JoinIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<CaseUserModel?> RetrieveAsync(Guid join, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(join, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<CaseUserMatch>> SearchAsync(ICaseUserCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}