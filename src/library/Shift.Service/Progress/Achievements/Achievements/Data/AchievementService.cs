namespace Shift.Service.Achievement;

using Shift.Contract;

using Shift.Common;

public class AchievementService : IEntityService
{
    private readonly QAchievementReader _reader;
    private readonly QAchievementAdapter _adapter = new QAchievementAdapter();

    public AchievementService(QAchievementReader reader)
    {
        _reader = reader;
    }

    public async Task<bool> AssertAsync(Guid achievement, CancellationToken cancellation)
        => await _reader.AssertAsync(achievement, cancellation);

    public async Task<AchievementModel?> RetrieveAsync(Guid achievement, CancellationToken cancellation)
    {
        var entity = await _reader.RetrieveAsync(achievement, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<int> CountAsync(IAchievementCriteria criteria, CancellationToken cancellation)
        => await _reader.CountAsync(criteria, cancellation);

    public async Task<IEnumerable<AchievementModel>> CollectAsync(IAchievementCriteria criteria, CancellationToken cancellation)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<IEnumerable<AchievementMatch>> SearchAsync(IAchievementCriteria criteria, CancellationToken cancellation)
        => await _reader.SearchAsync(criteria, cancellation);
}