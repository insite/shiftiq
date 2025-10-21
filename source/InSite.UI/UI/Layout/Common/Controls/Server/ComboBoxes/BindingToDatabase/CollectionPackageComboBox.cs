using System.Linq;

using InSite.Persistence;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class CollectionPackageComboBox : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            var values = TCollectionSearch.Distinct(x => x.CollectionPackage, x => x.CollectionPackage != null);
            foreach (var v in values.OrderBy(x => x))
                list.Add(v);

            return list;
        }
    }
}