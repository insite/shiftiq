using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Mvc;

using Shift.Service.Chatbot;

namespace Shift.Api.Chatbot;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class ChatbotController(IPrincipalProvider principalProvider, IChatbotService chatbotService) : ShiftControllerBase
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public record RetrieveSystemPromptResult(string SystemPrompt);

    public record ModifySystemPromptInput(string? SystemPrompt);

    public record ModifySystemPromptResult(string SystemPrompt);

    public record ModifyModelInput(string Model);

    public record CreateChatbotSessionResult(Guid SessionId, string? Model, string[]? AvailableModels);

    public record StreamChatbotSessionInput(string? UserInput = null);

    [HttpGet("api/chatbot/system-prompt")]
    [HybridPermission("chatbot/admin", DataAccess.Read)]
    public ActionResult<RetrieveSystemPromptResult> RetrieveSystemPrompt()
    {
        return new RetrieveSystemPromptResult(chatbotService.SystemPrompt);
    }

    [HttpPut("api/chatbot/system-prompt")]
    [HybridPermission("chatbot/admin", DataAccess.Update)]
    public ActionResult<ModifySystemPromptResult> ModifySystemPrompt(ModifySystemPromptInput input)
    {
        if (string.IsNullOrEmpty(input.SystemPrompt))
            chatbotService.SetDefaultSystemPrompt();
        else
            chatbotService.ModifySystemPrompt(input.SystemPrompt);

        return new ModifySystemPromptResult(chatbotService.SystemPrompt);
    }

    [HttpPut("api/chatbot/model")]
    [HybridPermission("chatbot/admin", DataAccess.Update)]
    public void ModifyModel(ModifyModelInput input)
    {
        chatbotService.ModifyModel(input.Model);
    }

    [HttpPost("api/chatbot")]
    [HybridPermission("chatbot", DataAccess.Update)]
    public ActionResult<CreateChatbotSessionResult> CreateChatbotSession()
    {
        var principal = principalProvider.GetPrincipal();
        var session = chatbotService.GetSession(principal);

        session.Reset();

        var model = principal.IsOperator ? chatbotService.Model : null;
        var availableModels = principal.IsOperator ? chatbotService.GetAvailableModels() : null;

        return new CreateChatbotSessionResult(session.SessionId, model, availableModels);
    }

    [HttpPost("api/chatbot/stream")]
    [HybridPermission("chatbot", DataAccess.Update)]
    public async Task StreamChatbotSessionAsync(StreamChatbotSessionInput input, CancellationToken ct)
    {
        var principal = principalProvider.GetPrincipal();
        var session = chatbotService.GetSession(principal);

        Response.ContentType = "application/x-ndjson";

        if (string.Equals(input.UserInput, "/new"))
        {
            session.Reset();
            await StreamJsonAsync(new DirectTextEvent("Started a new session."), ct);
            return;
        }

        await foreach (var ev in session.SendStreamingAsync(principal, input.UserInput ?? "", ct))
        {
            await StreamJsonAsync(ev, ct);
        }
    }

    private async Task StreamJsonAsync(ChatStreamEvent ev, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize<object>(ev, SerializerOptions);

        await Response.WriteAsync(json + "\n", ct);
        await Response.Body.FlushAsync(ct);        
    }
}