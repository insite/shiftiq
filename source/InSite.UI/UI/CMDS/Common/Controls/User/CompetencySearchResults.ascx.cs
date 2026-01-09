using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;

namespace InSite.Cmds.Controls.Talents.Competencies
{
    public partial class CompetencySearchResults : SearchResultsGridViewController<CompetencyFilter>
    {
        protected string GetCategories(Guid assetIdentifier)
        {
            var categories = StandardClassificationSearch
                .Bind(x => x.CategoryIdentifier, x => x.StandardIdentifier == assetIdentifier)
                .Select(x => TCollectionItemCache.GetName(x))
                .Where(x => x.IsNotEmpty())
                .OrderBy(x => x)
                .ToList();

            var html = new StringBuilder();

            foreach (var category in categories)
                html.Append($"<div>{category}</div>");

            return html.ToString();
        }

        protected override int SelectCount(CompetencyFilter filter)
        {
            return CompetencyRepository.CountSearchResults(filter);
        }

        protected override IListSource SelectData(CompetencyFilter filter)
        {
            return CompetencyRepository.SelectSearchResults(filter, true);
        }
    }
}