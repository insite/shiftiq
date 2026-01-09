using System.Linq;

namespace InSite.Persistence.Plugin.CMDS
{
    public static class SiteSearch
    {
        public static Partner[] GetPartners()
        {
            const string query = @"
SELECT CompanyName         AS Name
     , OrganizationLogoUrl AS Logo
FROM accounts.QOrganization
WHERE AccountClosed IS NULL
      AND OrganizationLogoUrl IS NOT NULL
      AND OrganizationCode NOT IN ( 'abc', 'acme', 'archive', 'cnrl', 'ogiu' )
      AND AccessGrantedToCmds = 1
ORDER BY CompanyName
";

            using (var context = new InternalDbContext())
                return context.Database.SqlQuery<Partner>(query).ToArray();
        }
    }
}
