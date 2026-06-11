using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Cases;

public class TCaseStatusConfiguration : IEntityTypeConfiguration<TCaseStatusEntity>
{
    public void Configure(EntityTypeBuilder<TCaseStatusEntity> builder)
    {
        builder.ToTable("TCaseStatus", "workflow");
        builder.HasKey(x => new { x.StatusIdentifier });

        builder.Property(x => x.CaseType).IsRequired().IsUnicode(false).HasMaxLength(50);
        builder.Property(x => x.StatusCategory).IsRequired().IsUnicode(false).HasMaxLength(120);
        builder.Property(x => x.ReportCategory).IsUnicode(false).HasMaxLength(10);
        builder.Property(x => x.StatusIdentifier).IsRequired();
        builder.Property(x => x.StatusName).IsRequired().IsUnicode(false).HasMaxLength(50);
        builder.Property(x => x.StatusDescription).IsUnicode(false).HasMaxLength(200);
    }
}
