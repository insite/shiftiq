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

    public async Task<bool> AssertAsync(Guid gradebook, Guid? organization, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(gradebook, organization, cancellation);
    }

    public async Task<IEnumerable<GradebookModel>> CollectAsync(IGradebookCriteria criteria, TimeZoneInfo? timeZone, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities, timeZone);
    }

    public async Task<int> CountAsync(IGradebookCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    }

    public async IAsyncEnumerable<GradebookModel> DownloadAsync(IGradebookCriteria criteria, TimeZoneInfo? timeZone, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity, timeZone);
        }
    }

    public async Task<GradebookModel?> RetrieveAsync(Guid gradebook, TimeZoneInfo? timeZone, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(gradebook, cancellation);

        return entity != null ? _adapter.ToModel(entity, timeZone) : null;
    }

    public async Task<IEnumerable<GradebookMatch>> SearchAsync(IGradebookCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }

    /// <summary>
    /// Exports the gradebooks with matching criteria to a file.
    /// </summary>
    public async Task ExportAsync(StartExport start, IGradebookCriteria criteria, TimeZoneInfo? timeZone, CancellationToken cancellation)
    {
        var gradebooks = await DownloadAsync(criteria, timeZone, cancellation).ToListAsync(cancellation);

        var content = Serialize(gradebooks, start.ExportFormat, criteria.Filter.Includes);

        System.IO.Directory.CreateDirectory(Path.GetDirectoryName(start.PhysicalFile)!);

        await using var writer = new StreamWriter(start.PhysicalFile);

        await writer.WriteAsync(content);

        await writer.FlushAsync();
    }
}