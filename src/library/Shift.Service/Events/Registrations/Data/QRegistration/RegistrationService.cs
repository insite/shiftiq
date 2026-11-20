using System.Runtime.CompilerServices;

using InSite.Application.Registrations.Write;

using Shift.Common;
using Shift.Common.Exceptions;
using Shift.Constant;
using Shift.Contract;
using Shift.Sdk.Service;
using Shift.Service.Utility;

namespace Shift.Service.Booking;

public class RegistrationService : IEntityService
{
    private readonly RegistrationReader _reader;
    private readonly RegistrationWriter _writer;
    private readonly RegistrationAdapter _adapter = new RegistrationAdapter();
    private readonly CollectionItemReader _collectionItemReader;
    private readonly ICommanderAsync _commander;

    public RegistrationService(RegistrationReader reader, RegistrationWriter writer, CollectionItemReader collectionItemReader, ICommanderAsync commander)
    {
        _reader = reader;
        _writer = writer;
        _commander = commander;
        _collectionItemReader = collectionItemReader;
    }

    public async Task<bool> AssertAsync(Guid registration, Guid? organization, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(registration, organization, cancellation);
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

    public async Task<int> CountDetailsAsync(IRegistrationDetailsCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountDetailsAsync(criteria, cancellation);
    }

    public async IAsyncEnumerable<RegistrationModel> DownloadAsync(IRegistrationCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
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

    public async Task<IEnumerable<RegistrationDetailsMatch>> SearchDetailsAsync(IRegistrationDetailsCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchDetailsAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }

    public async Task<bool> ModifyAttendanceStatusAsync(Guid organizationId, Guid registrationId, string? attendanceStatus, CancellationToken cancellation)
    {
        await ValidateAttendanceStatusAsync(organizationId, attendanceStatus, cancellation);

        var entity = await _reader.RetrieveAsync(registrationId, cancellation);
        if (entity == null || entity.OrganizationIdentifier != organizationId)
            return false;

        if (attendanceStatus == string.Empty)
            attendanceStatus = null;

        if (string.Equals(entity.AttendanceStatus ?? null, attendanceStatus, StringComparison.OrdinalIgnoreCase))
            return true;

        await _commander.SendCommandsAsync([new TakeAttendance(registrationId, attendanceStatus, null, null)], cancellation);

        return true;
    }

    private async Task ValidateAttendanceStatusAsync(Guid organizationId, string? attendanceStatus, CancellationToken cancellation)
    {
        if (string.IsNullOrEmpty(attendanceStatus))
            return;

        var items = await _collectionItemReader
            .CollectAsync(organizationId, CollectionName.Registrations_Attendance_Status, cancellation);

        if (!items.Any(x => string.Equals(x.ItemName, attendanceStatus, StringComparison.OrdinalIgnoreCase)))
            throw new ClientErrorException($"Invalid AttendanceStatus: {attendanceStatus}");
    }
}
