using System;
using System.Web.UI;

using InSite.Application.Contacts.Read;
using InSite.Application.Memberships.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.UI.Admin.Contacts.MembershipReasons
{
    public partial class Create : AdminBasePage
    {
        private const string EditUrl = "/ui/admin/contacts/memberships/reasons/edit";
        private const string SearchUrl = "/ui/admin/contacts/memberships/reasons/search";

        private Guid? UserIdentifier =>
            Guid.TryParse(Request["user"], out Guid value) ? value : (Guid?)null;

        private Guid? _reasonId;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        private void Open()
        {
            if (!CanCreate)
                HttpResponseHelper.Redirect(SearchUrl);

            var user = UserIdentifier.HasValue
                ? UserSearch.Select(UserIdentifier.Value)
                : null;
            if (user == null || user.IsCloaked && !User.IsCloaked)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(Page, qualifier: user.FullName);

            ReasonDetail.SetDefaultInputValues(user.UserIdentifier);

            CancelButton.NavigateUrl = GetReturnUrl();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var reason = new QMembershipReason();

            ReasonDetail.GetInputValues(reason);

            _reasonId = UniqueIdentifier.Create();

            ServiceLocator.SendCommand(new AddMembershipReason(
                reason.MembershipIdentifier,
                _reasonId.Value,
                "Referral",
                reason.ReasonSubtype,
                reason.ReasonEffective,
                reason.ReasonExpiry,
                reason.PersonOccupation));

            HttpResponseHelper.Redirect(GetReturnUrl());
        }

        protected override string GetReturnUrl()
        {
            var result = base.GetReturnUrl();

            if (result.IsEmpty())
                result = _reasonId.HasValue ? EditUrl + $"?reason={_reasonId.Value}" : SearchUrl;

            return result;
        }
    }
}