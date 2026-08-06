#pragma warning disable OPENAI001

using System.ClientModel.Primitives;

using OpenAI.Responses;

using Shift.Service.Chatbot.Tools;

namespace Shift.Service.Chatbot.OpenAI;

internal static class OpenAiTools
{
    public static (ToolExecutor toolExecutor, ResponseTool[] tools) Create(IEnumerable<ITool> tools, bool allowToolkits)
    {
        var responseTools = CreateTools(tools, allowToolkits);
        var toolExecutor = new ToolExecutor(tools);

        return (toolExecutor, responseTools);
    }

    private static ResponseTool[] CreateTools(IEnumerable<ITool> tools, bool allowToolkits)
    {
        var result = allowToolkits
            ? [..tools.Select(CreateTool)]
            : CreateToolsWithoutToolkits(tools);

        if (allowToolkits && tools.Any(x => x.Tools.Length > 0))
        {
            var toolSearchTool = ModelReaderWriter.Read<ResponseTool>(BinaryData.FromObjectAsJson(new
            {
                type = "tool_search"
            }))!;

            result.Add(toolSearchTool);
        }

        return [..result];
    }

    private static List<ResponseTool> CreateToolsWithoutToolkits(IEnumerable<ITool> tools)
    {
        var result = new List<ResponseTool>();

        foreach (var tool in tools)
        {
            if (tool.Tools.Length == 0)
                result.Add(ModelReaderWriter.Read<ResponseTool>(BinaryData.FromObjectAsJson(CreateFunctionTool(tool, false)))!);
            else
            {
                result.AddRange(tool.Tools.Select(x =>
                    ModelReaderWriter.Read<ResponseTool>(BinaryData.FromObjectAsJson(CreateFunctionTool(x, false)))!
                ));
            }
        }

        return result;
    }

    private static ResponseTool CreateTool(ITool tool)
    {
        var responseTool = tool.Tools.Length == 0
            ? CreateFunctionTool(tool, false)
            : CreateNamespaceTool(tool);

        return ModelReaderWriter.Read<ResponseTool>(BinaryData.FromObjectAsJson(responseTool))!;
    }

    private static object CreateNamespaceTool(ITool tool)
    {
        var childTools = tool.Tools.Select(x => CreateFunctionTool(x, true)).ToArray();

        return new
        {
            type = "namespace",
            name = tool.Name,
            description = tool.Description,
            tools = childTools
        };
    }

    private static object CreateFunctionTool(ITool tool, bool deferLoading)
    {
        var parameters = tool.CreateArgsSchema();

        parameters["additionalProperties"] = false;

        return new
        {
            type = "function",
            name = tool.Name,
            description = tool.Description,
            defer_loading = deferLoading,
            parameters,
        };
    }
}