using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Content;

public class FileActivityService : IEntityService
{
    private readonly IFileActivityReader _reader;
    private readonly IFileActivityWriter _writer;
    private readonly FileActivityAdapter _adapter = new FileActivityAdapter();

    public FileActivityService(IFileActivityReader reader, IFileActivityWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(
        Guid activity, 
        CancellationToken cancellation = default)
        => await _reader.AssertAsync(activity, cancellation);

    public async Task<IEnumerable<FileActivityModel>> CollectAsync(IFileActivityCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(
        IFileActivityCriteria criteria, 
        CancellationToken cancellation = default)
        => await _reader.CountAsync(criteria, cancellation);

    public async Task<bool> CreateAsync(CreateFileActivity create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(
        Guid activity, 
        CancellationToken cancellation = default)
        => await _writer.DeleteAsync(activity, cancellation);

    public async Task<IEnumerable<FileActivityModel>> DownloadAsync(
        IFileActivityCriteria criteria, 
        CancellationToken cancellation)
    {
        var entities = await _reader.DownloadAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<bool> ModifyAsync(
        ModifyFileActivity modify, 
        CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.ActivityIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<FileActivityModel?> RetrieveAsync(
        Guid activity, 
        CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(activity, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<FileActivityMatch>> SearchAsync(
        IFileActivityCriteria criteria, 
        CancellationToken cancellation = default)
        => await _reader.SearchAsync(criteria, cancellation);

    public string Serialize(IEnumerable<FileActivityModel> models, string format)
        => _adapter.Serialize(models, format);
}