using System.ComponentModel;

using Shift.Constant;
using Shift.Service.Chatbot.Tools;
using Shift.Service.Directory;

namespace Shift.Service.Chatbot.ShiftTools.People;

internal class ExportPeopleCsvToolArgs : SearchPeopleArgs
{
    [Description("Maximum number of rows to export. The hard limit is 5000.")]
    public int? Limit { get; set; }
}

internal class ExportPeopleCsvTool(PersonReader reader, CsvStore csvStore) :
    BaseTool<ExportPeopleCsvToolArgs, ExportCsvResult>(
        "export_people_csv",
        "Searches people and saves matching rows to a CSV file.",
        PermissionNames.Admin_Contacts
    )
{
    protected override async Task<InternalToolResponse> InternalExecuteAsync(ToolContext toolContext, ExportPeopleCsvToolArgs? args)
    {
        var limit = args?.Limit != null ? Math.Clamp(args.Limit.Value, 0, 5000) : 5000;

        var people = limit == 0
            ? []
            : await reader.SearchAsync(SearchPeopleArgs.ToCriteria(toolContext, args, limit));

        var dataForCsv = people.Select(x => new
        {
            UserId = x.UserId,
            PersonId = x.PersonId,
            PersonCode = x.PersonCode,
            FirstName = x.PersonFirstName,
            MiddleName = x.PersonMiddleName,
            LastName = x.PersonLastName,
            FullName = x.UserName,
            Email = x.UserEmail,
            TimeZone = x.TimeZone,
        }).ToList();

        return new InternalToolResponse(await csvStore.ExportAsync(toolContext, "people-search", dataForCsv));
    }
}