namespace Shift.Service.Security;

using Shift.Contract;

using Shift.Common;

public class OrganizationService : IEntityService
{
    private readonly QOrganizationReader _reader;
    private readonly QOrganizationAdapter _adapter;

    public OrganizationService(QOrganizationReader reader, QOrganizationAdapter adapter)
    {
        _reader = reader;
        _adapter = adapter;
    }

    public async Task<bool> AssertAsync(Guid organization, CancellationToken cancellation)
        => await _reader.AssertAsync(organization, cancellation);

    public async Task<OrganizationModel?> RetrieveAsync(Guid organization, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(organization, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<int> CountAsync(IOrganizationCriteria criteria, CancellationToken cancellation)
        => await _reader.CountAsync(criteria, cancellation);

    public async Task<IEnumerable<OrganizationModel>> CollectAsync(IOrganizationCriteria criteria, CancellationToken cancellation)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<IEnumerable<OrganizationMatch>> SearchAsync(IOrganizationCriteria criteria, CancellationToken cancellation)
        => await _reader.SearchAsync(criteria, cancellation);
}