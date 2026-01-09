namespace Shift.Service.Security;

using Shift.Contract;

using Shift.Common;

public class UserFieldService : IEntityService
{
    private readonly TUserFieldReader _reader;
    private readonly TUserFieldWriter _writer;

    private readonly TUserFieldAdapter _adapter = new TUserFieldAdapter();

    public UserFieldService(TUserFieldReader reader, TUserFieldWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid setting, CancellationToken cancellation)
        => await _reader.AssertAsync(setting, cancellation);

    public async Task<UserFieldModel?> RetrieveAsync(Guid setting, CancellationToken cancellation)
    {
        var entity = await _reader.RetrieveAsync(setting, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<int> CountAsync(IUserFieldCriteria criteria, CancellationToken cancellation)
        => await _reader.CountAsync(criteria, cancellation);

    public async Task<IEnumerable<UserFieldModel>> CollectAsync(IUserFieldCriteria criteria, CancellationToken cancellation)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<IEnumerable<UserFieldMatch>> SearchAsync(IUserFieldCriteria criteria, CancellationToken cancellation)
        => await _reader.SearchAsync(criteria, cancellation);

    public async Task<bool> CreateAsync(CreateUserField create, CancellationToken cancellation)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> ModifyAsync(ModifyUserField modify, CancellationToken cancellation)
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