using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Content;

public class InputService : IEntityService
{
    private readonly TInputReader _reader;
    private readonly TInputWriter _writer;

    private readonly TInputAdapter _adapter = new TInputAdapter();

    public InputService(TInputReader reader, TInputWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid content, CancellationToken cancellation = default)
        => await _reader.AssertAsync(content, cancellation);

    public async Task<InputModel?> RetrieveAsync(Guid content, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(content, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<int> CountAsync(IInputCriteria criteria, CancellationToken cancellation = default)
        => await _reader.CountAsync(criteria, cancellation);

    public async Task<IEnumerable<InputModel>> CollectAsync(IInputCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<IEnumerable<InputMatch>> SearchAsync(IInputCriteria criteria, CancellationToken cancellation = default)
        => await _reader.SearchAsync(criteria, cancellation);

    public async Task<bool> CreateAsync(CreateInput create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> ModifyAsync(ModifyInput modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.ContentIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid content, CancellationToken cancellation = default)
        => await _writer.DeleteAsync(content, cancellation);
}