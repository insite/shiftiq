using System.Linq;

namespace InSite.Persistence
{
    public static class TUserAuthenticationFactorSearch
    {

        public static TUserAuthenticationFactor GetMFARecordById(TUserAuthenticationFactorFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                if(filter.MFAIdentifier != null)
                {
                    return db.TUserAuthenticationFactors
                        .FirstOrDefault(x => x.UserAuthenticationFactorIdentifier == filter.MFAIdentifier);
                }
                if (filter.UserIdentifier != null)
                {
                    return db.TUserAuthenticationFactors
                        .FirstOrDefault(x => x.UserIdentifier == filter.UserIdentifier);
                }
                return default;
            }
               
        }

    }
}
