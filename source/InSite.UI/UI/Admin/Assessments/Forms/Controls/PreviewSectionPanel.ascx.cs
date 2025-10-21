using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using InSite.Admin.Assessments.Questions.Controls;
using InSite.Admin.Assessments.Questions.Utilities;
using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.UI.Admin.Assessments.Forms.Controls
{
    public partial class PreviewSectionPanel : BaseUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            QuestionRepeater.ItemDataBound += QuestionRepeater_ItemDataBound;
        }

        public void LoadData(string summary, IEnumerable<PreviewQuestionModel> questions)
        {
            Summary.Visible = summary.IsNotEmpty();
            Summary.InnerHtml = summary;

            QuestionRepeater.DataSource = questions;
            QuestionRepeater.DataBind();
        }

        private void QuestionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var previewPanel = (QuestionPreviewPanel)e.Item.FindControl("PreviewPanel");
            previewPanel.LoadData((PreviewQuestionModel)e.Item.DataItem);
        }
    }
}