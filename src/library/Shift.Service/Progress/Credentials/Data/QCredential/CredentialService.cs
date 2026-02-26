using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Progress;

public class CredentialService : IEntityService
{
    private readonly CredentialReader _reader;
    private readonly CredentialAdapter _adapter = new CredentialAdapter();

    public CredentialService(CredentialReader reader)
    {
        _reader = reader;
    }

    public async Task<bool> AssertAsync(Guid credential, Guid? organization, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(credential, organization, cancellation);
    }

    public async Task<IEnumerable<CredentialModel>> CollectAsync(ICredentialCriteria criteria, TimeZoneInfo? timeZone, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities, timeZone);
    }

    public async Task<int> CountAsync(ICredentialCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    }

    public async IAsyncEnumerable<CredentialModel> DownloadAsync(ICredentialCriteria criteria, TimeZoneInfo? timeZone, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity, timeZone);
        }
    }

    public async Task<CredentialModel?> RetrieveAsync(Guid credential, TimeZoneInfo? timeZone, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(credential, cancellation);

        return entity != null ? _adapter.ToModel(entity, timeZone) : null;
    }

    public async Task<IEnumerable<CredentialMatch>> SearchAsync(ICredentialCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}