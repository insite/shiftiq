using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Assessment;

public class BankFormQuestionGradeitemService : IEntityService
{
    private readonly BankFormQuestionGradeitemReader _reader;
    private readonly BankFormQuestionGradeitemWriter _writer;
    private readonly BankFormQuestionGradeitemAdapter _adapter = new BankFormQuestionGradeitemAdapter();

    public BankFormQuestionGradeitemService(BankFormQuestionGradeitemReader reader, BankFormQuestionGradeitemWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid form, Guid question, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(form, question, cancellation);
    }

    public async Task<IEnumerable<BankFormQuestionGradeitemModel>> CollectAsync(IBankFormQuestionGradeitemCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IBankFormQuestionGradeitemCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    } 

    public async Task<bool> CreateAsync(CreateBankFormQuestionGradeitem create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid form, Guid question, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(form, question, cancellation);
    }

    public async IAsyncEnumerable<BankFormQuestionGradeitemModel> DownloadAsync(IBankFormQuestionGradeitemCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyBankFormQuestionGradeitem modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.FormIdentifier, modify.QuestionIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<BankFormQuestionGradeitemModel?> RetrieveAsync(Guid form, Guid question, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(form, question, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<BankFormQuestionGradeitemMatch>> SearchAsync(IBankFormQuestionGradeitemCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}