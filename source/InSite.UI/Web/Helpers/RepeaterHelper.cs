using System.Web.UI.WebControls;

namespace InSite.Web.Helpers
{
    public class RepeaterHelper
    {
        public static bool IsContentItem(RepeaterItemEventArgs e) 
            => IsContentItem(e.Item);

        public static bool IsContentItem(RepeaterItem item)
            => item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem;
    }
}