using System;
using System.Web.UI.WebControls;

namespace InSite.Common.Web.UI
{
    public class PatternValidator : RegularExpressionValidator
    {
        private const string DefaultImageAlt = "Unexpected Format";

        public virtual string FieldName { get; set; }

        public PatternValidator()
        {
            Display = ValidatorDisplay.Static;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var imageAlt = DefaultImageAlt;
            if (!string.IsNullOrEmpty(ErrorMessage))
                imageAlt = ErrorMessage;

            if (string.IsNullOrEmpty(Text))
                Text = $"<sup class='text-danger'><i class='far fa-exclamation-circle fa-xs' title='{imageAlt}'></i></sup>";
        }

        public void ResetErrorMessage()
        {
            if (!string.IsNullOrEmpty(FieldName))
                ErrorMessage = string.Format(Shift.Constant.ErrorMessage.UnexpectedFormat, FieldName);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (string.IsNullOrEmpty(ErrorMessage))
                ResetErrorMessage();
        }
    }
}