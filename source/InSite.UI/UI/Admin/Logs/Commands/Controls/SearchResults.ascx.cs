using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using Humanizer;

using InSite.Admin.Logs.Commands.Utilities;
using InSite.Application.Logs.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Common.Events;

namespace InSite.Admin.Logs.Commands.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<CommandFilter>
    {
        #region Events

        public event AlertHandler Alert;

        private void OnAlert(AlertArgs args) => Alert?.Invoke(this, args);

        #endregion

        #region Fields

        private Dictionary<Guid, string> _users = null;

        #endregion

        #region Binding

        protected override int SelectCount(CommandFilter filter)
        {
            return ServiceLocator.CommandSearch.Count(filter);
        }

        protected override System.ComponentModel.IListSource SelectData(CommandFilter filter)
        {
            var data = ServiceLocator.CommandSearch.Get(filter);

            var userFilter = data.Select(x => x.OriginUser).Distinct().ToArray();

            _users = userFilter.IsNotEmpty()
                ? UserSearch
                    .Bind(
                        x => new { x.UserIdentifier, x.FullName },
                        new UserFilter { IncludeUserIdentifiers = userFilter })
                    .ToDictionary(x => x.UserIdentifier, x => x.FullName)
                : null;

            return data.ToSearchResult();
        }

        #endregion

        #region Event handlers

        protected override void OnRowCommand(GridViewRow row, string name, object argument)
        {
            if (name == "SendCommand")
            {
                var id = GetCommandID();
                var status = CommandHelper.SendCommand(id);

                OnAlert(status);

                SearchWithCurrentPageIndex(Filter);
            }
            else if (name == "RepeatCommand")
            {
                var id = GetCommandID();
                var status = CommandHelper.RepeatCommand(id);

                OnAlert(status);

                SearchWithCurrentPageIndex(Filter);
            }
            else if (name == "DeleteCommand")
            {
                var id = GetCommandID();

                ServiceLocator.CommandStore.Delete(id);

                SearchWithCurrentPageIndex(Filter);
            }
            else
                base.OnRowCommand(row, name, argument);

            Guid GetCommandID() => Grid.GetDataKey<Guid>(row);
        }

        #endregion

        #region Helper methods

        protected string GetCommandStatus(object o)
        {
            if (o == null)
                return string.Empty;

            var html = new StringBuilder();
            var command = (SerializedCommand)o;

            html.Append("<div>" + command.SendStatus);
            if (command.RecurrenceInterval.HasValue && command.RecurrenceUnit != null)
            {
                html.Append($" <span class='form-text'>every {command.RecurrenceUnit.ToQuantity(command.RecurrenceInterval.Value)}");
                if (command.RecurrenceWeekdays != null)
                    html.Append($" ({command.RecurrenceWeekdays})");
                html.Append("</span>");
            }
            html.Append("</div>");

            if (command.RecurrenceInterval.HasValue && command.RecurrenceUnit != null)
                html.Append("<span class='form-text'><i class='fas fa-repeat'></i> Recurring</span> ");

            if (command.BookmarkAdded.HasValue)
                html.Append("<span class='form-text'><i class='fas fa-bookmark'></i> Bookmarked</span> ");

            if (command.SendCancelled.HasValue)
                html.Append("<span class='form-text'><i class='fas fa-exclamation-triangle'></i> Cancelled</span> ");

            else if (command.SendCompleted.HasValue)
                html.Append("<span class='form-text'><i class='fas fa-check-circle'></i> Completed</span> ");

            else if (command.SendStarted.HasValue)
                html.Append("<span class='form-text'><i class='fas fa-hourglass-half'></i> Started</span> ");

            else
                html.Append("<span class='form-text'><i class='fas fa-clock'></i> Scheduled</span> ");

            return html.ToString();
        }

        protected string GetUserName(object o)
        {
            var id = (Guid)o;
            return _users != null && _users.ContainsKey(id) ? _users[id] : string.Empty;
        }

        protected string GetOrganizationCode(object o)
        {
            var id = (Guid)o;
            return OrganizationSearch.Select(id)?.OrganizationCode ?? string.Empty;
        }

        #endregion
    }
}