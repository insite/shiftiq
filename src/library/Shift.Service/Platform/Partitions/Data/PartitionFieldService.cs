namespace Shift.Service.Security;

using InSite.Domain;

using Shift.Contract;

public class PartitionFieldService
{
    private readonly TPartitionFieldReader _reader;
    private readonly TPartitionFieldWriter _writer;
    private readonly TPartitionFieldAdapter _adapter = new TPartitionFieldAdapter();

    private IPartitionModel? _partitionModel;

    public PartitionFieldService(TPartitionFieldReader reader, TPartitionFieldWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid setting, CancellationToken cancellation)
        => await _reader.AssertAsync(setting, cancellation);

    public async Task<PartitionFieldModel?> RetrieveAsync(Guid setting, CancellationToken cancellation)
    {
        var entity = await _reader.RetrieveAsync(setting, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IPartitionModel> RetrieveModelAsync(CancellationToken cancellation = default)
    {
        if (_partitionModel != null)
            return _partitionModel;

        var entities = await _reader.CollectAsync(new CollectPartitionFields(), cancellation);

        _partitionModel = new PartitionModel();

        _partitionModel.Host = GetValueAsString("Host:Name");
        _partitionModel.Name = GetValueAsString("Partition:Name");
        _partitionModel.Number = GetValueAsInt("Partition:Number");
        _partitionModel.Slug = GetValueAsString("Partition:Slug");
        _partitionModel.Email = GetValueAsString("Support:Email");
        _partitionModel.WhitelistDomains = GetValueAsString("Whitelist:Domains");
        _partitionModel.WhitelistEmails = GetValueAsString("Whitelist:Emails");
        _partitionModel.DatabaseMonitorLargeCommandSize = GetValueAsInt("Database:LargeCommandSize");

        return _partitionModel;

        string GetValueAsString(string name)
        {
            var entity = entities.FirstOrDefault(x => x.SettingName == name);
            if (entity == null)
                throw new ArgumentException($"Setting name not found: {name}", nameof(name));

            return entity.SettingValue;
        }

        int GetValueAsInt(string name)
        {
            var value = GetValueAsString(name);
            if (int.TryParse(value, out var valueAsInt))
                return valueAsInt;

            throw new ArgumentException($"Setting value for {name} is not an integer: {value}", nameof(name));
        }
    }

    public async Task<int> CountAsync(IPartitionFieldCriteria criteria, CancellationToken cancellation)
        => await _reader.CountAsync(criteria, cancellation);

    public async Task<IEnumerable<PartitionFieldModel>> CollectAsync(IPartitionFieldCriteria criteria, CancellationToken cancellation)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<IEnumerable<PartitionFieldMatch>> SearchAsync(IPartitionFieldCriteria criteria, CancellationToken cancellation)
        => await _reader.SearchAsync(criteria, cancellation);

    public async Task<bool> CreateAsync(CreatePartitionField create, CancellationToken cancellation)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> ModifyAsync(ModifyPartitionField modify, CancellationToken cancellation)
    {
        var entity = await _reader.RetrieveAsync(modify.SettingIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid setting, CancellationToken cancellation)
        => await _writer.DeleteAsync(setting, cancellation);
}