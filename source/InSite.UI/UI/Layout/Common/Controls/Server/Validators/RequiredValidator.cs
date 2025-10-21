using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using Humanizer;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class RequiredValidator : BaseValidator
    {
        #region Properties

        public virtual string FieldName
        {
            get => (string)ViewState[nameof(FieldName)];
            set
            {
                ViewState[nameof(FieldName)] = value;

                if (!string.IsNullOrEmpty(value))
                    ErrorMessage = string.Format(Shift.Constant.ErrorMessage.MissingRequiredField, value);
            }
        }

        public virtual string FieldNameControl
        {
            get => (string)ViewState[nameof(FieldNameControl)];
            set => ViewState[nameof(FieldNameControl)] = value;
        }

        public ValidatorRenderModeEnum RenderMode
        {
            get => (ValidatorRenderModeEnum)(ViewState[nameof(RenderMode)] ?? ValidatorRenderModeEnum.AsteriskAndExclamation);
            set => ViewState[nameof(RenderMode)] = Enum.IsDefined(typeof(ValidatorRenderModeEnum), value) ? value : (object)null;
        }

        public string InitialValue
        {
            get => (string)ViewState[nameof(InitialValue)] ?? string.Empty;
            set => ViewState[nameof(InitialValue)] = value;
        }

        #endregion

        #region Construction

        public RequiredValidator()
        {
            CssClass = "text-danger";
        }

        #endregion

        protected override bool EvaluateIsValid()
        {
            var value = GetControlValidationValue(ControlToValidate);

            return value == null || !value.Trim().Equals(InitialValue.Trim());
        }

        protected new string GetControlValidationValue(string name)
        {
            var ctrl = NamingContainer.FindControl(name);
            if (ctrl == null)
                return null;

            var prop = GetValidationProperty(ctrl);
            if (prop == null)
                return null;

            var value = prop.GetValue(ctrl);

            if (value is System.Web.UI.WebControls.ListItem listItem)
                return listItem.Value;

            if (value is IEnumerable<string> strEnum)
                return string.Join(",", strEnum);

            if (value is IEnumerable<Guid> guidEnum)
                return string.Join(",", guidEnum);

            if (value != null)
                return value.ToString();

            return string.Empty;
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (!string.IsNullOrEmpty(FieldNameControl))
            {
                var ctrl = NamingContainer.FindControl(FieldNameControl) as ITextControl
                    ?? throw ApplicationError.Create("ITextControl not found: {0}", FieldName);
                FieldName = ctrl.Text;
            }

            if (string.IsNullOrEmpty(Text))
            {
                if (RenderMode == ValidatorRenderModeEnum.Exclamation || RenderMode == ValidatorRenderModeEnum.AsteriskAndExclamation)
                    Text = "<sup class='text-danger'><i class='far fa-exclamation ms-1'></i></sup>";
                else if (RenderMode == ValidatorRenderModeEnum.Dot)
                    Text = "<sup class='text-danger'><i class='fas fa-circle fa-xs'></i></sup>";
            }

            if (string.IsNullOrEmpty(ErrorMessage))
                ErrorMessage = $"{ControlToValidate.Humanize(LetterCasing.Title)} is a required field.";

            base.OnPreRender(e);
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);

            AddAttribute("evaluationfunction", "inSite.common.validators.requiredValidator.validate", false);

            if (!string.IsNullOrEmpty(InitialValue))
                AddAttribute("initialvalue", InitialValue);

            void AddAttribute(string name, string value, bool encode = true)
            {
                if (Page.UnobtrusiveValidationMode == UnobtrusiveValidationMode.WebForms)
                    writer.AddAttribute("data-val-" + name, value, encode);
                else
                    ScriptManager.RegisterExpandoAttribute(this, ClientID, name, value, encode);
            }
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            if (Visible && RenderMode == ValidatorRenderModeEnum.AsteriskAndExclamation)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "text-danger");
                writer.RenderBeginTag(HtmlTextWriterTag.Sup);
                writer.Write("<i class='far fa-asterisk fa-xs'></i>");
                writer.RenderEndTag();
            }

            base.RenderControl(writer);
        }
    }
}