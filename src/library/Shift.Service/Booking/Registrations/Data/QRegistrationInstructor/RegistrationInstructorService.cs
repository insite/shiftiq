using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Booking;

public class RegistrationInstructorService : IEntityService
{
    private readonly RegistrationInstructorReader _reader;
    private readonly RegistrationInstructorWriter _writer;
    private readonly RegistrationInstructorAdapter _adapter = new RegistrationInstructorAdapter();

    public RegistrationInstructorService(RegistrationInstructorReader reader, RegistrationInstructorWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid instructor, Guid registration, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(instructor, registration, cancellation);
    }

    public async Task<IEnumerable<RegistrationInstructorModel>> CollectAsync(IRegistrationInstructorCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IRegistrationInstructorCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    } 

    public async Task<bool> CreateAsync(CreateRegistrationInstructor create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid instructor, Guid registration, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(instructor, registration, cancellation);
    }

    public async IAsyncEnumerable<RegistrationInstructorModel> DownloadAsync(IRegistrationInstructorCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyRegistrationInstructor modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.InstructorIdentifier, modify.RegistrationIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<RegistrationInstructorModel?> RetrieveAsync(Guid instructor, Guid registration, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(instructor, registration, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<RegistrationInstructorMatch>> SearchAsync(IRegistrationInstructorCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}