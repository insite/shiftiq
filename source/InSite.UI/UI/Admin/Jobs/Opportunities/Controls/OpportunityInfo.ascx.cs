using System.Web.UI;

using InSite.Persistence;

using Shift.Common;

namespace InSite.Admin.Jobs.Opportunities.Controls
{
    public partial class OpportunityInfo : UserControl
    {
        public void BindOpportunity(TOpportunity op)
        {
            OpportunityTitle.Text = op.JobTitle;
            OpportunityEmployer.Text = op.EmployerGroupName;
            OpportunityEmploymentType.Text = EmploymentTypeConverter(op.EmploymentType);
            OpportunityPublished.Text = op.WhenPublished.HasValue ?
                (TimeZones.Format(op.WhenPublished.Value, CurrentSessionState.Identity.User.TimeZone)) : "";

            OpportunityPositionType.Text = PositionTypeConverter(op.LocationType);
            OpportunityPositionLevel.Text = op.JobLevel;
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
    }
}