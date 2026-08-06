using Shift.Common;

namespace Shift.Service.Chatbot;

public interface IChatbotSession
{
    Guid SessionId { get; }
    Guid OrganizationId { get; }
    Guid UserId { get; }

    IAsyncEnumerable<ChatStreamEvent> SendStreamingAsync(IPrincipal principal, string userInput, CancellationToken ct);
    void Reset();
}