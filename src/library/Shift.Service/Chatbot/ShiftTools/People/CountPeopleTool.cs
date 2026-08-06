using Shift.Constant;
using Shift.Service.Chatbot.Tools;
using Shift.Service.Directory;

namespace Shift.Service.Chatbot.ShiftTools.People;

record CountPeopleToolResult(int Count);

internal class CountPeopleTool(PersonReader reader) :
    BaseTool<SearchPeopleArgs, CountPeopleToolResult>(
        "count_people",
        "Returns the number of matching people.",
        PermissionNames.Admin_Contacts
    )
{
    protected override async Task<InternalToolResponse> InternalExecuteAsync(ToolContext toolContext, SearchPeopleArgs? args)
    {
        var count = await reader.CountAsync(SearchPeopleArgs.ToCriteria(toolContext, args));
        return new InternalToolResponse(new CountPeopleToolResult(count));
    }
}