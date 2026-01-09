using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class DateTimeCompareValidator : BaseValidator
    {
        #region Properties

        public string ControlToCompare
        {
            get => (string)ViewState[nameof(ControlToCompare)] ?? string.Empty;
            set => ViewState[nameof(ControlToCompare)] = value;
        }

        public ValidationCompareOperator Operator
        {
            get => (ValidationCompareOperator)(ViewState[nameof(Operator)] ?? ValidationCompareOperator.Equal);
            set
            {
                if (!Enum.IsDefined(typeof(ValidationCompareOperator), value))
                    throw ApplicationError.Create("Invalid operator value: {0}", (int)value);

                if (value == ValidationCompareOperator.DataTypeCheck)
                    throw ApplicationError.Create("DataTypeCheck operator is not supported");

                ViewState[nameof(Operator)] = value;
            }
        }

        public string ValueToCompare
        {
            get => (string)ViewState[nameof(ValueToCompare)] ?? string.Empty;
            set
            {
                if (!string.IsNullOrEmpty(value) && !DateTimeOffset.TryParse(value, out _))
                    throw ApplicationError.Create("Invalid DateTimeOffset value: {0}", value);

                ViewState[nameof(ValueToCompare)] = value;
            }
        }

        #endregion

        protected override bool ControlPropertiesValid()
        {
            if (!string.IsNullOrEmpty(ControlToCompare))
            {
                if (string.Equals(ControlToValidate, ControlToCompare, StringComparison.OrdinalIgnoreCase))
                    throw ApplicationError.Create("Control '{0}' cannot have the same value '{1}' for both ControlToValidate and ControlToCompare", ClientID, ControlToCompare);

                CheckControlValidationProperty(ControlToCompare, "ControlToCompare");
            }
            else if (string.IsNullOrEmpty(ValueToCompare))
            {
                throw ApplicationError.Create("ValueToCompare is not defined");
            }

            return base.ControlPropertiesValid();
        }

        protected override bool EvaluateIsValid()
        {
            var ctrlValidate = NamingContainer.FindControl(ControlToValidate) as DateTimeOffsetSelector;
            if (ctrlValidate == null)
                throw ApplicationError.Create("Unexpected control type: {0}", ControlToValidate);

            if (!ctrlValidate.Value.HasValue)
                return true;

            DateTimeOffset? compareValue = null;

            if (!string.IsNullOrEmpty(ControlToCompare))
            {
                var ctrlCompare = NamingContainer.FindControl(ControlToCompare) as DateTimeOffsetSelector;
                if (ctrlCompare == null)
                    throw ApplicationError.Create("Unexpected control type: {0}", ControlToCompare);

                compareValue = ctrlCompare.Value;
            }
            else if (!string.IsNullOrEmpty(ValueToCompare))
            {
                compareValue = DateTimeOffset.Parse(ValueToCompare);
            }

            if (!compareValue.HasValue)
                return true;

            var index = ctrlValidate.Value.Value.CompareTo(compareValue.Value);

            switch (Operator)
            {
                case ValidationCompareOperator.Equal:
                    return index == 0;
                case ValidationCompareOperator.NotEqual: 
                    return index != 0;
                case ValidationCompareOperator.GreaterThan: 
                    return index > 0;
                case ValidationCompareOperator.GreaterThanEqual: 
                    return index >= 0;
                case ValidationCompareOperator.LessThan: 
                    return index < 0;
                case ValidationCompareOperator.LessThanEqual: 
                    return index <= 0;
                default: 
                    return true;
            }
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);

            AddAttribute("evaluationfunction", "inSite.common.validators.dateTimeCompareValidator.validate", false);

            if (!string.IsNullOrEmpty(ControlToCompare))
                AddAttribute("controltocompare", GetControlRenderID(ControlToCompare));

            if (!string.IsNullOrEmpty(ValueToCompare))
            {
                var date = DateTimeOffset.Parse(ValueToCompare);
                AddAttribute("valuetocompare", date.UtcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss"));
            }

            if (Operator != ValidationCompareOperator.Equal)
                AddAttribute("operator", Operator.GetName());

            void AddAttribute(string name, string value, bool encode = true)
            {
                if (Page.UnobtrusiveValidationMode == UnobtrusiveValidationMode.WebForms)
                    writer.AddAttribute("data-val-" + name, value, encode);
                else
                    ScriptManager.RegisterExpandoAttribute(this, ClientID, name, value, encode);
            }
        }
    }
}