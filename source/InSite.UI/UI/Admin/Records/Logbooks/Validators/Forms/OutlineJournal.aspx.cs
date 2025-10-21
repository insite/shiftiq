using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Admin.Records.Logbooks;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Records.Validators.Forms
{
    public partial class OutlineJournal : AdminBasePage, IHasParentLinkParameters
    {
        private Guid JournalSetupIdentifier => Guid.TryParse(Request.QueryString["journalSetup"], out var journalSetupIdentifier) ? journalSetupIdentifier : Guid.Empty;
        private Guid UserIdentifier => Guid.TryParse(Request.QueryString["user"], out var userIdentifier) ? userIdentifier : Guid.Empty;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                LoadData();

                if (Request.QueryString["panel"] == "comments")
                    CommentsPanel.IsSelected = true;
            }
        }

        private void LoadData()
        {
            var journalSetupUser = ServiceLocator.JournalSearch.GetJournalSetupUser(JournalSetupIdentifier, UserIdentifier, JournalSetupUserRole.Learner,
                x => x.JournalSetup.Event,
                x => x.JournalSetup.Achievement,
                x => x.JournalSetup.Framework
            );

            var journalSetup = journalSetupUser?.JournalSetup;

            if (journalSetup == null
                || journalSetup.OrganizationIdentifier != Organization.OrganizationIdentifier
                || !ServiceLocator.JournalSearch.ExistsJournalSetupUser(JournalSetupIdentifier, User.UserIdentifier, JournalSetupUserRole.Validator)
                )
            {
                HttpResponseHelper.Redirect("/ui/admin/records/logbooks/validators/searchjournal/search");
                return;
            }

            var header = LogbookHeaderHelper.GetLogbookHeader(journalSetup, User.TimeZone);

            PageHelper.AutoBindHeader(this, null, header);

            var student = ServiceLocator.PersonSearch.GetPerson(UserIdentifier, Organization.Identifier, x => x.User);
            if (student == null)
            {
                NavPanel.Visible = false;
                StatusAlert.AddMessage(AlertType.Error, "This logbook belongs to the user that is not registered in the current organization.");
                return;
            }

            var studentUrl = new ReturnUrl().GetRedirectUrl($"/ui/admin/records/logbooks/validators/user-journal?user={UserIdentifier}&journalsetup={JournalSetupIdentifier}");

            PersonDetail.BindPerson(student, User.TimeZone, studentUrl);
            LogbookDetail.BindLogbook(JournalSetupIdentifier, journalSetup, User.TimeZone, true);

            var journalIdentifier = ServiceLocator.JournalSearch.GetJournal(JournalSetupIdentifier, UserIdentifier)?.JournalIdentifier;
            var experiences = journalIdentifier.HasValue
                ? ServiceLocator.JournalSearch.GetExperiences(new QExperienceFilter { JournalIdentifier = journalIdentifier })
                : new List<QExperience>();

            LoadExperiences(experiences);

            var hasCompetencies = journalSetup.FrameworkStandardIdentifier.HasValue
                && ProgressGrid.LoadData(JournalSetupIdentifier, journalSetup.FrameworkStandardIdentifier.Value, UserIdentifier);

            ProgressPanel.Visible = hasCompetencies;

            Comments.LoadData(JournalSetupIdentifier, UserIdentifier, journalIdentifier, experiences, null);

            var returnUrl = new ReturnUrl($"journalSetup&user");

            AddExperienceLink.NavigateUrl = returnUrl.GetRedirectUrl($"/ui/admin/records/logbooks/validators/add-experience?journalSetup={JournalSetupIdentifier}&user={UserIdentifier}");
        }

        private void LoadExperiences(List<QExperience> journalExperiences)
        {
            var returnUrl = new ReturnUrl();

            var experiences = journalExperiences
                .Select(x =>
                {
                    var validator = x.ValidatorUserIdentifier.HasValue
                        ? UserSearch.Select(x.ValidatorUserIdentifier.Value)?.FullName
                        : null;

                    return new
                    {
                        ExperienceIdentifier = x.ExperienceIdentifier,
                        Sequence = x.Sequence,
                        Created = x.ExperienceCreated,
                        Status = x.ExperienceValidated.HasValue
                            ? $"Validated by {validator ?? UserNames.Someone}"
                            : "Not Validated",
                        ValidateButtonIcon = x.ExperienceValidated.HasValue
                            ? "graduation-cap"
                            : "question-circle",
                        ValidateButtonHint = x.ExperienceValidated.HasValue
                            ? "Validated"
                            : "Validate",
                        ViewExperienceUrl = returnUrl
                            .GetRedirectUrl($"/ui/admin/records/logbooks/validators/entries/view-experience?experience={x.ExperienceIdentifier}"),
                        DeleteUrl = $"/admin/records/logbooks/validators/entries/delete?experience={x.ExperienceIdentifier}"
                    };
                })
                .OrderByDescending(x => x.Created)
                .ToList();

            NoExperiencePanel.Visible = experiences.Count == 0;
            ExperiencePanel.Visible = experiences.Count > 0;

            if (experiences.Count > 0)
            {
                ExperienceRepeater.DataSource = experiences;
                ExperienceRepeater.DataBind();
            }
        }

        protected static string GetLocalTime(object obj)
        {
            if (obj == null)
                return null;

            var date = (DateTimeOffset)obj;
            return TimeZones.Format(date, User.TimeZone, true);
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"journalsetup={JournalSetupIdentifier}"
                : null;
        }
    }
}
