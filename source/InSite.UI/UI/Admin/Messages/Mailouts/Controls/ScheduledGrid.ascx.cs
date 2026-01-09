using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Application.Messages.Write;
using InSite.Common.Web.UI;
using InSite.Domain.Messages;

using Shift.Common;

namespace InSite.Admin.Messages.Mailouts.Controls
{
    public class ScheduledGridItem : Application.Messages.Read.VMailout
    {
        public string Age { get; set; }
        public bool IsOverdue { get; set; }
        public new bool IsStarted { get; set; }
        public bool IsCompleted { get; set; }
    }

    public partial class ScheduledGrid : SearchResultsGridViewController<MailoutFilter>
    {
        #region Properties

        protected override bool IsFinder => false;

        public Guid MessageThumbprint
        {
            get => (Guid)ViewState[nameof(MessageThumbprint)];
            set => ViewState[nameof(MessageThumbprint)] = value;
        }

        #endregion

        #region Methods (loading)

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Grid.RowCommand += Grid_ItemCommand;
        }

        public void LoadData(Guid messageThumbprint)
        {
            MessageThumbprint = messageThumbprint;

            Search(new MailoutFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                MessageIdentifier = messageThumbprint,
                IsCompleted = false,
                IsCancelled = false
            });
        }

        #endregion

        #region Methods (event handling)

        private void Grid_ItemCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Clear")
            {
                var argument = e.CommandArgument as string;
                if (string.IsNullOrEmpty(argument))
                {
                    return;
                }

                var id = Guid.Parse(argument);

                var mailout = ServiceLocator.MessageSearch.FindMailout(id);
                if (mailout?.MessageIdentifier != null)
                    ServiceLocator.SendCommand(new CancelMailout(mailout.MessageIdentifier.Value, mailout.MailoutIdentifier));

                SearchWithCurrentPageIndex(Filter);

                SelectCount(Filter);
            }
            else if (e.CommandName == "Complete")
            {
                var argument = e.CommandArgument as string;
                if (string.IsNullOrEmpty(argument))
                    return;

                var mailout = ServiceLocator.MessageSearch.FindMailout(Guid.Parse(argument));
                if (mailout?.MessageIdentifier != null)
                    ServiceLocator.SendCommand(new CompleteMailout(mailout.MessageIdentifier.Value, mailout.MailoutIdentifier));

                SearchWithCurrentPageIndex(Filter);
                SelectCount(Filter);
            }
        }

        #endregion

        #region Methods (data binding)

        protected override int SelectCount(MailoutFilter filter)
        {
            var count = ServiceLocator.MessageSearch.CountMailouts(filter);

            EmptyGrid.Visible = count == 0;

            return count;
        }

        protected override IListSource SelectData(MailoutFilter filter)
        {
            var mailouts = ServiceLocator.MessageSearch.GetMailouts(filter);

            var list = new List<ScheduledGridItem>();

            foreach (var mailout in mailouts)
            {
                var item = new ScheduledGridItem
                {
                    MailoutScheduled = mailout.MailoutScheduled,
                    MailoutStarted = mailout.MailoutStarted,
                    ContentSubject = mailout.ContentSubject,
                    SubscriberCount = mailout.SubscriberCount,
                    DeliveryCount = mailout.DeliveryCount,
                    MailoutIdentifier = mailout.MailoutIdentifier,

                    Age = mailout.MailoutScheduled.Humanize(),
                    IsOverdue = mailout.MailoutScheduled < DateTime.UtcNow,
                    IsStarted = mailout.MailoutStarted != null,
                    IsCompleted = mailout.MailoutCompleted != null
                };

                list.Add(item);
            }

            CacheFirstRecipientForEachMailout(list.Select(x => x.MailoutIdentifier).ToArray());

            return new SearchResultList(list);
        }

        protected string GetOneRecipientAddress(object o)
        {
            var mailout = (ScheduledGridItem)o;
            if (mailout.DeliveryCount == 1)
                return GetFirstRecipientFromCache(mailout.MailoutIdentifier);
            return null;
        }

        private string GetFirstRecipientFromCache(Guid mailout)
        {
            if (_cache.ContainsKey(mailout))
                return _cache[mailout];
            return null;
        }

        private Dictionary<Guid, string> _cache;
        private void CacheFirstRecipientForEachMailout(Guid[] mailouts)
        {
            _cache = ServiceLocator.MessageSearch.GetOneRecipientForEachMailout(mailouts);
        }

        #endregion
    }
}