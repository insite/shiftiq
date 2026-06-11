using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Directory;

public class PendingPersonService : IEntityService
{
    private readonly PendingPersonReader _reader;
    private readonly PendingPersonWriter _writer;
    private readonly PendingPersonAdapter _adapter = new PendingPersonAdapter();

    public PendingPersonService(PendingPersonReader reader, PendingPersonWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid pending, Guid? organization, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(pending, organization, cancellation);
    }

    public async Task<IEnumerable<PendingPersonModel>> CollectAsync(IPendingPersonCriteria criteria, TimeZoneInfo? timeZone, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities, timeZone);
    }

    public async Task<int> CountAsync(IPendingPersonCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    }

    public async Task<bool> CreateAsync(CreatePendingPerson create, IPrincipal principal, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        entity.OrganizationId = principal.OrganizationId;
        entity.PendingStatus = "Submitted";
        entity.SubmittedAt = DateTimeOffset.Now;
        entity.SubmittedBy = principal.UserId;

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid pending, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(pending, cancellation);
    }

    public async IAsyncEnumerable<PendingPersonModel> DownloadAsync(IPendingPersonCriteria criteria, TimeZoneInfo? timeZone, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity, timeZone);
        }
    }

    public async Task<bool> ModifyAsync(ModifyPendingPerson modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.PendingId, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<PendingPersonModel?> RetrieveAsync(Guid pending, TimeZoneInfo? timeZone, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(pending, cancellation);

        return entity != null ? _adapter.ToModel(entity, timeZone) : null;
    }

    public async Task<IEnumerable<PendingPersonMatch>> SearchAsync(IPendingPersonCriteria criteria, TimeZoneInfo? timeZone, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, timeZone, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}
