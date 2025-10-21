using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class MarkdownEditor : BaseEditor
    {
        #region Constants

        private const string FileUploadedJsCallback = "inSite.common.markdownEditor.onFileUploaded";

        #endregion

        #region Classes

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class ClientOptions
        {
            [JsonProperty(PropertyName = "id")]
            public string ControlID { get; set; }

            [JsonProperty(PropertyName = "translationId", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string TranslationID { get; set; }

            [JsonProperty(PropertyName = "onSetup", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string OnSetup { get; set; }

            [JsonProperty(PropertyName = "onInited", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string OnInited { get; set; }

            [JsonProperty(PropertyName = "onSetText", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string OnSetText { get; set; }

            [JsonProperty(PropertyName = "onGetText", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string OnGetText { get; set; }

            [JsonProperty(PropertyName = "onPreview", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string OnPreview { get; set; }
        }

        #endregion

        #region Properties

        public string TranslationControl
        {
            get => (string)ViewState[nameof(TranslationControl)];
            set => ViewState[nameof(TranslationControl)] = value;
        }

        public Unit MinHeight
        {
            get => (Unit)(ViewState[nameof(MinHeight)] ?? Unit.Empty);
            set => ViewState[nameof(MinHeight)] = value;
        }

        [PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public MarkdownEditorClientEvents ClientEvents => _clientEvents;

        #endregion

        #region Fields

        private MarkdownEditorClientEvents _clientEvents;

        #endregion

        #region Construction

        public MarkdownEditor()
        {
            _clientEvents = new MarkdownEditorClientEvents(nameof(ClientEvents), ViewState);
        }

        #endregion

        #region Initialization and Loading

        protected override void OnPreRender(EventArgs e)
        {
            var translationControl = TranslationControl.IsEmpty()
                ? null
                : (EditorTranslation)(NamingContainer is DynamicControl
                    ? NamingContainer.NamingContainer.FindControl(TranslationControl)
                    : NamingContainer.FindControl(TranslationControl));

            var options = new ClientOptions
            {
                ControlID = ClientID,
                TranslationID = translationControl?.ClientID,
                OnSetup = ClientEvents.OnSetup.NullIfEmpty(),
                OnInited = ClientEvents.OnInited.NullIfEmpty(),
                OnSetText = ClientEvents.OnSetText.NullIfEmpty(),
                OnGetText = ClientEvents.OnGetText.NullIfEmpty(),
                OnPreview = ClientEvents.OnPreview.NullIfEmpty()
            };

            PageFooterContentRenderer.RegisterScript(
                typeof(MarkdownEditor),
                "init_" + ClientID,
                $"inSite.common.markdownEditor.init({JsonHelper.SerializeJsObject(options)});");

            base.OnPreRender(e);
        }

        #endregion

        #region Event handlers

        protected override void OnUploadSuccess(EditorUpload.SuccessEventArgs args)
        {
            args.Callback.Function = FileUploadedJsCallback;
            args.Callback.Arguments = new UploadSuccessArgs(ClientID, args);
        }

        #endregion

        #region Rendering

        protected override void Render(HtmlTextWriter writer)
        {
            if (CssClass.IsNotEmpty())
                writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClass);

            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            { // textarea
                writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
                writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);

                var style = new Shift.Sdk.UI.CssStyleCollection();

                style["visibility"] = "hidden";

                if (!Width.IsEmpty)
                    style["width"] = Width.ToString();

                if (!Height.IsEmpty)
                    style["height"] = Height.ToString();

                if (!MinHeight.IsEmpty)
                    style["min-height"] = MinHeight.ToString();

                if (!style.IsEmpty)
                    writer.AddAttribute(HtmlTextWriterAttribute.Style, style.ToString());

                writer.RenderBeginTag(HtmlTextWriterTag.Textarea);

                HttpUtility.HtmlEncode(Value, writer);

                writer.RenderEndTag();
            }

            writer.RenderEndTag();
        }

        #endregion

        #region Helpers

        public void SetupCallback(EditorUpload.CallbackData callback, string title, string url)
        {
            callback.Function = FileUploadedJsCallback;
            callback.Arguments = new UploadSuccessArgs(ClientID, title, url);
        }

        #endregion
    }
}