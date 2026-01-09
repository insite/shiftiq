using System.Linq;

using InSite.Persistence;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class CollectionToolComboBox : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            var values = TCollectionSearch.Distinct(x => x.CollectionTool, x => x.CollectionTool != null);
            foreach (var value in values.OrderBy(x => x))
                list.Add(value);

            return list;
        }
    }
}