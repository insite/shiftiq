using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Messages.Read;
using InSite.Domain.Messages;

namespace InSite.Common.Web.UI
{
    public class FindMessage : BaseFindEntity<MessageFilter>
    {
        #region Properties

        public MessageFilter Filter => (MessageFilter)(ViewState[nameof(Filter)]
            ?? (ViewState[nameof(Filter)] = new MessageFilter { OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier }));

        #endregion

        protected override string GetEntityName() => "Message";

        protected override MessageFilter GetFilter(string keyword)
        {
            var filter = Filter.Clone();

            filter.Name = keyword;

            return filter;
        }

        protected override int Count(MessageFilter filter)
        {
            return ServiceLocator.MessageSearch.CountMessages(filter);
        }

        protected override DataItem[] Select(MessageFilter filter)
        {
            filter.OrderBy = nameof(QMessage.MessageTitle);

            return ServiceLocator.MessageSearch
                .GetMessages(filter)
                .Select(x => new DataItem
                {
                    Value = x.MessageIdentifier,
                    Text = x.MessageTitle
                })
                .ToArray();
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            return Select(new MessageFilter { MessageIdentifiers = ids });
        }
    }
}