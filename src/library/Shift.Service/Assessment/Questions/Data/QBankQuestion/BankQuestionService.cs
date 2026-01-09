using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Assessment;

public class BankQuestionService : IEntityService
{
    private readonly BankQuestionReader _reader;
    private readonly BankQuestionWriter _writer;
    private readonly BankQuestionAdapter _adapter = new BankQuestionAdapter();

    public BankQuestionService(BankQuestionReader reader, BankQuestionWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid question, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(question, cancellation);
    }

    public async Task<IEnumerable<BankQuestionModel>> CollectAsync(IBankQuestionCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IBankQuestionCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    } 

    public async Task<bool> CreateAsync(CreateBankQuestion create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid question, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(question, cancellation);
    }

    public async IAsyncEnumerable<BankQuestionModel> DownloadAsync(IBankQuestionCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyBankQuestion modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.QuestionIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<BankQuestionModel?> RetrieveAsync(Guid question, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(question, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<BankQuestionMatch>> SearchAsync(IBankQuestionCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}