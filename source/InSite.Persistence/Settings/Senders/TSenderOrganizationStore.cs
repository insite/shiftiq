using System;
using System.Linq;

using Shift.Common;

namespace InSite.Persistence
{
    public class TSenderOrganizationStore
    {
        public static void Insert(Guid organization, Guid sender)
        {
            using (var db = new InternalDbContext())
            {
                if (db.TSenderOrganizations.Any(x => x.SenderIdentifier == sender && x.OrganizationIdentifier == organization))
                    return;

                var senderOrganization = new TSenderOrganization { SenderIdentifier = sender, OrganizationIdentifier = organization, JoinIdentifier = UniqueIdentifier.Create() };

                db.TSenderOrganizations.Add(senderOrganization);
                db.SaveChanges();
            }
        }

        public static void Delete(Guid organization, Guid sender)
        {
            using (var db = new InternalDbContext())
            {
                var senderOrganization = db.TSenderOrganizations.SingleOrDefault(x => x.SenderIdentifier == sender && x.OrganizationIdentifier == organization);
                if (senderOrganization != null)
                {
                    db.TSenderOrganizations.Remove(senderOrganization);
                    db.SaveChanges();
                }
            }
        }
    }
}
