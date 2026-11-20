using Shift.Constant;
using Shift.Contract;

namespace Shift.Service.Content.Searches.Dtos;

internal static class SearchEntryViewDtoMapper
{
    private static readonly Func<string?, string[]?> _defaultGetAttributes = (attributes) =>
    {
        return !string.IsNullOrEmpty(attributes)
            ? attributes.Split('\t')
            : null;
    };

    private record MapItem(
        Func<SearchEntryDto, string> GetUrl,
        Func<string?, string[]?>? GetAttributes = null
    );

    private static readonly Dictionary<SearchObjectType, MapItem> _mapItems = new()
    {
        {
            SearchObjectType.Person,
            new MapItem(
                dto => $"/ui/admin/contacts/people/edit?contact={dto.ObjectIdentifier}"
            )
        },
        {
            SearchObjectType.Group,
            new MapItem(
                dto => $"/ui/admin/contacts/groups/edit?contact={dto.ObjectIdentifier}"
            )
        },
        {
            SearchObjectType.Bank,
            new MapItem(
                dto => $"/ui/admin/assessments/banks/outline?bank={dto.ObjectIdentifier}"
            )
        },
        {
            SearchObjectType.Course,
            new MapItem(
                dto => $"/ui/admin/courses/manage?course={dto.ObjectIdentifier}"
            )
        },
        {
            SearchObjectType.Event,
            new MapItem(
                dto => 
                {
                    var eventType = !string.IsNullOrEmpty(dto.Attributes)
                        ? dto.Attributes.Split('\t')[0]
                        : throw new ArgumentNullException($"Event {dto.ObjectIdentifier} Attributes is empty");

                    return string.Equals(eventType, EventType.Class.ToString())
                        ? $"/ui/admin/events/classes/outline?event={dto.ObjectIdentifier}"
                        : string.Equals(eventType, EventType.Exam.ToString())
                            ? $"/ui/admin/events/exams/outline?event={dto.ObjectIdentifier}"
                            : string.Equals(eventType, EventType.Appointment.ToString())
                                ? $"/ui/admin/events/appointments/outline?event={dto.ObjectIdentifier}"
                                : throw new NotImplementedException($"Event {dto.ObjectIdentifier} has non-supported EventType: {eventType}");
                }
            )
        },
        {
            SearchObjectType.Form,
            new MapItem(
                dto => $"/ui/admin/workflow/forms/outline?form={dto.ObjectIdentifier}"
            )
        },
        {
            SearchObjectType.Achievement,
            new MapItem(
                dto => $"/ui/admin/records/achievements/outline?id={dto.ObjectIdentifier}"
            )
        },
        {
            SearchObjectType.Gradebook,
            new MapItem(
                dto => $"/ui/admin/records/gradebooks/outline?id={dto.ObjectIdentifier}"
            )
        },
        {
            SearchObjectType.Logbook,
            new MapItem(
                dto => $"/ui/admin/records/logbooks/outline?journalsetup={dto.ObjectIdentifier}"
            )
        },
        {
            SearchObjectType.Program,
            new MapItem(
                dto => $"/ui/admin/learning/programs/outline?id={dto.ObjectIdentifier}"
            )
        },
        {
            SearchObjectType.Standard,
            new MapItem(
                dto => $"/ui/admin/standards/edit?id={dto.ObjectIdentifier}"
            )
        },
        {
            SearchObjectType.PlatformApp,
            new MapItem(
                dto =>
                {
                    var attributes = _defaultGetAttributes(dto.Attributes);

                    return attributes != null && attributes.Length > 0
                        ? "/" + attributes[0]
                        : throw new ArgumentNullException($"Platform App {dto.ObjectIdentifier} Attributes is empty");
                },

                attributesText => _defaultGetAttributes(attributesText)![1..]
            )
        },
        {
            SearchObjectType.App,
            new MapItem(
                dto =>
                {
                    var attributes = _defaultGetAttributes(dto.Attributes);

                    return attributes != null && attributes.Length > 0
                        ? "/" + attributes[0]
                        : throw new ArgumentNullException($"App {dto.ObjectIdentifier} Attributes is empty");
                },
                
                attributesText => _defaultGetAttributes(attributesText)![1..]
            )
        },
    };

    public static SearchEntryViewDto Map(SearchEntryDto input)
    {
        if (!Enum.TryParse<SearchObjectType>(input.ObjectType, out var objectType))
            throw new NotImplementedException($"Unknown object type: {input.ObjectType}");

        if (!_mapItems.TryGetValue(objectType, out var mapItem))
            throw new NotImplementedException($"No map item found for object type: {objectType}");

        var url = mapItem.GetUrl(input);
        
        var attributes = (mapItem.GetAttributes ?? _defaultGetAttributes)(input.Attributes);

        return new SearchEntryViewDto
        (
            objectType,
            input.ObjectIdentifier,
            input.Title,
            url,
            attributes
        );
    }
}
