using System;
using System.Globalization;
using System.Web.UI;

using InSite.Persistence;

using Shift.Common;

namespace InSite.UI.Portal.Jobs.Employers.Opportunities.Controls
{
    public partial class ViewDetails : UserControl
    {
        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {

            }
        }

        #endregion

        public void LoadData(TOpportunity jobOpportunity)
        {

            JobTitle.Text = jobOpportunity.JobTitle;
            Description.Text = jobOpportunity.JobDescription;
            Location.Text = jobOpportunity.LocationName;
            Organization.Text = jobOpportunity.EmployerGroupName;
            EmploymentType.Text = EmploymentTypeConverter(jobOpportunity.EmploymentType);
            PostedOn.Text = jobOpportunity.WhenPublished.HasValue ?
                (TimeZones.Format(jobOpportunity.WhenPublished.Value, CurrentSessionState.Identity.User.TimeZone)) : "";

            PositionType.Text = PositionTypeConverter(jobOpportunity.LocationType);
            PositionLevel.Text = jobOpportunity.JobLevel;
            Salary.Text = jobOpportunity.SalaryOther;

            IsResumeRequired.Text = (jobOpportunity.ApplicationRequiresResume.HasValue ? (jobOpportunity.ApplicationRequiresResume.Value ? "Yes" : "No") : "No");
            IsCoverLetterRequired.Text = (jobOpportunity.ApplicationRequiresLetter.HasValue ? (jobOpportunity.ApplicationRequiresLetter.Value ? "Yes" : "No") : "No");

            StartDateLabel.Visible = jobOpportunity.ApplicationOpen.HasValue;
            if (jobOpportunity.ApplicationOpen.HasValue)
                StartDate.Text = TimeZones.FormatDateOnly(jobOpportunity.ApplicationOpen.Value,
                    CurrentSessionState.Identity.User.TimeZone, CultureInfo.GetCultureInfo(CurrentSessionState.Identity.Language));

            ApplicationDeadlineLabel.Visible = jobOpportunity.ApplicationDeadline.HasValue;
            if (jobOpportunity.ApplicationDeadline.HasValue)
                ApplicationDeadline.Text = TimeZones.FormatDateOnly(jobOpportunity.ApplicationDeadline.Value,
                    CurrentSessionState.Identity.User.TimeZone, CultureInfo.GetCultureInfo(CurrentSessionState.Identity.Language));

            AboutCompanyLabel.Visible = !String.IsNullOrEmpty(jobOpportunity.EmployerGroupDescription);
            AboutCompany.Text = jobOpportunity.EmployerGroupDescription;

        }

        protected string EmploymentTypeConverter(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            switch (value)
            {
                case "Other":
                    return "Other";
                case "FullTime":
                    return "Full Time";
                case "PartTime":
                    return "Part Time";
                case "Contract":
                    return "Contract";
                case "Temporary":
                    return "Temporary";
                case "Seasonal":
                    return "Seasonal";
                case "Flexible":
                    return "Flexible";
            }

            return value;
        }

        protected string PositionTypeConverter(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            switch (value)
            {
                case "Not Remote":
                    return "Fully on site";
                case "Remote":
                    return "Fully remote";
                case "Potential Remote":
                    return "Potential for some remote";
            }

            return value;
        }
    }
}