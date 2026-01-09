using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Workflow;

public class CaseDocumentRequestService : IEntityService
{
    private readonly CaseDocumentRequestReader _reader;
    private readonly CaseDocumentRequestWriter _writer;
    private readonly CaseDocumentRequestAdapter _adapter = new CaseDocumentRequestAdapter();

    public CaseDocumentRequestService(CaseDocumentRequestReader reader, CaseDocumentRequestWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid issue, string requestedFileCategory, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(issue, requestedFileCategory, cancellation);
    }

    public async Task<IEnumerable<CaseDocumentRequestModel>> CollectAsync(ICaseDocumentRequestCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(ICaseDocumentRequestCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    }

    public async Task<bool> CreateAsync(CreateCaseDocumentRequest create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid issue, string requestedFileCategory, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(issue, requestedFileCategory, cancellation);
    }

    public async IAsyncEnumerable<CaseDocumentRequestModel> DownloadAsync(ICaseDocumentRequestCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyCaseDocumentRequest modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.CaseIdentifier, modify.RequestedFileCategory, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<CaseDocumentRequestModel?> RetrieveAsync(Guid issue, string requestedFileCategory, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(issue, requestedFileCategory, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<CaseDocumentRequestMatch>> SearchAsync(ICaseDocumentRequestCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}