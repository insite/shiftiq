using System;

using InSite.Domain.Surveys.Forms;

using Shift.Common;

namespace InSite.Admin.Surveys.Forms.Controls
{
    public partial class SurveyFormInfo : System.Web.UI.UserControl
    {
        public void BindSurvey(SurveyState survey, TimeZoneInfo tz, bool showName = true, bool showStatus = true)
        {
            SurveyLink.HRef = $"/ui/admin/surveys/forms/outline?survey={survey.Form.Identifier}";
            InternalName.Text = survey.Form.Name;
            ExternalTitle.Text = survey.Form.GetTitle();
            CurrentStatus.Text = survey.Form.Status.ToString();
            Opened.Text = survey.Form.Opened.HasValue ? survey.Form.Opened.Value.Format(tz, true) : "None";
            Closed.Text = survey.Form.Closed.HasValue ? survey.Form.Closed.Value.Format(tz, true) : "None";

            NameField.Visible = showName;
            StatusContainer.Visible = showStatus;
        }
    }
}