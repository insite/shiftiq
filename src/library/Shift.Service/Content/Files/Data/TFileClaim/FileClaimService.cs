using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Content;

public class FileClaimService : IEntityService
{
    private readonly FileClaimReader _reader;
    private readonly FileClaimWriter _writer;
    private readonly FileClaimAdapter _adapter = new FileClaimAdapter();

    public FileClaimService(FileClaimReader reader, FileClaimWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid claim, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(claim, cancellation);
    }

    public async Task<IEnumerable<FileClaimModel>> CollectAsync(IFileClaimCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IFileClaimCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    }

    public async Task<bool> CreateAsync(CreateFileClaim create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid claim, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(claim, cancellation);
    }

    public async IAsyncEnumerable<FileClaimModel> DownloadAsync(IFileClaimCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyFileClaim modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.ClaimIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<FileClaimModel?> RetrieveAsync(Guid claim, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(claim, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<FileClaimMatch>> SearchAsync(IFileClaimCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}