using Shift.Common;
using Shift.Contract;
using Shift.Service.Booking;

namespace Shift.Service.Gradebook;

public class GradebookService : IEntityService
{
    private readonly IShiftIdentityService _identityService;
    private readonly QGradebookReader _reader;
    private readonly QGradebookAdapter _adapter = new();

    private readonly EventUserReader _eventUserReader;

    public GradebookService(
        IShiftIdentityService identityService,
        QGradebookReader reader,
        EventUserReader eventUserReader)
    {
        _identityService = identityService;
        _reader = reader;
        _eventUserReader = eventUserReader;
    }

    public async Task<bool> AssertAsync(Guid gradebook, CancellationToken cancellation)
    {
        return await _reader.AssertAsync(gradebook, cancellation);
    }

    public async Task<GradebookModel?> RetrieveAsync(Guid gradebook, CancellationToken cancellation)
    {
        var entity = await _reader.RetrieveAsync(gradebook, cancellation);

        var tz = _identityService.GetTimeZone();

        return entity != null ? _adapter.ToModel(entity, tz) : null;
    }

    public async Task<int> CountAsync(IGradebookCriteria criteria, CancellationToken cancellation)
    {
        return await _reader.CountAsync(criteria, cancellation);
    }

    public async Task<IEnumerable<GradebookModel>> CollectAsync(IGradebookCriteria criteria, CancellationToken cancellation)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        var tz = _identityService.GetTimeZone();

        return _adapter.ToModel(entities, tz);
    }

    public async Task<IEnumerable<GradebookModel>> DownloadAsync(IGradebookCriteria criteria, CancellationToken cancellation)
    {
        var entities = await _reader.DownloadAsync(criteria, cancellation);

        var tz = _identityService.GetTimeZone();

        var models = _adapter.ToModel(entities, tz).ToArray();

        var instructors = await SearchEventUsers("Instructor");

        foreach (var model in models)
        {
            model.ClassInstructors = SearchEventUserNames(instructors, model.EventIdentifier);
        }

        return models;
    }

    private async Task<EventUserMatch[]> SearchEventUsers(string role)
    {
        var search = new SearchEventUsers() { Role = role };

        search.Filter.PageSize = 0; // Force the reader to return a non-paged result set

        var results = await _eventUserReader.SearchAsync(search);

        return results.ToArray();
    }

    private string? SearchEventUserNames(EventUserMatch[] eventUsers, Guid? eventId)
    {
        if (eventId == null)
            return null;

        var names = eventUsers
            .Where(x => x.EventId == eventId)
            .Select(x => x.UserName).Distinct().OrderBy(x => x);

        var csv = string.Join(", ", names);

        return csv;
    }

    public async Task<IEnumerable<GradebookMatch>> SearchAsync(IGradebookCriteria criteria, CancellationToken cancellation)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize(IEnumerable<GradebookModel> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }

    /// <summary>
    /// Exports the gradebooks with matching criteria to a file.
    /// </summary>
    public async Task ExportAsync(StartExport start, IGradebookCriteria criteria, CancellationToken cancellation)
    {
        var gradebooks = await DownloadAsync(criteria, cancellation);

        var content = Serialize(gradebooks, start.ExportFormat, criteria.Filter.Includes);

        System.IO.Directory.CreateDirectory(Path.GetDirectoryName(start.PhysicalFile)!);

        await using var writer = new StreamWriter(start.PhysicalFile);

        await writer.WriteAsync(content);

        await writer.FlushAsync();
    }
}