using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Booking;

public class EventService : IEntityService
{
    private readonly EventReader _reader;
    private readonly EventWriter _writer;
    private readonly EventAdapter _adapter = new EventAdapter();

    public EventService(EventReader reader, EventWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid @event, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(@event, cancellation);
    }

    public async Task<IEnumerable<EventModel>> CollectAsync(IEventCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IEventCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    }

    public async Task<bool> CreateAsync(CreateEvent create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid @event, CancellationToken cancellation = default)
        => await _writer.DeleteAsync(@event, cancellation);

    public async IAsyncEnumerable<EventModel> DownloadAsync(IEventCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyEvent modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.EventIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<EventModel?> RetrieveAsync(Guid @event, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(@event, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<EventMatch>> SearchAsync(IEventCriteria criteria, CancellationToken cancellation = default)
        => await _reader.SearchAsync(criteria, cancellation);

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}