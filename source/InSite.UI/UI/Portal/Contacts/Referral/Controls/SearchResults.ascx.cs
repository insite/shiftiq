using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;
using InSite.Common;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

using AdminSearchResults = InSite.Admin.Contacts.People.Controls.SearchResults;

namespace InSite.UI.Portal.Contacts.Referral.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<PersonFilter>
    {
        private InSite.Admin.Contacts.People.Models.PersonSearchResultsModel _model;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowDataBound += Grid_RowDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var personCodeColumn = Grid.Columns.FindByName("PersonCode");
            personCodeColumn.HeaderText = LabelHelper.GetLabelContentText("Person Code");
        }

        private void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var dataItem = e.Row.DataItem;
            var userIdentifier = (Guid)DataBinder.Eval(dataItem, "UserIdentifier");
            var caseTitles = (System.Web.UI.WebControls.Literal)e.Row.FindControl("CaseTitles");
            if (caseTitles != null)
                caseTitles.Text = String.Join("<br/>", _model.GetCases(userIdentifier));

            var icon = (HtmlGenericControl)e.Row.FindControl("HasAttachment");
            icon.Visible = _model.HasDocuments(userIdentifier);
        }

        protected override int SelectCount(PersonFilter filter)
            => filter.EmployerGroups.Length > 0 ? PersonCriteria.Count(filter) : 0;

        protected override IListSource SelectData(PersonFilter filter)
        {
            if (filter.EmployerGroups.Length == 0)
                return null;

            var items = PersonCriteria.SelectForPortalSearch(filter);
            var userIds = items.Select(x => x.UserIdentifier).ToArray();

            _model = AdminSearchResults.CreateModel(userIds);

            if (Organization.Toolkits.Contacts.PortalSearchActiveMembershipReasons)
                ApplySearchActiveMembershipReasons(filter, userIds, items);

            return items.ToSearchResult();
        }

        private static void ApplySearchActiveMembershipReasons(PersonFilter filter, Guid[] userIds, PersonPortalSearchResultItem[] items)
        {
            var allMemberships = ServiceLocator.MembershipSearch
                .Select(
                    new QMembershipFilter
                    {
                        UserIdentifiers = userIds,
                        GroupIdentifiers = filter.EmployerGroups
                    },
                    x => x.Reasons)
                .GroupBy(x => x.UserIdentifier)
                .ToDictionary(x => x.Key, x => x.ToArray());

            foreach (var item in items)
            {
                var userMemberships = allMemberships.GetOrDefault(item.UserIdentifier);
                if (userMemberships == null)
                    continue;

                var dateEffective = userMemberships.SelectMany(x => x.Reasons)
                    .Where(x => x.ReasonExpiry > filter.MembershipReasonExpirySince.Value)
                    .OrderByDescending(x => x.ReasonEffective)
                    .Select(x => (DateTimeOffset?)x.ReasonEffective)
                    .FirstOrDefault();

                if (dateEffective.HasValue)
                    item.Referred = dateEffective.Value;
            }
        }
    }
}