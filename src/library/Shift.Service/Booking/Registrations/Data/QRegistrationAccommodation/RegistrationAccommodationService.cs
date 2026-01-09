using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Booking;

public class RegistrationAccommodationService : IEntityService
{
    private readonly RegistrationAccommodationReader _reader;
    private readonly RegistrationAccommodationWriter _writer;
    private readonly RegistrationAccommodationAdapter _adapter = new RegistrationAccommodationAdapter();

    public RegistrationAccommodationService(RegistrationAccommodationReader reader, RegistrationAccommodationWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid accommodation, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(accommodation, cancellation);
    }

    public async Task<IEnumerable<RegistrationAccommodationModel>> CollectAsync(IRegistrationAccommodationCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IRegistrationAccommodationCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    } 

    public async Task<bool> CreateAsync(CreateRegistrationAccommodation create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid accommodation, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(accommodation, cancellation);
    }

    public async IAsyncEnumerable<RegistrationAccommodationModel> DownloadAsync(IRegistrationAccommodationCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyRegistrationAccommodation modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.AccommodationIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<RegistrationAccommodationModel?> RetrieveAsync(Guid accommodation, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(accommodation, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<RegistrationAccommodationMatch>> SearchAsync(IRegistrationAccommodationCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}