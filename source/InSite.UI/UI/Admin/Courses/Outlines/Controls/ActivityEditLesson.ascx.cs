using System;

using InSite.Application.Courses.Read;

using Shift.Constant;

namespace InSite.Admin.Courses.Outlines.Controls
{
    public partial class ActivityEditLesson : BaseActivityEdit
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            BindControlsToHandlers(ActivitySetup, Language, ContentRepeater, ActivitySaveButton, ActivityCancelButton);
        }

        protected override void OnAlert(AlertType type, string message)
        {
            ScreenStatus.AddMessage(type, message);
        }

        protected override void BindModelToControls(QActivity activity)
        {

        }

        protected override void BindControlsToModel(QActivity activity)
        {

        }
    }
}