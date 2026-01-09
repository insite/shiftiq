using System.Collections.Generic;
using System.Web.UI.WebControls;

using InSite.Admin.Assets.Contents.Controls;
using InSite.Common.Web.UI;
using InSite.Domain.Surveys.Forms;

namespace InSite.Admin.Workflow.Forms.Controls
{
    public partial class OutlineContentFieldList : BaseUserControl
    {
        private SurveyForm _survey;

        internal void LoadData(SurveyForm survey, List<SurveyContentLabel> labels, bool canEdit)
        {
            ChangeLink.NavigateUrl = $"/ui/admin/workflow/forms/change-content?form={survey.Identifier}&tab=Instructions";
            ChangeLink.Visible = canEdit;

            _survey = survey;

            LabelRepeater.ItemDataBound += LabelRepeater_ItemDataBound;
            LabelRepeater.DataSource = labels;
            LabelRepeater.DataBind();
        }

        private void LabelRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var label = (SurveyContentLabel)e.Item.DataItem;

            var output = (MultilingualStringInfo)e.Item.FindControl("Output");
            output.LoadData(_survey.Content?[label.Name]);
        }

        protected string GetLabelTitle()
        {
            var label = (SurveyContentLabel)Page.GetDataItem();
            return ChangeContent.GetLabelTitle(label.Name);
        }
    }
}