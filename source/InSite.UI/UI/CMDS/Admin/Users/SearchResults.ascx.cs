using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common.Linq;

using Core = InSite.Persistence;

namespace InSite.Cmds.Controls.Contacts.Persons
{
    public partial class PersonSearchResults : SearchResultsGridViewController<CmdsPersonFilter>
    {
        private class DataItem
        {
            public Guid UserIdentifier { get; set; }
            public string Name { get; set; }
            public string Organization { get; set; }
            public bool EmailEnabled { get; set; }
            public string Email { get; set; }
            public DateTimeOffset? LastAuthenticated { get; set; }
            public bool HasPrimaryProfile { get; set; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowDataBound += Grid_RowDataBound;
        }

        private void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            var item = (DataItem)e.Row.DataItem;

            var email = item.Email;
            var icon = "fa-solid fa-circle-check";
            var indicator = "text-success";
            var tooltip = "Email notification is enabled";

            if (!item.EmailEnabled)
            {
                icon = "fa-solid fa-circle-xmark";
                indicator = "text-danger";
                tooltip = "Email notification is disabled";
            }

            var control = (System.Web.UI.WebControls.Literal)e.Row.FindControl("Email");

            control.Text = $"<span title='{tooltip}'><i class='{indicator} {icon} me-1'></i>{email}</span>";

            control.Visible = item.Email != item.UserIdentifier.ToString().Substring(0, 8) + "@keyeracmds.com";
        }

        protected override int SelectCount(CmdsPersonFilter filter)
        {
            return ContactRepository3.CountSearchResults(filter);
        }

        protected override IListSource SelectData(CmdsPersonFilter filter)
        {
            var items = ContactRepository3
                .SelectSearchResults(filter, Organization.Identifier)
                .AsEnumerable()
                .Select(x => new DataItem
                {
                    UserIdentifier = (Guid)x["UserIdentifier"],
                    Name = (string)x["FullName"],
                    Organization = GetCompanyName((Guid)x["UserIdentifier"], x["CompanyMode"] as int?, x["CompanyName"] as string),
                    EmailEnabled = (bool)x["EmailEnabled"],
                    Email = x["Email"] as string,
                    LastAuthenticated = x["LastAuthenticated"] as DateTimeOffset?,
                    HasPrimaryProfile = (bool)x["HasPrimaryProfile"]
                })
                .ToList();

            foreach (var item in items)
            {
                var pattern = @"^([A-Za-z]+)\s([A-Za-z]+)\s\[(.*)\]$";

                var match = Regex.Match(item.Name, pattern);

                if (match.Success)
                {
                    var firstName = match.Groups[1].Value;
                    var lastName = match.Groups[2].Value;
                    var employeeType = match.Groups[3].Value;
                    item.Name = $"{firstName} {lastName} <span class='form-text'>[{employeeType}]</span>";
                }
            }

            return items.ToSearchResult();
        }

        private static string GetCompanyName(Guid userKey, int? companyMode, string companyName)
        {
            if (companyMode.HasValue)
            {
                if (companyMode == 1)
                    return companyName;

                return companyMode == 0 ? "N/A" : "(Multiple Organizations)";
            }

            DataTable table = ContactRepository3.SelectPersonOrganizations(userKey);

            if (table.Rows.Count == 0)
                return "N/A";

            return table.Rows.Count == 1 ? (string)table.Rows[0]["CompanyTitle"] : "(Multiple Organizations)";
        }

        protected string GetProfilesCount(object o)
        {
            var item = (DataItem)o;

            var userKey = item.UserIdentifier;

            var n = Core.DepartmentProfileUserSearch.Count(
                x => x.UserIdentifier == userKey &&
                x.Profile.StandardType == "Profile");

            var hasPrimaryProfile = item.HasPrimaryProfile;

            var textIndicator = hasPrimaryProfile
                ? "text-success"
                : "text-warning";

            var tooltip = hasPrimaryProfile
                ? ""
                : "Primary profile is not fully specified";

            var icon = hasPrimaryProfile
                ? ""
                : "<i class='fa-solid fa-exclamation-triangle me-2'></i>";

            var html = $"<a class='{textIndicator}' href='/ui/cmds/portal/validations/profiles/search?userid={userKey}'>" +
                $"<span class='{textIndicator}' title='{tooltip}'>" +
                $"{icon}{n}" +
                "</span></a>";

            return html;
        }
    }
}