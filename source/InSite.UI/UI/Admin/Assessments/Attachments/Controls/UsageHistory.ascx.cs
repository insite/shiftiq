using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Exceptions;

using InSite.Admin.Assessments.Attachments.Utilities;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Admin.Assessments.Attachments.Controls
{
    public partial class UsageHistory : BaseUserControl
    {
        #region Classes

        public class DataItem
        {
            public string Action { get; private set; }
            public string Asset { get; private set; }

            public Guid UserID { get; private set; }
            public int Version { get; private set; }
            public string Time { get; private set; }

            public string User { get; set; }
            public string InfoHtml { get; set; }

            public static DataItem Create(string action, IChange e, Attachment a, TimeZoneInfo timeZone)
            {
                return new DataItem
                {
                    Action = action,
                    Asset = $"{a.Asset}.{a.AssetVersion}",

                    UserID = e.OriginUser,
                    Version = e.AggregateVersion,
                    Time = e.ChangeTime.Format(timeZone, true, true)
                };
            }
        }

        #endregion

        #region Properties

        public int RowCount => Repeater.Items.Count;

        #endregion

        #region Fields

        private static readonly Type _bankType = typeof(BankState);
        private static readonly ConcurrentDictionary<Type, MethodInfo> _eventHandlers = new ConcurrentDictionary<Type, MethodInfo>();

        #endregion

        internal void LoadData(BankState bank, Guid attachmentId, bool showAllVersions)
        {
            var timeZone = User.TimeZone;
            var bankChanges = ServiceLocator.ChangeStore.GetChanges(bank.Identifier, -1);
            var dataItems = new List<DataItem>();

            var attachment = bank.FindAttachment(attachmentId);

            if (showAllVersions)
                attachment = attachment.GetFirstVersion();

            var traceBank = new BankState();
            var traceIds = new HashSet<Guid>() { attachment.Identifier };
            var usersIds = new HashSet<Guid>();

            foreach (var e in bankChanges)
            {
                var isHandled = false;
                var isAttachment = AttachmentHelper.IsAttachmentChange(e);
                DataItem item = null;

                if (isAttachment)
                {
                    if (e is BankAttachmentDeleted e1)
                    {
                        isHandled = true;

                        if (traceIds.Contains(e1.Attachment))
                        {
                            var a = traceBank.FindAttachment(e1.Attachment);
                            item = DataItem.Create("Removed from Bank", e, a, timeZone);
                        }
                    }
                }

                GetChangeHandler(e).Invoke(traceBank, new[] { e });

                if (isAttachment)
                {
                    if (e is AttachmentAdded e1)
                    {
                        isHandled = true;

                        if (traceIds.Contains(e1.Attachment))
                        {
                            var a = traceBank.FindAttachment(e1.Attachment);
                            item = DataItem.Create("Added to Bank", e, a, timeZone);
                        }
                    }
                    else if (e is AttachmentAddedToQuestion e2)
                    {
                        isHandled = true;

                        if (traceIds.Contains(e2.Attachment))
                        {
                            var a = traceBank.FindAttachment(e2.Attachment);
                            var q = traceBank.FindQuestion(e2.Question);

                            item = DataItem.Create("Added to Question", e, a, timeZone);

                            var html = Markdown.ToHtml(q.Content.Title.Default);

                            item.InfoHtml = $"<strong>Question #{q.BankIndex + 1} ({q.Asset}.{q.AssetVersion})</strong>:" +
                                $"<div style='white-space:pre-wrap;'>{html}</div>";
                        }
                    }
                    else if (e is AttachmentChanged e3)
                    {
                        isHandled = true;
                    }
                    else if (e is AttachmentDeletedFromQuestion e4)
                    {
                        isHandled = true;

                        if (traceIds.Contains(e4.Attachment))
                        {
                            var a = traceBank.FindAttachment(e4.Attachment);
                            var q = traceBank.FindQuestion(e4.Question);

                            item = DataItem.Create("Removed from Question", e, a, timeZone);

                            item.InfoHtml = $"<strong>Question #{q.BankIndex + 1} ({q.Asset}.{q.AssetVersion})</strong>:" +
                                $"<div style='white-space:pre-wrap;'>{HttpUtility.HtmlEncode(q.Content.Title.Default)}</div>";
                        }
                    }
                    else if (e is AttachmentUpgraded e5)
                    {
                        isHandled = true;

                        if (showAllVersions && traceIds.Contains(e5.CurrentAttachment) || traceIds.Contains(e5.UpgradedAttachment))
                        {
                            if (showAllVersions)
                                traceIds.Add(e5.UpgradedAttachment);

                            var ca = traceBank.FindAttachment(e5.CurrentAttachment);
                            var ua = traceBank.FindAttachment(e5.UpgradedAttachment);

                            item = DataItem.Create("Upgraded", e, ca, timeZone);

                            item.InfoHtml = $"Upgraded to <strong>{ua.Asset}.{ua.AssetVersion}</strong>";
                        }
                    }
                    else if (e is AttachmentImageChanged e6)
                    {
                        isHandled = true;
                    }
                }

                if (item != null)
                {
                    usersIds.Add(item.UserID);
                    dataItems.Add(item);
                }

                if (isAttachment && !isHandled)
                    throw ApplicationError.Create("Unexpected change type: " + e.GetType().FullName);
            }

            var users = UserSearch
                .Bind(
                    x => new { x.UserIdentifier, x.FullName },
                    new UserFilter { IncludeUserIdentifiers = usersIds.ToArray() })
                .ToDictionary(x => x.UserIdentifier, x => x);

            foreach (var i in dataItems)
                i.User = users.TryGetValue(i.UserID, out var user) ? user.FullName : "<i>(Unknown)</i>";

            Repeater.DataSource = dataItems.OrderByDescending(x => x.Version);
            Repeater.DataBind();
        }

        private static MethodInfo GetChangeHandler(IChange e)
        {
            var result = _eventHandlers.GetOrAdd(e.GetType(), t => _bankType.GetMethod("When", new[] { t }));

            if (result == null)
                throw new MethodNotFoundException(_bankType, "When", e.GetType());

            return result;
        }
    }
}