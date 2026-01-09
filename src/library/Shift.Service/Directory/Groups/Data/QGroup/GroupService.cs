using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Directory;

public class GroupService : IEntityService
{
    private readonly GroupReader _reader;
    private readonly GroupWriter _writer;
    private readonly GroupAdapter _adapter = new GroupAdapter();

    public GroupService(GroupReader reader, GroupWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid group, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(group, cancellation);
    }

    public async Task<IEnumerable<GroupModel>> CollectAsync(IGroupCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IGroupCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    }

    public async Task<bool> CreateAsync(CreateGroup create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid group, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(group, cancellation);
    }

    public async IAsyncEnumerable<GroupModel> DownloadAsync(IGroupCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyGroup modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.GroupIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<GroupModel?> RetrieveAsync(Guid group, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(group, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<GroupMatch>> SearchAsync(IGroupCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public async Task<string[]> SearchUserRolesAsync(Guid? parentOrganizationId, Guid organizationId, Guid userId)
    {
        return await _reader.SearchUserRolesAsync(parentOrganizationId, organizationId, userId);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}