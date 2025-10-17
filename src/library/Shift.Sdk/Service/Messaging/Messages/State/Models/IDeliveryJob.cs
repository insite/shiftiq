using Shift.Common;

namespace InSite.Domain.Messages
{
    public interface IDeliveryJob
    {
        string Language { get; }
        EmailDraft Email { get; }
    }
}
