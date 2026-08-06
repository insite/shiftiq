using System.ComponentModel;

using Shift.Contract;
using Shift.Service.Chatbot.Tools;

namespace Shift.Service.Chatbot.ShiftTools.People;

internal class SearchPeopleArgs
{
    [Description("A full or partial person's email.")]
    public string? EmailLike { get; set; }

    [Description("An exact person's email.")]
    public string? EmailExact { get; set; }

    [Description("A full or partial person's full name.")]
    public string? FullName { get; set; }

    [Description("Person's attendee role in the exam/class")]
    public string? EventRole { get; set; }

    [Description("Is user's account approved?")]
    public bool? IsApproved { get; set; }

    public static IPersonCriteria ToCriteria(ToolContext toolContext, SearchPeopleArgs? args, int? limit = null)
    {
        var criteria = new SearchPeople
        {
            OrganizationId = toolContext.Principal.OrganizationId,
            EmailLike = ToolUtils.Normalize(args?.EmailLike),
            EmailExact = ToolUtils.Normalize(args?.EmailExact),
            FullName = ToolUtils.Normalize(args?.FullName),
            EventRole = ToolUtils.Normalize(args?.EventRole),
            IsApproved = args?.IsApproved
        };

        if (limit == null)
            criteria.DisablePaging();
        else
            criteria.Filter.PageSize = limit.Value;

        return criteria;
    }
}