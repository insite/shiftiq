using System;
using System.Linq;
using System.Web.UI;

using Shift.Common;
namespace InSite.Admin.Assets.Contents.Controls
{
    public partial class MultilingualStringInfo : UserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ControlStyle.ContentKey = typeof(MultilingualStringInfo).FullName;
            CommonScript.ContentKey = typeof(MultilingualStringInfo).FullName;
        }

        protected override void OnPreRender(EventArgs e)
        {
            ScriptManager.RegisterStartupScript(
                Page,
                typeof(MultilingualStringInfo),
                "init_" + ClientID,
                $"multilingualStringInfo.init('{Repeater.ClientID}');",
                true);

            base.OnPreRender(e);
        }

        public bool LoadData(MultilingualString str, bool isHtml = false)
        {
            var hasData = str?.IsEmpty == false;

            Repeater.Visible = hasData;
            NoDataText.Visible = !hasData;

            if (hasData)
            {
                Repeater.DataSource = str.Languages.Select(lang => new
                {
                    Language = lang,
                    TranslationHtml = isHtml ? str[lang] : Markdown.ToHtml(str[lang])
                });
                Repeater.DataBind();
            }

            return hasData;
        }

        public bool LoadData(ContentContainerItem item)
        {
            var hasData = item?.IsEmpty == false;

            Repeater.Visible = hasData;
            NoDataText.Visible = !hasData;

            if (hasData)
            {
                Repeater.DataSource = item.Languages.Select(lang => new
                {
                    Language = lang,
                    TranslationHtml = item.GetHtml(lang)
                });
                Repeater.DataBind();
            }

            return hasData;
        }
    }
}