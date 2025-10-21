using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;

namespace InSite.Admin.Assessments.Web.UI
{
    public class ContentRepeater : System.Web.UI.Control
    {
        #region Events

        public event EventHandler ContentInitialization;

        private void OnContentInitialization() => ContentInitialization?.Invoke(this, EventArgs.Empty);

        public class DuplicateEventArgs
        {
            public ContentRepeater ContentRepeater { get; }

            public DuplicateEventArgs(ContentRepeater content)
            {
                ContentRepeater = content;
            }
        }

        public delegate void DuplicateEventHandler(object sender, DuplicateEventArgs e);

        public event DuplicateEventHandler DuplicateInitialization;

        private void OnDuplicateInitialization(ContentRepeater content) =>
            DuplicateInitialization?.Invoke(this, new DuplicateEventArgs(content));

        #endregion

        #region Properties

        public string Key
        {
            get => (string)ViewState[nameof(Key)];
            set => ViewState[nameof(Key)] = !string.IsNullOrEmpty(value) ? value : throw new ArgumentNullException(nameof(value));
        }

        public string ControlPath
        {
            get => (string)ViewState[nameof(ControlPath)];
            set => ViewState[nameof(ControlPath)] = value;
        }

        public object InitializationData { get; set; }

        public Control Control => _control;

        #endregion

        #region Fields

        private bool? _isContent;
        private Control _control;
        private List<ContentRepeater> _repeaters;

        #endregion

        #region Construction

        public ContentRepeater()
        {
            var key = typeof(ContentRepeater) + ".Keys";

            _repeaters = (List<ContentRepeater>)HttpContext.Current.Items[key];
            if (_repeaters == null)
                HttpContext.Current.Items[key] = _repeaters = new List<ContentRepeater>();
        }

        #endregion

        protected override void OnLoad(EventArgs e)
        {
            _repeaters.Add(this);

            if (!string.IsNullOrEmpty(ControlPath))
                Controls.Add(_control = Page.LoadControl(ControlPath));

            base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (Key != null && !_isContent.HasValue)
            {
                _isContent = true;

                OnContentInitialization();

                var script = new StringBuilder();
                script.Append("(function () {")
                    .AppendFormat("var $content = $('#{0}');", ClientID)
                    .Append("if ($content.length !== 1) return;")
                    .Append("var contentHtml = $content.html();");

                foreach (var ctrl in _repeaters)
                {
                    if (ctrl.Key != Key || ctrl == this)
                        continue;

                    ctrl._isContent = false;
                    ctrl.OnDuplicateInitialization(this);

                    script.AppendFormat("$('#{0}').replaceWith(contentHtml);", ctrl.ClientID);
                }

                script.Append("$content.find('> *').unwrap();")
                    .Append("})();");

                ScriptManager.RegisterStartupScript(Page, GetType(), "init_" + Key, script.ToString(), true);
            }

            base.OnPreRender(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            if (_isContent == true)
                base.Render(writer);

            writer.RenderEndTag();
        }
    }
}