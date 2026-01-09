using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Workflow;

public class SubmissionOptionService : IEntityService
{
    private readonly SubmissionOptionReader _reader;
    private readonly SubmissionOptionWriter _writer;
    private readonly SubmissionOptionAdapter _adapter = new SubmissionOptionAdapter();

    public SubmissionOptionService(SubmissionOptionReader reader, SubmissionOptionWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid responseSession, Guid surveyOption, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(responseSession, surveyOption, cancellation);
    }

    public async Task<IEnumerable<SubmissionOptionModel>> CollectAsync(ISubmissionOptionCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(ISubmissionOptionCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    } 

    public async Task<bool> CreateAsync(CreateSubmissionOption create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid responseSession, Guid surveyOption, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(responseSession, surveyOption, cancellation);
    }

    public async IAsyncEnumerable<SubmissionOptionModel> DownloadAsync(ISubmissionOptionCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifySubmissionOption modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.ResponseSessionIdentifier, modify.SurveyOptionIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<SubmissionOptionModel?> RetrieveAsync(Guid responseSession, Guid surveyOption, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(responseSession, surveyOption, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<SubmissionOptionMatch>> SearchAsync(ISubmissionOptionCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}