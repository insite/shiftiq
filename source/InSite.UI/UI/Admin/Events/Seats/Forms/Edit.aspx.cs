using System;

using InSite.Application.Events.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Events.Seats.Forms
{
    public partial class Edit : AdminBasePage, IHasParentLinkParameters, IOverrideWebRouteParent
    {
        private Guid SeatIdentifier
            => Guid.TryParse(Request.QueryString["id"], out var value) ? value : Guid.Empty;

        string DefaultParameters => EventIdentifier.HasValue ? $"event={EventIdentifier}&panel=seats" : GetParentLinkParameters(null, null);

        private Guid? EventIdentifier => Guid.TryParse(Request["event"], out var result) ? result : (Guid?)null;

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/outline") ? $"{DefaultParameters}" : GetParentLinkParameters(parent, null);

        IWebRoute IOverrideWebRouteParent.GetParent() =>
            GetParent();

        private string BackUrl
        {
            get => (string)ViewState[nameof(BackUrl)];
            set => ViewState[nameof(BackUrl)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += (s, a) => { if (Page.IsValid) Save(); };
        }

        public override void ApplyAccessControl()
        {
            base.ApplyAccessControl();

            SaveButton.Visible = CanEdit;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (Request.QueryString["status"] == "saved")
                SetStatus(ScreenStatus, StatusType.Saved);

            Open();
        }

        private void Open()
        {
            var seat = ServiceLocator.EventSearch.GetSeat(SeatIdentifier);
            if (seat?.Event == null || seat.Event.OrganizationIdentifier != Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect(GetRedirectUrl());

            PageHelper.AutoBindHeader(
                Page,
                qualifier: seat.SeatTitle);

            Detail.SetInputValues(seat);

            CancelButton.NavigateUrl = GetRedirectUrl();
        }

        private void Save()
        {
            var seat = ServiceLocator.EventSearch.GetSeat(SeatIdentifier);

            Detail.GetInputValues(seat);

            var command = new ReviseSeat(seat.EventIdentifier, seat.SeatIdentifier, seat.Configuration, seat.Content, seat.IsAvailable, seat.IsTaxable, seat.OrderSequence, seat.SeatTitle);
            ServiceLocator.SendCommand(command);

            HttpResponseHelper.Redirect(GetRedirectUrl());
        }

        private string GetRedirectUrl() => GetParentUrl(null);
    }
}