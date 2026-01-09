using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Assessment;

public class QuizAttemptService : IEntityService
{
    private readonly QuizAttemptReader _reader;
    private readonly QuizAttemptWriter _writer;
    private readonly QuizAttemptAdapter _adapter = new QuizAttemptAdapter();

    public QuizAttemptService(QuizAttemptReader reader, QuizAttemptWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid attempt, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(attempt, cancellation);
    }

    public async Task<IEnumerable<QuizAttemptModel>> CollectAsync(IQuizAttemptCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IQuizAttemptCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    } 

    public async Task<bool> CreateAsync(CreateQuizAttempt create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid attempt, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(attempt, cancellation);
    }

    public async IAsyncEnumerable<QuizAttemptModel> DownloadAsync(IQuizAttemptCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyQuizAttempt modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.AttemptIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<QuizAttemptModel?> RetrieveAsync(Guid attempt, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(attempt, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<QuizAttemptMatch>> SearchAsync(IQuizAttemptCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}