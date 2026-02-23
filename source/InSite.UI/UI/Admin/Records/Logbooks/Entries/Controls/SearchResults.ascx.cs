using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;
using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.UI.Admin.Records.Logbooks.Entries.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<QExperienceFilter>
    {
        public bool IsValidator { get; set; }

        protected override int SelectCount(QExperienceFilter filter)
        {
            return ServiceLocator.JournalSearch.CountExperiences(filter);
        }

        protected override IListSource SelectData(QExperienceFilter filter)
        {
            var returnUrl = new ReturnUrl();
            var data = ServiceLocator.JournalSearch
                .GetExperiences(filter, x => x.Journal.User, x => x.Journal.JournalSetup, x => x.Validator);

            var departments = ServiceLocator.MembershipSearch
                .Select(new QMembershipFilter
                {
                    GroupOrganizationIdentifier = Organization.Identifier,
                    UserIdentifiers = data.Select(x => x.Journal.UserIdentifier).Distinct().ToArray(),
                    MembershipFunction = "Department"
                }, x => x.Group)
                .GroupBy(x => x.UserIdentifier)
                .ToDictionary(x => x.Key, x => string.Join(", ", x.Select(y => y.Group.GroupName).OrderBy(y => y)));

            return data
                .Select(x => new
                {
                    ExperienceIdentifier = x.ExperienceIdentifier,
                    Sequence = x.Sequence,
                    Created = x.ExperienceCreated,
                    ExperienceValidated = x.ExperienceValidated,
                    Status = x.ExperienceValidated.HasValue
                        ? $"Validated by {x.Validator?.UserFullName ?? UserNames.Someone}"
                        : "Not Validated",
                    ValidateButtonIcon = x.ExperienceValidated.HasValue
                        ? "graduation-cap"
                        : "question-circle",
                    ValidateButtonHint = x.ExperienceValidated.HasValue
                        ? "Validated"
                        : "Validate",
                    JournalSetupIdentifier = x.Journal.JournalSetupIdentifier,
                    JournalIdentifier = x.Journal.JournalIdentifier,
                    JournalSetupName = x.Journal.JournalSetup.JournalSetupName,
                    JournalSetupCreated = x.Journal.JournalSetup.JournalSetupCreated,
                    UserIdentifier = x.Journal.UserIdentifier,
                    UserFullName = x.Journal.User.UserFullName,
                    UserEmail = x.Journal.User.UserEmail,
                    UserDepartment = departments.GetOrDefault(x.Journal.UserIdentifier),
                    TrainingType = x.TrainingType,
                    Employer = x.Employer,
                    Supervisor = x.Supervisor,
                    ExperienceStarted = x.ExperienceStarted,
                    ExperienceStopped = x.ExperienceStopped,
                    ExperienceHours = x.ExperienceHours,
                    DeleteUrl = IsValidator
                        ? returnUrl.GetRedirectUrl($"/admin/records/logbooks/validators/entries/delete?experience={x.ExperienceIdentifier}")
                        : returnUrl.GetRedirectUrl($"/ui/admin/records/logbooks/entries/delete?experience={x.ExperienceIdentifier}")
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