using System;

using Shift.Common.Timeline.Commands;

using InSite.Admin.Logs.Commands.Utilities;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Logs.Commands.Forms
{
    public partial class Create : AdminBasePage, IHasParentLinkParameters
    {
        private const string EditUrl = "/ui/admin/logs/commands/edit";
        private const string SearchUrl = "/ui/admin/logs/commands/search";

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            StatusComboBox.AutoPostBack = true;
            StatusComboBox.ValueChanged += StatusComboBox_ValueChanged;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanEdit)
                HttpResponseHelper.Redirect(SearchUrl);

            if (!IsPostBack)
            {
                CancelButton.NavigateUrl = GetBackUrl();

                PageHelper.AutoBindHeader(this);

                OrganizationIdentifier.Value = Organization.Identifier;
                UserIdentifier.Value = User.UserIdentifier;

                CommandDetails.SetOrganizationIdentifier(Organization.Identifier);

                if (Guid.TryParse(Request.QueryString["command"], out var sourceId))
                {
                    var sourceCommand = ServiceLocator.CommandSearch.Get(sourceId);
                    if (sourceCommand != null)
                        CommandDetails.SetInputValues(sourceCommand);
                }
            }
        }

        #endregion

        #region Event handlers

        private void StatusComboBox_ValueChanged(object sender, EventArgs e) => OnActionChanged();

        private void OnActionChanged()
        {
            var status = CommandHelper.GetStatus(StatusComboBox.Value);

            BookmarkExpiredField.Visible = status == Shift.Sdk.UI.StatusType.Bookmarked;
            BookmarkExpired.Value = null;

            ScheduledDateField.Visible = status == Shift.Sdk.UI.StatusType.Scheduled;
            ScheduledDate.Value = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow.AddMinutes(2), User.TimeZone);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var command = new SerializedCommand
            {
                CommandIdentifier = UniqueIdentifier.Create(),
                OriginOrganization = OrganizationIdentifier.Value.Value,
                OriginUser = UserIdentifier.Value.Value,
                SendStatus = StatusComboBox.Value,
            };

            CommandDetails.GetInputValues(command);
            RecurrenceField.GetInputValues(command);

            var status = CommandHelper.GetStatus(command.SendStatus);
            if (status == Shift.Sdk.UI.StatusType.Scheduled)
            {
                command.SendScheduled = ScheduledDate.Value.Value;
            }
            else if (status == Shift.Sdk.UI.StatusType.Bookmarked)
            {
                command.BookmarkAdded = DateTimeOffset.UtcNow;
                command.BookmarkExpired = BookmarkExpired.Value;
            }
            else
            {
                throw new NotImplementedException("Action: " + StatusComboBox.Value);
            }

            ServiceLocator.CommandStore.Insert(command);

            HttpResponseHelper.Redirect($"{EditUrl}?command={command.CommandIdentifier}");
        }

        #endregion

        #region IHasParentLinkParameters

        private string GetBackUrl() =>
            new ReturnUrl().GetReturnUrl() ?? GetReaderUrl();

        private string GetReaderUrl()
        {
            return SearchUrl;
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/edit") ? $"command={UserIdentifier}" : null;

        #endregion
    }
}