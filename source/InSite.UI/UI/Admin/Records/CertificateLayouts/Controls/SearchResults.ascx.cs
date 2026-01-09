using System.ComponentModel;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common.Linq;

namespace InSite.UI.Admin.Records.AchievementLayouts
{
    public partial class SearchResults : SearchResultsGridViewController<TCertificateLayoutFilter>
    {
        protected override int SelectCount(TCertificateLayoutFilter filter)
        {
            return TCertificateLayoutSearch.Count(filter);
        }

        protected override IListSource SelectData(TCertificateLayoutFilter filter)
        {
            filter.OrderBy = "CertificateLayoutCode";

            return TCertificateLayoutSearch
                .Bind(x => new
                    {
                        x.CertificateLayoutIdentifier,
                        x.CertificateLayoutCode,
                        x.CertificateLayoutData
                    },
                    filter
                )
                .ToSearchResult();
        }
    }
}