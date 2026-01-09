using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Assessment;

public class BankFormService : IEntityService
{
    private readonly BankFormReader _reader;
    private readonly BankFormWriter _writer;
    private readonly BankFormAdapter _adapter = new BankFormAdapter();

    public BankFormService(BankFormReader reader, BankFormWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid form, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(form, cancellation);
    }

    public async Task<IEnumerable<BankFormModel>> CollectAsync(IBankFormCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IBankFormCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    } 

    public async Task<bool> CreateAsync(CreateBankForm create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid form, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(form, cancellation);
    }

    public async IAsyncEnumerable<BankFormModel> DownloadAsync(IBankFormCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyBankForm modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.FormIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<BankFormModel?> RetrieveAsync(Guid form, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(form, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<BankFormMatch>> SearchAsync(IBankFormCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}