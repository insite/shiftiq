using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using InSite.Application.Attempts.Read;

using Newtonsoft.Json;

namespace InSite.Persistence
{
    public class TakerReportSearch : ITakerReportSearch
    {
        private class Attempt
        {
            public TakerReportItem Item { get; set; }
            public string FrameworkTags { get; set; }
        }

        internal InternalDbContext CreateContext() => new InternalDbContext(false);

        public List<TakerReportItem> GetTakerReport(QAttemptFilter filter)
        {
            if (filter.LearnerUserIdentifier == null)
                throw new ArgumentNullException("filter.LearnerUserIdentifier");

            if (filter.FormOrganizationIdentifier == null)
                throw new ArgumentNullException("filter.FormOrganizationIdentifier");

            var result = new List<TakerReportItem>();

            var attempts = GetAttempts(filter);
            foreach (var attempt in attempts)
            {
                if (!result.Any(x => x.AttemptIdentifier == attempt.Item.AttemptIdentifier)
                    && IsFieldOfPractice(attempt.FrameworkTags)
                    )
                {
                    result.Add(attempt.Item);
                }
            }

            return result
                .OrderByDescending(x => x.AttemptStarted)
                .ThenBy(x => x.AttemptIdentifier)
                .ToList();
        }

        private static bool IsFieldOfPractice(string frameworkTags)
        {
            var deserialized = JsonConvert.DeserializeObject<List<Tuple<string, List<string>>>>(frameworkTags);
            var reportingTag = deserialized.Find(x => string.Equals(x.Item1, "Reporting Tag", StringComparison.OrdinalIgnoreCase));
            return reportingTag?.Item2?.Find(x => x.Equals("Field of Practice", StringComparison.OrdinalIgnoreCase)) != null;
        }

        private List<Attempt> GetAttempts(QAttemptFilter filter)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                using (var db = CreateContext())
                {
                    var questionQuery = db.BankQuestions
                        .Join(db.Standards.Where(x => x.Parent.Parent.Tags != null),
                            question => question.CompetencyIdentifier,
                            competency => competency.StandardIdentifier,
                            (question, competency) => new
                            {
                                question.QuestionIdentifier,
                                competency.Parent.Parent.Tags
                            }
                        );

                    var query = AttemptSearch.CreateQuery(filter, db);

                    return query
                        .Join(db.QAttemptQuestions,
                            attempt => attempt.AttemptIdentifier,
                            question => question.AttemptIdentifier,
                            (attempt, question) => new
                            {
                                Item = new TakerReportItem
                                {
                                    AttemptIdentifier = attempt.AttemptIdentifier,
                                    FormIdentifier = attempt.FormIdentifier,
                                    BankIdentifier = attempt.Form.BankIdentifier,
                                    FormName = attempt.Form.FormName,
                                    FormTitle = attempt.Form.FormTitle,
                                    AttemptStarted = attempt.AttemptStarted,
                                    AttemptGraded = attempt.AttemptGraded,
                                    AttemptSubmitted = attempt.AttemptSubmitted,
                                    AttemptImported = attempt.AttemptImported,
                                    AttemptIsPassing = attempt.AttemptIsPassing,
                                    AttemptScore = attempt.AttemptScore,
                                    AttemptPoints = attempt.AttemptPoints,
                                    FormPoints = attempt.FormPoints,
                                    AttemptDuration = attempt.AttemptDuration,
                                    FormAssetVersion = attempt.Form.FormAssetVersion,
                                    FormFirstPublished = attempt.Form.FormFirstPublished,
                                    FormAsset = attempt.Form.FormAsset
                                },
                                question.QuestionIdentifier
                            }
                        )
                        .Join(questionQuery,
                            attempt => attempt.QuestionIdentifier,
                            question => question.QuestionIdentifier,
                            (attempt, question) => new Attempt
                            {
                                Item = attempt.Item,
                                FrameworkTags = question.Tags
                            }
                        )
                        .ToList();
                }
            }
        }
    }
}
