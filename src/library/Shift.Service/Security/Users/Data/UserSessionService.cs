using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Security;

public class UserSessionService : IEntityService
{
    private readonly TUserSessionReader _reader;
    private readonly TUserSessionWriter _writer;
    private readonly TUserSessionAdapter _adapter = new TUserSessionAdapter();

    public UserSessionService(TUserSessionReader reader, TUserSessionWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(
        Guid session, 
        CancellationToken cancellation = default)
        => await _reader.AssertAsync(session, cancellation);

    public async Task<IEnumerable<UserSessionModel>> CollectAsync(IUserSessionCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(
        IUserSessionCriteria criteria, 
        CancellationToken cancellation = default)
        => await _reader.CountAsync(criteria, cancellation);

    public async Task<bool> CreateAsync(CreateUserSession create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(
        Guid session, 
        CancellationToken cancellation = default)
        => await _writer.DeleteAsync(session, cancellation);

    public async Task<IEnumerable<UserSessionModel>> DownloadAsync(
        IUserSessionCriteria criteria, 
        CancellationToken cancellation)
    {
        var entities = await _reader.DownloadAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<bool> ModifyAsync(
        ModifyUserSession modify, 
        CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.SessionIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<UserSessionModel?> RetrieveAsync(
        Guid session, 
        CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(session, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<UserSessionMatch>> SearchAsync(
        IUserSessionCriteria criteria, 
        CancellationToken cancellation = default)
        => await _reader.SearchAsync(criteria, cancellation);

    public string Serialize(IEnumerable<UserSessionModel> models, string format)
        => _adapter.Serialize(models, format);
}