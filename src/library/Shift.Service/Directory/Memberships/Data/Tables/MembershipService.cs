using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Directory;

public class MembershipService : IEntityService
{
    private readonly QMembershipReader _reader;
    private readonly QMembershipWriter _writer;
    private readonly QMembershipAdapter _adapter = new QMembershipAdapter();

    public MembershipService(QMembershipReader reader, QMembershipWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(
        Guid membership,
        CancellationToken cancellation = default)
        => await _reader.AssertAsync(membership, cancellation);

    public async Task<IEnumerable<MembershipModel>> CollectAsync(IMembershipCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(
        IMembershipCriteria criteria,
        CancellationToken cancellation = default)
        => await _reader.CountAsync(criteria, cancellation);

    public async Task<bool> CreateAsync(CreateMembership create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(
        Guid membership,
        CancellationToken cancellation = default)
        => await _writer.DeleteAsync(membership, cancellation);

    public async Task<IEnumerable<MembershipModel>> DownloadAsync(
        IMembershipCriteria criteria,
        CancellationToken cancellation)
    {
        var entities = await _reader.DownloadAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<bool> ModifyAsync(
        ModifyMembership modify,
        CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.MembershipIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<MembershipModel?> RetrieveAsync(
        Guid membership,
        CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(membership, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<MembershipMatch>> SearchAsync(
        IMembershipCriteria criteria,
        CancellationToken cancellation = default)
        => await _reader.SearchAsync(criteria, cancellation);

    public string Serialize(IEnumerable<MembershipModel> models, string format, string includes)
        => _adapter.Serialize(models, format, includes);
}