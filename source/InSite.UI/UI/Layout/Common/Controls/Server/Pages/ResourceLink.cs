using System;
using System.Web;
using System.Web.UI;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class ResourceLink : Control
    {
        #region Enums

        public enum ResourceType
        {
            None,
            Css,
            JavaScript
        }

        #endregion

        #region Properties

        public ResourceType Type
        {
            get => (ResourceType)ViewState[nameof(Type)];
            set => ViewState[nameof(Type)] = value;
        }

        public string Url
        {
            get => (string)ViewState[nameof(Url)];
            set => ViewState[nameof(Url)] = value;
        }

        #endregion

        #region Fields

        private static readonly bool _isDebug;
        private static readonly bool _isLocal;
        private static readonly bool _isLinkDebug;
        private static readonly string _query;

        #endregion

        #region Construction

        static ResourceLink()
        {
            _isDebug = HttpContext.Current.IsDebuggingEnabled;
            _isLocal = ServiceLocator.AppSettings.Environment.Name == EnvironmentName.Local;
            _isLinkDebug = ServiceLocator.AppSettings.Application.ResourceLinkDebug;
            _query = _isLocal
                ? "_=" + DateTime.UtcNow.Ticks
                : "v=" + ServiceLocator.AppSettings.Release.Version;
        }

        #endregion

        #region Rendering

        public override void RenderControl(HtmlTextWriter writer)
        {
            if (!Visible || Type == ResourceType.None || Url.IsEmpty())
                return;

            var file = !_isDebug
                ? ResourceHelper.GetResourceFile(Url)
                : _isLocal && _isLinkDebug
                    ? ResourceHelper.GetDebugResourceFile(Url)
                    : null;
            var url = file == null
                ? HttpResponseHelper.BuildUrl(Url, _query)
                : file.ID != null
                    ? HttpResponseHelper.BuildUrl(file.Url, "h=" + file.ID)
                    : file.Url;

            if (Type == ResourceType.Css)
                writer.Write($"<link rel=\"stylesheet\" media=\"screen\" href=\"{url}\" />");
            else if (Type == ResourceType.JavaScript)
                writer.Write($"<script type=\"text/javascript\" src=\"{url}\"></script>");
        }

        #endregion
    }

    public class StyleLink : ResourceLink
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Type = ResourceType.Css;

            Url = ServiceLocator.AppSettings.Application.StylePath;
        }
    }

    public class FaviconLink : Literal
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var filename = ServiceLocator.Partition.IsE03()
                ? "favicon-cmds.ico"
                : "favicon.ico";

            var favicon = $"<link rel='icon' type='image/x-icon' href='/{filename}'>";

            Text = favicon;
        }
    }
}