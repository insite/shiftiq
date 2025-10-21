using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.UI.Admin.Records.Logbooks.Competencies.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<QExperienceCompetencyFilter>
    {
        public bool IsValidator { get; set; }

        protected override int SelectCount(QExperienceCompetencyFilter filter)
        {
            return ServiceLocator.JournalSearch.CountExperienceCompetencies(filter);
        }

        protected override IListSource SelectData(QExperienceCompetencyFilter filter)
        {
            return ServiceLocator.JournalSearch
                .GetExperienceCompetencies(filter,
                    x => x.Experience.Journal.User,
                    x => x.Experience.Journal.JournalSetup.Framework,
                    x => x.Experience.Validator,
                    x => x.Competency
                )
                .Select(x => new
                {
                    ExperienceIdentifier = x.ExperienceIdentifier,
                    Sequence = x.Experience.Sequence,
                    Created = x.Experience.ExperienceCreated,
                    ExperienceValidated = x.Experience.ExperienceValidated,
                    Status = x.Experience.ExperienceValidated.HasValue
                            ? $"Validated by {x.Experience.Validator?.UserFullName ?? UserNames.Someone}"
                            : "Not Validated",
                    ValidateButtonIcon = x.Experience.ExperienceValidated.HasValue
                            ? "graduation-cap"
                            : "question-circle",
                    ValidateButtonHint = x.Experience.ExperienceValidated.HasValue
                            ? "Validated"
                            : "Validate",
                    JournalSetupIdentifier = x.Experience.Journal.JournalSetupIdentifier,
                    JournalIdentifier = x.Experience.Journal.JournalIdentifier,
                    JournalSetupName = x.Experience.Journal.JournalSetup.JournalSetupName,
                    JournalSetupCreated = x.Experience.Journal.JournalSetup.JournalSetupCreated,
                    UserIdentifier = x.Experience.Journal.UserIdentifier,
                    UserFullName = x.Experience.Journal.User.UserFullName,
                    UserEmail = x.Experience.Journal.User.UserEmail,
                    FrameworkIdentifier = x.Experience.Journal.JournalSetup.FrameworkStandardIdentifier,
                    FrameworkName = x.Experience.Journal.JournalSetup.Framework.FrameworkTitle,
                    CompetencyIdentifier = x.CompetencyStandardIdentifier,
                    CompetencyName = x.Competency.CompetencyTitle,
                    Hours = x.CompetencyHours,
                    SatisfactionLevel = x.SatisfactionLevel,
                    SkillRating = x.SkillRating
                })
                .ToList()
                .ToSearchResult();
        }

        protected static string GetLocalTime(object obj)
        {
            var date = (DateTimeOffset)obj;
            return TimeZones.Format(date, User.TimeZone, true);
        }
    }
}