using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Cases;

public class TCaseStatusService : IEntityService
{
    private readonly TCaseStatusReader _reader;
    private readonly TCaseStatusWriter _writer;
    private readonly TCaseStatusAdapter _adapter = new();

    public TCaseStatusService(
        TCaseStatusReader reader,
        TCaseStatusWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    #region Read

    public async Task<bool> AssertAsync(Guid statusId, CancellationToken cancellation)
    {
        return await _reader.AssertAsync(statusId, cancellation);
    }

    public async Task<CaseStatusModel?> RetrieveAsync(Guid statusId, CancellationToken cancellation)
    {
        var entity = await _reader.RetrieveAsync(statusId, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<int> CountAsync(ICaseStatusCriteria criteria, CancellationToken cancellation)
    {
        return await _reader.CountAsync(criteria, cancellation);
    }

    public async Task<IEnumerable<CaseStatusModel>> CollectAsync(ICaseStatusCriteria criteria, CancellationToken cancellation)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<IEnumerable<CaseStatusModel>> DownloadAsync(ICaseStatusCriteria criteria, CancellationToken cancellation)
    {
        var entities = await _reader.DownloadAsync(criteria, cancellation);

        var models = _adapter.ToModel(entities).ToArray();

        return models;
    }

    public async Task<IEnumerable<CaseStatusMatch>> SearchAsync(ICaseStatusCriteria criteria, CancellationToken cancellation)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize(IEnumerable<CaseStatusModel> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }

    public async Task ExportAsync(StartExport start, ICaseStatusCriteria criteria, CancellationToken cancellation)
    {
        var caseStatuses = await DownloadAsync(criteria, cancellation);

        var content = Serialize(caseStatuses, start.ExportFormat, criteria.Filter.Includes);

        System.IO.Directory.CreateDirectory(Path.GetDirectoryName(start.PhysicalFile)!);

        await using var writer = new StreamWriter(start.PhysicalFile);

        await writer.WriteAsync(content);

        await writer.FlushAsync();
    }

    #endregion

    #region Write

    public async Task<CaseStatusModel?> CreateAsync(CreateCaseStatus command, CancellationToken cancellation)
    {
        var entity = await _writer.CreateAsync(command, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<CaseStatusModel?> UpdateAsync(Guid statusId, ModifyCaseStatus command, CancellationToken cancellation)
    {
        var entity = await _reader.RetrieveAsync(statusId, cancellation);
        if (entity == null)
            return null;

        _adapter.Copy(command, entity);

        await _writer.ModifyAsync(entity, cancellation);

        return _adapter.ToModel(entity);
    }

    public async Task<bool> DeleteAsync(Guid statusId, CancellationToken cancellation)
    {
        return await _writer.DeleteAsync(statusId, cancellation);
    }

    #endregion
}