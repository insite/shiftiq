using InSite.Application.Files.Read;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Content;

public class OrphanFileConfiguration : IEntityTypeConfiguration<OrphanFile>
{
    public void Configure(EntityTypeBuilder<OrphanFile> builder)
    {
        builder.ToTable("VOrphanFile", "assets");
        builder.HasKey(x => new { x.FileIdentifier });
    }
}