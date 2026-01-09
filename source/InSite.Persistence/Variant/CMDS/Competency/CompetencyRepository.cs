using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using Shift.Common.Timeline.Commands;

using InSite.Application.StandardValidations.Write;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence.Plugin.CMDS
{
    public static class CompetencyRepository
    {
        #region Classes

        public class ExpiringCompetency
        {
            public Guid OrganizationIdentifier { get; set; }
            public Guid UserIdentifier { get; set; }
            public Guid CompetencyStandardIdentifier { get; set; }

            public string CompanyName { get; set; }
            public string DepartmentNames { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public string CompetencyNumber { get; set; }
            public string CompetencyTitle { get; set; }

            public DateTimeOffset? DateCompleted { get; set; }
            public DateTimeOffset DateExpired { get; set; }
            public DateTimeOffset? Notified { get; set; }
        }

        #endregion

        #region Intialization

        private static Action<ICommand> _sendCommand;
        private static Action<IEnumerable<ICommand>> _sendCommands;

        public static void Initialize(Action<ICommand> sendCommand, Action<IEnumerable<ICommand>> sendCommands)
        {
            _sendCommand = sendCommand;
            _sendCommands = sendCommands;
        }

        #endregion

        #region SELECT

        public static Competency Select(Guid competencyStandardIdentifier)
        {
            using (var db = new InternalDbContext())
                return db.Competencies.FirstOrDefault(x => x.StandardIdentifier == competencyStandardIdentifier);
        }

        public static Competency Select(string number)
        {
            using (var db = new InternalDbContext())
                return db.Competencies.FirstOrDefault(x => x.Number == number);
        }

        public static IEnumerable<CompetencyCategory> SelectCompetencyCategories(Guid? profileStandardIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                var query = db.CompetencyCategories.AsQueryable();

                if (profileStandardIdentifier.HasValue)
                {
                    query = query
                        .Join(db.ProfileCompetencies.Where(x => x.ProfileStandardIdentifier == profileStandardIdentifier),
                            a => a.CompetencyStandardIdentifier,
                            b => b.CompetencyStandardIdentifier,
                            (a, b) => a
                        );
                }

                return query.ToList();
            }
        }

        public static List<ExpiringCompetency> SelectExpiredCompetencies(DateTimeOffset asAt)
        {
            const string query = "EXEC custom_cmds.SelectCompetenciesExpiring @NotifiedAt = @NotifiedAt, @ExpiredAt = @ExpiredAt";

            try
            {
                using (var db = new InternalDbContext())
                {
                    var start = asAt.AddYears(-10);

                    return db.Database.SqlQuery<ExpiringCompetency>(query, new SqlParameter[] {
                        new SqlParameter("@NotifiedAt", start),
                        new SqlParameter("@ExpiredAt", asAt)
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                var message = $"Unable to select expired competencies as at {asAt:g}.";
                throw new Exception(message, ex);
            }
        }

        #endregion

        #region SELECT download competencies

        public static DataTable SelectUploadCompetencies(Guid uploadId)
        {
            const string query = @"
SELECT
    Competency.*
FROM
    custom_cmds.Competency
    INNER JOIN resources.UploadRelation ON UploadRelation.ContainerIdentifier = Competency.StandardIdentifier
WHERE
    UploadRelation.UploadIdentifier = @UploadIdentifier
    AND Competency.IsDeleted = 0
ORDER BY
    Competency.Number
   ,Competency.Summary;";

            return DatabaseHelper.CreateDataTable(query, new SqlParameter("UploadIdentifier", uploadId));
        }

        public static DataTable SelectNewCompanyUploadCompetencies(Guid uploadId, Guid organizationId, string searchText)
        {
            var query = new StringBuilder(@"
SELECT
    c.*
FROM
    custom_cmds.Competency AS c
    INNER JOIN custom_cmds.VCmdsCompetencyOrganization AS cc ON cc.CompetencyStandardIdentifier = c.CompetencyStandardIdentifier
WHERE
    c.StandardIdentifier NOT IN (
        SELECT ContainerIdentifier
        FROM resources.UploadRelation
        WHERE UploadRelation.UploadIdentifier = @UploadIdentifier
    )
    AND cc.OrganizationIdentifier = @OrganizationIdentifier
    AND c.IsDeleted = 0");

            if (searchText.IsNotEmpty())
                query.Append(" AND (Summary LIKE @SearchText OR Number LIKE @SearchText)");

            query.Append(" ORDER BY Number, Summary");

            return DatabaseHelper.CreateDataTable(query.ToString(),
                new SqlParameter("UploadIdentifier", uploadId),
                new SqlParameter("OrganizationIdentifier", organizationId),
                new SqlParameter("SearchText", string.Format("%{0}%", searchText))
                );
        }

        #endregion

        #region SELECT resource competencies

        public static DataTable SelectAchievementCompetencies(Guid achievement)
        {
            using (var db = new InternalDbContext())
            {
                return db.VCmdsAchievementCompetencies
                    .Where(x => x.AchievementIdentifier == achievement && x.IsDeleted == false)
                    .Select(x => new
                    {
                        CompetencyStandardIdentifier = x.CompetencyStandardIdentifier,
                        Number = x.Number,
                        Summary = x.Summary
                    })
                    .OrderBy("Number, Summary")
                    .ToDataTable();
            }
        }

        public static DataTable SelectNewAchievementCompetencies(Guid achievementIdentifier, string searchText, Guid? copyAchievementIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                var query = db.Competencies
                    .Where(x =>
                        !db.VCmdsAchievementCompetencies.Any(y => y.AchievementIdentifier == achievementIdentifier && y.CompetencyStandardIdentifier == x.StandardIdentifier)
                        && x.IsDeleted == false
                    );

                if (searchText.IsNotEmpty())
                    query = query.Where(x => x.Summary.Contains(searchText) || x.Number.Contains(searchText));

                if (copyAchievementIdentifier.HasValue)
                    query = query.Where(x => db.VCmdsAchievementCompetencies.Any(y => y.AchievementIdentifier == copyAchievementIdentifier && y.CompetencyStandardIdentifier == x.StandardIdentifier));

                return query
                    .Select(x => new
                    {
                        CompetencyStandardIdentifier = x.StandardIdentifier,
                        Number = x.Number,
                        Summary = x.Summary
                    })
                    .OrderBy("Number, Summary")
                    .ToDataTable();
            }
        }

        #endregion

        #region SELECT profile competencies

        public static DataTable SelectProfileCompetencies(Guid profileStandardIdentifier)
        {
            const String query = @"
SELECT c.CompetencyStandardIdentifier
      ,c.*
      ,pc.ProfileStandardIdentifier
      ,pc.CertificationHoursCore
      ,pc.CertificationHoursNonCore
  FROM custom_cmds.Competency AS c
       INNER JOIN custom_cmds.ProfileCompetency AS pc
         ON pc.CompetencyStandardIdentifier = c.CompetencyStandardIdentifier
  WHERE pc.ProfileStandardIdentifier = @ProfileStandardIdentifier
        AND c.IsDeleted = 0
  ORDER BY c.Number, c.Summary";

            return DatabaseHelper.CreateDataTable(query, new SqlParameter("ProfileStandardIdentifier", profileStandardIdentifier));
        }

        public static DataTable SelectNewProfileCompetencies(Guid profileStandardIdentifier, String searchText, Guid? copyProfileStandardIdentifier)
        {
            StringBuilder query = new StringBuilder(@"
SELECT *
  FROM custom_cmds.Competency
  WHERE CompetencyStandardIdentifier NOT IN (
          SELECT CompetencyStandardIdentifier
            FROM custom_cmds.ProfileCompetency
            WHERE ProfileStandardIdentifier = @ProfileStandardIdentifier
        )
        AND IsDeleted = 0");

            if (searchText.IsNotEmpty())
                query.Append(" AND (Summary LIKE @SearchText OR Number LIKE @SearchText)");

            if (copyProfileStandardIdentifier.HasValue)
            {
                query.Append(@"
AND CompetencyStandardIdentifier IN (
  SELECT CompetencyStandardIdentifier
    FROM custom_cmds.ProfileCompetency
    WHERE ProfileStandardIdentifier = @CopyProfileStandardIdentifier
)");
            }

            query.Append(" ORDER BY Number, Summary");

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ProfileStandardIdentifier", profileStandardIdentifier));

            if (searchText.IsNotEmpty())
                parameters.Add(new SqlParameter("SearchText", string.Format("%{0}%", searchText)));

            if (copyProfileStandardIdentifier.HasValue)
                parameters.Add(new SqlParameter("CopyProfileStandardIdentifier", copyProfileStandardIdentifier));

            return DatabaseHelper.CreateDataTable(query.ToString(), parameters.ToArray());
        }

        #endregion

        #region SELECT (from reader)

        public static DataTable SelectRelatedGroups(Guid competencyStandardIdentifier)
        {
            const string query = @"
SELECT Organization.OrganizationIdentifier
      ,Organization.CompanyTitle AS CompanyName
      ,NULL AS DepartmentIdentifier
      ,NULL AS DepartmentName
  FROM custom_cmds.VCmdsCompetencyOrganization AS cc
       INNER JOIN accounts.QOrganization AS Organization
         ON Organization.OrganizationIdentifier = cc.OrganizationIdentifier
  WHERE cc.CompetencyStandardIdentifier = @CompetencyStandardIdentifier

UNION

SELECT Organization.OrganizationIdentifier
      ,Organization.CompanyTitle AS CompanyName
      ,department.DepartmentIdentifier
      ,department.DepartmentName
  FROM custom_cmds.DepartmentProfileCompetency AS cs
       INNER JOIN identities.Department
         ON Department.DepartmentIdentifier = cs.DepartmentIdentifier
       INNER JOIN accounts.QOrganization AS Organization
         ON Organization.OrganizationIdentifier = Department.OrganizationIdentifier
  WHERE cs.CompetencyStandardIdentifier = @CompetencyStandardIdentifier

ORDER BY CompanyName, DepartmentName";

            return DatabaseHelper.CreateDataTable(query, new SqlParameter("CompetencyStandardIdentifier", competencyStandardIdentifier));
        }

        public static DataTable SelectRelatedEmployees(Guid competencyStandardIdentifier)
        {
            const String query = @"
SELECT p.UserIdentifier
      ,p.FullName
  FROM custom_cmds.UserCompetency AS ec
       INNER JOIN identities.[User] AS p
         ON p.UserIdentifier = ec.UserIdentifier
  WHERE ec.CompetencyStandardIdentifier = @CompetencyStandardIdentifier
  ORDER BY p.FullName";

            return DatabaseHelper.CreateDataTable(query, new SqlParameter("CompetencyStandardIdentifier", competencyStandardIdentifier));
        }

        public static DataTable SelectRelatedProfiles(Guid competencyStandardIdentifier)
        {
            const String query = @"
SELECT p.ProfileStandardIdentifier
      ,p.ProfileNumber
      ,p.ProfileTitle
  FROM custom_cmds.ProfileCompetency pc
       INNER JOIN custom_cmds.[Profile] p
         ON p.ProfileStandardIdentifier = pc.ProfileStandardIdentifier
  WHERE pc.CompetencyStandardIdentifier = @CompetencyStandardIdentifier
  ORDER BY p.ProfileNumber";

            return DatabaseHelper.CreateDataTable(query, new SqlParameter("CompetencyStandardIdentifier", competencyStandardIdentifier));
        }

        /// <summary>
        /// This query must return competencies for which ANY of the following criteria is met:
        ///   - The new number is an exact match
        ///   - The old number is an exact match
        ///   - The old number starts with a match
        ///   - The old number contains a match
        ///   - The old number ends with a match
        /// It is assumed here that the value of an "old number" is a comma separated list, where
        /// there is a single blank space character after every comma.
        /// </summary>
        public static IEnumerable<Competency> SelectByNumber(String number)
        {
            const String query = @"
SELECT
    *
FROM
    custom_cmds.Competency
WHERE
    (
    Number = @Number
    OR NumberOld = @Number
    OR NumberOld LIKE @Number + ', %'
    OR NumberOld LIKE '%, ' + @Number + ', %'
    OR NumberOld LIKE '%, ' + @Number
    )
    AND IsDeleted = 0";

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<Competency>(query, new SqlParameter("Number", number), new SqlParameter("NumberOld", number)).ToArray();
        }

        #endregion

        #region Filtering

        public static DataTable SelectSearchResultsWithDepartment(CompetencyFilter filter, Guid department)
        {
            String where = CreateWhereForSelectSearchResults(filter, null);

            String profileJoin = filter.Profiles.IsNotEmpty()
                ? string.Format("AND cs.ProfileStandardIdentifier IN ({0})", CsvConverter.ConvertListToCsvText(filter.Profiles, true))
                : null;

            String query = string.Format(@"
SELECT c.CompetencyStandardIdentifier
      ,c.Number
      ,null as Category
      ,c.Summary
      ,cs.Criticality AS PriorityName
  FROM custom_cmds.Competency c
       INNER JOIN custom_cmds.DepartmentProfileCompetency cs
         ON cs.CompetencyStandardIdentifier = c.CompetencyStandardIdentifier
       {1}
  {0}
        AND cs.DepartmentIdentifier = @DepartmentIdentifier
  ORDER BY c.Number, cs.Criticality", where, profileJoin);

            return DatabaseHelper.CreateDataTable(query, GetParametersForSelectSearchResults(filter, null, department).ToArray());
        }

        public static DataTable SelectSearchResults(CompetencyFilter filter, bool includeCounts = false)
        {
            String where = CreateWhereForSelectSearchResults(filter, null);

            var sortExpression = "Number, Summary";

            String withSortExpression = sortExpression
                .Replace("PriorityName", "v.Text")
                .Replace("LastUpdatedOn", @"
(CASE WHEN f.Modified IS NOT NULL THEN f.Modified
      ELSE f.Created
 END)"
            );

            String extraColumns = @"
        ,CASE WHEN c.Modified IS NOT NULL THEN c.Modified
              ELSE c.Created
         END AS LastUpdatedOn";

            if (includeCounts)
                extraColumns += @"
       ,(SELECT COUNT(*) FROM standards.StandardOrganization WHERE StandardOrganization.StandardIdentifier = c.CompetencyStandardIdentifier) AS StandardOrganizationCount
       ,(SELECT COUNT(*) FROM standards.StandardValidation WHERE StandardValidation.StandardIdentifier = c.CompetencyStandardIdentifier) AS StandardValidationCount";

            String query = string.Format(@"
WITH OrderedCompetencies AS (
  SELECT c.*
{3}
        ,ROW_NUMBER() OVER(ORDER BY {2}) AS RowNumber
    FROM custom_cmds.Competency AS c
    {0}
)
SELECT *
  FROM OrderedCompetencies
  WHERE RowNumber BETWEEN @StartRow AND @EndRow
  ORDER BY {1}", where, sortExpression, withSortExpression, extraColumns);

            var (startRow, endRow) = filter.Paging != null ? filter.Paging.ToStartEnd() : (0, int.MaxValue);

            var parameters = GetParametersForSelectSearchResults(filter, null, null);
            parameters.Add(new SqlParameter("StartRow", startRow));
            parameters.Add(new SqlParameter("EndRow", endRow));

            return DatabaseHelper.CreateDataTable(query, parameters.ToArray());
        }

        public static int CountSearchResults(CompetencyFilter filter)
        {
            String where = CreateWhereForSelectSearchResults(filter, null);
            String query = string.Format(@"SELECT CAST(COUNT(*) AS INT) FROM custom_cmds.Competency AS c {0}", where);

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<int>(query, GetParametersForSelectSearchResults(filter, null, null).ToArray()).FirstOrDefault();
        }

        private static List<SqlParameter> GetParametersForSelectSearchResults(CompetencyFilter filter, String searchText, Guid? department)
        {
            var parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("IsDeleted", filter.IsDeleted));

            if (searchText.IsNotEmpty())
                parameters.Add(new SqlParameter("SearchText", string.Format("%{0}%", searchText)));
            else
            {
                if (filter.Number.IsNotEmpty())
                    parameters.Add(new SqlParameter("Number", string.Format("%{0}%", filter.Number)));

                if (filter.Summary.IsNotEmpty())
                    parameters.Add(new SqlParameter("Summary", string.Format("%{0}%", filter.Summary)));
            }

            if (filter.NumberOld.IsNotEmpty())
                parameters.Add(new SqlParameter("NumberOld", string.Format("%{0}%", filter.NumberOld)));

            if (filter.CategoryIdentifier.HasValue)
                parameters.Add(new SqlParameter("CategoryIdentifier", filter.CategoryIdentifier));

            if (department.HasValue)
                parameters.Add(new SqlParameter("DepartmentIdentifier", department));

            if (filter.Description.IsNotEmpty())
                parameters.Add(new SqlParameter("Description", string.Format("%{0}%", filter.Description)));

            if (filter.OrganizationIdentifier.HasValue)
                parameters.Add(new SqlParameter("OrganizationIdentifier", filter.OrganizationIdentifier));

            return parameters;
        }

        private static String CreateWhereForSelectSearchResults(CompetencyFilter filter, String searchText)
        {
            StringBuilder where = new StringBuilder();
            where.Append("WHERE c.IsDeleted = @IsDeleted");

            if (searchText.IsNotEmpty())
                where.Append(" AND (c.Number LIKE @SearchText OR c.Summary LIKE @SearchText)");
            else
            {
                if (filter.Number.IsNotEmpty())
                    where.Append(" AND c.Number LIKE @Number");

                if (filter.Summary.IsNotEmpty())
                    where.Append(" AND c.Summary LIKE @Summary");
            }

            if (filter.NumberOld.IsNotEmpty())
                where.Append(" AND c.NumberOld LIKE @NumberOld");

            if (filter.CategoryIdentifier.HasValue)
                where.Append(" AND c.CompetencyStandardIdentifier IN (SELECT Competency.StandardIdentifier FROM standards.StandardClassification INNER JOIN standards.[Standard] as Competency ON Competency.StandardIdentifier = StandardClassification.StandardIdentifier WHERE StandardClassification.CategoryIdentifier = @CategoryIdentifier)");

            if (filter.Description.IsNotEmpty())
                where.Append(" AND (c.Knowledge LIKE @Description OR c.Skills LIKE @Description)");

            if (filter.Profiles.IsNotEmpty())
            {
                String list = CsvConverter.ConvertListToCsvText(filter.Profiles, true);

                where.AppendFormat(@"
AND c.CompetencyStandardIdentifier IN (
  SELECT CompetencyStandardIdentifier
    FROM custom_cmds.ProfileCompetency
    WHERE ProfileStandardIdentifier IN ({0})
)"
                    , list
                );
            }

            if (filter.OrganizationIdentifier.HasValue)
            {
                where.Append(@"
AND c.CompetencyStandardIdentifier IN (
  SELECT CompetencyStandardIdentifier
    FROM custom_cmds.VCmdsCompetencyOrganization
    WHERE OrganizationIdentifier = @OrganizationIdentifier
)"
                );
            }

            if (filter.ExcludeCompetencies.IsNotEmpty())
                where.AppendFormat(" AND c.CompetencyStandardIdentifier NOT IN ({0})", CsvConverter.ConvertListToCsvText(filter.ExcludeCompetencies, true));

            return where.ToString();
        }

        #endregion

        #region SelectForSelector

        private const string SelectorQuery = @"
WITH OrderedCompetencies AS (
  SELECT CompetencyStandardIdentifier AS [Value]
        ,ISNULL(Number + ' - ', '') + ISNULL(Title, '???') AS [Text]
        ,ROW_NUMBER() OVER(ORDER BY ISNULL(Number + ' - ', '') + Summary) AS RowNumber
    FROM custom_cmds.Competency c
    {0}
)
SELECT *
  FROM OrderedCompetencies
  {1}
  ORDER BY RowNumber";

        public static DataTable SelectForSelector(CompetencyFilter filter, string searchText)
        {
            var (startRow, endRow) = filter.Paging != null ? filter.Paging.ToStartEnd() : (0, int.MaxValue);
            var where = CreateWhereForSelectSearchResults(filter, searchText);
            var curQuery = string.Format(SelectorQuery, where, "WHERE RowNumber BETWEEN @StartRow AND @EndRow");

            var parameters = GetParametersForSelectSearchResults(filter, searchText, null);
            parameters.Add(new SqlParameter("StartRow", startRow));
            parameters.Add(new SqlParameter("EndRow", endRow));

            return DatabaseHelper.CreateDataTable(curQuery, parameters.ToArray());
        }

        public static DataTable SelectForSelector(IEnumerable<Guid> ids)
        {
            var where = $"WHERE c.CompetencyStandardIdentifier IN ({CsvConverter.ConvertListToCsvText(ids, true)})";
            var curQuery = string.Format(SelectorQuery, where, string.Empty);

            return DatabaseHelper.CreateDataTable(curQuery);
        }

        public static Int32 SelectCountForSelector(CompetencyFilter filter, String searchText)
        {
            const String query = @"SELECT CAST(COUNT(*) AS INT) FROM custom_cmds.Competency AS c {0}";

            String where = CreateWhereForSelectSearchResults(filter, searchText);
            String curQuery = string.Format(query, where);

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<int>(curQuery, GetParametersForSelectSearchResults(filter, searchText, null).ToArray()).FirstOrDefault();
        }

        #endregion

        #region UPDATE

        public static void UpdateNotified(Guid competencyId, Guid userId, DateTimeOffset notified)
        {
            Guid? validationId;
            using (var db = new InternalDbContext())
            {
                validationId = db.QStandardValidations
                    .Where(x => x.UserIdentifier == userId && x.StandardIdentifier == competencyId)
                    .Select(x => (Guid?)x.StandardValidationIdentifier)
                    .FirstOrDefault();
            }

            if (validationId.HasValue)
                _sendCommand(new NotifyStandardValidation(validationId.Value, notified));
        }

        #endregion
    }
}