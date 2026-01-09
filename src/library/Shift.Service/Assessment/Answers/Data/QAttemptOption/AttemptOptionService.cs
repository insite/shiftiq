using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Assessment;

public class AttemptOptionService : IEntityService
{
    private readonly AttemptOptionReader _reader;
    private readonly AttemptOptionWriter _writer;
    private readonly AttemptOptionAdapter _adapter = new AttemptOptionAdapter();

    public AttemptOptionService(AttemptOptionReader reader, AttemptOptionWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid attempt, int optionKey, Guid question, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(attempt, optionKey, question, cancellation);
    }

    public async Task<IEnumerable<AttemptOptionModel>> CollectAsync(IAttemptOptionCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IAttemptOptionCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    } 

    public async Task<bool> CreateAsync(CreateAttemptOption create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid attempt, int optionKey, Guid question, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(attempt, optionKey, question, cancellation);
    }

    public async IAsyncEnumerable<AttemptOptionModel> DownloadAsync(IAttemptOptionCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyAttemptOption modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.AttemptIdentifier, modify.OptionKey, modify.QuestionIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<AttemptOptionModel?> RetrieveAsync(Guid attempt, int optionKey, Guid question, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(attempt, optionKey, question, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<AttemptOptionMatch>> SearchAsync(IAttemptOptionCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}