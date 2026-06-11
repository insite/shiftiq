using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Evaluation;

public class AssessmentService : IEntityService
{
    private readonly AssessmentReader _reader;
    private readonly AssessmentAdapter _adapter = new AssessmentAdapter();

    public AssessmentService(AssessmentReader reader)
    {
        _reader = reader;
    }

    public async Task<bool> AssertAsync(Guid form, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(form, cancellation);
    }

    public async Task<IEnumerable<AssessmentModel>> CollectAsync(IAssessmentCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IAssessmentCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    }

    public async IAsyncEnumerable<AssessmentModel> DownloadAsync(IAssessmentCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<AssessmentModel?> RetrieveAsync(Guid form, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(form, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<AssessmentMatch>> SearchAsync(IAssessmentCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}