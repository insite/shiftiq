using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Content;

public class FileClaimService : IEntityService
{
    private readonly IFileClaimReader _reader;
    private readonly IFileClaimWriter _writer;
    private readonly FileClaimAdapter _adapter = new FileClaimAdapter();

    public FileClaimService(IFileClaimReader reader, IFileClaimWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(
        Guid claim, 
        CancellationToken cancellation = default)
        => await _reader.AssertAsync(claim, cancellation);

    public async Task<IEnumerable<FileClaimModel>> CollectAsync(IFileClaimCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(
        IFileClaimCriteria criteria, 
        CancellationToken cancellation = default)
        => await _reader.CountAsync(criteria, cancellation);

    public async Task<bool> CreateAsync(CreateFileClaim create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(
        Guid claim, 
        CancellationToken cancellation = default)
        => await _writer.DeleteAsync(claim, cancellation);

    public async Task<IEnumerable<FileClaimModel>> DownloadAsync(
        IFileClaimCriteria criteria, 
        CancellationToken cancellation)
    {
        var entities = await _reader.DownloadAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<bool> ModifyAsync(
        ModifyFileClaim modify, 
        CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.ClaimIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<FileClaimModel?> RetrieveAsync(
        Guid claim, 
        CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(claim, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<FileClaimMatch>> SearchAsync(
        IFileClaimCriteria criteria, 
        CancellationToken cancellation = default)
        => await _reader.SearchAsync(criteria, cancellation);

    public string Serialize(IEnumerable<FileClaimModel> models, string format)
        => _adapter.Serialize(models, format);
}