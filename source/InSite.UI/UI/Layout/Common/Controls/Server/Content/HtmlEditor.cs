using System;
using System.Web;
using System.Web.UI;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class HtmlEditor : BaseEditor
    {
        #region Constants

        private const string FileUploadedJsCallback = "inSite.common.htmlEditor.onFileUploaded";

        #endregion

        #region Classes

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class ClientOptions
        {
            [JsonProperty(PropertyName = "id")]
            public string ControlID { get; set; }

            [JsonProperty(PropertyName = "translationId", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string TranslationID { get; set; }
        }

        #endregion

        #region Properties

        public string TranslationControl
        {
            get => (string)ViewState[nameof(TranslationControl)];
            set => ViewState[nameof(TranslationControl)] = value;
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
            };

            PageFooterContentRenderer.RegisterScript(
                typeof(HtmlEditor),
                "init_" + ClientID,
                $"inSite.common.htmlEditor.init({JsonHelper.SerializeJsObject(options)});");

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
                writer.AddAttribute(HtmlTextWriterAttribute.Style, "visibility:hidden;");

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