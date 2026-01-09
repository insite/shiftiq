using Shift.Common;

namespace InSite.Application.Messages.Read
{
    public interface IEmailStore
    {
        void Insert(EmailDraft email);
    }
}