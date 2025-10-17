using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Directory;

public class PersonService : IEntityService
{
    private readonly IPersonReader _reader;
    private readonly IPersonWriter _writer;
    private readonly PersonAdapter _adapter = new PersonAdapter();

    public PersonService(IPersonReader reader, IPersonWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task<bool> AssertAsync(
        Guid person,
        CancellationToken cancellation = default)
    {
        return await _reader.AssertAsync(person, cancellation);
    }

    public async Task<IEnumerable<PersonModel>> CollectAsync(IPersonCriteria criteria, CancellationToken cancellation = default)
    {
        var entities = await _reader.CollectAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<int> CountAsync(
        IPersonCriteria criteria,
        CancellationToken cancellation = default)
    {
        return await _reader.CountAsync(criteria, cancellation);
    }

    public async Task<bool> CreateAsync(CreatePerson create, CancellationToken cancellation = default)
    {
        var entity = _adapter.ToEntity(create);

        return await _writer.CreateAsync(entity, cancellation);
    }

    public async Task<bool> DeleteAsync(
        Guid person,
        CancellationToken cancellation = default)
        => await _writer.DeleteAsync(person, cancellation);

    public async Task<IEnumerable<PersonModel>> DownloadAsync(
        IPersonCriteria criteria,
        CancellationToken cancellation)
    {
        var entities = await _reader.DownloadAsync(criteria, cancellation);

        return _adapter.ToModel(entities);
    }

    public async Task<bool> ModifyAsync(
        ModifyPerson modify,
        CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(modify.PersonIdentifier, cancellation);

        if (entity == null)
            return false;

        _adapter.Copy(modify, entity);

        return await _writer.ModifyAsync(entity, cancellation);
    }

    public async Task<PersonModel?> RetrieveAsync(
        Guid person,
        CancellationToken cancellation = default)
    {
        var entity = await _reader.RetrieveAsync(person, cancellation);

        return entity != null ? _adapter.ToModel(entity) : null;
    }

    public async Task<IEnumerable<PersonMatch>> SearchAsync(
        IPersonCriteria criteria,
        CancellationToken cancellation = default)
    {
        return await _reader.SearchAsync(criteria, cancellation);
    }

    public string Serialize<T>(IEnumerable<T> items, string format, string includes)
    {
        return _adapter.Serialize(items, format, includes);
    }
}