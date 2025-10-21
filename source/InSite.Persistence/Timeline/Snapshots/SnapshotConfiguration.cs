﻿using System.Data.Entity.ModelConfiguration;

using Common.Timeline.Snapshots;

namespace InSite.Persistence
{
    public class SnapshotConfiguration : EntityTypeConfiguration<Snapshot>
    {
        public SnapshotConfiguration() : this("logs") { }

        public SnapshotConfiguration(string schema)
        {
            ToTable(schema + ".Snapshot");
            HasKey(x => new { x.AggregateIdentifier });

            Property(x => x.AggregateIdentifier).IsRequired();
            Property(x => x.AggregateState).IsRequired().IsUnicode(true);
            Property(x => x.AggregateVersion).IsRequired();
        }
    }
}
