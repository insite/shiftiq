using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Booking;

public class EventService : IEntityService
{
    private readonly QEventReader _reader;
    private readonly QEventWriter _writer;
    private readonly QEventAdapter _adapter = new QEventAdapter();

    public EventService(QEventReader reader, QEventWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(
        Guid @event,
        CancellationToken cancellation = default)
        => await _reader.AssertAsync(@event, cancellation);

    public async Task<IEnumerable<EventModel>> CollectAsync(IEventCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(
        IEventCriteria criteria,
        CancellationToken cancellation = default)
        => await _reader.CountAsync(criteria, cancellation);

    public async Task<bool> CreateAsync(CreateEvent create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(
        Guid @event,
        CancellationToken cancellation = default)
        => await _writer.DeleteAsync(@event, cancellation);

    public async Task<IEnumerable<EventModel>> DownloadAsync(
        IEventCriteria criteria,
        CancellationToken cancellation)
    {
        var entities = await _reader.DownloadAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<bool> ModifyAsync(
        ModifyEvent modify,
        CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.EventIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<EventModel?> RetrieveAsync(
        Guid @event,
        CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(@event, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<EventMatch>> SearchAsync(
        IEventCriteria criteria,
        CancellationToken cancellation = default)
        => await _reader.SearchAsync(criteria, cancellation);

    public string Serialize(IEnumerable<EventModel> models, string format)
        => _adapter.Serialize(models, format);
}