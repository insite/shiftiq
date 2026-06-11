using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Banks.Read;
using InSite.Application.Contents.Read;
using InSite.Application.Logs.Read;
using InSite.Domain.Banks;
using InSite.Persistence.Foundation;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Persistence
{
    public class BankSearch : IBankSearch
    {
        #region Constants

        private static readonly string[] QuestionChangedRangeTypes = new[]
        {
            nameof(QuestionContentChanged),
            nameof(QuestionScoringChanged),
            nameof(OptionChanged)
        };

        #endregion

        private readonly IAggregateSearch _aggregateSearch;

        public BankSearch(IAggregateSearch aggregateSearch)
        {
            _aggregateSearch = aggregateSearch;
        }

        internal InternalDbContext CreateContext() => new InternalDbContext(false);

        #region Banks

        public int CountBanks(QBankFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .Count();
            }
        }

        public List<Counter> CountBanksByType(QBankFilter filter)
        {
            var list = new List<Counter>();

            var appointments = GetBanks(filter);

            var types = appointments.Select(x => x.BankType).OrderBy(x => x).Distinct();

            foreach (var type in types)
            {
                var item = new Counter
                {
                    Name = type,
                    Value = appointments.Count(x => x.BankType == type)
                };
                list.Add(item);
            }

            return list;
        }

        public BankState GetBankState(Guid bank)
        {
            return _aggregateSearch.GetState<BankAggregate>(bank) as BankState;
        }

        public BankState[] GetBankStates(IEnumerable<Guid> banks)
        {
            var list = new List<BankState>();
            foreach (var bank in banks)
            {
                var item = GetBankState(bank);
                if (item != null)
                    list.Add(item);
            }
            return list.ToArray();
        }

        public QBank GetBank(Guid id)
        {
            using (var db = CreateContext())
            {
                return db.Banks
                    .AsNoTracking()
                    .FirstOrDefault(x => x.BankIdentifier == id);
            }
        }

        public QBank[] GetBanks(IEnumerable<Guid> id)
        {
            if (id == null || !id.Any())
                return new QBank[0];

            using (var db = CreateContext())
            {
                return db.Banks
                    .AsNoTracking()
                    .Where(x => id.Contains(x.BankIdentifier))
                    .ToArray();
            }
        }

        public List<QBank> GetBanks(QBankFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .OrderBy(x => x.BankTitle)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        public List<string> GetBankLevels(QBankFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .Select(x => x.BankLevel)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
            }
        }

        public MostRecentChange[] GetMostRecentlyChangedBanks(Guid organization, int count, string additionalWhere = null)
        {
            if (!string.IsNullOrEmpty(additionalWhere))
                additionalWhere = " and " + additionalWhere;

            var select = $@"
SELECT TOP ({count})
       b.BankIdentifier
     , b.BankName
     , b.LastChangeTime
     , b.LastChangeType
     , b.LastChangeUser
FROM
    banks.QBank AS b
WHERE
    b.OrganizationIdentifier = @Organization{additionalWhere}
ORDER BY
    b.LastChangeTime DESC
";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Organization", organization)
            };

            using (var db = new InternalDbContext())
            {
                return db.Database
                    .SqlQuery<MostRecentChange>(select, parameters)
                    .ToArray();
            }
        }

        private IQueryable<QBank> CreateQuery(QBankFilter filter, InternalDbContext db)
        {
            var query = db.Banks
                .AsNoTracking()
                .AsQueryable();

            if (filter == null)
                return query;

            if (filter.AssetNumber.HasValue)
                query = query.Where(x => x.AssetNumber == filter.AssetNumber);

            if (filter.BankEnable.HasValue)
                query = query.Where(x => x.IsActive == filter.BankEnable);

            if (filter.BankName.IsNotEmpty())
                query = query.Where(x => x.BankName.Contains(filter.BankName));

            if (filter.BankTitle.IsNotEmpty())
                query = query.Where(x => x.BankTitle.Contains(filter.BankTitle));

            if (filter.BankType.IsNotEmpty())
                query = query.Where(x => x.BankType == filter.BankType);

            if (filter.BankStatus.IsNotEmpty())
                query = query.Where(x => x.BankStatus == filter.BankStatus);

            if (filter.OrganizationIdentifiers.IsNotEmpty())
                query = query.Where(x => filter.OrganizationIdentifiers.Contains(x.OrganizationIdentifier));

            if (filter.OccupationIdentifier.HasValue)
                query = query.Where(x => db.VBanks.Any(y => y.BankIdentifier == x.BankIdentifier && y.OccupationIdentifier == filter.OccupationIdentifier));

            if (filter.FrameworkIdentifiers.IsNotEmpty())
                query = query.Where(x => x.FrameworkIdentifier != null && filter.FrameworkIdentifiers.ToList().Contains(x.FrameworkIdentifier.Value));

            if (filter.AttemptCandidateOrganizationIdentifiers.IsNotEmpty())
                query = query.Where(x => x.Forms.SelectMany(y => y.Attempts.Select(z => z.LearnerPerson)).Any(y => filter.AttemptCandidateOrganizationIdentifiers.Contains(y.OrganizationIdentifier)));

            if (filter.DepartmentIdentifier.HasValue)
                query = query.Where(x => x.DepartmentIdentifier == filter.DepartmentIdentifier.Value);

            if (filter.Keyword.IsNotEmpty())
            {
                var separatorIndex = filter.Keyword.IndexOf(":");

                if (separatorIndex < 0)
                {
                    query = query.Where(x =>
                        SqlFunctions.StringConvert((double)x.AssetNumber).Trim() == filter.Keyword ||
                        x.BankName.Contains(filter.Keyword) ||
                        x.BankTitle.Contains(filter.Keyword));
                }
                else
                {
                    var number = filter.Keyword.Substring(0, separatorIndex);
                    var title = separatorIndex < filter.Keyword.Length - 2
                        ? filter.Keyword.Substring(separatorIndex + 2)
                        : null;

                    query = query.Where(x => SqlFunctions.StringConvert((double)x.AssetNumber).EndsWith(number));

                    if (title.IsNotEmpty())
                        query = query.Where(x => x.BankTitle.Contains(title) || x.BankName.Contains(title));
                }
            }

            return query;
        }

        public int CountBankOccupations(Guid organizationId, string searchText)
        {
            using (var db = CreateContext())
                return GetBankOccupationsQuery(db, organizationId, searchText).Count();
        }

        public BankSummaryOccupationInfo[] GetBankOccupations(Guid organizationId, Guid[] occupationIds)
        {
            using (var db = CreateContext())
            {
                return GetBankOccupationsQuery(db, organizationId, null)
                    .Where(x => occupationIds.Contains(x.OccupationID))
                    .OrderBy(x => x.OccupationTitle)
                    .ToArray();
            }
        }

        public BankSummaryOccupationInfo[] GetBankOccupations(Guid organizationId, Paging paging, string searchText)
        {
            using (var db = CreateContext())
            {
                return GetBankOccupationsQuery(db, organizationId, searchText)
                    .OrderBy(x => x.OccupationTitle)
                    .ApplyPaging(paging)
                    .ToArray();
            }
        }

        public BankSummaryOccupationInfo GetBankOccupation(Guid organizationId, Guid id)
        {
            using (var db = CreateContext())
                return GetBankOccupationsQuery(db, organizationId, null).Where(x => x.OccupationID == id).FirstOrDefault();
        }

        private static IQueryable<BankSummaryOccupationInfo> GetBankOccupationsQuery(InternalDbContext db, Guid organizationId, string searchText)
        {
            var query = db.VBanks.AsNoTracking().AsQueryable()
                .Where(x => x.OrganizationIdentifier == organizationId && x.OccupationIdentifier.HasValue && x.OccupationIdentifier != Guid.Empty)
                .Select(x => new BankSummaryOccupationInfo { OccupationID = x.OccupationIdentifier.Value, OccupationTitle = x.OccupationTitle });

            if (searchText.IsNotEmpty())
                query = query.Where(x => x.OccupationTitle.Contains(searchText));

            return query.Distinct();
        }

        public int CountBankFrameworks(Guid organizationId, Guid? occupationId, string searchText)
        {
            using (var db = CreateContext())
                return GetBankFrameworksQuery(db, organizationId, occupationId, searchText).Count();
        }

        public BankSummaryFrameworkInfo[] GetBankFrameworks(Guid organizationId, Guid? occupationId, Paging paging, string searchText)
        {
            using (var db = CreateContext())
            {
                return GetBankFrameworksQuery(db, organizationId, occupationId, searchText)
                    .OrderBy(x => x.FrameworkTitle)
                    .ApplyPaging(paging)
                    .ToArray();
            }
        }

        public BankSummaryFrameworkInfo[] GetBankFrameworks(Guid organizationId, Guid[] frameworkIds)
        {
            using (var db = CreateContext())
            {
                return GetBankFrameworksQuery(db, organizationId, null, null)
                    .Where(x => frameworkIds.Contains(x.FrameworkID))
                    .OrderBy(x => x.FrameworkTitle)
                    .ToArray();
            }
        }

        public BankSummaryFrameworkInfo GetBankFramework(Guid organizationId, Guid id)
        {
            using (var db = CreateContext())
                return GetBankFrameworksQuery(db, organizationId, null, null).Where(x => x.FrameworkID == id).FirstOrDefault();
        }

        private static IQueryable<BankSummaryFrameworkInfo> GetBankFrameworksQuery(InternalDbContext db, Guid organizationId, Guid? occupationId, string searchText)
        {
            var query = db.VBanks.AsNoTracking().AsQueryable()
                .Where(x => x.OrganizationIdentifier == organizationId && x.FrameworkIdentifier.HasValue && x.FrameworkIdentifier != Guid.Empty);

            if (occupationId.HasValue)
                query = query.Where(x => x.OccupationIdentifier == occupationId.Value);

            if (searchText.IsNotEmpty())
                query = query.Where(x => x.FrameworkTitle.Contains(searchText));

            return query
                .Select(x => new BankSummaryFrameworkInfo { FrameworkID = x.FrameworkIdentifier.Value, FrameworkTitle = x.FrameworkTitle })
                .Distinct();
        }

        public List<Guid> GetBanksWithDuplicateFormAsset()
        {
            using (var db = CreateContext())
            {
                return db.BankForms
                    .GroupBy(x => new { x.BankIdentifier, x.FormAsset })
                    .Where(x => x.Count() > 1)
                    .Select(x => x.Key.BankIdentifier)
                    .Distinct()
                    .ToList();
            }
        }

        #endregion

        #region Forms

        public Form GetFormData(Guid id)
        {
            var formQuery = GetForm(id);
            if (formQuery == null)
                return null;

            var cursor = GetBankState(formQuery.BankIdentifier);
            return cursor.FindForm(id);
        }

        public QBankForm GetForm(Guid id)
        {
            using (var db = CreateContext())
            {
                return db.BankForms
                    .AsNoTracking()
                    .Include(x => x.Bank)
                    .FirstOrDefault(x => x.FormIdentifier == id);
            }
        }

        public List<QBankForm> GetForms(IEnumerable<Guid> formIdentifiers, params Expression<Func<QBankForm, object>>[] includes)
        {
            if (!formIdentifiers.Any())
                return new List<QBankForm>();

            using (var db = CreateContext())
            {
                var query = db.BankForms.AsNoTracking().ApplyIncludes(includes);

                return query.Where(x => formIdentifiers.Contains(x.FormIdentifier)).ToList();
            }
        }

        public QBankForm[] GetForms(QBankFormFilter filter, params Expression<Func<QBankForm, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db).ApplyIncludes(includes);

                return query
                    .OrderBy(x => x.FormName)
                    .ThenBy(x => x.FormTitle)
                    .ApplyPaging(filter)
                    .ToArray();
            }
        }

        public QBankForm[] GetForms(QBankFormFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .OrderBy(x => x.FormName)
                    .ThenBy(x => x.FormTitle)
                    .ApplyPaging(filter)
                    .ToArray();
            }
        }

        public int CountForms(QBankFormFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .Count();
            }
        }

        private IQueryable<QBankForm> CreateQuery(QBankFormFilter filter, InternalDbContext db)
        {
            var query = db.BankForms
                .AsNoTracking()
                .AsQueryable();

            if (filter == null)
                return query;

            if (filter.AssetNumber.HasValue)
                query = query.Where(x => x.FormAsset == filter.AssetNumber);

            if (filter.FormCode.IsNotEmpty())
                query = query.Where(x => x.FormCode == filter.FormCode);

            if (filter.FormName.IsNotEmpty())
                query = query.Where(x => x.FormName.Contains(filter.FormName));

            if (filter.FormTitle.IsNotEmpty())
                query = query.Where(x => x.FormTitle.Contains(filter.FormTitle));

            if (filter.ExcludeFormStatus.IsNotEmpty())
                query = query.Where(x => x.FormPublicationStatus != filter.ExcludeFormStatus);

            if (filter.IncludeFormStatus.IsNotEmpty())
                query = query.Where(x => x.FormPublicationStatus == filter.IncludeFormStatus);

            if (filter.OrganizationIdentifiers.IsNotEmpty())
                query = query.Where(x => filter.OrganizationIdentifiers.Contains(x.OrganizationIdentifier));

            if (filter.OccupationIdentifier.HasValue)
                query = query.Where(x => db.VBanks.Any(y => y.BankIdentifier == x.BankIdentifier && y.OccupationIdentifier == filter.OccupationIdentifier));

            if (filter.FrameworkIdentifier.HasValue)
                query = query.Where(x => x.Bank.FrameworkIdentifier == filter.FrameworkIdentifier.Value);

            if (filter.FormIdentifiers.IsNotEmpty())
                query = query.Where(x => filter.FormIdentifiers.Contains(x.FormIdentifier));

            if (filter.GradeItemIdentifier.HasValue)
                query = query.Where(x => db.QActivities.Any(a => a.AssessmentFormIdentifier == x.FormIdentifier && a.GradeItemIdentifier == filter.GradeItemIdentifier.Value));

            if (filter.BankIdentifier.HasValue)
                query = query.Where(x => x.BankIdentifier == filter.BankIdentifier);

            if (filter.AttemptCandidateOrganizationIdentifiers.IsNotEmpty())
                query = query.Where(x => x.Attempts.Select(y => y.LearnerPerson).Any(y => filter.AttemptCandidateOrganizationIdentifiers.Contains(y.OrganizationIdentifier)));

            if (filter.EventIdentifier.HasValue)
                query = query.Where(x => x.EventAssessmentForms.Any(y => y.EventIdentifier == filter.EventIdentifier));

            if (filter.Keyword.IsNotEmpty())
            {
                var separatorIndex = filter.Keyword.IndexOf(":");

                if (separatorIndex < 0)
                {
                    query = query.Where(x =>
                        SqlFunctions.StringConvert((double)x.FormAsset).Trim() == filter.Keyword
                        || SqlFunctions.StringConvert((double)x.FormAsset).Trim() + "." +
                           SqlFunctions.StringConvert((double)x.FormAssetVersion).Trim()
                           == filter.Keyword
                        || x.FormName.Contains(filter.Keyword)
                        || x.FormTitle.Contains(filter.Keyword));
                }
                else
                {
                    var number = filter.Keyword.Substring(0, separatorIndex);
                    var title = separatorIndex < filter.Keyword.Length - 2
                        ? filter.Keyword.Substring(separatorIndex + 2)
                        : null;

                    query = query.Where(x => SqlFunctions.StringConvert((double)x.FormAsset).EndsWith(number));

                    if (title.IsNotEmpty())
                        query = query.Where(x => x.FormTitle.Contains(title) || x.FormName.Contains(title));
                }
            }

            if (filter.SpecIdentifier != null)
                query = query.Where(x => x.BankSpecification.SpecIdentifier == filter.SpecIdentifier);

            if (filter.SpecType.IsNotEmpty())
                query = query.Where(x => x.BankSpecification.SpecType == filter.SpecType);

            return query;
        }

        #endregion

        #region Questions

        public QBankQuestion GetQuestion(Guid id)
        {
            using (var db = CreateContext())
                return db.BankQuestions.AsNoTracking().Where(x => x.QuestionIdentifier == id).SingleOrDefault();
        }

        public Question GetQuestionData(Guid id)
        {
            var questionQuery = GetQuestion(id);
            if (questionQuery == null)
                return null;

            var cursor = GetBankState(questionQuery.BankIdentifier);
            return cursor.FindQuestion(id);
        }

        public int CountQuestions(QBankQuestionFilter filter)
        {
            using (var db = CreateContext())
            {
                db.Database.CommandTimeout = 5 * 60;

                return CreateQuery(filter, db)
                    .Count();
            }
        }

        public List<QBankQuestion> GetQuestions(IEnumerable<Guid> ids, params Expression<Func<QBankQuestion, object>>[] includes)
        {
            if (!ids.Any())
                return new List<QBankQuestion>();

            using (var db = CreateContext())
            {
                var query = db.BankQuestions.AsNoTracking().ApplyIncludes(includes);

                return query.Where(x => ids.Contains(x.QuestionIdentifier)).ToList();
            }
        }

        public List<QBankQuestion> GetQuestions(QBankQuestionFilter filter, params Expression<Func<QBankQuestion, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db).ApplyIncludes(includes);

                if (filter.OrderBy.IsEmpty())
                    query = query.OrderBy(x => x.Bank.BankName).ThenBy(x => x.BankIndex);
                else
                    query = query.OrderBy(filter.OrderBy);

                return query.ApplyPaging(filter).ToList();
            }
        }

        public List<QBankQuestionDetail> GetQuestionDetails(QBankQuestionFilter filter)
        {
            using (var db = CreateContext())
            {
                db.Database.CommandTimeout = 5 * 60;

                var result = CreateQuery(filter, db)
                    .OrderBy(x => x.Bank.BankName)
                    .ThenBy(x => x.BankIndex)
                    .ApplyPaging(filter)
                    .Select(x => new QBankQuestionDetail
                    {
                        BankIdentifier = x.BankIdentifier,
                        QuestionIdentifier = x.QuestionIdentifier,
                        BankIndex = x.BankIndex,
                        BankName = x.Bank.BankName,
                        QuestionCode = x.QuestionCode,
                        QuestionDifficulty = x.QuestionDifficulty,
                        QuestionTaxonomy = x.QuestionTaxonomy,
                        QuestionText = x.QuestionText,
                        QuestionReference = x.QuestionReference,
                        QuestionTag = x.QuestionTag,
                        QuestionTags = x.QuestionTags,
                        QuestionCompetencyTitle = x.Competency.CompetencyTitle,
                        QuestionCompetencyIdentifier = x.Competency.CompetencyIdentifier,
                        QuestionType = x.QuestionType,
                        Rubric = x.Rubric.RubricTitle,
                        QuestionFlag = x.QuestionFlag,
                        QuestionPublicationStatus = x.QuestionPublicationStatus,
                    })
                    .ToList();

                return result;
            }
        }

        private IQueryable<QBankQuestion> CreateQuery(QBankQuestionFilter filter, InternalDbContext db)
        {
            var query = db.BankQuestions
                .AsNoTracking()
                .AsQueryable();

            if (filter == null)
                return query;

            if (filter.BankIdentifier.HasValue)
                query = query.Where(x => x.BankIdentifier == filter.BankIdentifier);

            if (filter.ParentQuestionIdentifier.HasValue)
                query = query.Where(x => x.ParentQuestionIdentifier == filter.ParentQuestionIdentifier.Value);

            if (filter.HasParentQuestion.HasValue)
            {
                if (filter.HasParentQuestion.Value)
                    query = query.Where(x => x.ParentQuestionIdentifier != null);
                else
                    query = query.Where(x => x.ParentQuestionIdentifier == null);
            }

            if (filter.QuestionAsset.IsNotEmpty())
            {
                var parts = filter.QuestionAsset.Split('.');
                var pattern = parts.Length == 2
                    ? $"%{parts[0]}%.{parts[1]}"
                    : $"%{parts[0]}%.%";

                query = query.Where(x => DbFunctions.Like(x.QuestionAssetNumber, pattern));
            }

            if (filter.QuestionCode.IsNotEmpty())
                query = query.Where(x => x.QuestionCode.Contains(filter.QuestionCode));

            if (filter.QuestionReference.IsNotEmpty())
                query = query.Where(x => x.QuestionReference.Contains(filter.QuestionReference));

            if (filter.QuestionText.IsNotEmpty())
                query = query.Where(x => x.QuestionText.Contains(filter.QuestionText));

            if (filter.QuestionDifficulty.HasValue)
                query = query.Where(x => x.QuestionDifficulty == filter.QuestionDifficulty);

            if (filter.QuestionTextWithAssetNumber.IsNotEmpty())
                query = query.Where(x => x.QuestionText.Contains(filter.QuestionTextWithAssetNumber)
                                    || x.QuestionAssetNumber.Contains(filter.QuestionTextWithAssetNumber));

            if (filter.QuestionTag.IsNotEmpty())
                query = query.Where(x => x.QuestionTag.Contains(filter.QuestionTag));

            if (filter.QuestionType.IsNotEmpty())
                query = query.Where(x => x.QuestionType.Equals(filter.QuestionType));

            if (filter.StandardCode.IsNotEmpty())
                query = query.Where(x => x.Competency.CompetencyCode != null && (x.Competency.AreaCode ?? string.Empty) + x.Competency.CompetencyCode == filter.StandardCode);

            if (filter.QuestionFlag.IsNotEmpty())
            {
                if (filter.QuestionFlag == "None")
                    query = query.Where(x => x.QuestionFlag == filter.QuestionFlag || x.QuestionFlag == null);
                else
                    query = query.Where(x => x.QuestionFlag == filter.QuestionFlag);
            }

            if (filter.RubricIdentifier.HasValue)
                query = query.Where(x => x.RubricIdentifier == filter.RubricIdentifier.Value);

            if (filter.QuestionBank.IsNotEmpty())
                query = query.Where(x => x.Bank.BankName.Contains(filter.QuestionBank));

            if (filter.QuestionCompetencyTitle.IsNotEmpty())
                query = query.Where(x => x.Competency.CompetencyTitle.Contains(filter.QuestionCompetencyTitle));

            if (filter.QuestionChangedRange?.IsEmpty == false)
            {
                var range = filter.QuestionChangedRange;
                if (range.Since.HasValue && range.Before.HasValue)
                    query = query.Where(x => x.LastChangeTime >= range.Since.Value && x.LastChangeTime < range.Before.Value);
                else if (range.Since.HasValue)
                    query = query.Where(x => x.LastChangeTime >= range.Since.Value);
                else
                    query = query.Where(x => x.LastChangeTime < range.Before.Value);
            }

            if (filter.QuestionPublicationStatus.HasValue)
            {
                var status = filter.QuestionPublicationStatus.Value.GetName();
                query = query.Where(x => x.QuestionPublicationStatus == status);
            }

            if (filter.QuestionClassificationTag.IsNotEmpty())
            {
                var tag = "|" + QBankQuestion.EscapeQuestionTag(filter.QuestionClassificationTag) + "|";
                query = query.Where(x => x.QuestionTags.Contains(tag));
            }

            query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            return query;
        }

        public List<QBankQuestionGradeItem> GetQuestionGradeItems(IEnumerable<Guid> questionIds)
        {
            using (var db = CreateContext())
            {
                return db.BankQuestionGradeItems
                    .Where(x => questionIds.Contains(x.QuestionIdentifier))
                    .ToList();
            }
        }

        public List<Guid> GetQuestionsNotConnectedToRubrics(IEnumerable<Guid> questionIds)
        {
            using (var db = CreateContext())
            {
                return db.BankQuestions
                    .Where(x => questionIds.Contains(x.QuestionIdentifier)
                             && (!x.RubricIdentifier.HasValue || !db.QRubrics.Any(y => y.RubricIdentifier == x.RubricIdentifier.Value)))
                    .Select(x => x.QuestionIdentifier)
                    .ToList();
            }
        }

        #endregion

        #region Specs

        public int Count(QBankSpecificationFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .Count();
            }
        }

        public QBankSpecification[] Get(QBankSpecificationFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .OrderBy(filter.OrderBy.IfNullOrEmpty("SpecName"))
                    .ApplyPaging(filter)
                    .ToArray();
            }
        }

        private IQueryable<QBankSpecification> CreateQuery(QBankSpecificationFilter filter, InternalDbContext db)
        {
            var query = db.BankSpecifications
                .AsNoTracking()
                .AsQueryable();

            if (filter == null)
                return query;

            if (filter.SpecAsset.HasValue)
                query = query.Where(x => x.SpecAsset == filter.SpecAsset);

            if (filter.SpecName.IsNotEmpty())
                query = query.Where(x => x.SpecName.Contains(filter.SpecName));

            if (filter.SpecType.IsNotEmpty())
                query = query.Where(x => x.SpecType.Contains(filter.SpecType));

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier.Value);

            if (filter.BankIdentifier.HasValue)
                query = query.Where(x => x.BankIdentifier == filter.BankIdentifier.Value);

            return query;
        }

        public QBankSpecification GetSpecification(Guid id)
        {
            using (var db = CreateContext())
            {
                return db.BankSpecifications
                    .AsNoTracking()
                    .FirstOrDefault(x => x.SpecIdentifier == id);
            }
        }

        #endregion

        #region Comments

        public VComment GetComment(Guid id)
        {
            using (var db = CreateContext())
            {
                return db.VComments
                    .AsNoTracking()
                    .FirstOrDefault(x => x.CommentIdentifier == id);
            }
        }

        public VComment[] GetComments(Guid bankId)
        {
            using (var db = CreateContext())
            {
                return db.VComments
                    .AsNoTracking()
                    .Where(x => x.AssessmentBankIdentifier == bankId)
                    .ToArray();
            }
        }

        public int CountComments(BankCommentaryFilter filter)
        {
            using (var db = CreateContext())
            {
                return Filter(db, db.VComments, filter).Count();
            }
        }

        public T[] BindComments<T>(Expression<Func<VComment, T>> binder, BankCommentaryFilter filter)
        {
            var orderBy = !string.IsNullOrEmpty(filter.OrderBy)
                ? filter.OrderBy
                : "CommentPosted DESC";

            using (var db = new InternalDbContext())
            {
                return Filter(db, db.VComments, filter)
                    .Select(binder)
                    .OrderBy(orderBy)
                    .ApplyPaging(filter)
                    .ToArray();
            }
        }

        private static IQueryable<VComment> Filter(InternalDbContext db, IQueryable<VComment> query, BankCommentaryFilter filter)
        {
            query = query.Where(x => x.ContainerType == "Assessment Bank" && x.AssessmentBankIdentifier.HasValue);

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier.Value);

            if (filter.CommentFlag.HasValue)
            {
                var flagName = filter.CommentFlag.Value.GetName();
                query = query.Where(x => x.CommentFlag == flagName);
            }

            if (filter.CommentText.IsNotEmpty())
                query = query.Where(x => x.CommentText.Contains(filter.CommentText));

            if (filter.EventFormat.IsNotEmpty())
                query = query.Where(x => x.EventFormat == filter.EventFormat);

            if (filter.AuthorRole.IsNotEmpty())
                query = query.Where(x => x.AuthorUserRole == filter.AuthorRole);

            if (filter.CommentPosted != null)
            {
                if (filter.CommentPosted.Since.HasValue)
                    query = query.Where(x => x.CommentPosted >= filter.CommentPosted.Since.Value);

                if (filter.CommentPosted.Before.HasValue)
                    query = query.Where(x => x.CommentPosted < filter.CommentPosted.Before.Value);
            }

            if (filter.CommentCategory.IsNotEmpty())
            {
                var includeNoCategory = false;
                var categories = new List<string>();

                foreach (var c in filter.CommentCategory)
                {
                    if (c.Equals("No Category", StringComparison.OrdinalIgnoreCase))
                        includeNoCategory = true;
                    else
                        categories.Add(c);
                }

                var predicate = PredicateBuilder.False<VComment>();

                if (includeNoCategory)
                    predicate = predicate.Or(x => x.CommentCategory == null);

                if (categories.Count > 0)
                    predicate = predicate.Or(x => categories.Contains(x.CommentCategory));

                query = query.Where(predicate.Expand());
            }

            if (filter.AttemptTag.IsNotEmpty())
            {
                var attempts = db.QAttempts.Where(attempt => filter.AttemptTag.Any(tag => tag == attempt.AttemptTag))
                    .Select(x => x.AttemptIdentifier);
                query = query.Where(x => attempts.Any(attempt => x.AssessmentAttemptIdentifier == attempt));
            }

            if (filter.AttemptRegistrationEventIdentifier.HasValue)
                query = query.Where(x => x.EventIdentifier == filter.AttemptRegistrationEventIdentifier.Value);

            var subjectFilter = PredicateBuilder.False<VComment>();
            var hasSubjectFilter = false;

            if (filter.BankIdentifier.IsNotEmpty())
            {
                subjectFilter = subjectFilter.Or(x => x.ContainerSubtype == "Bank" && filter.BankIdentifier.Any(s => s == x.AssessmentBankIdentifier));
                hasSubjectFilter = true;
            }

            if (filter.FieldIdentifier.IsNotEmpty())
            {
                subjectFilter = subjectFilter.Or(x => x.ContainerSubtype == "Field" && filter.FieldIdentifier.Any(s => s == x.AssessmentFieldIdentifier));
                hasSubjectFilter = true;
            }

            if (filter.FormIdentifier.IsNotEmpty())
            {
                subjectFilter = subjectFilter.Or(x => x.ContainerSubtype == "Form" && filter.FormIdentifier.Any(s => s == x.AssessmentFormIdentifier));
                hasSubjectFilter = true;
            }

            if (filter.QuestionIdentifier.IsNotEmpty())
            {
                subjectFilter = subjectFilter.Or(x => x.ContainerSubtype == "Question" && filter.QuestionIdentifier.Any(s => s == x.AssessmentQuestionIdentifier));
                hasSubjectFilter = true;
            }

            if (filter.SpecificationIdentifier.IsNotEmpty())
            {
                subjectFilter = subjectFilter.Or(x => x.ContainerSubtype == "Specification" && filter.SpecificationIdentifier.Any(s => s == x.AssessmentSpecificationIdentifier));
                hasSubjectFilter = true;
            }

            if (hasSubjectFilter)
                query = query.Where(subjectFilter.Expand());

            return query;
        }

        #endregion

        #region Registrations

        public VAssessmentFormRegistration[] GetAssessmentFormRegistrations(Guid? @event)
        {
            const string select = @"
SELECT
    R.EventIdentifier
  , R.RegistrationIdentifier
  
  , P.PersonCode         AS LearnerPersonCode
  , P.UserIdentifier     AS LearnerUserIdentifier
  
  , F.BankIdentifier     AS AssessmentBankIdentifier
  , R.ExamFormIdentifier AS AssessmentFormIdentifier

FROM
    registrations.QRegistration AS R

INNER JOIN banks.QBankForm      AS F
           ON R.ExamFormIdentifier = F.FormIdentifier

INNER JOIN contacts.Person      AS P
           ON R.CandidateIdentifier = P.UserIdentifier AND F.OrganizationIdentifier = P.OrganizationIdentifier

WHERE R.EventIdentifier = @EventIdentifier
";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@EventIdentifier", @event.HasValue ? (object)@event: DBNull.Value)
            };

            using (var db = new InternalDbContext())
            {
                return db.Database
                    .SqlQuery<VAssessmentFormRegistration>(select, parameters)
                    .ToArray();
            }
        }

        #endregion

        #region Helpers

        public BankEntityType GetEntityType(Guid id)
        {
            const string query = @"
SELECT
    CASE
        WHEN EXISTS(SELECT TOP 1 1 FROM banks.QBank WHERE BankIdentifier = @EntityID) THEN 'Bank'
        WHEN EXISTS(SELECT TOP 1 1 FROM banks.QBankSpecification WHERE SpecIdentifier = @EntityID) THEN 'Specification'
        WHEN EXISTS(SELECT TOP 1 1 FROM banks.QBankForm WHERE FormIdentifier = @EntityID) THEN 'Form'
        WHEN EXISTS(SELECT TOP 1 1 FROM banks.QBankQuestion WHERE QuestionIdentifier = @EntityID) THEN 'Question'
        WHEN EXISTS(SELECT TOP 1 1 FROM assets.QComment WHERE CommentIdentifier = @EntityID) THEN 'Comment'
        ELSE 'Unknown'
    END;
";
            var parameters = new[]
            {
                new SqlParameter("@EntityID", id)
            };

            using (var db = CreateContext())
            {
                var value = db.Database.SqlQuery<string>(query, parameters).FirstOrDefault();

                return value.ToEnum(BankEntityType.Unknown);
            }
        }

        private class CommentReadHelper : ReadHelper<VComment>
        {
            public static readonly CommentReadHelper Instance = new CommentReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<VComment>, TResult> func)
            {
                using (var context = new InternalDbContext(false))
                {
                    var query = context.VComments.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        #endregion
    }
}
