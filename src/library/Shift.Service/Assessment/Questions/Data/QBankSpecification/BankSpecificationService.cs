using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Assessment;

public class BankSpecificationService : IEntityService
{
    private readonly BankSpecificationReader _reader;
    private readonly BankSpecificationWriter _writer;
    private readonly BankSpecificationAdapter _adapter = new BankSpecificationAdapter();

    public BankSpecificationService(BankSpecificationReader reader, BankSpecificationWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid spec, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(spec, cancellation);
    }

    public async Task<IEnumerable<BankSpecificationModel>> CollectAsync(IBankSpecificationCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IBankSpecificationCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    } 

    public async Task<bool> CreateAsync(CreateBankSpecification create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid spec, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(spec, cancellation);
    }

    public async IAsyncEnumerable<BankSpecificationModel> DownloadAsync(IBankSpecificationCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyBankSpecification modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.SpecIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<BankSpecificationModel?> RetrieveAsync(Guid spec, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(spec, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<BankSpecificationMatch>> SearchAsync(IBankSpecificationCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}