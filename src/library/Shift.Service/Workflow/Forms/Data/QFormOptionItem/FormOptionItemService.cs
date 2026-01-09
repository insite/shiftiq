using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Workflow;

public class FormOptionItemService : IEntityService
{
    private readonly FormOptionItemReader _reader;
    private readonly FormOptionItemWriter _writer;
    private readonly FormOptionItemAdapter _adapter = new FormOptionItemAdapter();

    public FormOptionItemService(FormOptionItemReader reader, FormOptionItemWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid surveyOptionItem, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(surveyOptionItem, cancellation);
    }

    public async Task<IEnumerable<FormOptionItemModel>> CollectAsync(IFormOptionItemCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IFormOptionItemCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    } 

    public async Task<bool> CreateAsync(CreateFormOptionItem create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid surveyOptionItem, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(surveyOptionItem, cancellation);
    }

    public async IAsyncEnumerable<FormOptionItemModel> DownloadAsync(IFormOptionItemCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyFormOptionItem modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.SurveyOptionItemIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<FormOptionItemModel?> RetrieveAsync(Guid surveyOptionItem, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(surveyOptionItem, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<FormOptionItemMatch>> SearchAsync(IFormOptionItemCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}