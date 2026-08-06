using Shift.Common;

namespace Shift.Service.Chatbot;

public interface IChatbotService
{
    string Model { get; }
    string SystemPrompt { get; }

    IChatbotSession GetSession(IPrincipal principal);

    string[] GetAvailableModels();
    void ModifyModel(string model);
    void ModifySystemPrompt(string prompt);
    void SetDefaultSystemPrompt();
}