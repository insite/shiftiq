namespace Shift.Service.Chatbot.Tools;

public class ToolExecutor
{
    private readonly Dictionary<string, ITool> _tools = [];

    public ToolExecutor(IEnumerable<ITool> tools)
    {
        foreach (var tool in tools)
        {
            if (tool.Tools.Length == 0)
                _tools.Add(tool.Name, tool);
            else
            {
                foreach (var subtool in tool.Tools)
                    _tools.Add(subtool.Name, subtool);
            }
        }
    }

    public async Task<ToolResponse> ExecuteAsync(ToolContext toolContext, string toolName, BinaryData args)
    {
        if (!_tools.TryGetValue(toolName, out var tool))
            throw new InvalidOperationException($"Unknown tool: {toolName}");

        return await tool.ExecuteAsync(toolContext, args);
    }
}