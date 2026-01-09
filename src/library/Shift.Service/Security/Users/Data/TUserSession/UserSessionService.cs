using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Security;

public class UserSessionService : IEntityService
{
    private readonly UserSessionReader _reader;
    private readonly UserSessionWriter _writer;
    private readonly UserSessionAdapter _adapter = new UserSessionAdapter();

    public UserSessionService(UserSessionReader reader, UserSessionWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid session, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(session, cancellation);
    }

    public async Task<IEnumerable<UserSessionModel>> CollectAsync(IUserSessionCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IUserSessionCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    }

    public async Task<bool> CreateAsync(CreateUserSession create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid session, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(session, cancellation);
    }

    public async IAsyncEnumerable<UserSessionModel> DownloadAsync(IUserSessionCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyUserSession modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.SessionIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<UserSessionModel?> RetrieveAsync(Guid session, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(session, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<UserSessionMatch>> SearchAsync(IUserSessionCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}