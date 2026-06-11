using System;
using System.Linq;

using InSite.Persistence.Plugin.CMDS;

namespace InSite.Persistence
{
    public static class TCollegeCertificateSearch
    {
        public static TCollegeCertificate Select(Guid learner, Guid profile)
        {
            using (var db = new InternalDbContext())
            {
                return db.TCollegeCertificates.FirstOrDefault(x => x.LearnerIdentifier == learner && x.ProfileIdentifier == profile);
            }
        }
    }
}
