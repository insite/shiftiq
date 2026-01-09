using System;

using Shift.Common.Timeline.Commands;

using InSite.Admin.Logs.Commands.Utilities;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Logs.Commands.Forms
{
    public partial class Edit : AdminBasePage, IHasParentLinkParameters
    {
        private const string CreateUrl = "/ui/admin/logs/commands/create";
        private const string SearchUrl = "/ui/admin/logs/commands/search";

        #region Properties

        private Guid CommandIdentifier => Guid.TryParse(Request.QueryString["command"], out var value) ? value : Guid.Empty;

        private bool IsInited
        {
            get => (bool)(ViewState[nameof(IsInited)] ?? false);
            set => ViewState[nameof(IsInited)] = value;
        }

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanEdit)
                HttpResponseHelper.Redirect(SearchUrl);

            if (IsInited)
                return;

            IsInited = true;

            var command = LoadCommand();

            PageHelper.AutoBindHeader(this);

            CancelButton.NavigateUrl = GetBackUrl();

            CopyButton.NavigateUrl = new ReturnUrl($"command={command.CommandIdentifier}")
                    .GetRedirectUrl($"{CreateUrl}?command={command.CommandIdentifier}");

            OrganizationIdentifier.Value = command.OriginOrganization;
            UserIdentifier.Value = command.OriginUser;

            CommandDetails.SetOrganizationIdentifier(command.OriginOrganization);
            CommandDetails.SetInputValues(command);
            RecurrenceField.SetInputValues(command);

            SendStatus.Text = command.SendStatus;

            {
                var status = CommandHelper.GetStatus(command.SendStatus);
                var isBookmarked = status == Shift.Sdk.UI.StatusType.Bookmarked;
                var isScheduled = status == Shift.Sdk.UI.StatusType.Scheduled;

                SendContainer.Visible = !isBookmarked;
                BookmarkContainer.Visible = isBookmarked;
                ScheduledDateField.Visible = isScheduled;

                if (isBookmarked)
                {
                    BookmarkAdded.Text = command.BookmarkAdded.HasValue
                        ? command.BookmarkAdded.Format(User.TimeZone)
                        : string.Empty;
                    BookmarkExpired.Value = command.BookmarkExpired;
                }
                else
                {
                    SendError.Text = command.SendError;
                    SendErrorField.Visible = !string.IsNullOrEmpty(command.SendError);

                    SendStarted.Text = command.SendStarted.Format(User.TimeZone);
                    SendStartedField.Visible = command.SendStarted.HasValue;

                    SendCompleted.Text = command.SendCompleted.Format(User.TimeZone);
                    SendCompletedField.Visible = command.SendCompleted.HasValue;

                    SendCancelled.Text = command.SendCancelled.Format(User.TimeZone);
                    SendCancelledField.Visible = command.SendCancelled.HasValue;

                    if (isScheduled)
                    {
                        ScheduledDate.Value = command.SendScheduled;
                    }
                }
            }
        }

        #endregion

        #region Event handlers

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var command = LoadCommand();
            var status = CommandHelper.GetStatus(command.SendStatus);

            CommandDetails.GetInputValues(command);
            RecurrenceField.GetInputValues(command);

            if (status == Shift.Sdk.UI.StatusType.Bookmarked)
                command.BookmarkExpired = BookmarkExpired.Value.Value;
            else if (status == Shift.Sdk.UI.StatusType.Scheduled)
                command.SendScheduled = ScheduledDate.Value.Value;

            ServiceLocator.CommandStore.Update(command);

            HttpResponseHelper.Redirect($"{SearchUrl}");
        }

        #endregion

        #region Methods

        private SerializedCommand LoadCommand()
        {
            var command = ServiceLocator.CommandSearch.Get(CommandIdentifier);

            if (command == null)
                HttpResponseHelper.Redirect(GetBackUrl(), true);

            return command;
        }

        #endregion

        #region IHasParentLinkParameters

        private string GetBackUrl() =>
            new ReturnUrl().GetReturnUrl() ?? GetReaderUrl();

        private string GetReaderUrl()
        {
            return SearchUrl;
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent) =>
           GetParentLinkParameters(parent, null);

        #endregion
    }
}