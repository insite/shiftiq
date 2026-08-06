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

    public async Task<PendingPersonModel[]> CollectAsync(IPendingPersonCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<PendingPersonImportModel[]> CollectImportAsync(IPendingPersonCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CollectImportAsync(criteria, cancellation);
    }

    public async Task<int> CountAsync(IPendingPersonCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    }

    public async Task<Guid?> CreateAsync(CreatePendingPerson create, IPrincipal principal, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        entity.OrganizationIdentifier = principal.OrganizationId;
        entity.SubmittedAt = DateTimeOffset.Now;
        entity.SubmittedBy = principal.UserId;

        return await _writer.CreateAsync(entity, cancellation) ? entity.PendingPersonIdentifier : null;
    }

    public async Task<bool> DeleteAsync(Guid pending, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(pending, cancellation);
    }

    public async IAsyncEnumerable<PendingPersonModel> DownloadAsync(IPendingPersonCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyPendingPerson modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.PendingPersonIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<PendingPersonModel?> RetrieveAsync(Guid pending, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(pending, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}
