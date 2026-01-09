using System;
using System.Web;

using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.Admin.Sites.Pages.Controls
{
    public partial class ContentOutput : BaseUserControl
    {
        public void Setup(Guid id, string name, ContentContainerItem item, bool canWrite)
        {
            EditLink.NavigateUrl = $"/ui/admin/sites/pages/content?id={id}&tab={HttpUtility.UrlEncode(name.ToLower())}";
            EditLink.Visible = canWrite;

            MultilingualString str = null;
            var isHtml = false;

            if (item != null)
            {
                str = item.Html;
                isHtml = str != null && !str.IsEmpty;

                if (!isHtml)
                    str = item.Text;

                if (str == null)
                    str = new MultilingualString();
            }

            DataOutput.LoadData(str, isHtml);
        }
    }
}