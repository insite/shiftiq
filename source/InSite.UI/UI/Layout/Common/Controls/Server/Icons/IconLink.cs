using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class IconLink : Icon
    {
        #region Properties

        [UrlProperty]
        public string NavigateUrl
        {
            get { return (string)ViewState[nameof(NavigateUrl)]; }
            set { ViewState[nameof(NavigateUrl)] = value; }
        }

        [TypeConverter(typeof(TargetConverter))]
        public string Target
        {
            get { return (string)ViewState[nameof(Target)]; }
            set { ViewState[nameof(Target)] = value; }
        }

        public string ReturnUrl
        {
            get { return (string)ViewState[nameof(ReturnUrl)]; }
            set { ViewState[nameof(ReturnUrl)] = value; }
        }

        #endregion

        #region Rendering

        protected override void Render(HtmlTextWriter writer)
        {
            var navUrl = GetNavigateUrl();

            if (ReturnUrl.HasValue())
            {
                var returnUrl = new ReturnUrl(ReturnUrl);
                navUrl = returnUrl.GetRedirectUrl(navUrl);
            }

            if (!string.IsNullOrEmpty(navUrl))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
                writer.AddAttribute(HtmlTextWriterAttribute.Href, ResolveUrl(navUrl));

                if (!string.IsNullOrEmpty(Target))
                    writer.AddAttribute(HtmlTextWriterAttribute.Target, Target);

                AddAttributesToRender(writer);

                writer.RenderBeginTag(HtmlTextWriterTag.A);

                RenderIcon(writer, true);

                writer.RenderEndTag();
            }
            else
            {
                base.Render(writer);
            }
        }

        #endregion

        #region Helpers

        protected virtual string GetNavigateUrl()
        {
            return NavigateUrl;
        }

        #endregion
    }
}
