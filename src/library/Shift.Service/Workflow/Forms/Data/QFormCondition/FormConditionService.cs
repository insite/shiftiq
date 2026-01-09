using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Workflow;

public class FormConditionService : IEntityService
{
    private readonly FormConditionReader _reader;
    private readonly FormConditionWriter _writer;
    private readonly FormConditionAdapter _adapter = new FormConditionAdapter();

    public FormConditionService(FormConditionReader reader, FormConditionWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid maskedSurveyQuestion, Guid maskingSurveyOptionItem, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(maskedSurveyQuestion, maskingSurveyOptionItem, cancellation);
    }

    public async Task<IEnumerable<FormConditionModel>> CollectAsync(IFormConditionCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IFormConditionCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    } 

    public async Task<bool> CreateAsync(CreateFormCondition create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid maskedSurveyQuestion, Guid maskingSurveyOptionItem, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(maskedSurveyQuestion, maskingSurveyOptionItem, cancellation);
    }

    public async IAsyncEnumerable<FormConditionModel> DownloadAsync(IFormConditionCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyFormCondition modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.MaskedSurveyQuestionIdentifier, modify.MaskingSurveyOptionItemIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<FormConditionModel?> RetrieveAsync(Guid maskedSurveyQuestion, Guid maskingSurveyOptionItem, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(maskedSurveyQuestion, maskingSurveyOptionItem, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<FormConditionMatch>> SearchAsync(IFormConditionCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}