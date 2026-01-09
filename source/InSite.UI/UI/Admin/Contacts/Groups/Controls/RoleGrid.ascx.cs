using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;
using InSite.Common;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Web.Security;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

using Label = System.Web.UI.WebControls.Label;

namespace InSite.Admin.Contacts.Groups.Controls
{
    public partial class RoleGrid : SearchResultsGridViewController<NullFilter>
    {
        #region Classes

        private class MailingListLabel
        {
            public int RowNumber { get; set; }
            public string Col1_FirstName { get; set; }
            public string Col1_LastName { get; set; }
            public string Col1_Street1 { get; set; }
            public string Col1_Street2 { get; set; }
            public string Col1_City { get; set; }
            public string Col1_Province { get; set; }
            public string Col1_PostalCode { get; set; }
            public string Col2_FirstName { get; set; }
            public string Col2_LastName { get; set; }
            public string Col2_Street1 { get; set; }
            public string Col2_Street2 { get; set; }
            public string Col2_City { get; set; }
            public string Col2_Province { get; set; }
            public string Col2_PostalCode { get; set; }
            public string Col3_FirstName { get; set; }
            public string Col3_LastName { get; set; }
            public string Col3_Street1 { get; set; }
            public string Col3_Street2 { get; set; }
            public string Col3_City { get; set; }
            public string Col3_Province { get; set; }
            public string Col3_PostalCode { get; set; }
        }

        #endregion

        #region Events

        public event EventHandler Refreshed;

        private void OnRefreshed() =>
            Refreshed?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Properties

        protected override bool IsFinder => false;

        protected Guid GroupIdentifier
        {
            get => (Guid)(ViewState[nameof(GroupIdentifier)] ?? Guid.Empty);
            set => ViewState[nameof(GroupIdentifier)] = value;
        }

        protected string GroupType
        {
            get => (string)(ViewState[nameof(GroupType)] ?? string.Empty);
            set => ViewState[nameof(GroupType)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowDeleting += Grid_DeleteCommand;
            Grid.RowDataBound += Grid_ItemDataBound;

            SortType.AutoPostBack = true;
            SortType.ValueChanged += SortType_ValueChanged;

            FilterButton.Click += FilterButton_Click;

            DownloadBtn.Click += DownloadBtn_Click;
        }

        #endregion

        #region Public methods

        public void LoadData(QGroup group)
        {
            GroupIdentifier = group.GroupIdentifier;
            GroupType = group.GroupType;

            Search(new NullFilter());

            AddButton.NavigateUrl = $"/ui/admin/contacts/groups/create-membership?group={group.GroupIdentifier}";
            AddButton.Visible = MembershipPermissionHelper.CanModifyMembership(group);
        }

        public void Clear()
        {
            Clear(new NullFilter());
        }

        #endregion

        #region Event handlers

        private void SortType_ValueChanged(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            RefreshGrid();

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(RoleGrid),
                "filter_focus",
                $"inSite.common.baseInput.focus('{FilterTextBox.ClientID}', true);",
                true);
        }

        private void Grid_DeleteCommand(object sender, GridViewDeleteEventArgs e)
        {
            var grid = (Grid)sender;
            var user = grid.GetDataKey<Guid>(e);

            MembershipStore.Delete(MembershipSearch.Select(GroupIdentifier, user));

            SearchWithCurrentPageIndex(new NullFilter());

            OnRefreshed();
        }

        protected void Grid_ItemDataBound(object sender, GridViewRowEventArgs e)
        {
            SetContentLabelHeaders(e);

            if (!IsContentItem(e))
                return;

            var row = e.Row.DataItem;

            var link = (HyperLink)e.Row.FindControl("EmailLink");
            var label = (Label)e.Row.FindControl("EmailLabel");

            var address = (string)DataBinder.Eval(row, "Email");
            var enabled = (bool)DataBinder.Eval(row, "EmailEnabled");

            label.Text = address;
            link.Text = address;
            link.NavigateUrl = !string.IsNullOrEmpty(address) ? $"mailto:{address}" : null;

            label.Visible = !enabled;
            link.Visible = enabled;

            var membership = (Guid)DataBinder.Eval(row, "MembershipIdentifier");
            var history = (IconLink)e.Row.FindControl("HistoryLink");
            var returnUrl = $"/ui/admin/contacts/groups/edit?contact={GroupIdentifier}&panel=people";
            history.NavigateUrl = Logs.Aggregates.Outline.GetUrl(membership, returnUrl);

            var edit = (IconLink)e.Row.FindControl("EditButton");
            edit.NavigateUrl += "&returnURL=" + HttpUtility.UrlEncode(returnUrl);
        }

