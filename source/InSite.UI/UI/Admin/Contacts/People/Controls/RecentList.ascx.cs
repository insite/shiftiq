using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Admin.Contacts.People.Controls
{
    public partial class RecentList : BaseUserControl
    {
        public int ItemCount
        {
            get => (int?)ViewState[nameof(ItemCount)] ?? 0;
            set => ViewState[nameof(ItemCount)] = value;
        }

        public void LoadData(int count)
        {
            var users = UserSearch.SelectSearchResults("Modified DESC", Paging.SetSkipTake(0, count), Organization.OrganizationIdentifier);
            ItemCount = users.GetList().Count;

            PersonRepeater.DataSource = users;
            PersonRepeater.DataBind();
        }

        protected static string GetTimestampHtml(object id)
        {
            var person = ServiceLocator.PersonSearch.GetPerson((Guid)id, Organization.Identifier);
            return person != null
                ? UserSearch.GetTimestampHtml(person.ModifiedBy, "person", "changed", person.Modified)
                : string.Empty;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            PersonRepeater.ItemDataBound += Repeater_ItemDataBound;
        }

        private void Repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var item = e.Item.DataItem;

            var address = (string)DataBinder.Eval(item, "Email");
            var emailEnabled = (bool)DataBinder.Eval(item, "EmailEnabled");

            var email = (HyperLink)e.Item.FindControl("Email");
            email.NavigateUrl = SearchResults.CreateMailToLink(address);
            email.Text = address;
            email.Enabled = emailEnabled;
            if (!emailEnabled)
                email.CssClass = "email-disabled";
        }
    }
}