using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Content;

public class FileActivityService : IEntityService
{
    private readonly FileActivityReader _reader;
    private readonly FileActivityWriter _writer;
    private readonly FileActivityAdapter _adapter = new FileActivityAdapter();

    public FileActivityService(FileActivityReader reader, FileActivityWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid activity, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(activity, cancellation);
    }

    public async Task<IEnumerable<FileActivityModel>> CollectAsync(IFileActivityCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IFileActivityCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    }

    public async Task<bool> CreateAsync(CreateFileActivity create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid activity, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(activity, cancellation);
    }

    public async IAsyncEnumerable<FileActivityModel> DownloadAsync(IFileActivityCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyFileActivity modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.ActivityIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<FileActivityModel?> RetrieveAsync(Guid activity, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(activity, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<FileActivityMatch>> SearchAsync(IFileActivityCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}