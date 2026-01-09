using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using InSite.Application.Contacts.Read;
using InSite.Application.Surveys.Read;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Persistence.Plugin.NCSHA
{
    public class ProgramRepository
    {
        #region Classes

        public class Answer
        {
            public Guid QuestionIdentifier { get; set; }
            public Guid SurveyIdentifier { get; set; }
            public Guid RespondentIdentifier { get; set; }
            public Guid ResponseIdentifier { get; set; }
            public Guid ResponseGroup { get; set; }
            public Guid ResponsePeriod { get; set; }

            public string ReportColumn { get; set; }
            public string ReportTable { get; set; }
            public string ResponseAnswer { get; set; }
            public string ResponseAnswerText { get; set; }
            public string SurveyQuestionType { get; set; }
            public string RadioListSelection { get; set; }
        }

        public class Mapping
        {
            public Guid MappingIdentifier { get; set; }
            public string ReportTable { get; set; }
            public string ReportColumn { get; set; }
            public string ReportColumnOther { get; set; }
            public Guid SurveyIdentifier { get; set; }
            public Guid QuestionIdentifier { get; set; }

            public string SurveyName { get; set; }
            public string QuestionText { get; set; }
        }

        public class SurveyMigration
        {
            public List<Answer> Answers { get; set; }
            public List<Mapping> Mappings { get; set; }
            public List<string> Errors { get; set; }

            public SurveyMigration()
            {
                Mappings = GetMappings();
                Answers = GetAnswers();
                Errors = new List<string>();
            }

            public SurveyMigration(int year)
            {
                Mappings = GetMappings();
                Answers = GetAnswers(year);
                Errors = new List<string>();
            }
        }

        private class ReportColumnInfo
        {
            public string ReportColumn { get; set; }
            public int MaximumLength { get; set; }
        }

        #endregion

        #region SqlQueries

        private string rowExists = @"
            SELECT COUNT(*) FROM custom_ncsha.$ReportTableName
            WHERE SurveyYear = CONVERT(INT, @PeriodName)
            AND AgencyGroupIdentifier = @AgencyGroupIdentifier
            AND RespondentUserIdentifier = @UserIdentifier
";

        private string rowExistsHcProgram = @"
            SELECT COUNT(*) FROM custom_ncsha.$ReportTableName
            WHERE SurveyYear = CONVERT(INT, @PeriodName)
            AND AgencyGroupIdentifier = @AgencyGroupIdentifier
            AND OwnerUserIdentifier = @UserIdentifier
";

        private string deleteRow = @"
            DELETE FROM custom_ncsha.$ReportTableName
            WHERE SurveyYear = CONVERT(INT, @PeriodName)
            AND AgencyGroupIdentifier = @AgencyGroupIdentifier
            AND RespondentUserIdentifier = @UserIdentifier
";

        private string deleteRowHcProgram = @"
            DELETE FROM custom_ncsha.$ReportTableName
            WHERE SurveyYear = CONVERT(INT, @PeriodName)
            AND AgencyGroupIdentifier = @AgencyGroupIdentifier
            AND OwnerUserIdentifier = @UserIdentifier
";

        private string insertQueryTemplate = @"  
INSERT INTO custom_ncsha.$ReportTableName 
(
      AgencyGroupIdentifier
    , DateTimeSaved
    , DateTimeSubmitted
    , InsertedBy
    , InsertedOn
    , RespondentName
    , RespondentUserIdentifier
    , SurveyYear
    , UpdatedBy
    , UpdatedOn
    , OrganizationIdentifier
    , $ColumnNames
) 
VALUES 
(
      @AgencyGroupIdentifier
    , @LastChanged
    , @ResponseSessionCompleted
    , NULL
    , @ResponseSessionCreated
    , (SELECT MAX(U.FullName) FROM identities.[User] AS U WHERE U.UserIdentifier = @RespondentIdentifier)
    , @RespondentIdentifier
    , CONVERT(INT, @PeriodName)
    , NULL
    , @UpdatedOn
    , @OrganizationIdentifier
    , $FieldValues
)
";

        private string insertHCQueryTemplate = @"  
INSERT INTO custom_ncsha.$ReportTableName 
(
      AgencyGroupIdentifier
    , DateTimeSaved
    , DateTimeSubmitted
    , InsertedBy
    , InsertedOn
    , FirstName
    , LastName
    , OwnerUserIdentifier
    , SurveyYear
    , UpdatedBy
    , UpdatedOn
    , OrganizationIdentifier
    , $ColumnNames
) 
VALUES 
(
      @AgencyGroupIdentifier
    , @LastChanged
    , @ResponseSessionCompleted
    , NULL
    , @ResponseSessionCreated
    , (SELECT MAX(U.FirstName) FROM identities.[User] AS U WHERE U.UserIdentifier = @RespondentIdentifier)
    , (SELECT MAX(U.LastName) FROM identities.[User] AS U WHERE U.UserIdentifier = @RespondentIdentifier)
    , @RespondentIdentifier
    , CONVERT(INT, @PeriodName)
    , NULL
    , @UpdatedOn
    , @OrganizationIdentifier
    , $FieldValues
)
";

        #endregion

        #region Properties

        private const string DefaultDecimalFormat = "#,0.##";

        public string[] ReportTables => new string[] { "AbProgram", "PaProgram", "HiProgram", "HcProgram", "MfProgram", "MrProgram" };

        public Dictionary<string, Guid> SurveyIdentifiers => new Dictionary<string, Guid>
        {
            { "AbProgram", Guid.Parse("3A1DD3CB-2822-4719-A0EF-AEA9016722F8") },
            { "HcProgram", Guid.Parse("BF49BE88-47F2-4AC5-9B39-AEAC012695AF") },
            { "HiProgram", Guid.Parse("2C79BD6F-C610-444D-B7B1-AEAC00FB7C2B") },
            { "MfProgram", Guid.Parse("C359AD01-89F9-4CF8-8CE8-AEAD01048D59") },
            { "MrProgram", Guid.Parse("F04BDD76-9DD0-4D8E-92D2-AEAD00F49085") },
            { "PaProgram", Guid.Parse("A503E8FF-52CF-4AAB-8B5C-AEAD011E420D") }
        };

        #endregion

        #region Migration Functions

        public QGroup[] GetAgencies()
        {
            using (var db = new InternalDbContext())
            {
                return db.QGroups
                    .Where(x => x.OrganizationIdentifier == OrganizationIdentifiers.NCSHA)
                    .OrderBy(x => x.GroupName)
                    .ToArray();
            }
        }

        public void RefreshReportTables(string[] reportTables, SurveyMigration surveyMigration, string period)
        {
            var agencies = GetAgencies();
            foreach (var agency in agencies)
                foreach (string reportTable in reportTables)
                    RefreshReportTables(surveyMigration, reportTable, agency.GroupName, period, agency.GroupIdentifier);
        }

        private void RefreshReportTables(SurveyMigration surveyMigration, string reportTable, string groupName, string periodName, Guid groupIdentifier)
        {
            if (surveyMigration.Errors.Count > 0)
                return;

            var filter = new QResponseSessionFilter { SurveyFormIdentifier = SurveyIdentifiers[reportTable], AgencyGroupIdentifier = groupIdentifier };

            var responses = new SurveySearch(null).GetResponseSessions(filter);
            if (responses.Count > 0)
                foreach (var response in responses)
                {
                    if (reportTable == "HcProgram")
                        InsertRow(surveyMigration, insertHCQueryTemplate, reportTable, groupName, periodName, response);
                    else
                        InsertRow(surveyMigration, insertQueryTemplate, reportTable, groupName, periodName, response);
                }
        }

        private bool RowExists(SqlConnection connection, string queryTemplate, Guid groupIdentifier, string reportTable, string periodName, Guid userIdentifier)
        {
            int count;

            string query = queryTemplate.Replace("$ReportTableName", reportTable);

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@AgencyGroupIdentifier", groupIdentifier);
                command.Parameters.AddWithValue("@PeriodName", periodName);
                command.Parameters.AddWithValue("@UserIdentifier", userIdentifier);
                count = (int)command.ExecuteScalar();
            }

            return count > 0;
        }

        private void DeleteRow(SqlConnection connection, string queryTemplate, Guid groupIdentifier, string reportTable, string periodName, Guid userIdentifier)
        {
            string query = queryTemplate.Replace("$ReportTableName", reportTable);

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@AgencyGroupIdentifier", groupIdentifier);
                command.Parameters.AddWithValue("@PeriodName", periodName);
                command.Parameters.AddWithValue("@UserIdentifier", userIdentifier);
                command.ExecuteNonQuery();
            }
        }


        private void ProcessAnswer(Answer answer, List<string> columnNames, List<string> fieldValues, SurveyMigration surveyMigration)
        {
            if (answer.SurveyQuestionType.Equals("RadioList"))
            {
                fieldValues.Add(answer.RadioListSelection);
                ProcessRadioListAnswer(answer, columnNames, fieldValues, surveyMigration);
                return;
            }

            var fieldValue = (answer.SurveyQuestionType.Equals("CheckList") || answer.SurveyQuestionType.Equals("Selection"))
                ? answer.RadioListSelection
                : answer.ResponseAnswer?.Replace("'", "''");

            if (fieldValue.HasValue() && fieldValue.StartsWith(","))
                fieldValue = fieldValue.Substring(1).Trim();

            fieldValues.Add(fieldValue);
        }

        private void ProcessRadioListAnswer(Answer answer, List<string> columnNames, List<string> fieldValues, SurveyMigration surveyMigration)
        {
            if (answer.ResponseAnswerText.HasNoValue() || answer.RadioListSelection.HasNoValue())
                return;

            var reportColumnOther = surveyMigration.Mappings.FirstOrDefault(x => x.ReportColumn == answer.ReportColumn)?.ReportColumnOther;
            if (reportColumnOther.HasValue())
            {
                columnNames.Add(reportColumnOther);
                fieldValues.Add(answer.ResponseAnswerText?.Replace("'", "''"));
            }
        }

        private void InsertRow(SurveyMigration surveyMigration, string queryTemplate, string reportTable, string groupName, string periodName, ISurveyResponse response)
        {
            string survey = $"the {periodName} response from {groupName}";
            string respondent = $"{response.RespondentName ?? "Someone"}";

            using (var connection = new SqlConnection(DbSettings.ConnectionString))
            {
                connection.Open();

                string query = queryTemplate.Replace("$ReportTableName", reportTable);

                var columnNames = new List<string>();
                var fieldValues = new List<string>();

                var answers = surveyMigration.Answers.Where(
                    x => x.ReportTable == reportTable &&
                    x.ResponseIdentifier == response.ResponseSessionIdentifier &&
                    x.SurveyQuestionType != "CheckList"
                    ).ToList();

                var tempCheclistAnswers = surveyMigration.Answers.Where(
                    x => x.ReportTable == reportTable &&
                    x.ResponseIdentifier == response.ResponseSessionIdentifier &&
                    x.SurveyQuestionType == "CheckList" &&
                    x.RadioListSelection.HasValue()
                    ).ToArray();

                var tempRadioAnswers = surveyMigration.Answers.Where(
                    x => x.ReportTable == reportTable &&
                    x.ResponseIdentifier == response.ResponseSessionIdentifier &&
                    x.SurveyQuestionType == "RadioList" &&
                    x.RadioListSelection.HasValue()
                    ).ToArray();

                var consolidatedCheclistAnswers =
                    tempCheclistAnswers
                        .GroupBy(c => new
                        {
                            c.SurveyIdentifier,
                            c.QuestionIdentifier,
                            c.ReportTable,
                            c.ReportColumn,
                            c.ResponseIdentifier,
                            c.SurveyQuestionType,
                            c.ResponsePeriod,
                            c.ResponseGroup
                        })
                        .Select(gcs => new Answer()
                        {
                            QuestionIdentifier = gcs.Key.QuestionIdentifier,
                            ResponseGroup = gcs.Key.ResponseGroup,
                            ResponsePeriod = gcs.Key.ResponsePeriod,
                            ReportColumn = gcs.Key.ReportColumn,
                            ReportTable = gcs.Key.ReportTable,
                            ResponseIdentifier = gcs.Key.ResponseIdentifier,
                            SurveyIdentifier = gcs.Key.SurveyIdentifier,
                            SurveyQuestionType = gcs.Key.SurveyQuestionType,
                            RadioListSelection =
                                string.Join(", ", gcs.Where(x => x.RadioListSelection != "Other (please specify)").Select(x => x.RadioListSelection).ToList())
                                + (
                                    gcs.Any(x => x.ResponseAnswerText.HasValue()) ?
                                        (", " + string.Join(", ", gcs.Select(x => x.ResponseAnswerText).Distinct().ToList()))
                                : "")
                        }).ToList();

                if (consolidatedCheclistAnswers.Any())
                    answers.AddRange(consolidatedCheclistAnswers);

                if (!answers.Any())
                    return;

                foreach (var answer in answers)
                {
                    columnNames.Add(answer.ReportColumn);
                    ProcessAnswer(answer, columnNames, fieldValues, surveyMigration);
                }

                if (response.GroupIdentifier.HasValue)
                {
                    var existsQuery = (reportTable == "HcProgram" ? rowExistsHcProgram : rowExists);
                    var deleteQuery = (reportTable == "HcProgram" ? deleteRowHcProgram : deleteRow);

                    if (RowExists(connection, existsQuery, response.GroupIdentifier.Value, reportTable, periodName, response.RespondentUserIdentifier))
                        DeleteRow(connection, deleteQuery, response.GroupIdentifier.Value, reportTable, periodName, response.RespondentUserIdentifier);
                }

                ProcessCumulativeColumnAnswers(answers, reportTable, columnNames, fieldValues, response, int.Parse(periodName));

                query = query.Replace("$ColumnNames", GenerateRowString(columnNames, null));
                query = query.Replace("$FieldValues", GenerateRowString(surveyMigration, survey, respondent, response.ResponseSessionIdentifier, columnNames, fieldValues, "'"));

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@AgencyGroupIdentifier", response.GroupIdentifier);
                    command.Parameters.AddWithValue("@PeriodName", periodName);
                    command.Parameters.AddWithValue("@OrganizationIdentifier", OrganizationIdentifiers.NCSHA);

                    command.Parameters.AddWithValue("@LastChanged", response.LastChangeTime);
                    command.Parameters.AddWithValue("@UpdatedOn", DateTimeOffset.UtcNow);
                    command.Parameters.AddWithValue("@ResponseSessionCompleted", response.ResponseSessionCompleted ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ResponseSessionCreated", response.ResponseSessionCreated ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@RespondentIdentifier", response.RespondentUserIdentifier);

                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        private string GenerateRowString(List<string> list, string quotation)
        {
            const string dbNull = "NULL";
            const string dbComma = ", ";

            StringBuilder csv = new StringBuilder();

            for (var i = 0; i < list.Count; i++)
            {
                var item = list[i];

                if (string.IsNullOrEmpty(item))
                    csv.Append(dbNull);

                else
                {
                    if (quotation != null)
                        csv.Append(quotation);

                    csv.Append(item);

                    if (quotation != null)
                        csv.Append(quotation);
                }

                if (i < list.Count - 1)
                    csv.Append(dbComma);
            }

            return csv.ToString();
        }

        private string GenerateRowString(SurveyMigration migration, string survey, string respondent, Guid response, List<string> names, List<string> values, string quotation)
        {
            const string dbNull = "NULL";
            const string dbComma = ", ";

            StringBuilder csv = new StringBuilder();

            for (var i = 0; i < values.Count; i++)
            {
                var value = values[i];


                if (string.IsNullOrEmpty(value))
                    csv.Append(dbNull);

                else
                {
                    if (quotation != null)
                        csv.Append(quotation);

                    var name = names[i];
                    var maximumLength = GetMaximumLength(name);

                    if (maximumLength > 0)
                    {
                        var length = value.Length;

                        if (length > maximumLength)
                        {
                            var truncatedValue = value.Substring(0, maximumLength);

                            csv.Append(truncatedValue);

                            var question = migration.Mappings.FirstOrDefault(x => x.ReportColumn == name)?.QuestionText;

                            Warnings.Add($"The field value for {name} (\"{question}\") was truncated from {length} characters to {maximumLength} characters on {survey} submitted by {respondent}. You may want to edit this answer to ensure data isn’t lost. <a target='_blank' href='/ui/admin/workflow/forms/submissions/outline?session={response}'>View the response</a>.");
                        }
                        else
                        {
                            csv.Append(value);
                        }
                    }
                    else
                    {
                        csv.Append(value);
                    }

                    if (quotation != null)
                        csv.Append(quotation);
                }

                if (i < values.Count - 1)
                    csv.Append(dbComma);
            }

            return csv.ToString();
        }

        public List<string> Warnings = new List<string>();

        private Dictionary<string, int> ColumnSizes = new Dictionary<string, int>();

        private void GetColumnSizes()
        {
            var table = DatabaseHelper.CreateDataTable(@"
select ColumnName
     , MaximumLength
from databases.VTableColumn
where SchemaName = 'custom_ncsha'
      and len(ColumnName) = 5
      and (
            ColumnName like 'AB%'
            or ColumnName like 'HC%'
            or ColumnName like 'HI%'
            or ColumnName like 'MF%'
            or ColumnName like 'MR%'
            or ColumnName like 'PA%'
          )
      and MaximumLength > 0
");
            foreach (System.Data.DataRow row in table.Rows)
                ColumnSizes.Add(row["ColumnName"].ToString(), (int)row["MaximumLength"]);
        }

        private int GetMaximumLength(string name)
        {
            if (ColumnSizes.IsEmpty())
                GetColumnSizes();

            if (ColumnSizes.ContainsKey(name))
                return ColumnSizes[name];

            return 0;
        }

        public static List<Answer> GetAnswers()
        {
            string query = $@"
	SELECT R.SurveyFormIdentifier AS SurveyIdentifier
     , A.SurveyQuestionIdentifier AS QuestionIdentifier
     , M.ReportTable
     , M.ReportColumn
     , A.ResponseSessionIdentifier AS ResponseIdentifier
     , A.RespondentUserIdentifier AS RespondentIdentifier
     , A.ResponseAnswerText AS ResponseAnswer
     , R.GroupIdentifier AS ResponseGroup
     , R.PeriodIdentifier AS ResponsePeriod
	 , q.[SurveyQuestionType]
	 , case
           when A.ResponseAnswerText = '' then
               null
           else
               A.ResponseAnswerText
       end                           as ResponseAnswerText
     , 
     OptionItemContent.ContentText AS RadioListSelection

FROM surveys.QResponseAnswer AS A
     INNER JOIN surveys.QResponseSession AS R ON R.ResponseSessionIdentifier = A.ResponseSessionIdentifier AND R.OrganizationIdentifier = '{OrganizationIdentifiers.NCSHA}'
     INNER JOIN custom_ncsha.TReportMapping AS M ON M.SurveyIdentifier = R.SurveyFormIdentifier
                                                    AND M.QuestionIdentifier = A.SurveyQuestionIdentifier
	INNER JOIN [records].[QPeriod] as P on R.PeriodIdentifier = P.PeriodIdentifier
	Inner Join [surveys].[QSurveyQuestion] as Q on Q.SurveyQuestionIdentifier = A.SurveyQuestionIdentifier
	     left join(surveys.QSurveyOptionList            as L
               inner join surveys.QSurveyOptionItem as OptionItem on OptionItem.SurveyOptionListIdentifier = L.SurveyOptionListIdentifier
               inner join surveys.QResponseOption   as ResponseOptionItem on ResponseOptionItem.SurveyOptionIdentifier = OptionItem.SurveyOptionItemIdentifier
                                                                             and ResponseOptionItem.ResponseOptionIsSelected = 1
               inner join contents.TContent         as OptionItemContent on OptionItem.SurveyOptionItemIdentifier = OptionItemContent.ContainerIdentifier)on L.SurveyQuestionIdentifier = A.SurveyQuestionIdentifier
                                                                                                                                                             and ResponseOptionItem.ResponseSessionIdentifier = A.ResponseSessionIdentifier
";

            using (var context = new InternalDbContext())
            {
                context.Database.CommandTimeout = 60 * 5;
                return context.Database.SqlQuery<Answer>(query).ToList();
            }
        }

        public static List<Answer> GetAnswers(int year)
        {
            string query = $@"
	SELECT R.SurveyFormIdentifier AS SurveyIdentifier
     , A.SurveyQuestionIdentifier AS QuestionIdentifier
     , M.ReportTable
     , M.ReportColumn
     , A.ResponseSessionIdentifier AS ResponseIdentifier
     , R.RespondentUserIdentifier AS RespondentIdentifier
     , A.ResponseAnswerText AS ResponseAnswer
     , R.GroupIdentifier AS ResponseGroup
     , R.PeriodIdentifier AS ResponsePeriod
	 , q.[SurveyQuestionType]
	 , case
           when A.ResponseAnswerText = '' then
               null
           else
               A.ResponseAnswerText
       end                           as ResponseAnswerText
     , 
     OptionItemContent.ContentText AS RadioListSelection

FROM surveys.QResponseAnswer AS A
     INNER JOIN surveys.QResponseSession AS R ON R.ResponseSessionIdentifier = A.ResponseSessionIdentifier AND R.OrganizationIdentifier = '{OrganizationIdentifiers.NCSHA}'
     INNER JOIN custom_ncsha.TReportMapping AS M ON M.SurveyIdentifier = R.SurveyFormIdentifier
                                                    AND M.QuestionIdentifier = A.SurveyQuestionIdentifier
	INNER JOIN [records].[QPeriod] as P on R.PeriodIdentifier = P.PeriodIdentifier
	Inner Join [surveys].[QSurveyQuestion] as Q on Q.SurveyQuestionIdentifier = A.SurveyQuestionIdentifier
	     left join(surveys.QSurveyOptionList            as L
               inner join surveys.QSurveyOptionItem as OptionItem on OptionItem.SurveyOptionListIdentifier = L.SurveyOptionListIdentifier
               inner join surveys.QResponseOption   as ResponseOptionItem on ResponseOptionItem.SurveyOptionIdentifier = OptionItem.SurveyOptionItemIdentifier
                                                                             and ResponseOptionItem.ResponseOptionIsSelected = 1
               inner join contents.TContent         as OptionItemContent on OptionItem.SurveyOptionItemIdentifier = OptionItemContent.ContainerIdentifier)on L.SurveyQuestionIdentifier = A.SurveyQuestionIdentifier
                                                                                                                                                             and ResponseOptionItem.ResponseSessionIdentifier = A.ResponseSessionIdentifier
WHERE
	P.PeriodName = '{year}'
";

            using (var context = new InternalDbContext())
            {
                context.Database.CommandTimeout = 60 * 5;
                return context.Database.SqlQuery<Answer>(query).ToList();
            }
        }

        public static List<Mapping> GetMappings()
        {
            const string query = @"
select M.*
     , S.SurveyFormName as SurveyName
     , C.ContentText    as QuestionText
from custom_ncsha.TReportMapping        as M
     inner join surveys.QSurveyQuestion as Q on M.QuestionIdentifier = Q.SurveyQuestionIdentifier
     left join surveys.QSurveyForm      as S on S.SurveyFormIdentifier = Q.SurveyFormIdentifier
     left join contents.TContent        as C on Q.SurveyQuestionIdentifier = C.ContainerIdentifier
                                                and C.ContentLabel = 'Title'
ORDER BY ReportTable, ReportColumn";

            using (var context = new InternalDbContext())
            {
                return context.Database.SqlQuery<Mapping>(query).ToList();
            }
        }

        public static SearchResultList SelectAll(Guid agency)
        {
            using (var db = new InternalDbContext())
            {
                return db.Programs
                    .Where(x => x.AgencyGroupIdentifier == agency)
                    .OrderByDescending(x => x.ProgramYear)
                    .ThenBy(x => x.ProgramName)
                    .Select(x => new
                    {
                        x.AgencyGroupIdentifier,
                        x.ProgramYear,
                        x.StateName,
                        x.ProgramId,
                        x.ProgramName,
                        x.ProgramCode
                    })
                    .ToSearchResult();
            }
        }

        #endregion

        public static List<int> SubmissionReportYears()
        {
            return Enumerable
                .Range(2022, DateTime.Now.Year - 2022)
                .OrderByDescending(x => x)
                .ToList();
        }

        public static IReadOnlyList<NchsaFieldUsage> SelectInputValues(string fieldName)
        {
            var prefix = fieldName.Length >= 2 ? fieldName.Substring(0, 2) : fieldName;

            using (var db = new InternalDbContext())
            {
                var validQuery = $@"
SELECT
    CAST( 
        CASE WHEN EXISTS(SELECT TOP 1 1 FROM sys.all_columns WHERE object_id = OBJECT_ID('custom_ncsha.{prefix}Program') AND name = '{fieldName}')
                THEN 1
             ELSE 0
        END
    AS bit)";

                var isValid = db.Database.SqlQuery<bool>(validQuery).Single();
                if (!isValid)
                    return new NchsaFieldUsage[0];

                var selectQuery = $@"
SELECT
    Programs.{prefix}ProgramID AS ProgramID
   ,Programs.SurveyYear AS ProgramYear
   ,Agencies.GroupName AS AgencyName
   ,Programs.{fieldName} AS [InputValue]
   ,'{prefix}' AS Prefix
FROM
    custom_ncsha.{prefix}Program AS Programs
    INNER JOIN contacts.QGroup AS Agencies ON Programs.AgencyGroupIdentifier = Agencies.GroupIdentifier
WHERE
    Programs.{fieldName} IS NOT NULL
ORDER BY
    Programs.SurveyYear DESC, Programs.{fieldName};";

                return db.Database.SqlQuery<NchsaFieldUsage>(selectQuery).ToList();
            }
        }

        #region Cumulative Columns Calculations

        private void ProcessCumulativeColumnAnswers(List<Answer> answers, string reportTable, List<string> columnNames, List<string> fieldValues, ISurveyResponse response, int surveyYear)
        {
            switch (reportTable)
            {
                case "HcProgram":
                    ProcessHcProgramCumulativeColumnAnswers(answers, columnNames, fieldValues, response, surveyYear);
                    break;
                case "AbProgram":
                    ProcessAbProgramCumulativeColumnAnswers(answers, columnNames, fieldValues, response, surveyYear);
                    break;
                case "HiProgram":
                    ProcessHiProgramCumulativeColumnAnswers(answers, columnNames, fieldValues, response, surveyYear);
                    break;
                case "MfProgram":
                    ProcessMfProgramCumulativeColumnAnswers(answers, columnNames, fieldValues, response, surveyYear);
                    break;
                case "MrProgram":
                    ProcessMrProgramCumulativeColumnAnswers(answers, columnNames, fieldValues, response, surveyYear);
                    break;
                case "PaProgram":
                    ProcessPaProgramCumulativeColumnAnswers(answers, columnNames, fieldValues, response, surveyYear);
                    break;
            }
        }

        private void ProcessPaProgramCumulativeColumnAnswers(List<Answer> answers, List<string> columnNames, List<string> fieldValues, ISurveyResponse response, int surveyYear)
        {
            var PA044 = answers.Where(x => x.SurveyQuestionType == "Number" && (x.ReportColumn == "PA007" || x.ReportColumn == "PA008" || x.ReportColumn == "PA009" || x.ReportColumn == "PA035") && x.ResponseAnswer.HasValue())
                .Select(x => double.Parse(x.ResponseAnswer, System.Globalization.CultureInfo.InvariantCulture)).AsQueryable().Sum();
            var PA045 = answers.Where(x => x.SurveyQuestionType == "Number" && (x.ReportColumn == "PA012" || x.ReportColumn == "PA041") && x.ResponseAnswer.HasValue())
                .Select(x => double.Parse(x.ResponseAnswer, System.Globalization.CultureInfo.InvariantCulture)).AsQueryable().Sum();
            var PA046 = answers.Where(x => x.SurveyQuestionType == "Number" && (x.ReportColumn == "PA013" || x.ReportColumn == "PA042") && x.ResponseAnswer.HasValue())
                .Select(x => double.Parse(x.ResponseAnswer, System.Globalization.CultureInfo.InvariantCulture)).AsQueryable().Sum();
            var PA047 = answers.Where(x => x.SurveyQuestionType == "Number" && (x.ReportColumn == "PA017" || x.ReportColumn == "PA036") && x.ResponseAnswer.HasValue())
                .Select(x => double.Parse(x.ResponseAnswer, System.Globalization.CultureInfo.InvariantCulture)).AsQueryable().Sum();
            var PA048 = answers.Where(x => x.SurveyQuestionType == "Number" && (x.ReportColumn == "PA018" || x.ReportColumn == "PA037") && x.ResponseAnswer.HasValue())
                .Select(x => double.Parse(x.ResponseAnswer, System.Globalization.CultureInfo.InvariantCulture)).AsQueryable().Sum();
            var PA049 = answers.Where(x => x.SurveyQuestionType == "Number" && (x.ReportColumn == "PA019" || x.ReportColumn == "PA038") && x.ResponseAnswer.HasValue())
                .Select(x => double.Parse(x.ResponseAnswer, System.Globalization.CultureInfo.InvariantCulture)).AsQueryable().Sum();
            var PA050 = answers.Where(x => x.SurveyQuestionType == "Number" && (x.ReportColumn == "PA020" || x.ReportColumn == "PA039") && x.ResponseAnswer.HasValue())
                .Select(x => double.Parse(x.ResponseAnswer, System.Globalization.CultureInfo.InvariantCulture)).AsQueryable().Sum();
            var PA051 = answers.Where(x => x.SurveyQuestionType == "Number" && (x.ReportColumn == "PA021" || x.ReportColumn == "PA041") && x.ResponseAnswer.HasValue())
                .Select(x => double.Parse(x.ResponseAnswer, System.Globalization.CultureInfo.InvariantCulture)).AsQueryable().Sum();

            columnNames.Add("PA044");
            columnNames.Add("PA045");
            columnNames.Add("PA046");
            columnNames.Add("PA047");
            columnNames.Add("PA048");
            columnNames.Add("PA049");
            columnNames.Add("PA050");
            columnNames.Add("PA051");

            fieldValues.Add(PA044.ToString());
            fieldValues.Add(PA045.ToString());
            fieldValues.Add(PA046.ToString());
            fieldValues.Add(PA047.ToString());
            fieldValues.Add(PA048.ToString());
            fieldValues.Add(PA049.ToString());
            fieldValues.Add(PA050.ToString());
            fieldValues.Add(PA051.ToString());
        }

        private void ProcessMrProgramCumulativeColumnAnswers(List<Answer> answers, List<string> columnNames, List<string> fieldValues, ISurveyResponse response, int surveyYear)
        {
            var MR164 = answers.Where(x => x.SurveyQuestionType == "Number" && (x.ReportColumn == "MR088" || x.ReportColumn == "MR107" || x.ReportColumn == "MR113" || x.ReportColumn == "MR171") && x.ResponseAnswer.HasValue())
                .Select(x => double.Parse(x.ResponseAnswer, System.Globalization.CultureInfo.InvariantCulture)).AsQueryable().Sum();
            var MR165 = answers.Where(x => x.SurveyQuestionType == "Number" && (x.ReportColumn == "MR089" || x.ReportColumn == "MR108" || x.ReportColumn == "MR114") && x.ResponseAnswer.HasValue())
                .Select(x => double.Parse(x.ResponseAnswer, System.Globalization.CultureInfo.InvariantCulture)).AsQueryable().Sum();
            var MR166 = answers.Where(x => x.SurveyQuestionType == "Number" && (x.ReportColumn == "MR090" || x.ReportColumn == "MR109" || x.ReportColumn == "MR115") && x.ResponseAnswer.HasValue())
                .Select(x => double.Parse(x.ResponseAnswer, System.Globalization.CultureInfo.InvariantCulture)).AsQueryable().Sum();
            var MR167 = answers.Where(x => x.SurveyQuestionType == "Number" && (x.ReportColumn == "MR091" || x.ReportColumn == "MR110" || x.ReportColumn == "MR116") && x.ResponseAnswer.HasValue())
                .Select(x => double.Parse(x.ResponseAnswer, System.Globalization.CultureInfo.InvariantCulture)).AsQueryable().Sum();
            var MR168 = answers.Where(x => x.SurveyQuestionType == "Number" && (x.ReportColumn == "MR092" || x.ReportColumn == "MR111" || x.ReportColumn == "MR117") && x.ResponseAnswer.HasValue())
                .Select(x => double.Parse(x.ResponseAnswer, System.Globalization.CultureInfo.InvariantCulture)).AsQueryable().Sum();
            var MR169 = Multiply(Divide(4,
                (answers.Where(x => x.SurveyQuestionType == "Number" && (x.ReportColumn == "MR010" || x.ReportColumn == "MR086") && x.ResponseAnswer.HasValue())
                .Select(x => double.Parse(x.ResponseAnswer, System.Globalization.CultureInfo.InvariantCulture)).AsQueryable().Sum().ToString())
                , answers.FirstOrDefault(x => x.ReportColumn == "MR011")?.ResponseAnswer), "100");

            columnNames.Add("MR164");
            columnNames.Add("MR165");
            columnNames.Add("MR166");
            columnNames.Add("MR167");
            columnNames.Add("MR168");
            columnNames.Add("MR169");

            fieldValues.Add(MR164.ToString());
            fieldValues.Add(MR165.ToString());
            fieldValues.Add(MR166.ToString());
            fieldValues.Add(MR167.ToString());
            fieldValues.Add(MR168.ToString());
            fieldValues.Add(MR169.ToString());

            var previous = MrProgramRepository.SelectFirst(x => x.AgencyGroupIdentifier == response.GroupIdentifier && x.SurveyYear == surveyYear - 1);
            if (previous != null)
            {
                var MR082 = Add(previous.MR082, answers.FirstOrDefault(x => x.ReportColumn == "MR007")?.ResponseAnswer);
                var MR083 = Add(previous.MR083, answers.FirstOrDefault(x => x.ReportColumn == "MR011")?.ResponseAnswer);
                var MR084 = Add(previous.MR084, answers.FirstOrDefault(x => x.ReportColumn == "MR050")?.ResponseAnswer);

                columnNames.Add("MR082");
                columnNames.Add("MR083");
                columnNames.Add("MR084");

                fieldValues.Add(MR082?.ToString() ?? "");
                fieldValues.Add(MR083?.ToString() ?? "");
                fieldValues.Add(MR084?.ToString() ?? "");
            }
        }

        private void ProcessMfProgramCumulativeColumnAnswers(List<Answer> answers, List<string> columnNames, List<string> fieldValues, ISurveyResponse response, int surveyYear)
        {
            var previous = MfProgramRepository.SelectFirst(x => x.AgencyGroupIdentifier == response.GroupIdentifier && x.SurveyYear == surveyYear - 1);
            if (previous != null)
            {
                var MF027 = Add(previous.MF027, answers.FirstOrDefault(x => x.ReportColumn == "MF023")?.ResponseAnswer);
                var MF028 = Add(previous.MF028, answers.FirstOrDefault(x => x.ReportColumn == "MF024")?.ResponseAnswer);
                var MF079 = Add(previous.MF079, answers.FirstOrDefault(x => x.ReportColumn == "MF076")?.ResponseAnswer);
                var MF080 = Add(previous.MF080, answers.FirstOrDefault(x => x.ReportColumn == "MF077")?.ResponseAnswer);
                var MF081 = Add(previous.MF081, answers.FirstOrDefault(x => x.ReportColumn == "MF078")?.ResponseAnswer);
                var MF206 = Add(previous.MF206, answers.FirstOrDefault(x => x.ReportColumn == "MF037")?.ResponseAnswer);
                var MF207 = Add(previous.MF207, answers.FirstOrDefault(x => x.ReportColumn == "MF038")?.ResponseAnswer);

                columnNames.Add("MF027");
                columnNames.Add("MF028");
                columnNames.Add("MF079");
                columnNames.Add("MF080");
                columnNames.Add("MF081");
                columnNames.Add("MF206");
                columnNames.Add("MF207");

                fieldValues.Add(MF027?.ToString() ?? "");
                fieldValues.Add(MF028?.ToString() ?? "");
                fieldValues.Add(MF079?.ToString() ?? "");
                fieldValues.Add(MF080?.ToString() ?? "");
                fieldValues.Add(MF081?.ToString() ?? "");
                fieldValues.Add(MF206?.ToString() ?? "");
                fieldValues.Add(MF207?.ToString() ?? "");
            }
        }

        private void ProcessHiProgramCumulativeColumnAnswers(List<Answer> answers, List<string> columnNames, List<string> fieldValues, ISurveyResponse response, int surveyYear)
        {
            var HI209 = answers.Where(x => x.SurveyQuestionType == "Number" && (x.ReportColumn == "HI012" || x.ReportColumn == "HI013" || x.ReportColumn == "HI014") && x.ResponseAnswer.HasValue())
                .Select(x => double.Parse(x.ResponseAnswer, System.Globalization.CultureInfo.InvariantCulture)).AsQueryable().Sum();
            var HI210 = answers.Where(x => x.SurveyQuestionType == "Number" && (x.ReportColumn == "HI192" || x.ReportColumn == "HI193" || x.ReportColumn == "HI194") && x.ResponseAnswer.HasValue())
                .Select(x => double.Parse(x.ResponseAnswer, System.Globalization.CultureInfo.InvariantCulture)).AsQueryable().Sum();
            var HI211 = answers.Where(x => x.SurveyQuestionType == "Number" && (x.ReportColumn == "HI132" || x.ReportColumn == "HI134" || x.ReportColumn == "HI136") && x.ResponseAnswer.HasValue())
                .Select(x => double.Parse(x.ResponseAnswer, System.Globalization.CultureInfo.InvariantCulture)).AsQueryable().Sum();


            columnNames.Add("HI209");
            columnNames.Add("HI210");
            columnNames.Add("HI211");

            fieldValues.Add(HI209.ToString());
            fieldValues.Add(HI210.ToString());
            fieldValues.Add(HI211.ToString());

            var previous = HiProgramRepository.SelectFirst(x => x.AgencyGroupIdentifier == response.GroupIdentifier && x.SurveyYear == surveyYear - 1);
            if (previous != null)
            {
                var HI192 = Add(previous.HI192, answers.FirstOrDefault(x => x.ReportColumn == "HI012")?.ResponseAnswer);
                var HI193 = Add(previous.HI193, answers.FirstOrDefault(x => x.ReportColumn == "HI013")?.ResponseAnswer);
                var HI194 = Add(previous.HI194, answers.FirstOrDefault(x => x.ReportColumn == "HI014")?.ResponseAnswer);

                columnNames.Add("HI192");
                columnNames.Add("HI193");
                columnNames.Add("HI194");

                fieldValues.Add(HI192?.ToString() ?? "");
                fieldValues.Add(HI193?.ToString() ?? "");
                fieldValues.Add(HI194?.ToString() ?? "");
            }
        }

        private void ProcessAbProgramCumulativeColumnAnswers(List<Answer> answers, List<string> columnNames, List<string> fieldValues, ISurveyResponse response, int surveyYear)
        {
            var AB172 = answers.Where(x => x.SurveyQuestionType == "Number" && (x.ReportColumn == "AB037" || x.ReportColumn == "AB038" || x.ReportColumn == "AB039" || x.ReportColumn == "AB040") && x.ResponseAnswer.HasValue())
                .Select(x => double.Parse(x.ResponseAnswer, System.Globalization.CultureInfo.InvariantCulture)).AsQueryable().Sum();

            columnNames.Add("AB172");

            fieldValues.Add(AB172.ToString());


            var previous = AbProgramRepository.SelectFirst(x => x.AgencyGroupIdentifier == response.GroupIdentifier && x.SurveyYear == surveyYear - 1);
            if (previous != null)
            {
                var AB146 = Add(previous.AB146, answers.FirstOrDefault(x => x.ReportColumn == "AB145")?.ResponseAnswer);
                var AB149 = Add(previous.AB149, answers.FirstOrDefault(x => x.ReportColumn == "AB148")?.ResponseAnswer);

                columnNames.Add("AB146");
                columnNames.Add("AB149");

                fieldValues.Add(AB146?.ToString() ?? "");
                fieldValues.Add(AB149?.ToString() ?? "");
            }
        }

        private void ProcessHcProgramCumulativeColumnAnswers(List<Answer> answers, List<string> columnNames, List<string> fieldValues, ISurveyResponse response, int surveyYear)
        {
            var HC314 = answers.Where(x => x.SurveyQuestionType == "Number" && (x.ReportColumn == "HC222" || x.ReportColumn == "HC223") && x.ResponseAnswer.HasValue())
                .Select(x => double.Parse(x.ResponseAnswer, System.Globalization.CultureInfo.InvariantCulture)).AsQueryable().Sum();
            var HC315 = answers.Where(x => x.SurveyQuestionType == "Number" && (x.ReportColumn == "HC296" || x.ReportColumn == "HC298") && x.ResponseAnswer.HasValue())
                .Select(x => double.Parse(x.ResponseAnswer, System.Globalization.CultureInfo.InvariantCulture)).AsQueryable().Sum();
            var HC316 = answers.Where(x => x.SurveyQuestionType == "Number" && (x.ReportColumn == "HC297" || x.ReportColumn == "HC299") && x.ResponseAnswer.HasValue())
                .Select(x => double.Parse(x.ResponseAnswer, System.Globalization.CultureInfo.InvariantCulture)).AsQueryable().Sum();

            columnNames.Add("HC314");
            columnNames.Add("HC315");
            columnNames.Add("HC316");

            fieldValues.Add(HC314.ToString());
            fieldValues.Add(HC315.ToString());
            fieldValues.Add(HC316.ToString());

            var previous = HcProgramRepository.SelectFirst(x => x.AgencyGroupIdentifier == response.GroupIdentifier && x.SurveyYear == surveyYear - 1);
            if (previous != null)
            {
                var HC034 = Add(previous.HC034, answers.FirstOrDefault(x => x.ReportColumn == "HC030")?.ResponseAnswer);
                var HC053 = Add(previous.HC053, answers.FirstOrDefault(x => x.ReportColumn == "HC050")?.ResponseAnswer);
                var HC187 = Add(previous.HC187, answers.FirstOrDefault(x => x.ReportColumn == "HC014")?.ResponseAnswer);
                var HC194 = Subtract(
                    Subtract(
                        Add(previous.HC194, answers.FirstOrDefault(x => x.ReportColumn == "HC031")?.ResponseAnswer),
                        answers.FirstOrDefault(x => x.ReportColumn == "HC222")?.ResponseAnswer),
                    answers.FirstOrDefault(x => x.ReportColumn == "HC340")?.ResponseAnswer
                    );

                columnNames.Add("HC034");
                columnNames.Add("HC053");
                columnNames.Add("HC187");
                columnNames.Add("HC194");

                fieldValues.Add(HC034?.ToString() ?? "");
                fieldValues.Add(HC053?.ToString() ?? "");
                fieldValues.Add(HC187?.ToString() ?? "");
                fieldValues.Add(HC194?.ToString() ?? "");
            }
        }

        #endregion

        #region Helper Methods

        public static string Add(params string[] values) => Calculate(values, DefaultDecimalFormat, (x, y) => x + y);

        public static string Subtract(params string[] values) => Calculate(values, DefaultDecimalFormat, (x, y) => x - y);

        private static string Calculate(string[] values, string format, Func<decimal, decimal, decimal> getResult)
        {
            var result = ValueConverter.DecimalNotKnown;

            for (var i = 0; i < values.Length; i++)
            {
                var num = ValueConverter.ToDecimalNullable(values[i]);

                if (!num.HasValue || num == ValueConverter.DecimalNotKnown
                    || result == num && (num == ValueConverter.DecimalNotAvailable || num == ValueConverter.DecimalNotApplicable)
                    || result == ValueConverter.DecimalNotApplicable && num == ValueConverter.DecimalNotAvailable)
                    continue;

                if (result == ValueConverter.DecimalNotKnown || result == ValueConverter.DecimalNotAvailable && num == ValueConverter.DecimalNotApplicable)
                {
                    result = num.Value;
                    continue;
                }

                if (num == ValueConverter.DecimalNotAvailable || num == ValueConverter.DecimalNotApplicable)
                    num = 0;

                result = result == ValueConverter.DecimalNotAvailable || result == ValueConverter.DecimalNotApplicable
                    ? num.Value
                    : getResult(result, num.Value);
            }

            if (result == ValueConverter.DecimalNotKnown)
                return null;

            if (result == ValueConverter.DecimalNotAvailable)
                return "N/AV";

            if (result == ValueConverter.DecimalNotApplicable)
                return "N/AP";

            return result.ToString(format, Cultures.Default);
        }

        public static string Multiply(params string[] values) => Multiply(2, values);

        public static string Multiply(int decimalPlaces, params string[] values)
        {
            var format = "#,0";
            if (decimalPlaces > 0)
                format += "." + new string('#', decimalPlaces);

            return Calculate(values, format, (x, y) => x * y);
        }

        public static string Divide(params string[] values) => Divide(2, values);

        public static string Divide(int decimalPlaces, params string[] values)
        {
            var format = "#,0";
            if (decimalPlaces > 0)
                format += "." + new string('#', decimalPlaces);

            return Calculate(values, format, (x, y) => y == 0 ? 0 : x / y);
        }

        #endregion
    }
}
