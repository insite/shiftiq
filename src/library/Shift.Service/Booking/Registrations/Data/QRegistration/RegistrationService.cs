using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Booking;

public class RegistrationService : IEntityService
{
    private readonly RegistrationReader _reader;
    private readonly RegistrationWriter _writer;
    private readonly RegistrationAdapter _adapter = new RegistrationAdapter();

    public RegistrationService(RegistrationReader reader, RegistrationWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid registration, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(registration, cancellation);
    }

    public async Task<IEnumerable<RegistrationModel>> CollectAsync(IRegistrationCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IRegistrationCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    } 

    public async Task<bool> CreateAsync(CreateRegistration create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid registration, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(registration, cancellation);
    }

    public async IAsyncEnumerable<RegistrationModel> DownloadAsync(IRegistrationCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyRegistration modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.RegistrationIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<RegistrationModel?> RetrieveAsync(Guid registration, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(registration, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<RegistrationMatch>> SearchAsync(IRegistrationCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}