using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Admin.Assets.Glossaries.Utilities;
using InSite.Application.Glossaries.Read;

namespace InSite.Common.Web.UI
{
    public class FindGlossaryTerm : BaseFindEntity<GlossaryTermFilter>
    {
        #region Properties

        public GlossaryTermFilter Filter => (GlossaryTermFilter)(ViewState[nameof(Filter)]
            ?? (ViewState[nameof(Filter)] = new GlossaryTermFilter()));

        #endregion

        protected override string GetEntityName() => "Glossary Term";

        protected override GlossaryTermFilter GetFilter(string keyword)
        {
            var filter = Filter.Clone();

            filter.GlossaryIdentifier = GlossaryHelper.GlossaryIdentifier;
            filter.TermKeyword = keyword;

            return filter;
        }

        protected override int Count(GlossaryTermFilter filter)
        {
            return ServiceLocator.GlossarySearch.CountTerms(filter);
        }

        protected override DataItem[] Select(GlossaryTermFilter filter)
        {
            filter.OrderBy = nameof(QGlossaryTerm.TermName);

            return ServiceLocator.GlossarySearch
                .GetTerms(filter)
                .Select(GetDataItem)
                .ToArray();
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            return ServiceLocator.GlossarySearch
                .GetTerms(ids)
                .Select(GetDataItem);
        }

        private static DataItem GetDataItem(QGlossaryTerm x) => new DataItem
        {
            Value = x.TermIdentifier,
            Text = x.TermName,
        };
    }
}