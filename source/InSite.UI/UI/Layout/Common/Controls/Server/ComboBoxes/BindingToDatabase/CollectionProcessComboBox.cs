using System.Linq;

using InSite.Persistence;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class CollectionProcessComboBox : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            var values = TCollectionSearch.Distinct(x => x.CollectionProcess, x => x.CollectionProcess != null);
            foreach (var v in values.OrderBy(x => x))
                list.Add(v);

            return list;
        }
    }
}