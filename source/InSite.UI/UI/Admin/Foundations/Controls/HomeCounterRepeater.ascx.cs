using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Domain.Foundations;

using Shift.Common;

namespace InSite.UI.Admin.Foundations.Controls
{
    public partial class HomeCounterRepeater : BaseUserControl
    {
        public class DataItem
        {
            public string Url { get; set; }
            public int? Count { get; set; }
            public string Icon { get; set; }
            public string Title { get; set; }
        }

        public int ItemsCount => Repeater.Items.Count;

        public void LoadData(params DataItem[] items) => LoadData((IEnumerable<DataItem>)items);

        public void LoadData(IEnumerable<DataItem> items)
        {
            Repeater.DataSource = items.Where(x => x.Url.IsNotEmpty() && Identity.IsActionAuthorized(x.Url));
            Repeater.DataBind();
        }

        protected bool IsTrial()
        {
            var dataItem = (DataItem)Page.GetDataItem();
            var actionName = dataItem.Url;

            if (actionName.Contains("?"))
                actionName = actionName.Substring(0, actionName.IndexOf("?"));

            if (actionName.StartsWith("/"))
                actionName = actionName.Substring(1);

            var action = ApplicationContext.GetAction(actionName);
            if (action?.Permission == null)
                return false;

            return action != null
                && action.Permission.HasValue
                && Identity.Claims.IsTrial(action.Permission.Value);
        }
    }
}