        private void DownloadBtn_Click(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "Contacts")
            {
                DownloadContacts();
            }
            else if (e.CommandName == "ShippingMailingLabels")
            {
                var labels = GetMailingLabels(
                    x => x.User.Persons.Select(y => y.ShippingAddress),
                    x => x.User.Persons.FirstOrDefault(y => y.OrganizationIdentifier == Organization.Identifier).ShippingAddress);

                DownloadMailingLabels(labels);
            }
            else if (e.CommandName == "BillingMailingLabels")
            {
                var labels = GetMailingLabels(
                    x => x.User.Persons.Select(y => y.BillingAddress),
                    x => x.User.Persons.FirstOrDefault(y => y.OrganizationIdentifier == Organization.Identifier).BillingAddress);

                DownloadMailingLabels(labels);
            }
            else if (e.CommandName == "WorkMailingLabels")
            {
                var labels = GetMailingLabels(
                    x => x.User.Persons.Select(y => y.WorkAddress),
                    x => x.User.Persons.FirstOrDefault(y => y.OrganizationIdentifier == Organization.Identifier).WorkAddress);

                DownloadMailingLabels(labels);
            }
            else if (e.CommandName == "HomeMailingLabels")
            {
                var labels = GetMailingLabels(
                    x => x.User.Persons.Select(y => y.HomeAddress),
                    x => x.User.Persons.FirstOrDefault(y => y.OrganizationIdentifier == Organization.Identifier).HomeAddress);

                DownloadMailingLabels(labels);
            }
        }

        #endregion

        #region Search results

        public int GetAllRowsCount()
        {
            return MembershipSearch.Count(FilterQueryWithoutKeyword());
        }

        protected override int SelectCount(NullFilter filter)
        {
            var count = MembershipSearch.Count(FilterQuery());

            DownloadBtn.Visible = count > 0;

            return count;
        }

        protected override IListSource SelectData(NullFilter filter)
        {
            var sortExpression = GetSortExpression();

            return MembershipSearch
                .Bind(
                    x => new
                    {
                        x.MembershipIdentifier,
                        x.User.UserIdentifier,
                        x.User.FullName,
                        x.User.LastName,
                        x.User.Email,
                        x.MembershipType,
                        HasMembershipType = x.MembershipType != null,
                        x.Assigned,
                        x.MembershipExpiry,
                        x.Modified,
                        x.ModifiedBy,
                        Person = x.User.Persons.FirstOrDefault(y => y.OrganizationIdentifier == Organization.OrganizationIdentifier)
                    },
                    FilterQuery(),
                    filter.Paging,
                    sortExpression
                )
                .Select(x => new
                {
                    MembershipIdentifier = x.MembershipIdentifier,
                    UserIdentifier = x.UserIdentifier,
                    Name = x.FullName,
                    Email = x.Email,
                    AccountNumber = x.Person?.PersonCode,
                    EmailEnabled = x.Person?.EmailEnabled ?? false,
                    RoleType = x.MembershipType,
                    Assigned = x.Assigned,
                    MembershipExpiry = x.MembershipExpiry,
                    Modified = x.Modified,
                    ModifiedBy = x.ModifiedBy,
                })
                .ToList()
                .ToSearchResult();
        }

        private string GetSortExpression()
        {
            switch (SortType.Value)
            {
                case "ByFullName": return "FullName";
                case "ByEffectiveDate": return "Assigned DESC";
                case "ByMembershipFunction": return "HasMembershipType DESC,MembershipType,FullName";
                case "ByLastName": return "LastName,FullName";
                default: throw ApplicationError.Create("Unexpected sort type value: {0}", SortType.Value.IfNullOrEmpty("NULL"));
            }
        }

        private Expression<Func<Membership, bool>> FilterQuery()
        {
            if (string.IsNullOrEmpty(FilterTextBox.Text))
                return FilterQueryWithoutKeyword();

            var keyword = FilterTextBox.Text;

            Expression<Func<Membership, bool>> where = x =>
                x.GroupIdentifier == GroupIdentifier
                && (
                        x.User.FullName.Contains(keyword)
                    || x.User.Email.Contains(keyword)
                    || x.User.Persons.FirstOrDefault(y => y.OrganizationIdentifier == Organization.OrganizationIdentifier).PersonCode.Contains(keyword)
                )
            ;

            return where;
        }

