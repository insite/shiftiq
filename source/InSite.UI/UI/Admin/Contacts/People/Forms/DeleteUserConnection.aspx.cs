using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.UI.Admin.Contacts.People.Forms
{
    public partial class DeleteUserConnection : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? FromKey => Guid.TryParse(Request["from"], out var value) ? value : (Guid?)null;

        private Guid? ToKey => Guid.TryParse(Request["to"], out var value) ? value : (Guid?)null;

        private Guid? FromIdentifier
        {
            get => ViewState[nameof(FromIdentifier)] as Guid?;
            set => ViewState[nameof(FromIdentifier)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var connection = FromKey.HasValue && ToKey.HasValue ? UserConnectionSearch.Select(FromKey.Value, ToKey.Value, x => x.FromUser, x => x.ToUser) : null;

                if (connection == null)
                    HttpResponseHelper.Redirect($"/ui/admin/contacts/people/search", true);

                FromIdentifier = connection.FromUser.UserIdentifier;

                PageHelper.AutoBindHeader(this, null, connection.FromUser.FullName);

                ConnectedOn.Text = ((DateTimeOffset?)connection.Connected).Format(User.TimeZone, nullValue: "None", isHtml: true);
                Relationship.Text = connection.IsManager ? "Manager" : connection.IsSupervisor ? "Supervisor" : connection.IsValidator ? "Validator" : "Not Assigned";

                var FromPerson = ServiceLocator.PersonSearch.GetPerson(FromKey.Value, Organization.Key, x => x.User);
                var ToPerson = ServiceLocator.PersonSearch.GetPerson(ToKey.Value, Organization.Key, x => x.User);

                FromPersonDetail.BindPerson(FromPerson, User.TimeZone);
                ToPersonDetail.BindPerson(ToPerson, User.TimeZone);

                CancelButton.NavigateUrl = $"/ui/admin/contacts/people/edit?contact={FromIdentifier}&panel=people";
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            var connection = UserConnectionSearch.Select(FromKey.Value, ToKey.Value);

            UserConnectionStore.Delete(connection);

            HttpResponseHelper.Redirect($"/ui/admin/contacts/people/edit?contact={FromIdentifier}&panel=people");
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/edit")
                ? $"contact={FromIdentifier}&panel=people"
                : null;
        }
    }
}