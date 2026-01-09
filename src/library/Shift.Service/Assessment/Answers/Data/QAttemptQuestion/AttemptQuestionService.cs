using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Assessment;

public class AttemptQuestionService : IEntityService
{
    private readonly AttemptQuestionReader _reader;
    private readonly AttemptQuestionWriter _writer;
    private readonly AttemptQuestionAdapter _adapter = new AttemptQuestionAdapter();

    public AttemptQuestionService(AttemptQuestionReader reader, AttemptQuestionWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid attempt, Guid question, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(attempt, question, cancellation);
    }

    public async Task<IEnumerable<AttemptQuestionModel>> CollectAsync(IAttemptQuestionCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IAttemptQuestionCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    } 

    public async Task<bool> CreateAsync(CreateAttemptQuestion create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid attempt, Guid question, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(attempt, question, cancellation);
    }

    public async IAsyncEnumerable<AttemptQuestionModel> DownloadAsync(IAttemptQuestionCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyAttemptQuestion modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.AttemptIdentifier, modify.QuestionIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<AttemptQuestionModel?> RetrieveAsync(Guid attempt, Guid question, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(attempt, question, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<AttemptQuestionMatch>> SearchAsync(IAttemptQuestionCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}