using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Workflow;

public class CaseDocumentService : IEntityService
{
    private readonly CaseDocumentReader _reader;
    private readonly CaseDocumentWriter _writer;
    private readonly CaseDocumentAdapter _adapter = new CaseDocumentAdapter();

    public CaseDocumentService(CaseDocumentReader reader, CaseDocumentWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid attachment, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(attachment, cancellation);
    }

    public async Task<IEnumerable<CaseDocumentModel>> CollectAsync(ICaseDocumentCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(ICaseDocumentCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    } 

    public async Task<bool> CreateAsync(CreateCaseDocument create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid attachment, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(attachment, cancellation);
    }

    public async IAsyncEnumerable<CaseDocumentModel> DownloadAsync(ICaseDocumentCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyCaseDocument modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.AttachmentIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<CaseDocumentModel?> RetrieveAsync(Guid attachment, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(attachment, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<CaseDocumentMatch>> SearchAsync(ICaseDocumentCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}