using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Persistence;

namespace InSite.Common.Web.UI
{
    public class FindStandard : BaseFindEntity<StandardFilter>
    {
        public StandardFilter Filter => (StandardFilter)(ViewState[nameof(Filter)]
            ?? (ViewState[nameof(Filter)] = new StandardFilter { OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier }));

        public FindStandardHelper.TextType TextType
        {
            get => (FindStandardHelper.TextType)(ViewState[nameof(TextType)] ?? FindStandardHelper.TextType.TypeNumberTitle);
            set => ViewState[nameof(TextType)] = value;
        }

        private string Language => EnableTranslation ? CurrentSessionState.Identity.Language : "en";

        protected override string GetEntityName() => "Standard";

        protected override StandardFilter GetFilter(string keyword)
        {
            var filter = Filter.Clone();

            filter.SelectorText = keyword;

            return filter;
        }

        protected override int Count(StandardFilter filter)
        {
            return StandardSearch.Count(filter);
        }

        protected override DataItem[] Select(StandardFilter filter)
        {
            return FindStandardHelper
                .Select(filter, TextType, (value, text) => new DataItem
                {
                    Value = value,
                    Text = text
                }, Language)
                .ToArray();
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            return FindStandardHelper.Select(ids, TextType, GetDataItem, Language);
        }

        private static DataItem GetDataItem(Guid value, string text) => new DataItem
        {
            Value = value,
            Text = text
        };
    }
}