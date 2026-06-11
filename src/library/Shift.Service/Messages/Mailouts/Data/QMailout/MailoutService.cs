using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Messaging;

public class MailoutService : IEntityService
{
    private readonly MailoutReader _reader;
    private readonly MailoutWriter _writer;
    private readonly MailoutAdapter _adapter = new MailoutAdapter();

    public MailoutService(MailoutReader reader, MailoutWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid mailout, Guid? organization, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(mailout, organization, cancellation);
    }

    public async Task<IEnumerable<MailoutModel>> CollectAsync(IMailoutCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IMailoutCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    }

    public async Task<bool> CreateAsync(CreateMailout create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid mailout, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(mailout, cancellation);
    }

    public async IAsyncEnumerable<MailoutModel> DownloadAsync(IMailoutCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyMailout modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.MailoutId, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<MailoutModel?> RetrieveAsync(Guid mailout, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(mailout, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<MailoutMatch>> SearchAsync(IMailoutCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}