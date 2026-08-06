#pragma warning disable OPENAI001

using System.Runtime.CompilerServices;

using OpenAI.Responses;

using Shift.Common;
using Shift.Service.Chatbot.Tools;

namespace Shift.Service.Chatbot.OpenAI;

internal class OpenAiChatbotSession: IChatbotSession
{
    private class PromptStats
    {
        public int InputTokenCount { get; set; }
        public int OutputTokenCount { get; set; }
        public List<string> Tools { get; } = [];
    }

    private const int MaxToolIterations = 5;

    private string _model;
    private string _systemPrompt;
    
    private readonly PermissionCache _permissionCache;
    private readonly ResponsesClient _client;
    private readonly ToolExecutor _toolExecutor;
    private readonly ResponseTool[] _tools;
    private readonly List<ResponseItem> _inputItems = [];

    public Guid SessionId { get; }
    public Guid OrganizationId { get; }
    public Guid UserId { get; }

    public OpenAiChatbotSession(
        Guid sessionId,
        Guid organizationId,
        Guid userId,
        PermissionCache permissionCache,
        string model,
        string systemPrompt,
        ResponsesClient client,
        ToolExecutor toolExecutor,
        ResponseTool[] tools
    )
    {
        SessionId = sessionId;
        OrganizationId = organizationId;
        UserId = userId;

        _model = model;
        _systemPrompt = systemPrompt;
        
        _permissionCache = permissionCache;
        _client = client;
        _toolExecutor = toolExecutor;
        _tools = tools;

        Reset();
    }

    public async IAsyncEnumerable<ChatStreamEvent> SendStreamingAsync(IPrincipal principal, string userInput, [EnumeratorCancellation] CancellationToken ct)
    {
        var stats = new PromptStats();

        var toolContext = new ToolContext(_permissionCache, principal);

        _inputItems.Add(ResponseItem.CreateUserMessageItem(userInput));

        for (var i = 0; i < MaxToolIterations; i++)
        {
            var hasToolResponse = false;
            var options = CreateOptions();
            var updates = _client.CreateResponseStreamingAsync(options, ct);

            await foreach (var update in updates)
            {
                if (update is StreamingResponseOutputTextDeltaUpdate textUpdate)
                    yield return new DeltaEvent(textUpdate.Delta);

                if (update is StreamingResponseOutputItemDoneUpdate itemDone)
                {
                    _inputItems.Add(itemDone.Item);

                    if (itemDone.Item is FunctionCallResponseItem toolCall)
                    {
                        var toolResponse = await _toolExecutor.ExecuteAsync(toolContext, toolCall.FunctionName, toolCall.FunctionArguments);

                        if (!string.IsNullOrEmpty(toolResponse.DirectText))
                            yield return new DirectTextEvent(toolResponse.DirectText);

                        if (!string.IsNullOrEmpty(toolResponse.Table))
                            yield return new TableEvent(toolResponse.Table);

                        _inputItems.Add(new FunctionCallOutputResponseItem(toolCall.CallId, toolResponse.ToLLM));

                        stats.Tools.Add(toolCall.FunctionName);

                        hasToolResponse = true;
                    }
                }

                if (update is StreamingResponseCompletedUpdate completed)
                    AddTokenUsage(stats, completed.Response);
            }

            if (!hasToolResponse)
            {
                if (principal.IsOperator)
                    yield return new SummaryEvent(_model, stats.InputTokenCount, stats.OutputTokenCount, [..stats.Tools]);

                yield break;
            }
        }

        throw new InvalidOperationException("The assistant requested too many tool calls in a single turn.");
    }

    public void Reset()
    {
        _inputItems.Clear();
        _inputItems.Add(ResponseItem.CreateSystemMessageItem(_systemPrompt));
    }

    public void ModifySettings(string model, string systemPrompt)
    {
        _model = model;
        _systemPrompt = systemPrompt;
    }

    private static void AddTokenUsage(PromptStats stats, ResponseResult responseResult)
    {
        var usage = responseResult.Usage;

        stats.InputTokenCount += usage.InputTokenCount;
        stats.OutputTokenCount += usage.OutputTokenCount;
    }

    private CreateResponseOptions CreateOptions()
    {
        var options = new CreateResponseOptions(_model, _inputItems)
        {
            Temperature = 0.1f,
            StreamingEnabled = true
        };

        foreach (var tool in _tools)
            options.Tools.Add(tool);

        return options;
    }
}