        private Expression<Func<Membership, bool>> FilterQueryWithoutKeyword()
        {
            Expression<Func<Membership, bool>> where = x => x.GroupIdentifier == GroupIdentifier;
            return where;
        }

        #endregion

        #region Download

        private void DownloadContacts()
        {
            var info = ServiceLocator.GroupSearch.GetGroup(GroupIdentifier);
            var list = GetExportData().GetList();

            if (list.Count == 0)
                return;

            var modifierIds = list.Cast<object>()
                .Select(x => (Guid)DataBinder.Eval(x, "ModifiedBy"))
                .Distinct()
                .ToArray();
            var modifierUsers = UserSearch.Select(new UserFilter { IncludeUserIdentifiers = modifierIds })
                .ToDictionary(x => x.UserIdentifier);

            var filename = $"{StringHelper.Sanitize(info.GroupName, '-')}";
            var helper = new CsvExportHelper(list);

            helper.AddMapping("Name", "Full Name");
            helper.AddMapping("Email", "Email");
            helper.AddMapping("AccountNumber", LabelHelper.GetLabelContentText("Person Code"));
            helper.AddMapping("RoleType", "Function");
            helper.AddMapping("Assigned", "Effective Date", "{0:MMM d, yyyy} {0:h:mm tt}");
            helper.AddMapping("MembershipExpiry", "Expiry Date", "{0:MMM d, yyyy} {0:h:mm tt}");
            helper.AddMapping("Modified", "Modified", "{0:MMM d, yyyy} {0:h:mm tt}");
            helper.AddMapping("ModifiedBy", "Modified By", parts =>
            {
                var id = (Guid)parts[0];
                if (modifierUsers.TryGetValue(id, out var user))
                    return user.FullName;

                return UserNames.Someone;
            });

            var bytes = helper.GetBytes(Encoding.UTF8);

            Page.Response.SendFile(filename, "csv", bytes);
        }

        private void DownloadMailingLabels(List<MailingListLabel> labels)
        {
            PostRequest(labels);
        }

        private List<MailingListLabel> GetMailingLabels(Expression<Func<Membership, object>> addressInclude, Expression<Func<Membership, object>> addressExpression)
        {
            var result = new List<MailingListLabel>();
            var members = MembershipSearch.Select(x => x.GroupIdentifier == GroupIdentifier, addressInclude);

            MailingListLabel item = null;
            int index = 0;

            foreach (var row in members)
            {
                int rowNumber = index / 3;
                int colNumber = index % 3;
                var user = row.User;
                var address = (Address)addressExpression.Invoke(row);

                if (colNumber == 0)
                {
                    result.Add(item = new MailingListLabel());

                    item.Col1_FirstName = user.FirstName;
                    item.Col1_LastName = user.LastName;

                    if (address != null)
                    {
                        item.Col1_Street1 = address.Street1;
                        item.Col1_Street2 = address.Street2;
                        item.Col1_City = address.City;
                        item.Col1_Province = address.Province;
                        item.Col1_PostalCode = address.PostalCode;
                    }
                }
                else if (colNumber == 1)
                {
                    item.Col2_FirstName = user.FirstName;
                    item.Col2_LastName = user.LastName;

                    if (address != null)
                    {
                        item.Col2_Street1 = address.Street1;
                        item.Col2_Street2 = address.Street2;
                        item.Col2_City = address.City;
                        item.Col2_Province = address.Province;
                        item.Col2_PostalCode = address.PostalCode;
                    }
                }
                else
                {
                    item.Col3_FirstName = user.FirstName;
                    item.Col3_LastName = user.LastName;

                    if (address != null)
                    {
                        item.Col3_Street1 = address.Street1;
                        item.Col3_Street2 = address.Street2;
                        item.Col3_City = address.City;
                        item.Col3_Province = address.Province;
                        item.Col3_PostalCode = address.PostalCode;
                    }
                }

                item.RowNumber = rowNumber;

                index++;
            }

            return result;
        }

        #endregion

        private void PostRequest(List<MailingListLabel> labels)
        {
            var json = JsonConvert.SerializeObject(labels);

            var encoded = StringHelper.EncodeBase64Url(json);

            CurrentSessionState.ContactGroupEnvelopes = encoded;

            HttpResponseHelper.Redirect($"~/UI/Admin/Contacts/Reports/Envelopes/MailingListLabels.aspx");
        }

        private void SetContentLabelHeaders(GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (e.Row.Cells[i].Text == "Person Code")
                        e.Row.Cells[i].Text = LabelHelper.GetLabelContentText("Person Code");
                }
            }
        }
    }
}