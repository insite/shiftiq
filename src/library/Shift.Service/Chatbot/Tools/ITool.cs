using System.Text.Json.Nodes;

namespace Shift.Service.Chatbot.Tools;

public interface ITool
{
    string Name { get; }
    string Description { get; }
    ITool[] Tools { get; }

    bool IsAllowed(ToolContext toolContext);
    JsonNode CreateArgsSchema();
    Task<ToolResponse> ExecuteAsync(ToolContext toolContext, BinaryData args);
}