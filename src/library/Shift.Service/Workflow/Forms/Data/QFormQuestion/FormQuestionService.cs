using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Workflow;

public class FormQuestionService : IEntityService
{
    private readonly FormQuestionReader _reader;
    private readonly FormQuestionWriter _writer;
    private readonly FormQuestionAdapter _adapter = new FormQuestionAdapter();

    public FormQuestionService(FormQuestionReader reader, FormQuestionWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid surveyQuestion, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(surveyQuestion, cancellation);
    }

    public async Task<IEnumerable<FormQuestionModel>> CollectAsync(IFormQuestionCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IFormQuestionCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    } 

    public async Task<bool> CreateAsync(CreateFormQuestion create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid surveyQuestion, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(surveyQuestion, cancellation);
    }

    public async IAsyncEnumerable<FormQuestionModel> DownloadAsync(IFormQuestionCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyFormQuestion modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.SurveyQuestionIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<FormQuestionModel?> RetrieveAsync(Guid surveyQuestion, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(surveyQuestion, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<FormQuestionMatch>> SearchAsync(IFormQuestionCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}