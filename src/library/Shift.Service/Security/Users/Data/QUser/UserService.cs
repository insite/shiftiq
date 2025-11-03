using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Security;

public class UserService : IEntityService
{
    private readonly UserReader _reader;
    private readonly UserWriter _writer;
    private readonly UserAdapter _adapter = new UserAdapter();

    public UserService(UserReader reader, UserWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid user, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(user, cancellation);
    }

    public async Task<IEnumerable<UserModel>> CollectAsync(IUserCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IUserCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    }

    public async Task<bool> CreateAsync(CreateUser create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid user, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(user, cancellation);
    }

    public async IAsyncEnumerable<UserModel> DownloadAsync(IUserCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyUser modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.UserIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<UserModel?> RetrieveAsync(Guid user, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(user, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<UserMatch>> SearchAsync(IUserCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}