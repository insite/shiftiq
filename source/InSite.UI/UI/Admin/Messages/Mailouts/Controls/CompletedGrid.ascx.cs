using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;

using InSite.Application.Messages.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Messages;

using Shift.Common;

namespace InSite.Admin.Messages.Mailouts.Controls
{
    public partial class CompletedGrid : SearchResultsGridViewController<MailoutFilter>
    {
        protected override bool IsFinder => false;

        public void LoadData(Guid messageThumbprint)
        {
            Search(new MailoutFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                MessageIdentifier = messageThumbprint,
                IsCompleted = true
            });
        }

        protected override int SelectCount(MailoutFilter filter)
        {
            filter.Completed = new DateTimeOffsetRange
            {
                Since = DateTimeOffset.UtcNow.AddDays(-90)
            };

            var count = ServiceLocator.MessageSearch.CountMailouts(filter);
            EmptyGrid.Visible = count == 0;
            return count;
        }

        protected override IListSource SelectData(MailoutFilter filter)
        {
            filter.Completed = new DateTimeOffsetRange
            {
                Since = DateTimeOffset.UtcNow.AddDays(-90)
            };

            var mailouts = ServiceLocator.MessageSearch.GetMailouts(filter);
            CacheFirstRecipientForEachMailout(mailouts.Select(x => x.MailoutIdentifier).ToArray());
            return new SearchResultList(mailouts);
        }

        protected string GetOneRecipientAddress()
        {
            var mailout = (VMailout)Page.GetDataItem();
            return mailout.DeliveryCount == 1
                ? GetFirstRecipientFromCache(mailout.MailoutIdentifier)
                : null;
        }

        private string GetFirstRecipientFromCache(Guid mailout)
        {
            return _cache.ContainsKey(mailout) ? _cache[mailout] : null;
        }

        private Dictionary<Guid, string> _cache;
        private void CacheFirstRecipientForEachMailout(Guid[] mailouts)
        {
            _cache = ServiceLocator.MessageSearch.GetOneRecipientForEachMailout(mailouts);
        }
    }
}