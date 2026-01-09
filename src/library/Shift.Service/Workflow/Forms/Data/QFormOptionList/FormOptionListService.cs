using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Workflow;

public class FormOptionListService : IEntityService
{
    private readonly FormOptionListReader _reader;
    private readonly FormOptionListWriter _writer;
    private readonly FormOptionListAdapter _adapter = new FormOptionListAdapter();

    public FormOptionListService(FormOptionListReader reader, FormOptionListWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid surveyOptionList, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(surveyOptionList, cancellation);
    }

    public async Task<IEnumerable<FormOptionListModel>> CollectAsync(IFormOptionListCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IFormOptionListCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    } 

    public async Task<bool> CreateAsync(CreateFormOptionList create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid surveyOptionList, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(surveyOptionList, cancellation);
    }

    public async IAsyncEnumerable<FormOptionListModel> DownloadAsync(IFormOptionListCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyFormOptionList modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.SurveyOptionListIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<FormOptionListModel?> RetrieveAsync(Guid surveyOptionList, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(surveyOptionList, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<FormOptionListMatch>> SearchAsync(IFormOptionListCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}