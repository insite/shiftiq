using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Security;

public class UserConnectionService : IEntityService
{
    private readonly UserConnectionReader _reader;
    private readonly UserConnectionWriter _writer;
    private readonly UserConnectionAdapter _adapter = new UserConnectionAdapter();

    public UserConnectionService(UserConnectionReader reader, UserConnectionWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid fromUser, Guid toUser, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(fromUser, toUser, cancellation);
    }

    public async Task<IEnumerable<UserConnectionModel>> CollectAsync(IUserConnectionCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IUserConnectionCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    }

    public async Task<bool> CreateAsync(CreateUserConnection create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid fromUser, Guid toUser, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(fromUser, toUser, cancellation);
    }

    public async IAsyncEnumerable<UserConnectionModel> DownloadAsync(IUserConnectionCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyUserConnection modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.FromUserIdentifier, modify.ToUserIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<UserConnectionModel?> RetrieveAsync(Guid fromUser, Guid toUser, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(fromUser, toUser, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<UserConnectionMatch>> SearchAsync(IUserConnectionCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}