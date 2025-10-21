using System;

using InSite.Common.Web.UI;

namespace InSite.Admin.Messages.Messages.Controls
{
    public partial class FieldInputSurvey : BaseUserControl
    {
        public Guid? Value
        {
            get => SurveySelector.Value;
            set
            {
                SurveySelector.Value = value;

                SurveyEdit.Visible = value.HasValue;
                if (value.HasValue)
                    SurveyEdit.NavigateUrl = "/ui/admin/surveys/forms/outline?survey=" + value.Value;
            }
        }

        protected override void SetupValidationGroup(string groupName)
        {
            SurveyValidator.ValidationGroup = groupName;
        }
    }
}