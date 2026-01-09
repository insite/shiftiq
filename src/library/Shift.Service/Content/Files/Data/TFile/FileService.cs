using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Content;

public class FileService : IEntityService
{
    private readonly FileReader _reader;
    private readonly FileWriter _writer;
    private readonly FileAdapter _adapter = new FileAdapter();

    public FileService(FileReader reader, FileWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid file, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(file, cancellation);
    }

    public async Task<IEnumerable<FileModel>> CollectAsync(IFileCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IFileCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    }

    public async Task<bool> CreateAsync(CreateFile create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid file, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(file, cancellation);
    }

    public async IAsyncEnumerable<FileModel> DownloadAsync(IFileCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyFile modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.FileIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<FileModel?> RetrieveAsync(Guid file, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(file, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<FileMatch>> SearchAsync(IFileCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}