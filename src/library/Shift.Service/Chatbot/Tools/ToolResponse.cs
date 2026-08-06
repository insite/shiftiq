namespace Shift.Service.Chatbot.Tools;

public record ToolResponse(string ToLLM, string? DirectText, string? Table);