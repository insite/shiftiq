using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Security;

public class PermissionService : IEntityService
{
    private readonly TPermissionReader _reader;
    private readonly TPermissionWriter _writer;
    private readonly TPermissionAdapter _adapter = new TPermissionAdapter();

    public PermissionService(TPermissionReader reader, TPermissionWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(
        Guid permission,
        CancellationToken cancellation = default)
        => await _reader.AssertAsync(permission, cancellation);

    public async Task<IEnumerable<PermissionModel>> CollectAsync(IPermissionCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(
        IPermissionCriteria criteria,
        CancellationToken cancellation = default)
        => await _reader.CountAsync(criteria, cancellation);

    public async Task<bool> CreateAsync(CreatePermission create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(
        Guid permission,
        CancellationToken cancellation = default)
        => await _writer.DeleteAsync(permission, cancellation);

    public async Task<IEnumerable<PermissionModel>> DownloadAsync(
        IPermissionCriteria criteria,
        CancellationToken cancellation)
    {
        var entities = await _reader.DownloadAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<bool> ModifyAsync(
        ModifyPermission modify,
        CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.PermissionIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<PermissionModel?> RetrieveAsync(
        Guid permission,
        CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(permission, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<PermissionMatch>> SearchAsync(
        IPermissionCriteria criteria,
        CancellationToken cancellation = default)
        => await _reader.SearchAsync(criteria, cancellation);

    public string Serialize(IEnumerable<PermissionModel> models, string format)
        => _adapter.Serialize(models, format);
}