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

using CheckBox = InSite.Common.Web.UI.CheckBox;

namespace InSite.UI.Portal.Jobs.Employers.MyProfile.Controls
{
    public partial class MyJobOpportunityGrid : SearchResultsGridViewController<TOpportunityFilter>
    {
        protected override bool IsFinder => false;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowCreated += Grid_RowCreated;
            Grid.RowCommand += Grid_RowCommand;
        }

        public void LoadData(Guid employerGroupIdentifier)
        {
            var filter = new TOpportunityFilter
            {
                EmployerGroupIdentifier = employerGroupIdentifier,
                IsArchived = false
            };

            Search(filter);

            Count.Text = $"{RowCount:n0}";
            AddNewLink.NavigateUrl = $"/ui/portal/job/employers/opportunities/create?group={employerGroupIdentifier}";
        }

        protected override int SelectCount(TOpportunityFilter filter)
            => TOpportunitySearch.Count(filter);

        protected override IListSource SelectData(TOpportunityFilter filter)
        {
            return TOpportunitySearch.SelectSearchResults(filter)
                .Select(x => new
                {
                    LocationType =
                    (
                        x.LocationType == "Remote" ? "Fully remote" :
                        x.LocationType == "Potential Remote" ? "Potential for some remote" :
                        x.LocationType == "Not Remote" ? "Fully on site" : ""
                    ),
                    x.JobTitle,
                    x.EmploymentType,
                    x.WhenPublished,
                    x.WhenCreated,
                    WhenModified = x.WhenModified ?? x.WhenCreated,
                    x.LocationName,
                    x.EmployerGroupName,
                    x.JobDescription,
                    x.WhenClosed,
                    x.SalaryMaximum,
                    x.SalaryMinimum,
                    x.SalaryOther,
                    x.JobLevel,
                    x.OpportunityIdentifier
                })
                .ToList()
                .ToSearchResult();
        }


        private void Grid_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var published = (CheckBox)e.Row.FindControl("Published");
            published.AutoPostBack = true;
            published.CheckedChanged += T2202_CheckedChanged;
        }

        private void Grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!Guid.TryParse(e.CommandArgument.ToString(), out var opportunityId))
                return;

            var opportunity = TOpportunitySearch.Select(opportunityId);
            if (opportunity != null)
            {
                if (e.CommandName == "DeleteJob")
                    DeleteOpportunity(opportunity);
                else
                {
                    switch (e.CommandName)
                    {
                        case "DeleteJob":
                            DeleteOpportunity(opportunity);
                            break;
                        case "Reopen":
                            opportunity.WhenClosed = null;
                            break;
                        case "Close":
                            opportunity.WhenClosed = DateTime.UtcNow;
                            break;
                        case "Publish":
                            opportunity.WhenPublished = DateTime.UtcNow;
                            break;
                        case "Unpublish":
                            opportunity.WhenPublished = null;
                            break;
                    }

                    TOpportunityStore.Update(opportunity);
                }
            }

            Search(Filter);
        }

        private void T2202_CheckedChanged(object sender, EventArgs e)
        {
            var published = (CheckBox)sender;
            var row = (GridViewRow)published.NamingContainer;
            var publishedOn = (System.Web.UI.WebControls.Literal)row.FindControl("PublishedOn");
            var grid = (Grid)row.NamingContainer;
            var opportunityIdentifier = grid.GetDataKey<Guid>(row);

            var opportunity = TOpportunitySearch.Select(opportunityIdentifier);

            if (published.Checked)
                opportunity.WhenPublished = DateTimeOffset.UtcNow;
            else
                opportunity.WhenPublished = null;

            publishedOn.Text = GetDateString("Published on", opportunity.WhenPublished);
            TOpportunityStore.Update(opportunity);
        }

        private void DeleteOpportunity(TOpportunity opportunity)
        {
            TApplicationStore.DeleteJByOpportunity(opportunity.OpportunityIdentifier);
            TOpportunityStore.Delete(opportunity.OpportunityIdentifier);
        }

        #region Helper Methods

        protected string GetDateString(string label, DateTimeOffset? date)
        {
            if (date != null && date.HasValue)
                return $"{label} " + TimeZones.FormatDateOnly(date.Value, User.TimeZone, CultureInfo.GetCultureInfo(Identity.Language));
            return null;
        }

        protected string GetRedirectUrl(Guid id)
        {
            return new ReturnUrl()
                .GetRedirectUrl($"/ui/portal/job/employers/opportunities/view?id={id}");
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