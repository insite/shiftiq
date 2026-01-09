using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Events.Read;

namespace InSite.Common.Web.UI
{
    public class FindEvent : BaseFindEntity<QEventFilter>
    {
        #region Properties

        public QEventFilter Filter => (QEventFilter)(ViewState[nameof(Filter)]
            ?? (ViewState[nameof(Filter)] = new QEventFilter { OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier }));

        public bool ShowPrefix
        {
            get => (bool?)ViewState[nameof(ShowPrefix)] ?? true;
            set => ViewState[nameof(ShowPrefix)] = value;
        }

        #endregion

        protected override string GetEntityName() => "Event";

        protected override QEventFilter GetFilter(string keyword)
        {
            var filter = Filter.Clone();

            filter.Keyword = keyword;

            return filter;
        }

        protected override int Count(QEventFilter filter)
        {
            return ServiceLocator.EventSearch.CountEvents(filter);
        }

        protected override DataItem[] Select(QEventFilter filter)
        {
            filter.OrderBy = $"{nameof(QEvent.EventType)},{nameof(QEvent.EventNumber)},{nameof(QEvent.EventBillingType)}";

            return ServiceLocator.EventSearch
                .GetEvents(filter)
                .Select(GetDataItem)
                .ToArray();
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            return ServiceLocator.EventSearch.GetEvents(ids).Select(GetDataItem);
        }

        private DataItem GetDataItem(QEvent x) => new DataItem
        {
            Value = x.EventIdentifier,
            Text = ShowPrefix
                ? $"{x.EventType} {x.EventNumber}-{x.EventBillingType}: {x.EventTitle}"
                : $"{x.EventTitle}"
        };
    }
}