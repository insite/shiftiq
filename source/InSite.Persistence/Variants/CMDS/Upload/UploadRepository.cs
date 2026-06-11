using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Persistence.Plugin.CMDS
{
    public static class UploadRepository
    {
        #region SELECT

        public static bool IsAchievementHasRealtionships(Guid achievement)
        {
            using (var db = new InternalDbContext())
            {
                return db.UploadRelations
                    .Where(x => x.ContainerIdentifier == achievement)
                    .Join(db.Groups,
                        a => a.Upload.ContainerIdentifier,
                        b => b.GroupIdentifier,
                        (a, b) => a
                    )
                    .Any();
            }
        }

        public static Guid[] SelectAchievements(Guid uploadId)
        {
            using (var db = new InternalDbContext())
            {
                return db.UploadRelations
                    .Where(x => x.UploadIdentifier == uploadId)
                    .Join(db.VCmdsAchievements,
                        a => a.ContainerIdentifier,
                        b => b.AchievementIdentifier,
                        (a, b) => a.ContainerIdentifier
                    )
                    .ToArray();
            }
        }

        public static Guid[] SelecteCompetencies(Guid uploadId)
        {
            const string query = @"
SELECT
    Competency.CompetencyStandardIdentifier
FROM
    custom_cmds.Competency
    INNER JOIN resources.UploadRelation ON UploadRelation.ContainerIdentifier = Competency.StandardIdentifier
WHERE
    UploadRelation.UploadIdentifier = @UploadIdentifier;";

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<Guid>(query, new SqlParameter("UploadIdentifier", uploadId)).ToArray();
        }

        public static int CountRedundant(string type)
        {
            const string query = @"
SELECT COUNT(*)
FROM
    resources.Upload
WHERE
    UploadType = @UploadType
    AND ContainerIdentifier NOT IN (
        SELECT QGroup.GroupIdentifier FROM contacts.QGroup
        UNION ALL
        SELECT Competency.StandardIdentifier FROM custom_cmds.Competency
        UNION ALL
        SELECT ContactExperience.UserIdentifier FROM contacts.ContactExperience
        UNION ALL
        SELECT VCmdsAchievement.AchievementIdentifier FROM custom_cmds.VCmdsAchievement
        UNION ALL
        SELECT VCmdsCredential.CredentialIdentifier FROM custom_cmds.VCmdsCredential
    )";

            using (var db = new InternalDbContext())
            {
                db.Database.CommandTimeout = 60 * 5;
                return db.Database.SqlQuery<int>(query, new SqlParameter("UploadType", type)).First();
            }
        }

        public static List<Upload> SelectRedundant(string type)
        {
            const string query = @"
SELECT TOP 100
    *
FROM
    resources.Upload
WHERE
    UploadType = @UploadType
    AND ContainerIdentifier NOT IN (
        SELECT QGroup.GroupIdentifier FROM contacts.QGroup
        UNION ALL
        SELECT Competency.StandardIdentifier FROM custom_cmds.Competency
        UNION ALL
        SELECT ContactExperience.UserIdentifier FROM contacts.ContactExperience
        UNION ALL
        SELECT VCmdsAchievement.AchievementIdentifier FROM custom_cmds.VCmdsAchievement
        UNION ALL
        SELECT VCmdsCredential.CredentialIdentifier FROM custom_cmds.VCmdsCredential
    )
ORDER BY [Name];";

            using (var db = new InternalDbContext())
            {
                db.Database.CommandTimeout = 60 * 5;
                return db.Database.SqlQuery<Upload>(query, new SqlParameter("UploadType", type)).ToList();
            }
        }

        public static List<Upload> SelectAchievementUploads(Guid achievement)
        {
            using (var db = new InternalDbContext())
            {
                return db.Uploads
                    .Join(db.VCmdsAchievements.Where(x => x.AchievementIdentifier == achievement),
                        a => a.ContainerIdentifier,
                        b => b.AchievementIdentifier,
                        (a, b) => a
                    )
                    .ToList();
            }
        }

        public static DataTable SelectCompanyPotentialDataIssues(Guid containerId)
        {
            const string query = @"
SELECT
    Upload.UploadIdentifier
   ,Upload.[Name] AS UploadName
   ,Upload.UploadType
   ,QAchievement.AchievementIdentifier
   ,QAchievement.AchievementTitle
   ,Competency.StandardIdentifier CompetencyStandardIdentifier
   ,Competency.Code AS CompetencyNumber
FROM
    resources.Upload
    INNER JOIN resources.UploadRelation AS ResourceUploadRelation ON ResourceUploadRelation.UploadIdentifier = Upload.UploadIdentifier
    INNER JOIN achievements.QAchievement ON QAchievement.AchievementIdentifier = ResourceUploadRelation.ContainerIdentifier
    INNER JOIN resources.UploadRelation AS CompetencyUploadRelation ON CompetencyUploadRelation.UploadIdentifier = Upload.UploadIdentifier
    INNER JOIN standards.Standard AS Competency ON Competency.StandardIdentifier = CompetencyUploadRelation.ContainerIdentifier
WHERE
    Upload.ContainerIdentifier = @ContainerGuid
    AND Competency.StandardType = 'Competency'
    AND NOT EXISTS (
        SELECT
            *
        FROM
            achievements.TAchievementStandard
        WHERE
            StandardIdentifier = Competency.StandardIdentifier
            AND AchievementIdentifier = QAchievement.AchievementIdentifier
    )
ORDER BY
    UploadType
   ,UploadName;
";

            return DatabaseHelper.CreateDataTable(query, new SqlParameter("ContainerGuid", containerId));
        }

        public static DataTable SelectOrganizationAchievements(Guid organizationId, string achievementType, bool excludeElm, bool excludeTssc)
        {
            const string query = @"EXEC custom_cmds.SelectOrganizationAchievements @OrganizationIdentifier, @AchievementType, @ExcludeElm, @ExcludeTss";

            var parameters = new[]
            {
                new SqlParameter("@OrganizationIdentifier", organizationId),
                new SqlParameter("@AchievementType", achievementType),
                new SqlParameter("@ExcludeElm", excludeElm),
                new SqlParameter("@ExcludeTss", excludeTssc)
            };

            return DatabaseHelper.CreateDataTable(query, null, parameters.ToArray());
        }

        public static string[] SelectCompanyAchievementTypes(Guid organizationId)
        {
            const string query = @"
SELECT     DISTINCT
           A.AchievementLabel
FROM
           resources.Upload
INNER JOIN achievements.QAchievement AS A
           ON A.AchievementIdentifier = Upload.ContainerIdentifier

INNER JOIN accounts.QOrganization AS Organization
           ON A.OrganizationIdentifier = Organization.OrganizationIdentifier
WHERE
           Organization.OrganizationIdentifier = @OrganizationIdentifier
UNION
SELECT     DISTINCT
           A.AchievementLabel
FROM
           resources.UploadRelation
INNER JOIN achievements.QAchievement AS A
           ON A.AchievementIdentifier = UploadRelation.ContainerIdentifier

INNER JOIN accounts.QOrganization AS Organization
           ON A.OrganizationIdentifier = Organization.OrganizationIdentifier
WHERE
           Organization.OrganizationIdentifier = @OrganizationIdentifier
ORDER BY
    1;";

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<string>(query, new SqlParameter("OrganizationIdentifier", organizationId)).ToArray();
        }

        public static DataTable SelectCompetencyUploads(Guid organizationId, Guid competencyId)
        {
            const string query = @"
SELECT
    Organization.OrganizationIdentifier
   ,Organization.CompanyTitle AS GroupName
   ,Upload.ContainerIdentifier AS UploadContainerIdentifier
   ,Upload.[Name] AS UploadName
   ,Upload.Title AS UploadTitle
   ,Upload.UploadType
FROM
    accounts.QOrganization AS Organization
    INNER JOIN resources.Upload ON Upload.ContainerIdentifier = Organization.OrganizationIdentifier
    INNER JOIN resources.UploadRelation ON UploadRelation.UploadIdentifier = Upload.UploadIdentifier
    INNER JOIN custom_cmds.Competency ON Competency.StandardIdentifier = UploadRelation.ContainerIdentifier
WHERE
    Organization.OrganizationIdentifier = @OrganizationIdentifier
    AND Competency.CompetencyStandardIdentifier = @CompetencyStandardIdentifier
ORDER BY
    GroupName
   ,UploadTitle;";

            return DatabaseHelper.CreateDataTable(query, new SqlParameter("OrganizationIdentifier", organizationId), new SqlParameter("CompetencyStandardIdentifier", competencyId));
        }

        public static DataTable SelectAllAchievementUploads(Guid achievementIdentifier, Paging paging)
        {
            using (var db = new InternalDbContext())
            {
                var query1 = db.Uploads
                    .Where(x => x.ContainerIdentifier == achievementIdentifier)
                    .Select(x => new
                    {
                        x.UploadIdentifier,
                        x.UploadType,
                        x.ContainerIdentifier,
                        x.Name,
                        x.Title,
                        x.Description,
                        x.ContentSize,
                        x.Uploaded,
                        x.Uploader,
                        IsResourceUpload = true,
                        CompanyName = (string)null,
                        DepartmentName = (string)null
                    });

                var query2 = db.UploadRelations
                    .Where(x => x.ContainerIdentifier == achievementIdentifier)
                    .GroupJoin(db.Groups,
                        a => a.Upload.ContainerIdentifier,
                        b => b.GroupIdentifier,
                        (a, b) => new { a.Upload, Groups = b.DefaultIfEmpty() }
                    )
                    .SelectMany(x => x.Groups.Select(y => new
                    {
                        x.Upload.UploadIdentifier,
                        x.Upload.UploadType,
                        x.Upload.ContainerIdentifier,
                        x.Upload.Name,
                        x.Upload.Title,
                        x.Upload.Description,
                        x.Upload.ContentSize,
                        x.Upload.Uploaded,
                        x.Upload.Uploader,
                        IsResourceUpload = false,
                        CompanyName = y.GroupName,
                        DepartmentName = (string)null
                    }));

                return query1
                    .Union(query2)
                    .OrderBy(x => x.Title)
                    .ApplyPaging(paging)
                    .ToDataTable();
            }
        }

        public static int CountAllAchievementUploads(Guid achievementIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                var count1 = db.Uploads
                    .Where(x => x.ContainerIdentifier == achievementIdentifier)
                    .Count();

                var count2 = db.UploadRelations
                    .Where(x => x.ContainerIdentifier == achievementIdentifier)
                    .Count();

                return count1 + count2;
            }
        }

        public static DataTable SelectAllCompanyUploads(Guid organizationId, string type, int pageIndex, int pageSize)
        {
            const string query = @"
SELECT
    Upload.UploadIdentifier
   ,Upload.ContainerIdentifier
   ,Upload.[Name]
   ,Upload.UploadType
   ,Upload.Title
   ,Upload.[Description]
   ,Upload.ContentSize
   ,Upload.Uploaded
FROM
    accounts.QOrganization as Organization
    INNER JOIN resources.Upload ON Upload.ContainerIdentifier = Organization.OrganizationIdentifier
WHERE
    Organization.OrganizationIdentifier = @OrganizationIdentifier
    {0}

UNION

SELECT
    Upload.UploadIdentifier
   ,Upload.ContainerIdentifier
   ,Upload.[Name]
   ,Upload.UploadType
   ,Upload.Title
   ,Upload.[Description]
   ,Upload.ContentSize
   ,Upload.Uploaded
FROM
    resources.Upload
    INNER JOIN custom_cmds.VCmdsAchievement ON VCmdsAchievement.AchievementIdentifier = Upload.ContainerIdentifier
    INNER JOIN custom_cmds.VCmdsAchievementOrganization ON VCmdsAchievementOrganization.AchievementIdentifier = VCmdsAchievement.AchievementIdentifier
WHERE
    VCmdsAchievementOrganization.OrganizationIdentifier = @OrganizationIdentifier
    {0}

UNION

SELECT
    Upload.UploadIdentifier
   ,Upload.ContainerIdentifier
   ,Upload.[Name]
   ,Upload.UploadType
   ,Upload.Title
   ,Upload.[Description]
   ,Upload.ContentSize
   ,Upload.Uploaded
FROM
    resources.Upload
    INNER JOIN custom_cmds.VCmdsAchievement ON VCmdsAchievement.AchievementIdentifier = Upload.ContainerIdentifier
WHERE
    VCmdsAchievement.OrganizationIdentifier = @OrganizationIdentifier
    {0}

ORDER BY Title
OFFSET (@PageIndex * @PageSize)
ROWS FETCH NEXT @PageSize ROWS ONLY;";

            var where = !string.IsNullOrEmpty(type) ? "AND Upload.UploadType = @Type" : null;
            var curQuery = string.Format(query, where);

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("OrganizationIdentifier", organizationId)
            };

            if (!string.IsNullOrEmpty(type))
                parameters.Add(new SqlParameter("Type", type));

            parameters.Add(new SqlParameter("PageIndex", pageIndex));
            parameters.Add(new SqlParameter("PageSize", pageSize));

            return DatabaseHelper.CreateDataTable(curQuery, parameters.ToArray());
        }

        public static int CountAllCompanyUploads(Guid organizationId, string type)
        {
            const string query = @"
WITH CTE AS (
    SELECT
        Upload.UploadIdentifier
    FROM
        accounts.QOrganization AS Organization
        INNER JOIN resources.Upload ON Upload.ContainerIdentifier = Organization.OrganizationIdentifier
    WHERE
        Organization.OrganizationIdentifier = @OrganizationIdentifier
        {0}

    UNION

    SELECT
        Upload.UploadIdentifier
    FROM
        resources.Upload
        INNER JOIN custom_cmds.VCmdsAchievement ON VCmdsAchievement.AchievementIdentifier = Upload.ContainerIdentifier
        INNER JOIN custom_cmds.VCmdsAchievementOrganization ON VCmdsAchievementOrganization.AchievementIdentifier = VCmdsAchievement.AchievementIdentifier
    WHERE
        VCmdsAchievementOrganization.OrganizationIdentifier = @OrganizationIdentifier
        {0}

    UNION

    SELECT
        Upload.UploadIdentifier
    FROM
        resources.Upload
        INNER JOIN custom_cmds.VCmdsAchievement ON VCmdsAchievement.AchievementIdentifier = Upload.ContainerIdentifier
    WHERE
        VCmdsAchievement.OrganizationIdentifier = @OrganizationIdentifier
        {0}
)
SELECT COUNT(*) FROM CTE;";

            var where = !string.IsNullOrEmpty(type) ? "AND Upload.UploadType = @Type" : null;
            var curQuery = string.Format(query, where);

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("OrganizationIdentifier", organizationId)
            };

            if (!string.IsNullOrEmpty(type))
                parameters.Add(new SqlParameter("Type", type));

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<int>(curQuery, parameters.ToArray()).FirstOrDefault();
        }

        public static DataTable SelectUploadAchievements(Guid uploadId)
        {
            const string query = @"
SELECT
    VCmdsAchievement.AchievementIdentifier
   ,VCmdsAchievement.AchievementTitle
FROM
    custom_cmds.VCmdsAchievement
    INNER JOIN resources.UploadRelation ON UploadRelation.ContainerIdentifier = VCmdsAchievement.AchievementIdentifier
WHERE
    UploadRelation.UploadIdentifier = @UploadIdentifier

UNION

SELECT
    VCmdsAchievement.AchievementIdentifier
   ,VCmdsAchievement.AchievementTitle
FROM
    resources.Upload
    INNER JOIN custom_cmds.VCmdsAchievement ON VCmdsAchievement.AchievementIdentifier = Upload.ContainerIdentifier
WHERE
    Upload.UploadIdentifier = @UploadIdentifier

ORDER BY
    AchievementTitle;";

            return DatabaseHelper.CreateDataTable(query, new SqlParameter("UploadIdentifier", uploadId));
        }

        public static DataTable SelectUploadCompetencies(Guid uploadId)
        {
            const string query = @"
SELECT
    Competency.CompetencyStandardIdentifier
   ,Competency.Number
   ,CAST(0 AS BIT) IsIndirectAssignment
FROM
    custom_cmds.Competency
    INNER JOIN resources.UploadRelation ON UploadRelation.ContainerIdentifier = Competency.StandardIdentifier
WHERE
    UploadRelation.UploadIdentifier = @UploadIdentifier
  
UNION

SELECT
    Competency.CompetencyStandardIdentifier
   ,Competency.Number
   ,CAST(1 AS BIT) IsIndirectAssignment
FROM
    custom_cmds.VCmdsAchievement
    INNER JOIN custom_cmds.VCmdsAchievementCompetency ON VCmdsAchievementCompetency.AchievementIdentifier = VCmdsAchievement.AchievementIdentifier
    INNER JOIN custom_cmds.Competency ON Competency.CompetencyStandardIdentifier = VCmdsAchievementCompetency.CompetencyStandardIdentifier
    INNER JOIN resources.UploadRelation ON UploadRelation.ContainerIdentifier = VCmdsAchievement.AchievementIdentifier
WHERE
    UploadRelation.UploadIdentifier = @UploadIdentifier
    AND VCmdsAchievementCompetency.CompetencyStandardIdentifier NOT IN (
        SELECT
            Competency.CompetencyStandardIdentifier
        FROM
            custom_cmds.Competency
            INNER JOIN resources.UploadRelation ON UploadRelation.ContainerIdentifier = Competency.StandardIdentifier
        WHERE
            UploadRelation.UploadIdentifier = @UploadIdentifier
    )

ORDER BY
    Number;";

            return DatabaseHelper.CreateDataTable(query, new SqlParameter("UploadIdentifier", uploadId));
        }

        public static bool IsAchievementHasUploads(Guid achievement)
        {
            using (var db = new InternalDbContext())
            {
                return db.Uploads
                    .Where(x => x.ContainerIdentifier == achievement)
                    .Any();
            }
        }

        public static DataTable SelectWithCounters(Guid containerId, string[] types, int pageIndex, int pageSize)
        {
            const string query = @"
SELECT
    Upload.UploadIdentifier
   ,Upload.Title
   ,Upload.ContainerIdentifier
   ,Upload.[Name]
   ,Upload.UploadType
   ,Upload.[Description]
   ,Upload.ContentSize
   ,Upload.Uploaded
   ,(
        SELECT
            COUNT(*)
        FROM
            custom_cmds.Competency
            INNER JOIN resources.UploadRelation ON UploadRelation.ContainerIdentifier = Competency.StandardIdentifier
        WHERE
            UploadRelation.UploadIdentifier = Upload.UploadIdentifier
    ) AS NumberOfCompetencies
   ,(
        SELECT
            COUNT(*)
        FROM
            custom_cmds.VCmdsAchievement
            INNER JOIN resources.UploadRelation ON UploadRelation.ContainerIdentifier = VCmdsAchievement.AchievementIdentifier
        WHERE
            UploadRelation.UploadIdentifier = Upload.UploadIdentifier
    ) AS NumberOfResources
FROM
    resources.Upload
WHERE
    Upload.ContainerIdentifier = @ContainerIdentifier
    {0}
ORDER BY
    Upload.Title

OFFSET (@PageIndex * @PageSize)
ROWS FETCH NEXT @PageSize ROWS ONLY";

            var where = string.Empty;
            var hasTypes = types.IsNotEmpty();

            if (hasTypes)
            {
                var sb = new StringBuilder("AND Upload.UploadType IN (");

                for (var i = 0; i < types.Length; i++)
                {
                    if (i > 0)
                        sb.Append(",");

                    sb.Append("@UploadType").Append(i);
                }

                sb.Append(")");

                where += sb.ToString();
            }

            var curQuery = string.Format(query, where);

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("ContainerIdentifier", containerId),
                new SqlParameter("PageIndex", pageIndex),
                new SqlParameter("PageSize", pageSize)
            };

            if (hasTypes)
            {
                for (var i = 0; i < types.Length; i++)
                    parameters.Add(new SqlParameter("UploadType" + i, types[i]));
            }

            return DatabaseHelper.CreateDataTable(curQuery, parameters.ToArray());
        }

        public static int Count(Guid containerId, string[] types)
        {
            const string query = "SELECT COUNT(*) FROM resources.Upload WHERE Upload.ContainerIdentifier = @ContainerIdentifier {0}";

            var where = string.Empty;
            var hasTypes = types.IsNotEmpty();

            if (hasTypes)
            {
                var sb = new StringBuilder("AND Upload.UploadType IN (");

                for (var i = 0; i < types.Length; i++)
                {
                    if (i > 0)
                        sb.Append(",");

                    sb.Append("@UploadType").Append(i);
                }

                sb.Append(")");

                where += sb.ToString();
            }

            var curQuery = string.Format(query, where);

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("ContainerIdentifier", containerId)
            };

            if (hasTypes)
            {
                for (var i = 0; i < types.Length; i++)
                    parameters.Add(new SqlParameter("UploadType" + i, types[i]));
            }

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<int>(curQuery, parameters.ToArray()).FirstOrDefault();
        }

        public static int CountByAchievement(Guid achievementIdentifier)
        {
            const string query = @"
SELECT
    COUNT(*)
FROM
    resources.Upload
WHERE
    ContainerIdentifier = @AchievementIdentifier;";

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<int>(query, new SqlParameter("AchievementIdentifier", achievementIdentifier)).FirstOrDefault();
        }

        #endregion

        #region Data manipulation

        public static bool AttachAchievement(Guid uploadId, Guid achievementIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                if (db.UploadRelations.Where(x => x.ContainerIdentifier == achievementIdentifier && x.UploadIdentifier == uploadId).Any())
                    return false;

                db.UploadRelations.Add(new UploadRelation
                {
                    ContainerIdentifier = achievementIdentifier,
                    UploadIdentifier = uploadId,
                    ContainerType = UploadContainerType.Asset
                });

                db.SaveChanges();
            }

            return true;
        }

        public static void DetachAchievement(Guid uploadId, Guid achievementIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                var entity = db.UploadRelations
                    .Where(x => x.ContainerIdentifier == achievementIdentifier && x.UploadIdentifier == uploadId)
                    .FirstOrDefault();

                if (entity != null)
                {
                    db.UploadRelations.Remove(entity);
                    db.SaveChanges();
                }
            }
        }

        public static void DetachAchievement(Guid achievement)
        {
            using (var db = new InternalDbContext())
            {
                var list = db.UploadRelations
                    .Where(x => x.ContainerIdentifier == achievement)
                    .ToList();

                db.UploadRelations.RemoveRange(list);
                db.SaveChanges();
            }
        }

        public static bool AttachCompetency(Guid uploadId, Guid competencyId)
        {
            const string query = @"
DECLARE @CompetencyThumbprint uniqueidentifier = (SELECT StandardIdentifier FROM custom_cmds.Competency WHERE CompetencyStandardIdentifier = @CompetencyStandardIdentifier)

IF @CompetencyThumbprint IS NOT NULL AND NOT EXISTS(SELECT * FROM resources.UploadRelation WHERE ContainerIdentifier = @CompetencyThumbprint AND UploadIdentifier = @UploadIdentifier)
    INSERT INTO resources.UploadRelation (ContainerIdentifier, UploadIdentifier, ContainerType) VALUES (@CompetencyThumbprint, @UploadIdentifier, @ContainerType);

SELECT @@ROWCOUNT;
";

            using (var db = new InternalDbContext())
            {
                return db.Database.SqlQuery<int>(query,
                    new SqlParameter("UploadIdentifier", uploadId),
                    new SqlParameter("CompetencyStandardIdentifier", competencyId),
                    new SqlParameter("ContainerType", UploadContainerType.Asset)
                    ).FirstOrDefault() > 0;
            }
        }

        public static void DetachCompetency(Guid uploadId, Guid competencyId)
        {
            const string query = @"
DELETE
    resources.UploadRelation
FROM
    resources.UploadRelation
    INNER JOIN custom_cmds.Competency ON Competency.StandardIdentifier = UploadRelation.ContainerIdentifier
WHERE
    UploadRelation.UploadIdentifier = @UploadIdentifier
    AND Competency.CompetencyStandardIdentifier = @CompetencyStandardIdentifier;
";

            using (var db = new InternalDbContext())
            {
                db.Database.ExecuteSqlCommand(query,
                    new SqlParameter("UploadIdentifier", uploadId),
                    new SqlParameter("CompetencyStandardIdentifier", competencyId)
                    );
            }
        }

        public static void DeleteRelations(Guid uploadId)
        {
            const string query = @"
DELETE
    resources.UploadRelation
WHERE
    UploadRelation.UploadIdentifier = @UploadIdentifier;";

            using (var db = new InternalDbContext())
                db.Database.ExecuteSqlCommand(query, new SqlParameter("UploadIdentifier", uploadId));
        }

        #endregion
    }
}
