using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Directory;

public class GroupService : IEntityService
{
    private readonly QGroupReader _reader;
    private readonly QGroupWriter _writer;
    private readonly QGroupAdapter _adapter = new QGroupAdapter();

    public GroupService(QGroupReader reader, QGroupWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(
        Guid group,
        CancellationToken cancellation = default)
        => await _reader.AssertAsync(group, cancellation);

    public async Task<IEnumerable<GroupModel>> CollectAsync(IGroupCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(
        IGroupCriteria criteria,
        CancellationToken cancellation = default)
        => await _reader.CountAsync(criteria, cancellation);

    public async Task<bool> CreateAsync(CreateGroup create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(
        Guid group,
        CancellationToken cancellation = default)
        => await _writer.DeleteAsync(group, cancellation);

    public async Task<IEnumerable<GroupModel>> DownloadAsync(
        IGroupCriteria criteria,
        CancellationToken cancellation)
    {
        var entities = await _reader.DownloadAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<bool> ModifyAsync(
        ModifyGroup modify,
        CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.GroupIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<GroupModel?> RetrieveAsync(
        Guid group,
        CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(group, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<Guid[]> SearchUserRolesAsync(Guid? parentOrganizationId, Guid organizationId, Guid userId)
    {
        return await _reader.SearchUserRolesAsync(parentOrganizationId, organizationId, userId);
    }

    public async Task<IEnumerable<GroupMatch>> SearchAsync(
        IGroupCriteria criteria,
        CancellationToken cancellation = default)
        => await _reader.SearchAsync(criteria, cancellation);

    public string Serialize(IEnumerable<GroupModel> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}