using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Assessment;

public class AttemptMatchService : IEntityService
{
    private readonly AttemptMatchReader _reader;
    private readonly AttemptMatchWriter _writer;
    private readonly AttemptMatchAdapter _adapter = new AttemptMatchAdapter();

    public AttemptMatchService(AttemptMatchReader reader, AttemptMatchWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid attempt, int matchSequence, Guid question, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(attempt, matchSequence, question, cancellation);
    }

    public async Task<IEnumerable<AttemptMatchModel>> CollectAsync(IAttemptMatchCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IAttemptMatchCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    } 

    public async Task<bool> CreateAsync(CreateAttemptMatch create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid attempt, int matchSequence, Guid question, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(attempt, matchSequence, question, cancellation);
    }

    public async IAsyncEnumerable<AttemptMatchModel> DownloadAsync(IAttemptMatchCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyAttemptMatch modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.AttemptIdentifier, modify.MatchSequence, modify.QuestionIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<AttemptMatchModel?> RetrieveAsync(Guid attempt, int matchSequence, Guid question, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(attempt, matchSequence, question, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<AttemptMatchMatch>> SearchAsync(IAttemptMatchCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}