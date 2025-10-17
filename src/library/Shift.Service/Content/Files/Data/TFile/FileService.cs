using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Content;

public class FileService : IEntityService
{
    private readonly IFileReader _reader;
    private readonly IFileWriter _writer;
    private readonly FileAdapter _adapter = new FileAdapter();

    public FileService(IFileReader reader, IFileWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(
        Guid file,
        CancellationToken cancellation = default)
        => await _reader.AssertAsync(file, cancellation);

    public async Task<IEnumerable<FileModel>> CollectAsync(IFileCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(
        IFileCriteria criteria,
        CancellationToken cancellation = default)
        => await _reader.CountAsync(criteria, cancellation);

    public async Task<bool> CreateAsync(CreateFile create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(
        Guid file,
        CancellationToken cancellation = default)
        => await _writer.DeleteAsync(file, cancellation);

    public async Task<IEnumerable<FileDownload>> DownloadAsync(
        IFileCriteria criteria,
        CancellationToken cancellation)
    {
        var entities = await _reader.DownloadAsync(criteria, cancellation);

        return _adapter.ToDownload(entities);
    }

    public async Task<bool> ModifyAsync(
        ModifyFile modify,
        CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.FileIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<FileModel?> RetrieveAsync(
        Guid file,
        CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(file, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<FileMatch>> SearchAsync(
        IFileCriteria criteria,
        CancellationToken cancellation = default)
        => await _reader.SearchAsync(criteria, cancellation);

    public string Serialize(IEnumerable<FileDownload> models, string format, string includes)
        => _adapter.Serialize(models, format, includes);
}