using InSite.Application.Files.Read;

using Shift.Common;
using Shift.Service.Chatbot.ShiftTools.Common;
using Shift.Service.Chatbot.ShiftTools.People;
using Shift.Service.Chatbot.Tools;
using Shift.Service.Directory;

namespace Shift.Service.Chatbot.ShiftTools;

public class ShiftChatbotToolService : IChatbotToolService
{
    private readonly PermissionCache _permissionCache;
    private readonly ITool[] _tools;

    public ShiftChatbotToolService(IStorageServiceAsync storageService, PermissionCache permissionCache, PersonReader personReader)
    {
        _permissionCache = permissionCache;

        var csvStore = new CsvStore(storageService);

        var peopleToolkit = new ToolkitTool("people", "Tools for people search", [
            new ExportPeopleCsvTool(personReader, csvStore),
            new CountPeopleTool(personReader)
        ]);

        _tools =
        [
            peopleToolkit,
            new DisplayCsvTool(csvStore)
        ];
    }

    public IEnumerable<ITool> GetTools(IPrincipal principal)
    {
        var toolContext = new ToolContext(_permissionCache, principal);
        var result = new List<ITool>();

        foreach (var tool in _tools)
        {
            if (tool.Tools.Length == 0)
            {
                if (tool.IsAllowed(toolContext))
                    result.Add(tool);

                continue;
            }

            var subtools = tool.Tools.Where(x => x.IsAllowed(toolContext)).ToArray();

            if (subtools.Length > 0)
                result.Add(new ToolkitTool(tool.Name, tool.Description, subtools));
        }

        return result;
    }
}