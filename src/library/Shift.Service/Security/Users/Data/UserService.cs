using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Security;

public class UserService : IEntityService
{
    private readonly QUserReader _reader;
    private readonly QUserWriter _writer;
    private readonly QUserAdapter _adapter = new QUserAdapter();

    public UserService(QUserReader reader, QUserWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(
        Guid user,
        CancellationToken cancellation = default)
        => await _reader.AssertAsync(user, cancellation);

    public async Task<IEnumerable<UserModel>> CollectAsync(IUserCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(
        IUserCriteria criteria,
        CancellationToken cancellation = default)
        => await _reader.CountAsync(criteria, cancellation);

    public async Task<bool> CreateAsync(CreateUser create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(
        Guid user,
        CancellationToken cancellation = default)
        => await _writer.DeleteAsync(user, cancellation);

    public async Task<IEnumerable<UserModel>> DownloadAsync(
        IUserCriteria criteria,
        CancellationToken cancellation)
    {
        var entities = await _reader.DownloadAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<bool> ModifyAsync(
        ModifyUser modify,
        CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.UserIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<UserModel?> RetrieveAsync(
        Guid user,
        CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(user, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<UserMatch>> SearchAsync(
        IUserCriteria criteria,
        CancellationToken cancellation = default)
        => await _reader.SearchAsync(criteria, cancellation);

    public string Serialize(IEnumerable<UserModel> models, string format, string includes)
        => _adapter.Serialize(models, format, includes);
}