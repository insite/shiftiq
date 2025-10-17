using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Progress;

public class PeriodService : IEntityService
{
    private readonly IPeriodReader _reader;
    private readonly IPeriodWriter _writer;
    private readonly PeriodAdapter _adapter = new PeriodAdapter();

    public PeriodService(IPeriodReader reader, IPeriodWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(
        Guid period,
        CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(period, cancellation);
    }

    public async Task<IEnumerable<PeriodModel>> CollectAsync(IPeriodCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(
        IPeriodCriteria criteria,
        CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    }

    public async Task<bool> CreateAsync(CreatePeriod create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(
        Guid period,
        CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(period, cancellation);
    }

    public async Task<IEnumerable<PeriodModel>> DownloadAsync(
        IPeriodCriteria criteria,
        CancellationToken cancellation)
    {
        var entities = await _reader.DownloadAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<bool> ModifyAsync(
        ModifyPeriod modify,
        CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.PeriodIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<PeriodModel?> RetrieveAsync(
        Guid period,
        CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(period, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<PeriodMatch>> SearchAsync(
        IPeriodCriteria criteria,
        CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> items, string format, string includes)
    {
        return _adapter.Serialize(items, format, includes);
    }
}