using System;
using System.Linq;
using System.Web;

using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.Admin.Standards.Standards.Controls
{
    public partial class ContentOutput : BaseUserControl
    {
        public void Setup(Guid id, string name, ContentContainerItem item, bool canWrite)
        {
            EditLink.NavigateUrl = $"/ui/admin/standards/content?container={id}&tab={HttpUtility.UrlEncode(name.ToLower())}";
            EditLink.Visible = canWrite;

            MultilingualString str = null;
            var isHtml = false;

            if (item != null)
            {
                var languages = Organization.Languages.Select(x => x.TwoLetterISOLanguageName).ToArray();

                str = item.Html?.Clone(languages);
                isHtml = str != null && !str.IsEmpty;

                if (!isHtml)
                    str = item.Text?.Clone(languages);

                if (str == null)
                    str = new MultilingualString();
            }

            DataOutput.LoadData(str, isHtml);
        }
    }
}