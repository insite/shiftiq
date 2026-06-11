using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Messaging;

public class RecipientService : IEntityService
{
    private readonly RecipientReader _reader;
    private readonly RecipientWriter _writer;
    private readonly RecipientAdapter _adapter = new RecipientAdapter();

    public RecipientService(RecipientReader reader, RecipientWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid recipient, Guid? organization, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(recipient, organization, cancellation);
    }

    public async Task<IEnumerable<RecipientModel>> CollectAsync(IRecipientCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IRecipientCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    }

    public async Task<bool> CreateAsync(CreateRecipient create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid recipient, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(recipient, cancellation);
    }

    public async IAsyncEnumerable<RecipientModel> DownloadAsync(IRecipientCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyRecipient modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.RecipientId, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<RecipientModel?> RetrieveAsync(Guid recipient, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(recipient, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<RecipientMatch>> SearchAsync(IRecipientCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}