using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Booking;

public class EventUserService : IEntityService
{
    private readonly IEventUserReader _reader;
    private readonly IEventUserWriter _writer;
    private readonly EventUserAdapter _adapter = new EventUserAdapter();

    public EventUserService(IEventUserReader reader, IEventUserWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(
        Guid @event, Guid user,
        CancellationToken cancellation = default)
        => await _reader.AssertAsync(@event, user, cancellation);

    public async Task<IEnumerable<EventUserModel>> CollectAsync(IEventUserCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(
        IEventUserCriteria criteria,
        CancellationToken cancellation = default)
        => await _reader.CountAsync(criteria, cancellation);

    public async Task<bool> CreateAsync(CreateEventUser create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(
        Guid @event, Guid user,
        CancellationToken cancellation = default)
        => await _writer.DeleteAsync(@event, user, cancellation);

    public async Task<IEnumerable<EventUserModel>> DownloadAsync(
        IEventUserCriteria criteria,
        CancellationToken cancellation)
    {
        var entities = await _reader.DownloadAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<bool> ModifyAsync(
        ModifyEventUser modify,
        CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.EventIdentifier, modify.UserIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<EventUserModel?> RetrieveAsync(
        Guid @event, Guid user,
        CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(@event, user, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<EventUserMatch>> SearchAsync(
        IEventUserCriteria criteria,
        CancellationToken cancellation = default)
        => await _reader.SearchAsync(criteria, cancellation);

    public string Serialize(IEnumerable<EventUserModel> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}