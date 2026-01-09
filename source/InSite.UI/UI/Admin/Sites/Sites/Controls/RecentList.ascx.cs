using System;
using System.Web.UI.WebControls;

using InSite.Admin.Sites.Utilities;
using InSite.Application.Sites.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.UI.Admin.Sites.Sites.Controls
{
    public partial class RecentList : BaseUserControl
    {
        public int ItemCount
        {
            get => (int?)ViewState[nameof(ItemCount)] ?? 0;
            set => ViewState[nameof(ItemCount)] = value;
        }

        public void LoadData(int count)
        {
            var data = ServiceLocator.SiteSearch.SelectRecent(Organization.OrganizationIdentifier, count);

            ItemCount = data.Length;

            Repeater.DataSource = data;
            Repeater.DataBind();
        }

        #region Methods (binding)

        protected string GetTimestampHtml(string noun)
        {
            var modifiedOn = (DateTimeOffset)Eval(nameof(RecentInfo.Modified));
            var modifiedBy = (Guid)Eval(nameof(RecentInfo.ModifiedBy));

            return UserSearch.GetTimestampHtml(modifiedBy, noun, "changed", modifiedOn);
        }

        protected string GetIconClass()
        {
            var type = (string)Eval(nameof(RecentInfo.Type));

            return SiteHelper.GetIconCssClass(type);
        }

        protected string GetEditUrl()
        {
            var id = (Guid)Eval(nameof(RecentInfo.Identifier));
            var type = (string)Eval(nameof(RecentInfo.Type));

            return SiteHelper.GetEditUrl(id, type);
        }

        #endregion
    }
}