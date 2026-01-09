using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Directory;

public class MembershipService : IEntityService
{
    private readonly MembershipReader _reader;
    private readonly MembershipWriter _writer;
    private readonly MembershipAdapter _adapter = new MembershipAdapter();

    public MembershipService(MembershipReader reader, MembershipWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid membership, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(membership, cancellation);
    }

    public async Task<IEnumerable<MembershipModel>> CollectAsync(IMembershipCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IMembershipCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    }

    public async Task<bool> CreateAsync(CreateMembership create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid membership, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(membership, cancellation);
    }

    public async IAsyncEnumerable<MembershipModel> DownloadAsync(IMembershipCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyMembership modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.MembershipIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<MembershipModel?> RetrieveAsync(Guid membership, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(membership, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<MembershipMatch>> SearchAsync(IMembershipCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}