using System;

using InSite.Application.Memberships.Write;
using InSite.Common.Web;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.UI.Admin.Contacts.MembershipReasons
{
    public partial class Delete : AdminBasePage
    {
        private const string EditUrl = "/ui/admin/contacts/memberships/reasons/edit";
        private const string SearchUrl = "/ui/admin/contacts/memberships/reasons/search";

        private Guid ReasonId => Guid.TryParse(Request["reason"], out var value) ? value : Guid.Empty;

        private Guid MembershipId
        {
            get => (Guid)ViewState[nameof(MembershipId)];
            set => ViewState[nameof(MembershipId)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!CanDelete)
                HttpResponseHelper.Redirect(SearchUrl);

            LoadData();

            CancelButton.NavigateUrl = GetReturnUrl().IfNullOrEmpty(EditUrl + $"?reason={ReasonId}");
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            ServiceLocator.SendCommand(new RemoveMembershipReason(MembershipId, ReasonId));

            HttpResponseHelper.Redirect(GetReturnUrl().IfNullOrEmpty(SearchUrl));
        }

        private void LoadData()
        {
            var reason = ServiceLocator.MembershipReasonSearch.Select(ReasonId, x => x.Membership.Group, x => x.Membership.User);
            if (reason == null || reason.Membership.OrganizationIdentifier != Organization.Identifier || reason.Membership.Group.OrganizationIdentifier != Organization.Identifier)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(Page, qualifier: reason.Membership.User.FullName);

            MembershipId = reason.MembershipIdentifier;

            MembershipGroupName.Text = reason.Membership.Group.GroupName;
            ReasonType.Text = reason.ReasonType;
            ReasonSubtype.Text = reason.ReasonSubtype;
            ReasonSubtypeField.Visible = reason.ReasonSubtype.IsNotEmpty();
            ReasonEffectiveDate.Text = reason.ReasonEffective.Format(User.TimeZone, true);
            ReasonExpiryDate.Text = reason.ReasonExpiry.Format(User.TimeZone, true);
            ReasonExpiryDateField.Visible = reason.ReasonExpiry.HasValue;
        }
    }
}