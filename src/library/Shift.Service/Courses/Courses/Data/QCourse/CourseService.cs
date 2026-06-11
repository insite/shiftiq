using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Learning;

public class CourseService : IEntityService
{
    private readonly CourseReader _reader;
    private readonly CourseAdapter _adapter = new CourseAdapter();

    public CourseService(CourseReader reader)
    {
        _reader = reader;
    }

    public async Task<bool> AssertAsync(Guid course, Guid? organization, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(course, organization, cancellation);
    }

    public async Task<IEnumerable<CourseModel>> CollectAsync(ICourseCriteria criteria, TimeZoneInfo? timeZone, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities, timeZone);
    }

    public async Task<int> CountAsync(ICourseCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    }

    public async IAsyncEnumerable<CourseModel> DownloadAsync(ICourseCriteria criteria, TimeZoneInfo? timeZone, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity, timeZone);
        }
    }

    public async Task<CourseModel?> RetrieveAsync(Guid course, TimeZoneInfo? timeZone, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(course, cancellation);

        return entity != null ? _adapter.ToModel(entity, timeZone) : null;
    }

    public async Task<IEnumerable<CourseMatch>> SearchAsync(ICourseCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}