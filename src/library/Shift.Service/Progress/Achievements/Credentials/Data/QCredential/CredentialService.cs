using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Progress;

public class CredentialService : IEntityService
{
    private readonly QCredentialReader _reader;
    private readonly QCredentialWriter _writer;

    private readonly QCredentialAdapter _adapter = new QCredentialAdapter();

    public CredentialService(QCredentialReader reader, QCredentialWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid credential, CancellationToken cancellation = default)
        => await _reader.AssertAsync(credential, cancellation);

    public async Task<CredentialModel?> RetrieveAsync(Guid credential, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(credential, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<int> CountAsync(ICredentialCriteria criteria, CancellationToken cancellation = default)
        => await _reader.CountAsync(criteria, cancellation);

    public async Task<IEnumerable<CredentialModel>> CollectAsync(ICredentialCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<IEnumerable<CredentialMatch>> SearchAsync(ICredentialCriteria criteria, CancellationToken cancellation = default)
        => await _reader.SearchAsync(criteria, cancellation);

    public async Task<bool> CreateAsync(CreateCredential create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> ModifyAsync(ModifyCredential modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.CredentialIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid credential, CancellationToken cancellation = default)
        => await _writer.DeleteAsync(credential, cancellation);
}