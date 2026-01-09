using System;
using System.Reflection;
using System.Web;
using System.Web.UI;

using Shift.Common;

using IoFile = System.IO.File;

namespace InSite.Common.Web.UI
{
    public interface IResourceBundleItem
    {
        void WriteJs(HtmlTextWriter writer, string query);
        void WriteCss(HtmlTextWriter writer, string query);

        ResourceHelper.BundleContent GetBundleContent();
    }

    public abstract class ResourceBundleItem : IResourceBundleItem
    {
        protected abstract void WriteJs(HtmlTextWriter writer, string query);
        protected abstract void WriteCss(HtmlTextWriter writer, string query);

        protected abstract ResourceHelper.BundleContent GetBundleContent();

        void IResourceBundleItem.WriteJs(HtmlTextWriter writer, string query) => WriteJs(writer, query);

        void IResourceBundleItem.WriteCss(HtmlTextWriter writer, string query) => WriteCss(writer, query);

        ResourceHelper.BundleContent IResourceBundleItem.GetBundleContent() => GetBundleContent();
    }

    public sealed class ResourceBundleFile : ResourceBundleItem
    {
        #region Properties

        public string Url { get; set; }

        #endregion

        #region Fields

        private static readonly bool _isDebug;
        private static readonly bool _isLocal;
        private static readonly bool _isLinkDebug;

        #endregion

        #region Construction

        static ResourceBundleFile()
        {
            _isDebug = HttpContext.Current.IsDebuggingEnabled;
            _isLocal = ServiceLocator.AppSettings.Environment.Name == EnvironmentName.Local;
            _isLinkDebug = ServiceLocator.AppSettings.Application.ResourceLinkDebug;
        }

        #endregion

        #region Rendering

        protected override void WriteCss(HtmlTextWriter writer, string query)
        {
            if (Url.IsEmpty())
                return;

            var url = GetUrl(query);

            writer.Write("<link rel=\"stylesheet\" media=\"screen\" href=\"" + url + "\" />");
        }

        protected override void WriteJs(HtmlTextWriter writer, string query)
        {
            if (Url.IsEmpty())
                return;

            var url = GetUrl(query);

            writer.Write("<script type=\"text/javascript\" src=\"" + url + "\"></script>");
        }

        private string GetUrl(string query)
        {
            var file = !_isDebug
                ? ResourceHelper.GetResourceFile(Url)
                : _isLocal && _isLinkDebug
                    ? ResourceHelper.GetDebugResourceFile(Url)
                    : null;
            return file == null
                ? HttpResponseHelper.BuildUrl(Url, query)
                : file.ID != null
                    ? HttpResponseHelper.BuildUrl(file.Url, "h=" + file.ID)
                    : file.Url;
        }

        #endregion

        #region Bundling

        protected override ResourceHelper.BundleContent GetBundleContent()
        {
            return new ResourceHelper.BundleContent(Url.ToLower())
            {
                Content = GetContent()
            };
        }

        private string GetContent()
        {
            var result = string.Empty;

            if (Url.IsNotEmpty())
            {
                try
                {
                    var filePath = HttpContext.Current.Server.MapPath(Url);
                    if (!IoFile.Exists(filePath))
                        AppSentry.SentryError(ApplicationError.Create("File not found: " + filePath));

                    filePath = ResourceHelper.GetMinFilePath(filePath);

                    var content = IoFile.ReadAllText(filePath);

                    if (content.HasValue())
                        result = content;
                    else
                        AppSentry.SentryError(ApplicationError.Create("File has no content: " + filePath));
                }
                catch (Exception ex)
                {
                    AppSentry.SentryError(ex);
                }
            }
            else
            {
                AppSentry.SentryError(ApplicationError.Create("URL is undefined"));
            }

            return result;
        }

        #endregion
    }

    public sealed class ResourceBundleMethod : ResourceBundleItem
    {
        #region Properties

        public string Type { get; set; }

        public string Name { get; set; }

        #endregion

        #region Binding

        private System.Type _type = null;
        private bool _typeInited = false;

        private Type FindType()
        {
            if (!_typeInited)
            {
                _typeInited = true;

                if (Type.IsNotEmpty())
                {
                    _type = System.Type.GetType(Type);

                    if (_type == null)
                        throw ApplicationError.Create("Can't find type: " + Type);
                }
                else
                    throw ApplicationError.Create("Type is undefined");
            }

            return _type;
        }

        #endregion

        #region Rendering

        protected override void WriteCss(HtmlTextWriter writer, string query)
        {
            var content = GetContent();
            if (content.HasValue())
                writer.Write("<style type=\"text/css\">" + content + "</style>");
        }

        protected override void WriteJs(HtmlTextWriter writer, string query)
        {
            var content = GetContent();
            if (content.HasValue())
                writer.Write("<script type=\"text/javascript\">" + content + "</script>");
        }

        #endregion

        #region Bundling

        protected override ResourceHelper.BundleContent GetBundleContent()
        {
            var name = string.Empty;

            if (Name.IsNotEmpty())
            {
                var type = FindType();
                if (type != null)
                    name = "/" + StringHelper.Sanitize(type.FullName + "." + Name, '/', true);
            }

            return new ResourceHelper.BundleContent(name)
            {
                Content = GetContent()
            };
        }

        private string GetContent()
        {
            if (Name.IsEmpty())
                throw ApplicationError.Create("Name of method is undefined");

            var type = FindType();
            var result = string.Empty;

            if (type != null)
            {
                var method = type.GetMethod(
                    Name,
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                if (method == null)
                    throw ApplicationError.Create($"Method not found: {Name} ({type.FullName})");

                if (method.ReturnType != typeof(string) || method.GetParameters().Length != 0)
                    throw ApplicationError.Create($"Invalid method: {Name} ({type.FullName})");

                result = (string)method.Invoke(null, null);
            }

            return result;
        }

        #endregion
    }
}