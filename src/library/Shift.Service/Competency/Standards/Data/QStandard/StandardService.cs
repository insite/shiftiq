using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Competency;

public class StandardService : IEntityService
{
    private readonly StandardReader _reader;
    private readonly StandardWriter _writer;
    private readonly StandardAdapter _adapter = new StandardAdapter();

    public StandardService(StandardReader reader, StandardWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid standard, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(standard, cancellation);
    }

    public async Task<IEnumerable<StandardModel>> CollectAsync(IStandardCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IStandardCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    }

    public async Task<bool> CreateAsync(CreateStandard create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid standard, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(standard, cancellation);
    }

    public async IAsyncEnumerable<StandardModel> DownloadAsync(IStandardCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyStandard modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.StandardIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<StandardModel?> RetrieveAsync(Guid standard, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(standard, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<StandardMatch>> SearchAsync(IStandardCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}