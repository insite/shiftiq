using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Workflow;

public class SubmissionAnswerService : IEntityService
{
    private readonly SubmissionAnswerReader _reader;
    private readonly SubmissionAnswerWriter _writer;
    private readonly SubmissionAnswerAdapter _adapter = new SubmissionAnswerAdapter();

    public SubmissionAnswerService(SubmissionAnswerReader reader, SubmissionAnswerWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid responseSession, Guid surveyQuestion, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(responseSession, surveyQuestion, cancellation);
    }

    public async Task<IEnumerable<SubmissionAnswerModel>> CollectAsync(ISubmissionAnswerCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(ISubmissionAnswerCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    } 

    public async Task<bool> CreateAsync(CreateSubmissionAnswer create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid responseSession, Guid surveyQuestion, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(responseSession, surveyQuestion, cancellation);
    }

    public async IAsyncEnumerable<SubmissionAnswerModel> DownloadAsync(ISubmissionAnswerCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifySubmissionAnswer modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.ResponseSessionIdentifier, modify.SurveyQuestionIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<SubmissionAnswerModel?> RetrieveAsync(Guid responseSession, Guid surveyQuestion, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(responseSession, surveyQuestion, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<SubmissionAnswerMatch>> SearchAsync(ISubmissionAnswerCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}