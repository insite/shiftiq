using System.Data.Entity;

using InSite.Application.Invoices.Read;

namespace InSite.Persistence
{
    internal class ReportDbContext : DbContext
    {
        // databases
        public DbSet<TEntity> TEntities { get; set; }
        public DbSet<VForeignKey> VForeignKeys { get; set; }
        public DbSet<VForeignKeyConstraint> VForeignKeyConstraints { get; set; }
        public DbSet<VPrimaryKey> VPrimaryKeys { get; set; }
        public DbSet<VSchema> VSchemas { get; set; }
        public DbSet<VTable> VTables { get; set; }
        public DbSet<VTableColumn> VTableColumns { get; set; }
        public DbSet<VView> VViews { get; set; }
        public DbSet<VViewColumn> VViewColumns { get; set; }

        // sales
        public DbSet<VEventRegistrationPayment> VEventRegistrationPayments { get; set; }
        
        // sites
        public DbSet<VSitemap> VSitemaps { get; set; }

        protected override void OnModelCreating(DbModelBuilder builder)
        {
            base.OnModelCreating(builder);
            AddConfigurations(builder);
        }

        public static void AddConfigurations(DbModelBuilder builder)
        {
            // databases
            builder.Configurations.Add(new TEntityConfiguration());
            builder.Configurations.Add(new VForeignKeyConfiguration());
            builder.Configurations.Add(new VForeignKeyConstraintConfiguration());
            builder.Configurations.Add(new VPrimaryKeyConfiguration());
            builder.Configurations.Add(new VSchemaConfiguration());
            builder.Configurations.Add(new VTableColumnConfiguration());
            builder.Configurations.Add(new VTableConfiguration());
            builder.Configurations.Add(new VViewColumnConfiguration());
            builder.Configurations.Add(new VViewConfiguration());

            // sales
            builder.Configurations.Add(new VEventRegistrationPaymentConfiguration());

            // sites
            builder.Configurations.Add(new VSitemapConfiguration());
        }

        static ReportDbContext()
        {
            Database.SetInitializer<ReportDbContext>(null);
        }

        public ReportDbContext(bool proxy = false, bool lazy = true)
            : base(DbSettings.ConnectionString)
        {
            Configuration.ProxyCreationEnabled = proxy;
            Configuration.LazyLoadingEnabled = lazy;
        }
    }
}