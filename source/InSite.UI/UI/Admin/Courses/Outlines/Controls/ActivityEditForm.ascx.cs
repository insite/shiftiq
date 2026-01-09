using System;

using InSite.Application.Courses.Read;
using InSite.Persistence;

using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Courses.Outlines.Controls
{
    public partial class ActivityEditForm : BaseActivityEdit
    {
        private Guid? OriginalSurveyFormIdentifier
        {
            get => (Guid?)ViewState[nameof(OriginalSurveyFormIdentifier)];
            set => ViewState[nameof(OriginalSurveyFormIdentifier)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SurveyFormIdentifier.AutoPostBack = true;
            SurveyFormIdentifier.ValueChanged += SurveyFormIdentifier_ValueChanged;

            BindControlsToHandlers(ActivitySetup, Language, ContentRepeater, ActivitySaveButton, ActivityCancelButton);
        }

        protected override void OnAlert(AlertType type, string message)
        {
            ScreenStatus.AddMessage(type, message);
        }

        protected override void BindModelToControls(QActivity activity)
        {
            OriginalSurveyFormIdentifier = activity.SurveyFormIdentifier;
            SurveyFormIdentifier.Value = activity.SurveyFormIdentifier;
        }

        protected override void BindControlsToModel(QActivity activity)
        {
            activity.SurveyFormIdentifier = SurveyFormIdentifier.Value;
        }

        private void SurveyFormIdentifier_ValueChanged(object sender, FindEntityValueChangedEventArgs e)
        {
            SurveyFormError.Visible = false;

            if (!e.NewValue.HasValue)
                return;

            if (!CourseSearch.ActivityExists(x => x.ActivityIdentifier != ActivityIdentifier && x.SurveyFormIdentifier == e.NewValue.Value))
                return;

            var entity = ServiceLocator.SurveySearch.GetSurveyForm(e.NewValue.Value);

            SurveyFormError.Visible = true;
            SurveyFormError.InnerHtml = $"<strong>{entity?.SurveyFormName}</strong> is already assigned to another course activity.";

            SurveyFormIdentifier.Value = OriginalSurveyFormIdentifier;
        }
    }
}