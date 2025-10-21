using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Jobs.Employers.Opportunities.Controls
{
    public partial class Details : UserControl
    {
        private string JobDescriptionHtml
        {
            get => ViewState[nameof(JobDescriptionHtml)] as string;
            set => ViewState[nameof(JobDescriptionHtml)] = value;
        }

        protected Guid EmployerGroupIdentifier
        {
            get => (Guid)ViewState[nameof(EmployerGroupIdentifier)];
            set => ViewState[nameof(EmployerGroupIdentifier)] = value;
        }

        protected string EmployerGroupName
        {
            get => (string)ViewState[nameof(EmployerGroupName)];
            set => ViewState[nameof(EmployerGroupName)] = value;
        }

        private DateTimeOffset? PublishedDateTime
        {
            get => ViewState[nameof(PublishedDateTime)] as DateTimeOffset? ?? null;
            set => ViewState[nameof(PublishedDateTime)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PublishedCheckBox.AutoPostBack = true;
            PublishedCheckBox.CheckedChanged += PublishedCheckBox_CheckedChanged;

            JobDescriptionValidator.ServerValidate += JobDescriptionValidator_ServerValidate;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            OccupationIdentifier.ListFilter.OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier;
            OccupationIdentifier.ListFilter.StandardTypes = new[] { StandardType.Profile };
            OccupationIdentifier.EnsureDataBound();
        }

        public void SetInputValues(TOpportunity entity)
        {
            if (string.IsNullOrEmpty(entity.EmployerGroupName) && !string.IsNullOrEmpty(EmployerGroupName))
                EmployerGroup.Text = EmployerGroupName;
            else if (!string.IsNullOrEmpty(entity.EmployerGroupName))
                EmployerGroup.Text = entity.EmployerGroupName;

            if (entity.EmployerGroupIdentifier.HasValue)
                EmployerGroupIdentifier = entity.EmployerGroupIdentifier.Value;

            if (string.IsNullOrEmpty(EmployerContact.Text))
                EmployerContact.Text = CurrentSessionState.Identity.User.FullName;

            JobPosition.Text = entity.JobTitle;
            JobDescriptionText.Value = entity.JobDescription;
            WebSiteUrl.Text = entity.ApplicationWebSiteUrl;
            AboutTheCompany.Text = entity.EmployerGroupDescription;
            EmailAddress.Text = entity.ApplicationEmail;
            JobLocation.Text = entity.LocationName;
            PublishedCheckBox.Checked = entity.WhenPublished.HasValue;
            OccupationIdentifier.ValueAsGuid = entity.OccupationStandardIdentifier;
            PublishedDateTime = entity.WhenPublished;
            PositionType.Value = entity.LocationType;
            EmploymentType.Value = entity.EmploymentType;
            SubmitResume.ValueAsBoolean = entity.ApplicationRequiresResume;
            SubmitCoverLetter.ValueAsBoolean = entity.ApplicationRequiresLetter;

            SetPublishDateTimeLabel(PublishedCheckBox.Checked);
        }

        public void GetInputValues(TOpportunity entity)
        {
            if (EmployerGroupIdentifier == null)
                return;

            entity.EmployerGroupIdentifier = EmployerGroupIdentifier;
            entity.EmployerGroupName = EmployerGroup.Text;
            entity.EmployerUserIdentifier = CurrentSessionState.Identity.User.UserIdentifier;

            entity.JobTitle = JobPosition.Text;
            entity.JobDescription = JobDescriptionText.Value;
            entity.LocationName = JobLocation.Text;
            entity.LocationType = PositionType.Value;
            entity.EmploymentType = EmploymentType.Value;
            entity.ApplicationWebSiteUrl = WebSiteUrl.Text;
            entity.EmployerGroupDescription = AboutTheCompany.Text;
            entity.ApplicationEmail = EmailAddress.Text;
            entity.OccupationStandardIdentifier = OccupationIdentifier.ValueAsGuid;

            if (PublishedCheckBox.Checked)
                entity.WhenPublished = PublishedDateTime.HasValue ? PublishedDateTime.Value : DateTimeOffset.Now;
            else
                entity.WhenPublished = null;

            entity.ApplicationRequiresResume = SubmitResume.ValueAsBoolean;
            entity.ApplicationRequiresLetter = SubmitCoverLetter.ValueAsBoolean;

            if (entity.OpportunityIdentifier == null || entity.OpportunityIdentifier == Guid.Empty)
                entity.OpportunityIdentifier = UniqueIdentifier.Create();
        }

        internal void SetUserData(QGroup group)
        {
            if (group != null)
            {
                EmailAddress.Text = group.GroupEmail;
                EmployerGroupIdentifier = group.GroupIdentifier;
                EmployerGroupName = group.GroupName;

                WebSiteUrl.Text = group.GroupWebSiteUrl;
                AboutTheCompany.Text = group.GroupDescription;
            }

            EmployerGroup.Text = group.GroupName;
            EmployerContact.Text = CurrentSessionState.Identity.User.FullName;

            PublishedDateTime = DateTimeOffset.Now;
            SetPublishDateTimeLabel(true);
        }

        private void SetPublishDateTimeLabel(bool isPublished)
        {
            PublishedCheckBoxDateTime.Text = isPublished
                ? PublishedDateTime.Value.Format(CurrentSessionState.Identity.User.TimeZone, true)
                : string.Empty;
        }

        private void PublishedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            var publichCheckBox = (ICheckBox)sender;

            if (publichCheckBox.Checked)
                PublishedDateTime = DateTimeOffset.Now;
            else
                PublishedDateTime = null;

            SetPublishDateTimeLabel(publichCheckBox.Checked);
        }

        private void JobDescriptionValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = !string.IsNullOrEmpty(JobDescriptionText.Value);

            if (!args.IsValid)
                JobDescriptionValidator.ErrorMessage = $@"Job description is required.";
        }
    }
}