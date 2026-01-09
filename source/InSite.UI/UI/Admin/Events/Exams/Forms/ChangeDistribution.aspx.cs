using System;

using Humanizer;

using InSite.Application.Events.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Events.Exams.Forms
{
    public partial class ChangeDistribution : AdminBasePage, IHasParentLinkParameters
    {
        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/outline") ? $"event={EventIdentifier}" : null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Identity.IsGranted(Route.ToolkitName, PermissionOperation.Write))
                NavigateToSearch();

            if (!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            var ev = GetEvent();
            if (ev == null || ev.OrganizationIdentifier != Organization.OrganizationIdentifier)
                NavigateToSearch();

            DistributionProcessInput.LoadItems(DistributionProcess.Items, "Value", "Text");

            BackButton.NavigateUrl = OutlineUrl;

            PageHelper.AutoBindHeader(this, null, ev.EventTitle);

            EventScheduledStart.Text = ev.EventScheduledStart.Format(null, true);
            ExamStarted.Value = ev.ExamStarted;
            ScheduleTimer.Text = ev.ExamMaterialReturnShipmentReceived == null
                ? ev.EventScheduledStart.Humanize()
                : (ev.ExamMaterialReturnShipmentReceived.Value - ev.EventScheduledStart).Humanize();

            var isNotOnline = ev.EventFormat != EventExamFormat.Online.Value;
            if (isNotOnline)
            {
                DistributionOrdered.Value = ev.DistributionOrdered;
                DistributionExpected.Value = ev.DistributionExpected;
                DistributionShipped.Value = ev.DistributionShipped;
                DistributionProcessInput.Value = ev.DistributionProcess;
            }

            DistributionOrderedField.Visible = isNotOnline;
            DistributionExpectedField.Visible = isNotOnline;
            DistributionShippedField.Visible = isNotOnline;
            DistributionProcessField.Visible = isNotOnline;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var @event = GetEvent();
            var command = new Application.Events.Write.ChangeDistribution(
                @event.EventIdentifier,
                @event.DistributionProcess,
                @event.DistributionOrdered,
                @event.DistributionExpected,
                @event.DistributionShipped,
                ExamStarted.Value);

            if (@event.EventFormat != EventExamFormat.Online.Value)
            {
                command.DistributionProcess = DistributionProcessInput.Value;
                command.DistributionOrdered = DistributionOrdered.Value;
                command.DistributionExpected = DistributionExpected.Value
                    ?? QEvent.GetDefaultDistributionExpected(@event.EventScheduledStart);
                command.DistributionShipped = DistributionShipped.Value;
            }

            var isChanged = command.AttemptStarted != @event.ExamStarted
                || command.DistributionProcess.NullIfEmpty() != @event.DistributionProcess.NullIfEmpty()
                || command.DistributionOrdered != @event.DistributionOrdered
                || command.DistributionExpected != @event.DistributionExpected
                || command.DistributionShipped != @event.DistributionShipped;

            if (isChanged)
                ServiceLocator.SendCommand(command);

            HttpResponseHelper.Redirect(OutlineUrl);
        }
    }
}
