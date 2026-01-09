using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Assessment;

public class BankService : IEntityService
{
    private readonly BankReader _reader;
    private readonly BankWriter _writer;
    private readonly BankAdapter _adapter = new BankAdapter();

    public BankService(BankReader reader, BankWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid bank, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(bank, cancellation);
    }

    public async Task<IEnumerable<BankModel>> CollectAsync(IBankCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IBankCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    } 

    public async Task<bool> CreateAsync(CreateBank create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid bank, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(bank, cancellation);
    }

    public async IAsyncEnumerable<BankModel> DownloadAsync(IBankCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyBank modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.BankIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<BankModel?> RetrieveAsync(Guid bank, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(bank, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<BankMatch>> SearchAsync(IBankCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}