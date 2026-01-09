using System;

using InSite.Application.Logs.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Logs.Commands.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<CommandFilter>
    {
        public override CommandFilter Filter
        {
            get
            {
                var filter = new CommandFilter
                {
                    OriginOrganization = Organization.Identifier,

                    CommandState = CommandState.Value.IsNotEmpty() ? CommandState.Value.ToEnum<CommandState>() : (CommandState?)null,
                    CommandClass = CommandClass.Text,
                    CommandType = CommandType.Text,
                    CommandData = CommandData.Text,

                    IsRecurring = IsRecurring.ValueAsBoolean,

                    SendError = SendError.Text,
                };

                if (Guid.TryParse(UserIdentifier.Text, out Guid id))
                    filter.OriginUser = id;

                var dateRange = new DateTimeOffsetRange
                {
                    Since = DateSince.Value,
                    Before = DateBefore.Value
                };

                if (filter.CommandState == Shift.Constant.CommandState.Scheduled)
                    filter.SendScheduled = dateRange;
                else if (filter.CommandState == Shift.Constant.CommandState.Started)
                    filter.SendStarted = dateRange;
                else if (filter.CommandState == Shift.Constant.CommandState.Completed)
                    filter.SendCompleted = dateRange;
                else if (filter.CommandState == Shift.Constant.CommandState.Cancelled)
                    filter.SendCancelled = dateRange;
                else if (filter.CommandState == Shift.Constant.CommandState.Bookmarked)
                    filter.BookmarkAdded = dateRange;

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                var filter = value;
                if (filter == null)
                {
                    Clear();
                    return;
                }

                if (filter.OriginUser.HasValue)
                    UserIdentifier.Text = filter.OriginUser.ToString();

                DateTimeOffsetRange dateRange = null;

                if (filter.CommandState.HasValue)
                {
                    CommandState.Value = filter.CommandState.Value.GetName();

                    if (filter.CommandState == Shift.Constant.CommandState.Scheduled)
                        dateRange = filter.SendScheduled;
                    else if (filter.CommandState == Shift.Constant.CommandState.Started)
                        dateRange = filter.SendStarted;
                    else if (filter.CommandState == Shift.Constant.CommandState.Completed)
                        dateRange = filter.SendCompleted;
                    else if (filter.CommandState == Shift.Constant.CommandState.Cancelled)
                        dateRange = filter.SendCancelled;
                    else if (filter.CommandState == Shift.Constant.CommandState.Bookmarked)
                        dateRange = filter.BookmarkAdded;
                }
                else
                    CommandState.ClearSelection();

                OnCommandStateChanged();

                DateSince.Value = dateRange?.Since;
                DateBefore.Value = dateRange?.Before;

                CommandClass.Text = filter.CommandClass;
                CommandType.Text = filter.CommandType;
                CommandData.Text = filter.CommandData;

                IsRecurring.ValueAsBoolean = filter.IsRecurring;

                SendError.Text = filter.SendError;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CommandState.AutoPostBack = true;
            CommandState.ValueChanged += CommandState_ValueChanged;
        }

        private void CommandState_ValueChanged(object sender, EventArgs e) => OnCommandStateChanged();

        private void OnCommandStateChanged()
        {
            var option = CommandState.GetSelectedOption();
            var hasSelection = option != null && option.Value.IsNotEmpty();

            DateContainer.Visible = hasSelection;

            if (!hasSelection)
                return;

            DateSince.EmptyMessage = $"{option.Text} &ge;";
            DateSince.Value = null;

            DateBefore.EmptyMessage = $"{option.Text} &lt;";
            DateBefore.Value = null;
        }

        public override void Clear()
        {
            UserIdentifier.Text = null;

            CommandState.ClearSelection();
            OnCommandStateChanged();
            IsRecurring.ClearSelection();
            DateSince.Value = null;
            DateBefore.Value = null;

            CommandClass.Text = string.Empty;
            CommandType.Text = string.Empty;
            CommandData.Text = string.Empty;

            SendError.Text = string.Empty;
        }
    }
}