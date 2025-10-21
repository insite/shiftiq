using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class PageFooterContentRenderer : PageContentRenderer
    {
        public override ContextType RenderContext
        {
            get => ContextType.Footer;
            set { }
        }

        public override string ContextKey
        {
            get => null;
            set => throw new NotImplementedException();
        }

        #region Script Block

        private class ScriptKey : MultiKey<Type, string>
        {
            public ScriptKey(Type type, string key) : base(type, key)
            {

            }
        }

        private class ScriptBlockContent : IPageContent
        {
            #region IPageContent

            public string Key => _key;

            public bool Visible => true;

            public bool RenderRequired => true;

            public Action<HtmlTextWriter> Render => RenderScript;

            private string _key;

            public ScriptBlockContent()
            {
                _key = "script_" + HttpServerUtility.UrlTokenEncode(BitConverter.GetBytes((int)DateTime.UtcNow.TimeOfDay.TotalMilliseconds));
            }

            #endregion

            private HashSet<ScriptKey> _scriptKeys = new HashSet<ScriptKey>();
            private List<string> _scriptDefs = new List<string>();

            public bool RegisterScript(Type type, string key, string script)
            {
                var scriptKey = new ScriptKey(type, key.ToLower());
                var isNew = !_scriptKeys.Contains(scriptKey);

                if (isNew)
                    _scriptDefs.Add(script);

                return isNew;
            }

            public bool IsScriptRegistered(Type type, string key)
            {
                var scriptKey = new ScriptKey(type, key.ToLower());
                return _scriptKeys.Contains(scriptKey);
            }

            private void RenderScript(HtmlTextWriter writer)
            {
                writer.Write("\r\n<script type=\"text/javascript\">\r\n//<![CDATA[\r\n");

                for (var i = 0; i < _scriptDefs.Count; i++)
                    writer.Write(_scriptDefs[i]);

                writer.Write("//]]>\r\n</script>\r\n");
            }
        }

        private ScriptBlockContent _scriptBlock = null;

        public static bool RegisterScript(Type type, string key, string script)
        {
            if (script.IsEmpty())
                return false;

            var instance = (PageFooterContentRenderer)Get(ContextType.Footer, null);

            if (instance._scriptBlock == null)
            {
                instance._scriptBlock = new ScriptBlockContent();

                instance.Register(instance._scriptBlock);
            }

            return instance._scriptBlock.RegisterScript(type, key, script);
        }

        #endregion
    }
}