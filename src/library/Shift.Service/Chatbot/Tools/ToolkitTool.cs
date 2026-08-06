using System.Text.Json.Nodes;

namespace Shift.Service.Chatbot.Tools;

public class ToolkitTool(string name, string description, ITool[] tools) : ITool
{
    public string Name => name;
    public string Description => description;
    public ITool[] Tools => tools;

    public bool IsAllowed(ToolContext toolContext)
    {
        throw new NotImplementedException();
    }

    public JsonNode CreateArgsSchema()
    {
        throw new NotImplementedException();
    }

    public Task<ToolResponse> ExecuteAsync(ToolContext context, BinaryData args)
    {
        throw new NotImplementedException();
    }
}