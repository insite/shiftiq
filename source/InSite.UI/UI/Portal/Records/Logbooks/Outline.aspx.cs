using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Admin.Records.Programs.Utilities;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;
using InSite.UI.Portal.Records.Logbooks.Utilities;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Records.Logbooks
{
    public partial class Outline : PortalBasePage
    {
        private Guid JournalSetupIdentifier => Guid.TryParse(Request["journalsetup"], out var id) ? id : Guid.Empty;

        private Guid UserIdentifier => User.UserIdentifier;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DownloadPDFButton.Click += DownloadPDFButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            LoadData();

            if (Request.QueryString["panel"] == "comments")
                CommentsPanel.IsSelected = true;
        }

        private void LoadData()
        {
            var journalSetup = ServiceLocator.JournalSearch.GetJournalSetup(JournalSetupIdentifier);
            if (journalSetup == null || journalSetup.OrganizationIdentifier != Organization.OrganizationIdentifier)
            {
                HttpResponseHelper.Redirect("/ui/portal/records/logbooks/search");
                return;
            }

            if (!IsEnrolled())
            {
                HttpResponseHelper.Redirect("/ui/portal/records/logbooks/search");
                return;
            }

            var content = ServiceLocator.ContentSearch.GetBlock(JournalSetupIdentifier);

            PageHelper.AutoBindHeader(this, qualifier: content?.Title?.Text.Get(Identity.Language) ?? journalSetup.JournalSetupName);

            var experiences = ServiceLocator.JournalSearch
                .GetExperiences(new QExperienceFilter { JournalSetupIdentifier = JournalSetupIdentifier, UserIdentifier = UserIdentifier });

            var locked = journalSetup.JournalSetupLocked.HasValue;

            LoadExperiences(experiences, locked);

            var hasCompetencies = journalSetup.FrameworkStandardIdentifier.HasValue
                && ProgressGrid.LoadData(JournalSetupIdentifier, journalSetup.FrameworkStandardIdentifier.Value, UserIdentifier);

            ProgressPanel.Visible = hasCompetencies;
            DownloadPDFButton.Visible = journalSetup.AllowLogbookDownload == true;

            Comments.LoadData(JournalSetupIdentifier, null, experiences);

            AddEntryLink.NavigateUrl = new ReturnUrl("journalsetup")
                .GetRedirectUrl($"/ui/portal/records/logbooks/learners/add-experience?journalsetup={JournalSetupIdentifier}");

            AddEntryLink.Visible = !locked;
            if (locked)
                StatusAlert.AddMessage(AlertType.Error, Translate("This logbook has been locked by an administrator and you are no longer able to add new entries."));
        }

        private bool IsEnrolled()
        {
            ProgramHelper.EnrollLearnerByObjectId(Organization.Identifier, UserIdentifier, JournalSetupIdentifier);

            return ServiceLocator.JournalSearch.GetEnrollmentStatus(JournalSetupIdentifier, UserIdentifier) != LogbookEnrollmentStatus.NotEnrolled;
        }

        private void LoadExperiences(List<QExperience> experiences, bool isJournalSetupLocked)
        {
            ExperienceRepeater.Visible = experiences.Count > 0;

            if (experiences.Count == 0)
            {
                StatusAlert.AddMessage(AlertType.Warning, Translate("There are no entries in this logbook."));
                return;
            }

            var list = experiences
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
                        IsValidated = x.ExperienceValidated.HasValue,
                        IsJournalSetupLocked = isJournalSetupLocked
                    };
                })
                .ToList();

            ExperienceRepeater.DataSource = list;
            ExperienceRepeater.DataBind();
        }

        private void DownloadPDFButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            var pdf = GetPdf();
            Response.SendFile("logbook.pdf", pdf, "application/pdf");
        }

        private byte[] GetPdf()
        {
            var journalSetupResultCreator = new Shift.Toolbox.Progress.LogbookResultCreator();

            var journalSetupPDFModel = LogbookResultHelper.GetLogbookResultPdfModel(
                JournalSetupIdentifier,
                UserIdentifier,
                Page.Server,
                CurrentSessionState.Identity.Organization.Identifier,
                User.TimeZone,
                Identity.Language);

            return journalSetupResultCreator.CreatePdf(journalSetupPDFModel);
        }

        protected string GetLocalTime(object obj)
        {
            var date = (DateTimeOffset)obj;
            return TimeZones.Format(date, User.TimeZone, true);
        }
    }
}