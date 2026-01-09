using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Assessment;

public class QuizService : IEntityService
{
    private readonly QuizReader _reader;
    private readonly QuizWriter _writer;
    private readonly QuizAdapter _adapter = new QuizAdapter();

    public QuizService(QuizReader reader, QuizWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid quiz, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(quiz, cancellation);
    }

    public async Task<IEnumerable<QuizModel>> CollectAsync(IQuizCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IQuizCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    } 

    public async Task<bool> CreateAsync(CreateQuiz create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid quiz, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(quiz, cancellation);
    }

    public async IAsyncEnumerable<QuizModel> DownloadAsync(IQuizCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyQuiz modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.QuizIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<QuizModel?> RetrieveAsync(Guid quiz, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(quiz, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<QuizMatch>> SearchAsync(IQuizCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}