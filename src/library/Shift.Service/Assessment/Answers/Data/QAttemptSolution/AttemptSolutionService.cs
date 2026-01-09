using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Assessment;

public class AttemptSolutionService : IEntityService
{
    private readonly AttemptSolutionReader _reader;
    private readonly AttemptSolutionWriter _writer;
    private readonly AttemptSolutionAdapter _adapter = new AttemptSolutionAdapter();

    public AttemptSolutionService(AttemptSolutionReader reader, AttemptSolutionWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid attempt, Guid question, Guid solution, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(attempt, question, solution, cancellation);
    }

    public async Task<IEnumerable<AttemptSolutionModel>> CollectAsync(IAttemptSolutionCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IAttemptSolutionCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    } 

    public async Task<bool> CreateAsync(CreateAttemptSolution create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid attempt, Guid question, Guid solution, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(attempt, question, solution, cancellation);
    }

    public async IAsyncEnumerable<AttemptSolutionModel> DownloadAsync(IAttemptSolutionCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyAttemptSolution modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.AttemptIdentifier, modify.QuestionIdentifier, modify.SolutionIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<AttemptSolutionModel?> RetrieveAsync(Guid attempt, Guid question, Guid solution, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(attempt, question, solution, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<AttemptSolutionMatch>> SearchAsync(IAttemptSolutionCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}