using System;
using System.Collections.Generic;
using System.Web.UI;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    [ParseChildren(true, "Text")]
    public class Alert : Control
    {
        #region Properties

        public string CssClass
        {
            get;
            set;
        }

        public AlertType Indicator
        {
            get;
            set;
        }

        public string Icon
        {
            get;
            set;
        }

        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public string Text
        {
            get;
            set;
        }

        public bool ShowClose
        {
            get => (bool?)ViewState[nameof(ShowClose)] ?? false;
            set => ViewState[nameof(ShowClose)] = value;
        }

        public bool HasMessage => Text.IsNotEmpty() || _messages.Count > 0;

        #endregion

        #region Fields

        private List<Tuple<AlertType, string, string>> _messages;

        #endregion

        #region Construction

        public Alert()
        {
            EnableViewState = false;

            _messages = new List<Tuple<AlertType, string, string>>();
        }

        #endregion

        #region Public methods

        public void AddMessage(AlertArgs args)
        {
            if (args.Icon == null)
                AddMessage(args.Type, args.Text);
            else
                AddMessage(args.Type, args.Icon, args.Text);
        }

        public void AddMessage(AlertType indicator, string text, bool isMarkdown = false)
        {
            var icon = indicator.GetIconClass();
            if (icon != null)
                icon = $"fa-solid fa-{icon} fs-xl";

            AddMessage(indicator, icon, text, isMarkdown);
        }

        public void AddMessage(AlertType indicator, string icon, string text, bool isMarkdown = false)
        {
            if (string.IsNullOrEmpty(text))
                return;

            foreach (var t in _messages)
            {
                if (t.Item3 == text)
                    return;
            }

            var message = isMarkdown ? Markdown.ToHtml(text) : text;
            _messages.Add(new Tuple<AlertType, string, string>(indicator, icon, message));
        }

        public IEnumerable<Tuple<AlertType, string, string>> GetMessages() => _messages.ToArray();

        public void Clear()
        {
            Text = null;
            Icon = null;
            Indicator = AlertType.None;

            _messages.Clear();
        }

        #endregion

        #region Rendering

        public override void RenderControl(HtmlTextWriter writer)
        {
            if (Visible)
                RenderControlInternal(writer);
        }

        public void RenderControlInternal(HtmlTextWriter writer)
        {
            var closeHtml = ShowClose
                ? "<button type='button' class='btn-close' data-bs-dismiss='alert' aria-label='Close'></button>"
                : string.Empty;

            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            RenderMessage(Indicator, Icon, Text);

            foreach (var message in _messages)
                RenderMessage(message.Item1, message.Item2, message.Item3);

            writer.RenderEndTag();

            void RenderMessage(AlertType indicator, string icon, string text)
            {
                if (string.IsNullOrEmpty(text))
                    return;

                writer.AddAttribute(
                    HtmlTextWriterAttribute.Class,
                    ControlHelper.MergeCssClasses(
                        "alert d-flex",
                        ShowClose ? "alert-dismissible fade show" : null,
                        "alert-" + indicator.GetContextualClass(),
                        CssClass));
                writer.AddAttribute("role", "alert");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                if (icon.IsNotEmpty())
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, $"{icon} me-2");
                    writer.RenderBeginTag(HtmlTextWriterTag.I);
                    writer.RenderEndTag();
                }

                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.Write(text);
                writer.RenderEndTag();

                writer.Write(closeHtml);

                writer.RenderEndTag();
            }
        }

        #endregion
    }
}