using System;
using System.Web.UI;

using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.UI.Lobby.Controls
{
    public partial class PasswordStrength : UserControl
    {
        private const int MinimumPasswordLength = 12;

        public string ControlID
        {
            get => (string)ViewState[nameof(ControlID)];
            set => ViewState[nameof(ControlID)] = value;
        }

        public string ValidationError { get; set; } = null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            HeaderLiteral.ContentKey = typeof(PasswordStrength).FullName;
        }

        protected override void OnPreRender(EventArgs e)
        {
            var control = GetControl(ControlID);

            Field.Attributes["data-control"] = control.ClientID;

            base.OnPreRender(e);
        }

        private Control GetControl(string id)
        {
            return NamingContainer.FindControl(id)
                ?? throw ApplicationError.Create("Control not found: " + ControlID);
        }

        public bool Validate()
        {
            var password = GetPasswordValue();

            if (password.HasValue())
            {
                var hasUpperCase = false;
                var hasLowerCase = false;
                var hasNumber = false;
                var hasOther = false;

                var isValid = false;

                if (password.Length >= MinimumPasswordLength)
                {
                    foreach (var ch in password)
                    {
                        if (ch >= 'A' && ch <= 'Z')
                            hasUpperCase = true;
                        else if (ch >= 'a' && ch <= 'z')
                            hasLowerCase = true;
                        else if (ch >= '0' && ch <= '9')
                            hasNumber = true;
                        else if (ch >= '!' && ch <= '/' || ch >= ':' && ch <= '@' || ch >= '[' && ch <= '`' || ch >= '{' && ch <= '~')
                            hasOther = true;

                        if (hasUpperCase && hasLowerCase && hasNumber && hasOther)
                        {
                            isValid = true;
                            break;
                        }
                    }
                }

                if (!isValid)
                {
                    ValidationError = Common.LabelHelper.GetTranslation($"Your new password must contain at least {MinimumPasswordLength} characters and it must include letters (uppercase and lowercase), numbers and symbols.");
                    return false;
                }

                foreach (var ch in password)
                {
                    if (ch < ' ' || ch > '~')
                    {
                        ValidationError = Common.LabelHelper.GetTranslation("Your new password can contain only Latin characters (uppercase and lowercase), numbers and symbols.");
                        return false;
                    }
                }
            }

            return true;
        }

        private string GetPasswordValue()
        {
            var control = GetControl(ControlID);

            if (control is ITextBox text)
                return text.Text;

            return null;
        }
    }
}