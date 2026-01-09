using System;

using InSite.Common.Web.UI;

namespace InSite.Admin.Messages.Messages.Controls
{
    public partial class FieldInputForm : BaseUserControl
    {
        public Guid? Value
        {
            get => SurveySelector.Value;
            set
            {
                SurveySelector.Value = value;

                SurveyEdit.Visible = value.HasValue;
                if (value.HasValue)
                    SurveyEdit.NavigateUrl = "/ui/admin/workflow/forms/outline?form=" + value.Value;
            }
        }

        protected override void SetupValidationGroup(string groupName)
        {
            SurveyValidator.ValidationGroup = groupName;
        }
    }
}