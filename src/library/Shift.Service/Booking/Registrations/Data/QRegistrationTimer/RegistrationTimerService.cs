using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Booking;

public class RegistrationTimerService : IEntityService
{
    private readonly RegistrationTimerReader _reader;
    private readonly RegistrationTimerWriter _writer;
    private readonly RegistrationTimerAdapter _adapter = new RegistrationTimerAdapter();

    public RegistrationTimerService(RegistrationTimerReader reader, RegistrationTimerWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid triggerCommand, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(triggerCommand, cancellation);
    }

    public async Task<IEnumerable<RegistrationTimerModel>> CollectAsync(IRegistrationTimerCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IRegistrationTimerCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    } 

    public async Task<bool> CreateAsync(CreateRegistrationTimer create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid triggerCommand, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(triggerCommand, cancellation);
    }

    public async IAsyncEnumerable<RegistrationTimerModel> DownloadAsync(IRegistrationTimerCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyRegistrationTimer modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.TriggerCommand, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<RegistrationTimerModel?> RetrieveAsync(Guid triggerCommand, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(triggerCommand, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<RegistrationTimerMatch>> SearchAsync(IRegistrationTimerCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}