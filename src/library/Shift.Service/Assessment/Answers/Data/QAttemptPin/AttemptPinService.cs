using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Assessment;

public class AttemptPinService : IEntityService
{
    private readonly AttemptPinReader _reader;
    private readonly AttemptPinWriter _writer;
    private readonly AttemptPinAdapter _adapter = new AttemptPinAdapter();

    public AttemptPinService(AttemptPinReader reader, AttemptPinWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid attempt, int pinSequence, Guid question, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(attempt, pinSequence, question, cancellation);
    }

    public async Task<IEnumerable<AttemptPinModel>> CollectAsync(IAttemptPinCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IAttemptPinCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    } 

    public async Task<bool> CreateAsync(CreateAttemptPin create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid attempt, int pinSequence, Guid question, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(attempt, pinSequence, question, cancellation);
    }

    public async IAsyncEnumerable<AttemptPinModel> DownloadAsync(IAttemptPinCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyAttemptPin modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.AttemptIdentifier, modify.PinSequence, modify.QuestionIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<AttemptPinModel?> RetrieveAsync(Guid attempt, int pinSequence, Guid question, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(attempt, pinSequence, question, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<AttemptPinMatch>> SearchAsync(IAttemptPinCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}