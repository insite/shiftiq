using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Contacts.Read;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class FindGroup : BaseFindEntity<QGroupSelectorFilter>
    {
        #region Classes

        [Serializable]
        public class GroupDataItem : DataItem
        {
            public override Guid Value { get; set; }
            public override string Text => GroupName + (ShippingCity.IsNotEmpty() ? " [" + ShippingCity + "]" : "");

            public string GroupName { get; set; }
            public string ShippingCity { get; set; }
        }

        #endregion

        #region Properties

        public bool ShowCities
        {
            get { return ViewState[nameof(ShowCities)] as bool? ?? false; }
            set { ViewState[nameof(ShowCities)] = value; }
        }

        public bool CurrentOrganizationOnly
        {
            get { return ViewState[nameof(CurrentOrganizationOnly)] as bool? ?? false; }
            set { ViewState[nameof(CurrentOrganizationOnly)] = value; }
        }

        public GroupDataItem DefaultOptions
        {
            get { return ViewState[nameof(DefaultOptions)] as GroupDataItem; }
            set { ViewState[nameof(DefaultOptions)] = value; }
        }

        public QGroupSelectorFilter Filter => (QGroupSelectorFilter)(ViewState[nameof(Filter)]
            ?? (ViewState[nameof(Filter)] = new QGroupSelectorFilter()));

        #endregion

        protected override string GetEntityName() => "Group";

        protected override string GetEditorUrl() => "/ui/admin/contacts/groups/edit?contact={value}";

        protected override QGroupSelectorFilter GetFilter(string keyword)
        {
            var filter = Filter.Clone();

            if (CurrentOrganizationOnly)
                filter.OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier;

            filter.AlwaysIncludeGroupIdentifiers = Values;
            filter.Keyword = keyword;

            return filter;
        }

        protected override int Count(QGroupSelectorFilter filter)
        {
            var count =  ServiceLocator.GroupSearch.CountSelectorGroups(filter);
            return DefaultOptions != null ? (count + 1) : count;
        }

        protected override DataItem[] Select(QGroupSelectorFilter filter)
        {
            var groups = ServiceLocator.GroupSearch.GetSelectorGroups(filter, ShowCities);

            var data = ShowCities
                ? groups.Select(x => new GroupDataItem
                {
                    Value = x.GroupIdentifier,
                    GroupName = x.GroupName,
                    ShippingCity = x.ShippingAddress?.City
                }).ToList()
                : groups.Select(x => new GroupDataItem
                {
                    Value = x.GroupIdentifier,
                    GroupName = x.GroupName
                }).ToList();

            if(DefaultOptions != null && !filter.ExcludeDefaultValue)
                data.Add(DefaultOptions);
            else if(DefaultOptions != null && filter.ExcludeDefaultValue && data.Count == 0)
                data.Add(DefaultOptions);

            return data.ToArray();
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            var filter = new QGroupSelectorFilter { IncludeGroupIdentifiers = ids };
            filter.ExcludeDefaultValue = true;

            return Select(filter);
        }
    }
}