using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Assessment;

public class AttemptSectionService : IEntityService
{
    private readonly AttemptSectionReader _reader;
    private readonly AttemptSectionWriter _writer;
    private readonly AttemptSectionAdapter _adapter = new AttemptSectionAdapter();

    public AttemptSectionService(AttemptSectionReader reader, AttemptSectionWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid attempt, int sectionIndex, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(attempt, sectionIndex, cancellation);
    }

    public async Task<IEnumerable<AttemptSectionModel>> CollectAsync(IAttemptSectionCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IAttemptSectionCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    } 

    public async Task<bool> CreateAsync(CreateAttemptSection create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid attempt, int sectionIndex, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(attempt, sectionIndex, cancellation);
    }

    public async IAsyncEnumerable<AttemptSectionModel> DownloadAsync(IAttemptSectionCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyAttemptSection modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.AttemptIdentifier, modify.SectionIndex, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<AttemptSectionModel?> RetrieveAsync(Guid attempt, int sectionIndex, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(attempt, sectionIndex, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<AttemptSectionMatch>> SearchAsync(IAttemptSectionCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}