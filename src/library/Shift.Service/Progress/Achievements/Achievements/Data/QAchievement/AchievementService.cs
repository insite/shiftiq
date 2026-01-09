using System.Runtime.CompilerServices;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Progress;

public class AchievementService : IEntityService
{
    private readonly AchievementReader _reader;
    private readonly AchievementWriter _writer;
    private readonly AchievementAdapter _adapter = new AchievementAdapter();

    public AchievementService(AchievementReader reader, AchievementWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid achievement, CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(achievement, cancellation);
    }

    public async Task<IEnumerable<AchievementModel>> CollectAsync(IAchievementCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(IAchievementCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    }

    public async Task<bool> CreateAsync(CreateAchievement create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid achievement, CancellationToken cancellation = default)
    {
        return await _writer.DeleteAsync(achievement, cancellation);
    }

    public async IAsyncEnumerable<AchievementModel> DownloadAsync(IAchievementCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var entity in _reader.DownloadAsync(criteria, cancellation))
        {
            yield return _adapter.ToModel(entity);
        }
    }

    public async Task<bool> ModifyAsync(ModifyAchievement modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.AchievementIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<AchievementModel?> RetrieveAsync(Guid achievement, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(achievement, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<AchievementMatch>> SearchAsync(IAchievementCriteria criteria, CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return _adapter.Serialize(models, format, includes);
    }
}