using System.Linq;

using Shift.Common.Timeline.Changes;

using InSite.Domain.Registrations;

namespace InSite.Persistence.Plugin.RCABC
{
    public class RcabcRegistrationChangeProjector
    {
        public RcabcRegistrationChangeProjector(IChangeQueue publisher)
        {
            var rcabc = Shift.Constant.OrganizationIdentifiers.RCABC;

            publisher.Extend<RegistrationCancelled>(Handle, rcabc);
        }

        public void Handle(RegistrationCancelled e)
        {
            using (var db = new InternalDbContext())
            {
                var registration = db.Registrations
                    .Where(x => x.RegistrationIdentifier == e.AggregateIdentifier)
                    .FirstOrDefault();

                /*
                if (string.Equals(registration.ApprovalStatus, "Registered", StringComparison.OrdinalIgnoreCase))
                    registration.ApprovalStatus = null;

                registration.AttendanceStatus = "Withdrawn/Cancelled";
                */

                registration.ApprovalStatus = "Moved";
                registration.IncludeInT2202 = false;

                db.SaveChanges();
            }
        }
    }
}
