using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Progress;

public class GradebookEnrollmentService : IEntityService
{
    private readonly QGradebookEnrollmentReader _reader;
    private readonly QGradebookEnrollmentWriter _writer;

    private readonly QGradebookEnrollmentAdapter _adapter = new QGradebookEnrollmentAdapter();

    public GradebookEnrollmentService(QGradebookEnrollmentReader reader, QGradebookEnrollmentWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(Guid enrollment, CancellationToken cancellation = default)
        => await _reader.AssertAsync(enrollment, cancellation);

    public async Task<GradebookEnrollmentModel?> RetrieveAsync(Guid enrollment, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(enrollment, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<int> CountAsync(IGradebookEnrollmentCriteria criteria, CancellationToken cancellation = default)
        => await _reader.CountAsync(criteria, cancellation);

    public async Task<IEnumerable<GradebookEnrollmentModel>> CollectAsync(IGradebookEnrollmentCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<IEnumerable<GradebookEnrollmentMatch>> SearchAsync(IGradebookEnrollmentCriteria criteria, CancellationToken cancellation = default)
        => await _reader.SearchAsync(criteria, cancellation);

    public async Task<bool> CreateAsync(CreateGradebookEnrollment create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> ModifyAsync(ModifyGradebookEnrollment modify, CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.EnrollmentIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(Guid enrollment, CancellationToken cancellation = default)
        => await _writer.DeleteAsync(enrollment, cancellation);
}