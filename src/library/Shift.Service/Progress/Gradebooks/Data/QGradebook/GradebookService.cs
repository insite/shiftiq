using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Progress;

public class GradebookService : IEntityService
{
    private readonly GradebookReader _reader;
    private readonly GradebookAdapter _adapter = new GradebookAdapter();

    public GradebookService(GradebookReader reader)
    {
        _reader = reader;
    }

    public async Task<bool> AssertAsync(Guid gradebook, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(gradebook, cancellation);
    }

    public async Task<IEnumerable<GradebookModel>> CollectAsync(IGradebookCriteria criteria, CancellationToken cancellation = default)
    {
        var timezone = _reader.GetTimeZone();

        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities, timezone);
    }

    public async Task<int> CountAsync(IGradebookCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    }

    public async IAsyncEnumerable<GradebookModel> DownloadAsync(IGradebookCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        var timezone = _reader.GetTimeZone();

        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity, timezone);
        }
    }

    public async Task<GradebookModel?> RetrieveAsync(Guid gradebook, CancellationToken cancellation = default)
    {
        var timezone = _reader.GetTimeZone();

        var entity = await _reader.RetrieveAsync(gradebook, cancellation);

        return entity != null ? _adapter.ToModel(entity, timezone) : null;
    }

    public async Task<IEnumerable<GradebookMatch>> SearchAsync(IGradebookCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}