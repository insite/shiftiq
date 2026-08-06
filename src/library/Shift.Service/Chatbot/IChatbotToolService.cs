using Shift.Common;
using Shift.Service.Chatbot.Tools;

namespace Shift.Service.Chatbot;

public interface IChatbotToolService
{
    IEnumerable<ITool> GetTools(IPrincipal principal);
}