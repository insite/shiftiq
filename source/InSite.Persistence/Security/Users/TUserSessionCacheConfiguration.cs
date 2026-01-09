using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class TUserSessionCacheConfiguration : EntityTypeConfiguration<TUserSessionCache>
    {
        public TUserSessionCacheConfiguration() : this("accounts")
        {
        }

        public TUserSessionCacheConfiguration(string schema)
        {
            ToTable(schema + ".TUserSessionCache");
            HasKey(x => x.CacheIdentifier);
        }
    }
}
