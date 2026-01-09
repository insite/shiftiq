using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Directory;

public class PersonSecretService : IEntityService
{
    private readonly QPersonSecretReader _reader;
    private readonly QPersonSecretWriter _writer;
    private readonly QPersonSecretAdapter _adapter = new QPersonSecretAdapter();

    public PersonSecretService(QPersonSecretReader reader, QPersonSecretWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(
        Guid secret, 
        CancellationToken cancellation = default)
        => await _reader.AssertAsync(secret, cancellation);

    public async Task<IEnumerable<PersonSecretModel>> CollectAsync(IPersonSecretCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(
        IPersonSecretCriteria criteria, 
        CancellationToken cancellation = default)
        => await _reader.CountAsync(criteria, cancellation);

    public async Task<bool> CreateAsync(CreatePersonSecret create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(
        Guid secret, 
        CancellationToken cancellation = default)
        => await _writer.DeleteAsync(secret, cancellation);

    public async Task<IEnumerable<PersonSecretModel>> DownloadAsync(
        IPersonSecretCriteria criteria, 
        CancellationToken cancellation)
    {
        var entities = await _reader.DownloadAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<bool> ModifyAsync(
        ModifyPersonSecret modify, 
        CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.SecretIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<PersonSecretModel?> RetrieveAsync(
        Guid secret, 
        CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(secret, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<PersonSecretMatch>> SearchAsync(
        IPersonSecretCriteria criteria, 
        CancellationToken cancellation = default)
        => await _reader.SearchAsync(criteria, cancellation);

    public string Serialize(IEnumerable<PersonSecretModel> models, string format)
        => _adapter.Serialize(models, format);
}