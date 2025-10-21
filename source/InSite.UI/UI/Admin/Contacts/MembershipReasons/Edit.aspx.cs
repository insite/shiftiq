using System;
using System.Web.UI;

using InSite.Application.Memberships.Write;
using InSite.Common.Web;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.UI.Admin.Contacts.MembershipReasons
{
    public partial class Edit : AdminBasePage
    {
        private const string EditUrl = "/ui/admin/contacts/memberships/reasons/edit";
        private const string SearchUrl = "/ui/admin/contacts/memberships/reasons/search";
        private const string DeleteUrl = "/ui/admin/contacts/memberships/reasons/delete";

        private Guid ReasonId => Guid.TryParse(Request["reason"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        public override void ApplyAccessControl()
        {
            base.ApplyAccessControl();

            SaveButton.Visible = CanEdit;
            DeleteButton.Visible = CanDelete;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (Request.QueryString["status"] == "saved")
                SetStatus(ScreenStatus, StatusType.Saved);

            Open();

            DeleteButton.NavigateUrl = new ReturnUrl().GetRedirectUrl(DeleteUrl + $"?reason={ReasonId}");
            CancelButton.NavigateUrl = GetReturnUrl();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var reason = ServiceLocator.MembershipReasonSearch.Select(ReasonId);

            ReasonDetail.GetInputValues(reason);

            ServiceLocator.SendCommand(new ModifyMembershipReason(
                reason.MembershipIdentifier,
                reason.ReasonIdentifier,
                reason.ReasonType,
                reason.ReasonSubtype,
                reason.ReasonEffective,
                reason.ReasonExpiry,
                reason.PersonOccupation));

            HttpResponseHelper.Redirect(GetReturnUrl());
        }

        private void Open()
        {
            var reason = ServiceLocator.MembershipReasonSearch.Select(ReasonId, x => x.Membership.Group, x => x.Membership.User);
            if (reason == null || reason.Membership.OrganizationIdentifier != Organization.Identifier || reason.Membership.Group.OrganizationIdentifier != Organization.Identifier)
                HttpResponseHelper.Redirect(GetReturnUrl());

            PageHelper.AutoBindHeader(Page, qualifier: reason.Membership.User.FullName);

            ReasonDetail.SetInputValues(reason);
        }

        protected override string GetReturnUrl()
        {
            return base.GetReturnUrl().IfNullOrEmpty(SearchUrl);
        }
    }
}