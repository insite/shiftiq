using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common;

using Shift.Constant;

namespace InSite.Common.Web.UI
{
    [ParseChildren(true)]
    public class Modal : Control
    {
        #region Classes

        public class ContentControl : Control, INamingContainer
        {

        }

        #endregion

        #region Properties

        [PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(ContentControl)), TemplateInstance(TemplateInstance.Single)]
        public ITemplate HeaderTemplate
        {
            get { return _headerTemplate; }
            set { _headerTemplate = value; InitTemplate(_headerTemplate, ref _headerContent); }
        }

        [PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(ContentControl)), TemplateInstance(TemplateInstance.Single)]
        public ITemplate ContentTemplate
        {
            get { return _bodyTemplate; }
            set { _bodyTemplate = value; InitTemplate(_bodyTemplate, ref _bodyContent); }
        }

        [PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(ContentControl)), TemplateInstance(TemplateInstance.Single)]
        public ITemplate FooterTemplate
        {
            get { return _footerTemplate; }
            set { _footerTemplate = value; InitTemplate(_footerTemplate, ref _footerContent); }
        }

        public string Title
        {
            get => (string)ViewState[nameof(Title)];
            set => ViewState[nameof(Title)] = value;
        }

        public bool EnableAnimation
        {
            get => (bool)(ViewState[nameof(EnableAnimation)] ?? false);
            set => ViewState[nameof(EnableAnimation)] = value;
        }

        public string CssClass
        {
            get => (string)ViewState[nameof(CssClass)];
            set => ViewState[nameof(CssClass)] = value;
        }

        public Unit Width
        {
            get => (Unit)(ViewState[nameof(Width)] ?? Unit.Empty);
            set => ViewState[nameof(Width)] = value;
        }

        public Unit MinHeight
        {
            get => (Unit)(ViewState[nameof(MinHeight)] ?? Unit.Empty);
            set => ViewState[nameof(MinHeight)] = value;
        }

        public bool VisibleOnLoad
        {
            get => (bool)(ViewState[nameof(VisibleOnLoad)] ?? false);
            set => ViewState[nameof(VisibleOnLoad)] = value;
        }

        public bool EnableCloseButton
        {
            get => (bool)(ViewState[nameof(EnableCloseButton)] ?? true);
            set => ViewState[nameof(EnableCloseButton)] = value;
        }

        public bool EnalbeCloseOnEscape
        {
            get => (bool)(ViewState[nameof(EnalbeCloseOnEscape)] ?? true);
            set => ViewState[nameof(EnalbeCloseOnEscape)] = value;
        }

        public bool EnableStaticBackdrop
        {
            get => (bool)(ViewState[nameof(EnableStaticBackdrop)] ?? false);
            set => ViewState[nameof(EnableStaticBackdrop)] = value;
        }

        public bool Scrollable
        {
            get => (bool)(ViewState[nameof(Scrollable)] ?? false);
            set => ViewState[nameof(Scrollable)] = value;
        }

        public bool Centered
        {
            get => (bool)(ViewState[nameof(Centered)] ?? false);
            set => ViewState[nameof(Centered)] = value;
        }

        public ModalSize Size
        {
            get => (ModalSize)(ViewState[nameof(Size)] ?? ModalSize.Default);
            set => ViewState[nameof(Size)] = value;
        }

        public ModalFullscreen Fullscreen
        {
            get => (ModalFullscreen)(ViewState[nameof(Fullscreen)] ?? ModalFullscreen.Disabled);
            set => ViewState[nameof(Fullscreen)] = value;
        }

        private string HeaderTitleClientID => $"{ClientID}_label";

        #endregion

        #region Fields

        private Control _headerContent = null;
        private Control _bodyContent = null;
        private Control _footerContent = null;
        private ITemplate _headerTemplate = null;
        private ITemplate _bodyTemplate = null;
        private ITemplate _footerTemplate = null;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            EnsureChildControls();

            base.OnInit(e);
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            InitTemplate(HeaderTemplate, ref _headerContent);
            InitTemplate(ContentTemplate, ref _bodyContent);
            InitTemplate(FooterTemplate, ref _footerContent);
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (VisibleOnLoad)
                ScriptManager.RegisterStartupScript(
                    Page,
                    typeof(Modal),
                    "show_onload",
                    $"$(document).ready(function () {{ $('#{ClientID}').modal('show'); }});",
                    true);

            base.OnPreRender(e);
        }

        private void InitTemplate(ITemplate template, ref Control content)
        {
            if (template == null || content != null)
                return;

            content = new ContentControl();
            template.InstantiateIn(content);
            Controls.Add(content);
        }

        public override void DataBind()
        {
            _headerContent?.DataBind();
            _bodyContent?.DataBind();
            _footerContent?.DataBind();
        }

        #endregion

        #region Rendering

        public override void RenderControl(HtmlTextWriter writer)
        {
            if (!Visible)
                return;

            var cssClass = "modal insite-modal";

            if (EnableAnimation)
                cssClass += " fade";

            if (VisibleOnLoad)
                cssClass += " show";

            // DIV.modal

            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, ControlHelper.MergeCssClasses(cssClass, CssClass));
            writer.AddAttribute(HtmlTextWriterAttribute.Tabindex, "-1");
            writer.AddAttribute("role", "dialog");
            writer.AddAttribute("aria-labelledby", HeaderTitleClientID);
            writer.AddAttribute("aria-hidden", "true");

            if (!EnalbeCloseOnEscape)
                writer.AddAttribute("data-bs-keyboard", "false");

            if (EnableStaticBackdrop)
                writer.AddAttribute("data-bs-backdrop", "static");

            writer.AddAttribute("data-content", _bodyContent == null ? "dynamic" : "static");

            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            WriteDialog(writer);

            writer.RenderEndTag();
        }

        private void WriteDialog(HtmlTextWriter writer)
        {
            // DIV.modal-dialog

            if (!Width.IsEmpty)
                writer.AddStyleAttribute("max-width", Width.ToString());

            var cssClass = "modal-dialog";

            if (Size != ModalSize.Default)
                cssClass += " " + Size.GetContextualClass();

            if (Fullscreen != ModalFullscreen.Disabled)
                cssClass += " " + Fullscreen.GetContextualClass();

            if (Scrollable)
                cssClass += " modal-dialog-scrollable";

            if (Centered)
                cssClass += " modal-dialog-centered";

            if (Centered)
                cssClass += " modal-dialog-centered";

            writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);
            writer.AddAttribute("role", "document");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            WriteContent(writer);

            writer.RenderEndTag();
        }

        private void WriteContent(HtmlTextWriter writer)
        {
            // DIV.modal-content

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "modal-content");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            WriteHeader(writer);
            WriteBody(writer);
            WriteFooter(writer);

            writer.RenderEndTag();
        }

        private void WriteHeader(HtmlTextWriter writer)
        {
            // DIV.modal-header

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "modal-header");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            if (_headerContent == null)
            {
                WriteHeaderTitle(writer);
                WriteHeaderCloseButton(writer);
            }
            else
            {
                _headerContent.RenderControl(writer);
            }

            writer.RenderEndTag();
        }

        private void WriteHeaderCloseButton(HtmlTextWriter writer)
        {
            if (!EnableCloseButton)
                return;

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "btn-close");
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "button");
            writer.AddAttribute("data-bs-dismiss", "modal");
            writer.AddAttribute("aria-label", "Close");
            writer.RenderBeginTag(HtmlTextWriterTag.Button);

            writer.RenderEndTag();
        }

        private void WriteHeaderTitle(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Id, HeaderTitleClientID);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "modal-title");

            writer.RenderBeginTag(HtmlTextWriterTag.H5);

            if (Title.IsNotEmpty())
                writer.Write(Title);

            writer.RenderEndTag();
        }

        private void WriteBody(HtmlTextWriter writer)
        {
            // DIV.modal-body

            if (!MinHeight.IsEmpty)
                writer.AddAttribute(HtmlTextWriterAttribute.Style, $"min-height:{MinHeight}");

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "modal-body");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            if (_bodyContent != null)
                _bodyContent.RenderControl(writer);

            writer.RenderEndTag();
        }

        private void WriteFooter(HtmlTextWriter writer)
        {
            if (_footerContent == null)
                return;

            // DIV.modal-footer

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "modal-footer");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            _footerContent.RenderControl(writer);

            writer.RenderEndTag();
        }

        #endregion
    }
}