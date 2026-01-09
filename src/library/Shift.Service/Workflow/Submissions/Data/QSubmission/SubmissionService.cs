using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Workflow;

public class SubmissionService : IEntityService
{
    private readonly SubmissionReader _reader;
    private readonly SubmissionWriter _writer;
    private readonly SubmissionAdapter _adapter = new SubmissionAdapter();

    public SubmissionService(SubmissionReader reader, SubmissionWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid responseSession, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(responseSession, cancellation);
    }

    public async Task<IEnumerable<SubmissionModel>> CollectAsync(ISubmissionCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(ISubmissionCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    } 

    public async Task<bool> CreateAsync(CreateSubmission create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid responseSession, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(responseSession, cancellation);
    }

    public async IAsyncEnumerable<SubmissionModel> DownloadAsync(ISubmissionCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifySubmission modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.ResponseSessionIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<SubmissionModel?> RetrieveAsync(Guid responseSession, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(responseSession, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<SubmissionMatch>> SearchAsync(ISubmissionCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}