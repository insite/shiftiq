using Shift.Contract;

namespace Shift.Service.Content.Searches.Dtos;

public record SearchEntryViewDto(
    SearchObjectType ObjectType,
    Guid ObjectId,
    string Title,
    string Url,
    string[]? Attributes
);