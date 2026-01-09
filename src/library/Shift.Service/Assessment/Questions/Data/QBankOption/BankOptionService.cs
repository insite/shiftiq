using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Assessment;

public class BankOptionService : IEntityService
{
    private readonly BankOptionReader _reader;
    private readonly BankOptionWriter _writer;
    private readonly BankOptionAdapter _adapter = new BankOptionAdapter();

    public BankOptionService(BankOptionReader reader, BankOptionWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(int optionKey, Guid question, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(optionKey, question, cancellation);
    }

    public async Task<IEnumerable<BankOptionModel>> CollectAsync(IBankOptionCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IBankOptionCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    } 

    public async Task<bool> CreateAsync(CreateBankOption create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(int optionKey, Guid question, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(optionKey, question, cancellation);
    }

    public async IAsyncEnumerable<BankOptionModel> DownloadAsync(IBankOptionCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyBankOption modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.OptionKey, modify.QuestionIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<BankOptionModel?> RetrieveAsync(int optionKey, Guid question, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(optionKey, question, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<BankOptionMatch>> SearchAsync(IBankOptionCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}