namespace Shift.Service.Chatbot.Tools;

public static class ToolUtils
{
    public static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}