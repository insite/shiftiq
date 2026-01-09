using System.Web;

using InSite.Common.Web.UI;
using InSite.Domain.Surveys.Forms;

namespace InSite.Admin.Workflow.Forms.Controls
{
    public partial class OutlineContentField : BaseUserControl
    {
        internal void LoadData(SurveyForm survey, SurveyContentLabel label, bool canEdit)
        {
            //ChangeLink.Title = "Change " + label.Name;
            ChangeLink.NavigateUrl = $"/ui/admin/workflow/forms/change-content?form={survey.Identifier}&tab={HttpUtility.UrlEncode(label.Name)}";
            ChangeLink.Visible = canEdit;

            Output.LoadData(survey.Content?[label.Name]);

            FieldDescription.InnerText = label.Description;
        }
    }
}