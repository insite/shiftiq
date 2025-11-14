using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;
using InSite.Persistence;

using Shift.Common;

namespace InSite.UI.Admin.Jobs.Opportunities.Controls
{
    public partial class Details : UserControl
    {
        #region Properties

        private DateTimeOffset? PublishedDateTime
        {
            get => ViewState[nameof(PublishedDateTime)] as DateTimeOffset? ?? null;
            set => ViewState[nameof(PublishedDateTime)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EmployerGroupIdentifier.AutoPostBack = true;
            EmployerGroupIdentifier.ValueChanged += EmployerGroupIdentifier_ValueChanged;

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
            OccupationIdentifier.ListFilter.StandardTypes = new[] { Shift.Constant.StandardType.Profile };
            OccupationIdentifier.EnsureDataBound();

            EmployerGroupIdentifier.Filter.OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier;
            EmployerGroupIdentifier.Filter.GroupType = "Employer";

            EmployerContactIdentifier.Filter.OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier;
            EmployerContactIdentifier.Filter.GroupIdentifier =
                ServiceLocator.GroupSearch.GetGroups(
                    new QGroupFilter()
                    {
                        OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier,
                        GroupName = "JobConnect Employers",
                    }).FirstOrDefault()?.GroupIdentifier;
        }

        #endregion

        #region Event handlers

        private void EmployerGroupIdentifier_ValueChanged(object sender, EventArgs e)
        {
            EmployerGroupIdentifierSelected();
        }

        private void PublishedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            var publichCheckBox = (CheckBox)sender;

            if (publichCheckBox.Checked)
                PublishedDateTime = DateTimeOffset.Now;
            else
                PublishedDateTime = null;

            SetPublishDateTimeLabel(publichCheckBox.Checked);
        }

        private void SetPublishDateTimeLabel(bool isPublished)
        {
            if (isPublished)
                PublishedCheckBoxDateTime.Text = TimeZones.Format((DateTimeOffset)PublishedDateTime.Value, CurrentSessionState.Identity.User.TimeZone, true);
            else
                PublishedCheckBoxDateTime.Text = String.Empty;
        }

        private void JobDescriptionValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = !string.IsNullOrEmpty(JobDescriptionText.Value);

            if (!args.IsValid)
                JobDescriptionValidator.ErrorMessage = $@"Job description is required.";
        }

        private void EmployerGroupIdentifierSelected()
        {
            if (EmployerGroupIdentifier.Value.HasValue)
            {
                var employerGroup = EmployerGroupIdentifier.Value.Value;
                var group = ServiceLocator.GroupSearch.GetGroup(employerGroup);
                if (group != null)
                {
                    WebSiteUrl.Text = group.GroupWebSiteUrl;
                    AboutTheCompany.Text = group.GroupDescription;
                    EmailAddress.Text = group.GroupEmail;
                }
                else
                {
                    ResetEmployerGroupInformation();
                }
            }
            else
            {
                ResetEmployerGroupInformation();
            }
        }

        #endregion

        #region Setting and getting input values

        public void LoadDefault()
        {
            PublishedDateTime = DateTimeOffset.Now;
            SetPublishDateTimeLabel(true);
        }

        public void SetInputValues(TOpportunity entity)
        {
            EmployerGroupIdentifier.Value = entity.EmployerGroupIdentifier;
            EmployerContactIdentifier.Value = entity.EmployerUserIdentifier;
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
            if (EmployerGroupIdentifier.Value.HasValue)
            {
                entity.EmployerGroupIdentifier = EmployerGroupIdentifier.Value.Value;
                entity.EmployerGroupName = EmployerGroupIdentifier.Items.First().Text;
            }

            if (EmployerContactIdentifier.Value.HasValue)
                entity.EmployerUserIdentifier = EmployerContactIdentifier.Value.Value;

            entity.JobTitle = JobPosition.Text;
            entity.JobDescription = JobDescriptionText.Value;
            entity.LocationName = JobLocation.Text;
            entity.LocationType = PositionType.Value;
            entity.EmploymentType = EmploymentType.Value;
            entity.ApplicationWebSiteUrl = WebSiteUrl.Text;
            entity.EmployerGroupDescription = AboutTheCompany.Text;
            entity.ApplicationEmail = EmailAddress.Text;
            entity.OccupationStandardIdentifier = OccupationIdentifier.ValueAsGuid;
            entity.WhenPublished = !PublishedCheckBox.Checked
                ? (DateTimeOffset?)null
                : PublishedDateTime.HasValue
                    ? PublishedDateTime.Value
                    : DateTimeOffset.Now;

            entity.ApplicationRequiresResume = SubmitResume.ValueAsBoolean;
            entity.ApplicationRequiresLetter = SubmitCoverLetter.ValueAsBoolean;

            if (entity.OpportunityIdentifier == null || entity.OpportunityIdentifier == Guid.Empty)
                entity.OpportunityIdentifier = UniqueIdentifier.Create();
        }

        private void ResetEmployerGroupInformation()
        {
            WebSiteUrl.Text = string.Empty;
            AboutTheCompany.Text = string.Empty;
            EmailAddress.Text = string.Empty;
        }

        #endregion
    }
}