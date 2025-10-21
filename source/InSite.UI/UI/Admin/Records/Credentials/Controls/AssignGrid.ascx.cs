using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Linq;

using CheckBox = System.Web.UI.WebControls.CheckBox;

namespace InSite.UI.Admin.Records.Credentials.Controls
{
    public partial class AssignGrid : SearchResultsGridViewController<NullFilter>
    {
        protected override bool IsFinder => false;

        public ICollection<Guid> SelectedContacts => (ICollection<Guid>)(ViewState[nameof(SelectedContacts)]
                ?? (ViewState[nameof(SelectedContacts)] = new HashSet<Guid>()));

        private string ContactName
        {
            get => (string)ViewState[nameof(ContactName)];
            set => ViewState[nameof(ContactName)] = value;
        }

        private string Email
        {
            get => (string)ViewState[nameof(Email)];
            set => ViewState[nameof(Email)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowCreated += Grid_ItemCreated;
            Grid.RowDataBound += Grid_ItemDataBound;
        }

        private void Grid_ItemCreated(object sender, GridViewRowEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var isSelected = (CheckBox)e.Row.FindControl("IsSelected");
            isSelected.CheckedChanged += IsSelected_CheckedChanged;
        }

        private void Grid_ItemDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var id = (Guid)DataBinder.Eval(e.Row.DataItem, "UserIdentifier");

            var isSelected = (CheckBox)e.Row.FindControl("IsSelected");
            isSelected.Checked = SelectedContacts.Contains(id);
        }

        private void IsSelected_CheckedChanged(object sender, EventArgs e)
        {
            var chk = (CheckBox)sender;
            var row = (GridViewRow)chk.NamingContainer;
            var grid = (Grid)row.NamingContainer;
            var id = grid.GetDataKey<Guid>(row);

            if (!chk.Checked && SelectedContacts.Contains(id))
                SelectedContacts.Remove(id);
            else if (chk.Checked && !SelectedContacts.Contains(id))
                SelectedContacts.Add(id);
        }

        public int LoadData(string contactName, string email)
        {
            ContactName = contactName;
            Email = email;

            Search(new NullFilter());

            return RowCount;
        }

        protected override int SelectCount(NullFilter filter)
        {
            return ServiceLocator.PersonSearch.CountPersons(CreateFilter());
        }

        protected override IListSource SelectData(NullFilter inputFilter)
        {
            var filter = CreateFilter();
            filter.OrderBy = "User.FullName";
            filter.Paging = inputFilter.Paging;

            return ServiceLocator.PersonSearch.GetPersons(filter, x => x.User).Select(x => new
            {
                UserIdentifier = x.UserIdentifier,
                FullName = x.User.FullName,
                Email = x.User.Email,
                EmailAlternate = x.User.EmailAlternate,
                PersonCode = x.PersonCode,
            }).ToList().ToSearchResult();
        }

        private QPersonFilter CreateFilter()
        {
            return new QPersonFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                UserNameContains = ContactName,
                UserEmailContains = Email,
            };
        }
    }
}