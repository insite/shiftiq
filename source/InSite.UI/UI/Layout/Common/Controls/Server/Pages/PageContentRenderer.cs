using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class PageContentRenderer : Control
    {
        #region Nested Items

        internal interface IPageContent
        {
            string Key { get; }
            Action<HtmlTextWriter> Render { get; }
            bool Visible { get; }
            bool RenderRequired { get; }
        }

        #endregion

        #region Properties

        public virtual ContextType RenderContext
        {
            get => (ContextType)ViewState[nameof(RenderContext)];
            set => ViewState[nameof(RenderContext)] = value;
        }

        public virtual string ContextKey
        {
            get => (string)ViewState[nameof(ContextKey)];
            set => ViewState[nameof(ContextKey)] = value;
        }

        internal RandomStringGenerator ContextKeyGenerator => (RandomStringGenerator)(ViewState[nameof(ContextKeyGenerator)]
            ?? (ViewState[nameof(ContextKeyGenerator)] = new RandomStringGenerator(RandomStringType.Alphanumeric, 4)));

        private string[] RenderedKeys
        {
            get => (string[])ViewState[nameof(RenderedKeys)];
            set => ViewState[nameof(RenderedKeys)] = value;
        }

        private static Dictionary<MultiKey<int, string>, PageContentRenderer> Instances
        {
            get => (Dictionary<MultiKey<int, string>, PageContentRenderer>)HttpContext.Current.Items[_instanceStorageKey];
            set => HttpContext.Current.Items[_instanceStorageKey] = value;
        }

        #endregion

        #region Fields

        private Dictionary<string, List<IPageContent>> _contents = new Dictionary<string, List<IPageContent>>();

        private static readonly string _instanceStorageKey = typeof(PageContentRenderer) + "." + nameof(Instances);

        #endregion

        protected override void OnInit(EventArgs e)
        {
            if (Instances == null)
                Instances = new Dictionary<MultiKey<int, string>, PageContentRenderer>();

            var instanceKey = GetKey(RenderContext, ContextKey);
            if (Instances.ContainsKey(instanceKey))
                throw ApplicationError.Create("Can't create more than one instance of {0}.", instanceKey);

            Instances.Add(instanceKey, this);

            base.OnInit(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (HttpRequestHelper.IsAjaxRequest)
            {
                var html = new StringBuilder();
                var script = new StringBuilder();
                var renderedKeys = new HashSet<string>(RenderedKeys ?? new string[0]);

                using (var stringWriter = new StringWriter(html))
                {
                    using (var htmlWriter = new HtmlTextWriter(stringWriter))
                    {
                        foreach (var keyValue in _contents)
                        {
                            var content = keyValue.Value.FirstOrDefault(x => x.RenderRequired || x.Visible);
                            if (content == null)
                                continue;

                            var key = keyValue.Key;
                            if (renderedKeys.Contains(key))
                                renderedKeys.Remove(key);
                            else
                                RenderContent(htmlWriter, content);
                        }
                    }
                }

                foreach (var key in renderedKeys)
                    script
                        .AppendFormat("$('#{0} > div[data-key=\"{1}\"]').remove();", ClientID, GetJsKey(key))
                        .AppendLine();

                if (html.Length > 0)
                    script
                        .AppendFormat("$('#{0}').append({1});", ClientID, HttpUtility.JavaScriptStringEncode(html.ToString(), true))
                        .AppendLine();

                if (script.Length > 0)
                    ScriptManager.RegisterClientScriptBlock(
                        Page,
                        typeof(PageContentRenderer),
                        "update_content:" + ClientID,
                        script.ToString(),
                        true);
            }

            base.OnPreRender(e);

            RenderedKeys = _contents.Where(kv => kv.Value.Any(v => v.RenderRequired || v.Visible)).Select(kv => kv.Key).ToArray();
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            if (HttpRequestHelper.IsAjaxRequest)
                return;

            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
            writer.AddAttribute(HtmlTextWriterAttribute.Style, "display:none !important;");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            foreach (var contentList in _contents.Values)
            {
                var content = contentList.FirstOrDefault(x => x.RenderRequired || x.Visible);

                if (content != null)
                    RenderContent(writer, content);
            }

            writer.RenderEndTag();
        }

        private void RenderContent(HtmlTextWriter writer, IPageContent content)
        {
            writer.AddAttribute("data-key", GetJsKey(content.Key));
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            content.Render(writer);

            writer.RenderEndTag();
        }

        internal static PageContentRenderer Get(ContextType context, string contextKey)
        {
            var instanceKey = GetKey(context, contextKey);
            if (!Instances.TryGetValue(instanceKey, out var renderer))
                throw ApplicationError.Create("Renderer not found: {0}.", instanceKey);

            return renderer;
        }

        internal void Register(IPageContent content)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            if (string.IsNullOrEmpty(content.Key))
                throw new ArgumentNullException(nameof(content.Key));

            if (!_contents.TryGetValue(content.Key, out var list))
                _contents.Add(content.Key, list = new List<IPageContent>());

            list.Add(content);
        }

        private static MultiKey<int, string> GetKey(ContextType context, string key)
        {
            if (context != ContextType.Custom)
                return new MultiKey<int, string>((int)context, string.Empty);

            if (string.IsNullOrEmpty(key))
                throw ApplicationError.Create("The key cannot be null for Custom context.");

            return new MultiKey<int, string>((int)context, key.ToLower());
        }

        private static string GetJsKey(string value) => StringHelper.Sanitize(value, '_', true);
    }
}