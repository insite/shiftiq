using System;
using System.Web.UI.WebControls;

using InSite.Application.Events.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Events.Classes.Forms
{
    public partial class PublishForm : AdminBasePage, IHasParentLinkParameters
    {
        private const string SearchUrl = "/ui/admin/events/classes/search";

        private Guid EventIdentifier
            => Guid.TryParse(Request.QueryString["event"], out var value) ? value : Guid.Empty;

        private string OutlineUrl
            => $"/ui/admin/events/classes/outline?event={EventIdentifier}&panel=class";

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/outline") ? $"event={EventIdentifier}&panel=class" : null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PublishButton.Click += (s, a) => Publish();
            UnpublishButton.Click += (s, a) => Unpublish();

            RegistrationDateValidator.ServerValidate += RegistrationDateValidator_ServerValidate;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!CanEdit)
                HttpResponseHelper.Redirect(SearchUrl);

            Open();
        }

        private void Open()
        {
            var @event = ServiceLocator.EventSearch.GetEvent(EventIdentifier, x => x.VenueLocation);
            if (@event == null || @event.OrganizationIdentifier != Organization.Identifier)
                HttpResponseHelper.Redirect(SearchUrl);

            var isPublished = string.Equals(@event.EventPublicationStatus, PublicationStatus.Published.GetDescription(), StringComparison.OrdinalIgnoreCase);
            var isCancelled = string.Equals(@event.EventSchedulingStatus, "Cancelled", StringComparison.OrdinalIgnoreCase);
            var allowPublish = !isPublished && !isCancelled;

            PageHelper.AutoBindHeader(
                Page,
                qualifier: $"{@event.EventTitle} <span class='form-text'>scheduled to start {@event.EventScheduledStart.FormatDateOnly(User.TimeZone)}</span>");

            SummaryInfo.Bind(@event);
            LocationInfo.Bind(@event);

            ClassCalendarColor.Value = @event.EventCalendarColor ?? "Primary";

            PublishButton.Visible = allowPublish;
            UnpublishButton.Visible = isPublished;

            RegistrationStartField.Visible = allowPublish;

            RegistrationDeadline.Value = @event.RegistrationDeadline;
            RegistrationDeadlineField.Visible = allowPublish;

            CancelButton.NavigateUrl = OutlineUrl;
        }

        private void Publish()
        {
            if (!Page.IsValid)
                return;

            ServiceLocator.SendCommand(new PublishEvent(EventIdentifier, RegistrationStart.Value, RegistrationDeadline.Value));
            ServiceLocator.SendCommand(new ModifyEventCalendarColor(EventIdentifier, ClassCalendarColor.Value));

            HttpResponseHelper.Redirect(OutlineUrl);
        }

        private void Unpublish()
        {
            if (!Page.IsValid)
                return;

            ServiceLocator.SendCommand(new UnpublishEvent(EventIdentifier));

            HttpResponseHelper.Redirect(OutlineUrl);
        }

        private void RegistrationDateValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = RegistrationStart.Value == null
                || RegistrationDeadline.Value == null
                || RegistrationStart.Value < RegistrationDeadline.Value;
        }
    }
}