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
                UpdateValidationExpression();
            }
        }

        public bool AllowMissingTld
        {
            get { return (bool?)ViewState[nameof(AllowMissingTld)] ?? false; }
            set
            {
                ViewState[nameof(AllowMissingTld)] = value;
                UpdateValidationExpression();
            }
        }

        private void UpdateValidationExpression()
        {
            var basePattern = AllowMissingTld ? Pattern.ValidEmailNoTld : Pattern.ValidEmail;

            ValidationExpression = IsEmailList
                ? string.Format("^({0})( *; *({0}))* *;? *$",
                    basePattern.Substring(1, basePattern.Length - 2))
                : basePattern;
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