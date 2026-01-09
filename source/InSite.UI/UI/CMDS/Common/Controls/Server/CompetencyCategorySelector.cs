using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public class CompetencyCategorySelector : ComboBox
    {
        public CompetencyCategorySelector()
        {
            DropDown.Size = 15;
            DropDown.Width = Unit.Pixel(250);
        }

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            var categories = TCollectionItemCache.Query(new TCollectionItemFilter
            {
                CollectionName = CollectionName.Standards_Standards_Category_Name,
                ItemFolder = StandardType.Competency
            });

            if (categories.Any(x => x.OrganizationIdentifier == OrganizationIdentifiers.CMDS))
                categories = categories.Where(x => x.OrganizationIdentifier == OrganizationIdentifiers.CMDS);
            else
                categories = categories.Where(x => x.OrganizationIdentifier == OrganizationIdentifiers.Global);

            foreach (var category in categories.AsEnumerable())
                list.Add(category.ItemIdentifier.ToString(), category.ItemName);

            return list;
        }
    }
}