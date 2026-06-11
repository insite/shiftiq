using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.CMDS
{
    public class ZUserStatusConfiguration : EntityTypeConfiguration<ZUserStatus>
    {
        public ZUserStatusConfiguration() : this("custom_cmds") { }

        public ZUserStatusConfiguration(string schema)
        {
            ToTable(schema + ".ZUserStatus");
            HasKey(x => new { x.SnapshotKey });

            Property(x => x.CountRQ_Requirement).IsOptional();
            Property(x => x.CountVA_Requirement).IsOptional();
            Property(x => x.Score_Requirement).IsOptional().HasPrecision(7, 4);
            Property(x => x.CountRQ_Code).IsOptional();
            Property(x => x.CountVA_Code).IsOptional();
            Property(x => x.Score_Code).IsOptional().HasPrecision(7, 4);
            Property(x => x.CountEX_Competency).IsOptional();
            Property(x => x.CountNT_Competency).IsOptional();
            Property(x => x.CountNA_Competency).IsOptional();
            Property(x => x.CountNC_Competency).IsOptional();
            Property(x => x.CountRQ_Competency).IsOptional();
            Property(x => x.CountSA_Competency).IsOptional();
            Property(x => x.CountSV_Competency).IsOptional();
            Property(x => x.CountVA_Competency).IsOptional();
            Property(x => x.ScoreEX_Competency).IsOptional().HasPrecision(7, 4);
            Property(x => x.ScoreNT_Competency).IsOptional().HasPrecision(7, 4);
            Property(x => x.ScoreNA_Competency).IsOptional().HasPrecision(7, 4);
            Property(x => x.ScoreNC_Competency).IsOptional().HasPrecision(7, 4);
            Property(x => x.ScoreRQ_Competency).IsOptional().HasPrecision(7, 4);
            Property(x => x.ScoreSA_Competency).IsOptional().HasPrecision(7, 4);
            Property(x => x.ScoreSV_Competency).IsOptional().HasPrecision(7, 4);
            Property(x => x.ScoreVA_Competency).IsOptional().HasPrecision(7, 4);
            Property(x => x.Score_Competency).IsOptional().HasPrecision(7, 4);
            Property(x => x.CountEX_CompetencyCritical).IsOptional();
            Property(x => x.CountNT_CompetencyCritical).IsOptional();
            Property(x => x.CountNA_CompetencyCritical).IsOptional();
            Property(x => x.CountNC_CompetencyCritical).IsOptional();
            Property(x => x.CountRQ_CompetencyCritical).IsOptional();
            Property(x => x.CountSA_CompetencyCritical).IsOptional();
            Property(x => x.CountSV_CompetencyCritical).IsOptional();
            Property(x => x.CountVA_CompetencyCritical).IsOptional();
            Property(x => x.ScoreEX_CompetencyCritical).IsOptional().HasPrecision(7, 4);
            Property(x => x.ScoreNT_CompetencyCritical).IsOptional().HasPrecision(7, 4);
            Property(x => x.ScoreNA_CompetencyCritical).IsOptional().HasPrecision(7, 4);
            Property(x => x.ScoreNC_CompetencyCritical).IsOptional().HasPrecision(7, 4);
            Property(x => x.ScoreRQ_CompetencyCritical).IsOptional().HasPrecision(7, 4);
            Property(x => x.ScoreSA_CompetencyCritical).IsOptional().HasPrecision(7, 4);
            Property(x => x.ScoreSV_CompetencyCritical).IsOptional().HasPrecision(7, 4);
            Property(x => x.ScoreVA_CompetencyCritical).IsOptional().HasPrecision(7, 4);
            Property(x => x.Score_CompetencyCritical).IsOptional().HasPrecision(7, 4);
            Property(x => x.CountEX_CompetencyCriticalMandatory).IsOptional();
            Property(x => x.CountNT_CompetencyCriticalMandatory).IsOptional();
            Property(x => x.CountNA_CompetencyCriticalMandatory).IsOptional();
            Property(x => x.CountNC_CompetencyCriticalMandatory).IsOptional();
            Property(x => x.CountRQ_CompetencyCriticalMandatory).IsOptional();
            Property(x => x.CountSA_CompetencyCriticalMandatory).IsOptional();
            Property(x => x.CountSV_CompetencyCriticalMandatory).IsOptional();
            Property(x => x.CountVA_CompetencyCriticalMandatory).IsOptional();
            Property(x => x.ScoreEX_CompetencyCriticalMandatory).IsOptional().HasPrecision(7, 4);
            Property(x => x.ScoreNT_CompetencyCriticalMandatory).IsOptional().HasPrecision(7, 4);
            Property(x => x.ScoreNA_CompetencyCriticalMandatory).IsOptional().HasPrecision(7, 4);
            Property(x => x.ScoreNC_CompetencyCriticalMandatory).IsOptional().HasPrecision(7, 4);
            Property(x => x.ScoreSA_CompetencyCriticalMandatory).IsOptional().HasPrecision(7, 4);
            Property(x => x.ScoreSV_CompetencyCriticalMandatory).IsOptional().HasPrecision(7, 4);
            Property(x => x.ScoreVA_CompetencyCriticalMandatory).IsOptional().HasPrecision(7, 4);
            Property(x => x.Score_CompetencyCriticalMandatory).IsOptional().HasPrecision(7, 4);
            Property(x => x.Score_Document).IsOptional().HasPrecision(7, 4);
            Property(x => x.Score_Module).IsOptional().HasPrecision(7, 4);
            Property(x => x.ScoreEX_CompetencyNoncritical).IsOptional().HasPrecision(7, 4);
            Property(x => x.ScoreNT_CompetencyNoncritical).IsOptional().HasPrecision(7, 4);
            Property(x => x.ScoreNA_CompetencyNoncritical).IsOptional().HasPrecision(7, 4);
            Property(x => x.ScoreNC_CompetencyNoncritical).IsOptional().HasPrecision(7, 4);
            Property(x => x.ScoreRQ_CompetencyNoncritical).IsOptional().HasPrecision(7, 4);
            Property(x => x.ScoreSA_CompetencyNoncritical).IsOptional().HasPrecision(7, 4);
            Property(x => x.ScoreSV_CompetencyNoncritical).IsOptional().HasPrecision(7, 4);
            Property(x => x.ScoreVA_CompetencyNoncritical).IsOptional().HasPrecision(7, 4);
            Property(x => x.Score_CompetencyNoncritical).IsOptional().HasPrecision(7, 4);
            Property(x => x.ScoreEX_CompetencyNoncriticalMandatory).IsOptional().HasPrecision(7, 4);
            Property(x => x.ScoreNT_CompetencyNoncriticalMandatory).IsOptional().HasPrecision(7, 4);
            Property(x => x.ScoreNA_CompetencyNoncriticalMandatory).IsOptional().HasPrecision(7, 4);
            Property(x => x.ScoreNC_CompetencyNoncriticalMandatory).IsOptional().HasPrecision(7, 4);
            Property(x => x.ScoreSA_CompetencyNoncriticalMandatory).IsOptional().HasPrecision(7, 4);
            Property(x => x.ScoreSV_CompetencyNoncriticalMandatory).IsOptional().HasPrecision(7, 4);
            Property(x => x.ScoreVA_CompetencyNoncriticalMandatory).IsOptional().HasPrecision(7, 4);
            Property(x => x.Score_CompetencyNoncriticalMandatory).IsOptional().HasPrecision(7, 4);
            Property(x => x.Score_Practice).IsOptional().HasPrecision(7, 4);
            Property(x => x.Score_Procedure).IsOptional().HasPrecision(7, 4);
            Property(x => x.Score_Certificate).IsOptional().HasPrecision(7, 4);
            Property(x => x.Score_Guide).IsOptional().HasPrecision(7, 4);

            Property(x => x.DepartmentIdentifier).IsRequired();
            Property(x => x.PrimaryProfileIdentifier).IsOptional();
            Property(x => x.AsAt).IsRequired();
            Property(x => x.OrganizationIdentifier).IsOptional();
            Property(x => x.SnapshotKey).IsRequired();
            Property(x => x.UserIdentifier).IsRequired();
        }
    }
}