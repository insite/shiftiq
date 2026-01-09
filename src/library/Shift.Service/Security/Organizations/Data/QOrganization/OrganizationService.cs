using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Security;

public class OrganizationService : IEntityService
{
    private readonly OrganizationReader _reader;
    private readonly OrganizationWriter _writer;
    private readonly OrganizationAdapter _adapter = new OrganizationAdapter();

    public OrganizationService(OrganizationReader reader, OrganizationWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid organization, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(organization, cancellation);
    }

    public async Task<IEnumerable<OrganizationModel>> CollectAsync(IOrganizationCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IOrganizationCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    }

    public async Task<bool> CreateAsync(CreateOrganization create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid organization, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(organization, cancellation);
    }

    public async IAsyncEnumerable<OrganizationModel> DownloadAsync(IOrganizationCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyOrganization modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.OrganizationIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<OrganizationModel?> RetrieveAsync(Guid organization, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(organization, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<OrganizationMatch>> SearchAsync(IOrganizationCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}