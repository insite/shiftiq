using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Workflow;

public class FormService : IEntityService
{
    private readonly FormReader _reader;
    private readonly FormWriter _writer;
    private readonly FormAdapter _adapter = new FormAdapter();

    public FormService(FormReader reader, FormWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid surveyForm, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(surveyForm, cancellation);
    }

    public async Task<IEnumerable<FormModel>> CollectAsync(IFormCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IFormCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    } 

    public async Task<bool> CreateAsync(CreateForm create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid surveyForm, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(surveyForm, cancellation);
    }

    public async IAsyncEnumerable<FormModel> DownloadAsync(IFormCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyForm modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.SurveyFormIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<FormModel?> RetrieveAsync(Guid surveyForm, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(surveyForm, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<FormMatch>> SearchAsync(IFormCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}