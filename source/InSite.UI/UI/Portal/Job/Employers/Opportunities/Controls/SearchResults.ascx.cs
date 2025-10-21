using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.UI.Portal.Jobs.Employers.Opportunities.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<TOpportunityFilter>
    {
        #region Fields

        private Person _currentUser;
        private bool _isCurrentUserLoaded;

        #endregion

        #region Properties

        public Person CurrentUser
        {
            get
            {
                if (!_isCurrentUserLoaded)
                {
                    _currentUser = PersonSearch.Select(Organization.OrganizationIdentifier, CurrentSessionState.Identity.User.UserIdentifier);
                    _isCurrentUserLoaded = true;
                }

                return _currentUser;
            }
        }

        public bool IsUserApproved =>
            CurrentUser != null && CurrentUser.JobsApproved.HasValue && CurrentUser.UserAccessGranted.HasValue;

        public string CurrentUserRole
        {
            get
            {
                if (CurrentUser == null)
                    return null;

                return CurrentUser.EmployerGroupIdentifier.HasValue
                    ? "Employer"
                    : "Unknown";
            }
        }

        #endregion

        #region Classes

        public class ExportDataItem
        {
            public string JobTitle { get; set; }
            public string LocationLabel { get; set; }
            public string JobLevel { get; set; }

            public DateTimeOffset? Published { get; set; }
        }


        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        #endregion

        #region Methods (data binding)

        protected override int SelectCount(TOpportunityFilter filter)
            => TOpportunitySearch.Count(filter);

        #endregion

        #region Methods (export)

        public override IListSource GetExportData(TOpportunityFilter filter, bool empty)
        {
            return SelectData(filter).GetList().Cast<TOpportunity>().Select(x => new ExportDataItem
            {
                Published = x.WhenPublished,
                JobTitle = x.JobTitle,
                LocationLabel = x.LocationDescription,
                JobLevel = x.JobLevel
            }).ToList().ToSearchResult();
        }

        protected override IListSource SelectData(TOpportunityFilter filter)
        {
            filter.OrderBy = "WhenCreated desc";

            return TOpportunitySearch.SelectSearchResults(filter).ToSearchResult();
        }
        #endregion

        #region Helper Methods
        protected string GetDateString(DateTimeOffset? date)
        {
            if (date != null && date.HasValue)
                return "Posted on " + TimeZones.FormatDateOnly(date.Value, User.TimeZone, CultureInfo.GetCultureInfo(Identity.Language));
            return null;
        }

        protected bool CanEdit(Guid? oppprtunityEmployerId, Guid? personEmployerId)
        {
            if (oppprtunityEmployerId.HasValue && personEmployerId.HasValue)
                return oppprtunityEmployerId == personEmployerId.Value;

            return false;
        }


        protected string GetJobPositionAdditionalInfoHtml(string value, string label)
        {
            if (String.IsNullOrEmpty(value))
                return string.Empty;

            var html = new StringBuilder();

            html.AppendLine("<div class='form-text mt1'>");
            html.AppendLine("<div style='font-weight:bold;'>");
            html.AppendLine(label);
            html.AppendLine("</div>");
            html.AppendLine(value);
            html.AppendLine("</div>");

            return html.ToString();
        }

        protected string GetDescriptionAdditionalInfoHtml(string value, string label)
        {
            if (String.IsNullOrEmpty(value))
                return string.Empty;

            var html = new StringBuilder();

            html.AppendLine("<div class='form-text mt1'>");
            html.AppendLine(label);
            html.AppendLine("</div>");

            value = StringHelper.StripHtml(value);

            if (value.Length > 200)
            {
                html.AppendLine(value.Substring(0, 200));
                html.AppendLine("...");
            }
            else
                html.AppendLine(value);

            return html.ToString();
        }

        protected string GetLocation(string value1, string value2)
        {
            if (String.IsNullOrEmpty(value1))
                return string.Empty;

            if (String.IsNullOrEmpty(value2))
                return value1;

            return $"{value1} ({value2})";
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
        #endregion
    }
}