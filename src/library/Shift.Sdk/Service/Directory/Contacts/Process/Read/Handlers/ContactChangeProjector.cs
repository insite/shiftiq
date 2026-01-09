using Shift.Common.Timeline.Changes;

using InSite.Domain.Contacts;

namespace InSite.Application.Contacts.Read
{
    public class ContactChangeProjector
    {
        public ContactChangeProjector(IChangeQueue queue)
        {
            queue.Subscribe<PersonEmailChanged>(Handle);
            queue.Subscribe<PersonRenamed>(Handle);
        }

        public void Handle(PersonEmailChanged e)
        {
            
        }

        public void Handle(PersonRenamed e)
        {
            
        }
    }
}
