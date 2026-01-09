using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace InSite.Persistence.Plugin.CMDS
{
    public static class ZUserStatusSearch
    {
        public static void GetMinimumAndMaximumDates(out DateTimeOffset? from, out DateTimeOffset? thru)
        {
            using (var db = new InternalDbContext())
            {
                from = db.ZUserStatuses.Min(x => x.AsAt);
                thru = db.ZUserStatuses.Max(x => x.AsAt);
            }
        }

        public static DataTable SelectByFilter(EmployeeComplianceSnapshotFilter filter)
        {
            const string query = @"
WITH Snapshots AS
(
  SELECT
      AsAt

     ,SUM(CountRQ_Certificate) AS TimeSensitiveSafetyCertificateCountRequired
     ,SUM(CountVA_Certificate) AS TimeSensitiveSafetyCertificateCountValidated
     ,AVG(Score_Certificate) AS TimeSensitiveSafetyCertificateScore

     ,SUM(CountRQ_Requirement) AS AdditionalComplianceRequirementCountRequired
     ,SUM(CountVA_Requirement) AS AdditionalComplianceRequirementCountValidated
     ,AVG(Score_Requirement) AS AdditionalComplianceRequirementScore

     ,SUM(CountRQ_CompetencyCritical) AS CriticalCompetencyCountRequired
     ,SUM(CountVA_CompetencyCritical) AS CriticalCompetencyCountValidated
     ,AVG(Score_CompetencyCritical) AS CriticalCompetencyScore

     ,SUM(CountRQ_CompetencyNoncritical) AS NonCriticalCompetencyCountRequired
     ,SUM(CountVA_CompetencyNoncritical) AS NonCriticalCompetencyCountValidated
     ,AVG(Score_CompetencyNoncritical) AS NonCriticalCompetencyScore

     ,SUM(CountRQ_Code) AS CodesOfPracticeCountRequired
     ,SUM(CountVA_Code) AS CodesOfPracticeCountValidated
     ,AVG(Score_Code) AS CodesOfPracticeScore

     ,SUM(CountRQ_Practice) AS SafeOperatingPracticeCountRequired
     ,SUM(CountVA_Practice) AS SafeOperatingPracticeCountValidated
     ,AVG(Score_Practice) AS SafeOperatingPracticeScore

     ,SUM(CountRQ_Competency) AS CompetencyCountRequired
     ,COUNT(DISTINCT UserIdentifier) AS EmployeeCount
     ,COUNT(DISTINCT OrganizationIdentifier) AS CompanyCount
    
     ,ROW_NUMBER() OVER(ORDER BY {2}) AS RowNumber
    FROM custom_cmds.ZUserStatus
    {0}
    GROUP BY 
      AsAt
)
SELECT * FROM Snapshots
  WHERE RowNumber BETWEEN @StartRow AND @EndRow
  ORDER BY {1}
";

            var sortExpression = "AsAt DESC";

            var withSortExpression = sortExpression
                .Replace("CompliancePercentage", "AVG(CompliancePercentage)")
                .Replace("CountRQ_Competency", "SUM(CountRQ_Competency)")
                .Replace("EmployeeCount", "COUNT(DISTINCT UserIdentifier)")
                .Replace("CompanyCount", "COUNT(DISTINCT OrganizationIdentifier)")
                ;

            var where = CreateWhereForFilter(filter);
            var curQuery = string.Format(query, where, sortExpression, withSortExpression);

            var (startRow, endRow) = filter?.Paging != null
                ? filter.Paging.ToStartEnd()
                : (0, 0);

            var parameters = GetParametersForFilter(filter);
            parameters.Add(new SqlParameter("StartRow", startRow));
            parameters.Add(new SqlParameter("EndRow", endRow));

            return DatabaseHelper.CreateDataTable(curQuery, parameters.ToArray());
        }

        public static int CountByFilter(EmployeeComplianceSnapshotFilter filter)
        {
            const string query = "SELECT COUNT(DISTINCT AsAt) FROM custom_cmds.ZUserStatus {0}";

            var where = CreateWhereForFilter(filter);
            var curQuery = string.Format(query, where);

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<int>(curQuery, GetParametersForFilter(filter).ToArray()).FirstOrDefault();
        }

        private static List<SqlParameter> GetParametersForFilter(EmployeeComplianceSnapshotFilter filter)
        {
            var parameters = new List<SqlParameter>();

            if (filter.OrganizationIdentifier.HasValue)
                parameters.Add(new SqlParameter("OrganizationIdentifier", filter.OrganizationIdentifier));

            if (filter.DepartmentIdentifier.HasValue)
                parameters.Add(new SqlParameter("DepartmentIdentifier", filter.DepartmentIdentifier));

            if (filter.UserIdentifier.HasValue)
                parameters.Add(new SqlParameter("UserIdentifier", filter.UserIdentifier));

            if (filter.PrimaryProfileIdentifier.HasValue)
                parameters.Add(new SqlParameter("PrimaryProfileIdentifier", filter.PrimaryProfileIdentifier));

            return parameters;
        }

        private static string CreateWhereForFilter(EmployeeComplianceSnapshotFilter filter)
        {
            var where = new StringBuilder();
            where.Append("WHERE 1 = 1");

            if (filter.OrganizationIdentifier.HasValue)
                where.Append(" AND OrganizationIdentifier = @OrganizationIdentifier");

            if (filter.DepartmentIdentifier.HasValue)
                where.Append(" AND DepartmentIdentifier = @DepartmentIdentifier");

            if (filter.UserIdentifier.HasValue)
                where.Append(" AND UserIdentifier = @UserIdentifier");

            if (filter.PrimaryProfileIdentifier.HasValue)
                where.Append(" AND PrimaryProfileIdentifier = @PrimaryProfileIdentifier");

            return where.ToString();
        }
    }
}
