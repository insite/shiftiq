namespace Shift.Service.Content.Searches.Dtos;

internal record SearchEntryDto(
    string ObjectType,
    Guid ObjectIdentifier,
    string Title,
    string? Attributes,
    int Rank
);