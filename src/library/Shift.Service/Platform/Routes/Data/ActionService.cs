namespace Shift.Service.Metadata;

using Shift.Contract;

public class ActionService : IActionService
{
    private readonly TActionReader _reader;
    private readonly TActionWriter _writer;

    private readonly TActionAdapter _adapter = new();
    private readonly List<ActionModel> _actions = new();
    private readonly Dictionary<string, ActionModel> _actionsByUrl = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<Guid, ActionModel> _actionsById = new();

    public ActionService(TActionReader reader, TActionWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<IEnumerable<ActionModel>> DownloadAsync(
        IActionCriteria criteria,
        CancellationToken cancellation = default)
    {
        var entities = await _reader.DownloadAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task RefreshAsync()
    {
        var actions = await _reader.DownloadAsync(new CollectActions());

        lock (_actions)
        {
            _actions.Clear();
            _actionsByUrl.Clear();
            _actionsById.Clear();

            foreach (var action in actions)
            {
                var model = _adapter.ToModel(action);

                _actions.Add(model);
                _actionsByUrl.Add(model.ActionUrl, model);
                _actionsById.Add(model.ActionIdentifier, model);
            }
        }
    }

    public List<ActionModel> Search(Func<ActionModel, bool> predicate)
    {
        return _actions.Where(predicate).ToList();
    }

    public ActionModel? Retrieve(Guid action)
    {
        _actionsById.TryGetValue(action, out var model);
        return model;
    }

    public ActionModel? Retrieve(string actionUrl)
    {
        _actionsByUrl.TryGetValue(actionUrl, out var model);
        return model;
    }

    public async Task<ActionModel?> RetrieveAsync(Guid action, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(action, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<ActionModel?> RetrieveAsync(string actionUrl, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(actionUrl, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<bool> AssertAsync(Guid action, CancellationToken cancellation = default)
        => await _reader.AssertAsync(action, cancellation);

    public async Task<int> CountAsync(IActionCriteria criteria, CancellationToken cancellation = default)
        => await _reader.CountAsync(criteria, cancellation);

    public async Task<IEnumerable<ActionModel>> CollectAsync(IActionCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<IEnumerable<ActionMatch>> SearchAsync(IActionCriteria criteria, CancellationToken cancellation = default)
        => await _reader.SearchAsync(criteria, cancellation);

    public async Task<bool> CreateAsync(CreateAction create, CancellationToken cancellation)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> ModifyAsync(ModifyAction modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.ActionIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid action, CancellationToken cancellation = default)
        => await _writer.DeleteAsync(action, cancellation);
}