using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class EmailValidator : PatternValidator
    {
        public bool IsEmailList
        {
            get { return (bool?)ViewState[nameof(IsEmailList)] ?? false; }
            set
            {
                ViewState[nameof(IsEmailList)] = value;

                ValidationExpression = value
                    ? string.Format("^({0})( *; *({0}))* *;? *$",
                        Pattern.ValidEmail.Substring(1, Pattern.ValidEmail.Length - 2))
                    : Pattern.ValidEmail;
            }
        }

        public string Identifier { get; set; }

        public override string FieldName
        {
            get { return Global.Translate(base.FieldName); }
            set
            {
                base.FieldName = value;
                Identifier = value;
            }
        }

        public EmailValidator()
        {
            ValidationExpression = Pattern.ValidEmail;
        }
    }
}