using System;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Admin.Assessments.Questions.Utilities;
using InSite.Domain.Attempts;

namespace InSite.UI.Admin.Assessments.Questions.Controls
{
    public partial class QuestionPreviewLikert : QuestionPreviewControl
    {
        private int _selectedOptionKey;
        private string _groupName;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            LikertRowRepeater.ItemDataBound += LikertRowRepeater_ItemDataBound;
        }

        private void LikertRowRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var item = e.Item;
            var row = (AttemptQuestionDefault)item.DataItem;

            _selectedOptionKey = row.Options.OrderByDescending(x => x.Points).First().Key;

            var repeater = (Repeater)item.FindControl("LikertOptionRepeater");
            repeater.DataSource = row.Options;
            repeater.DataBind();
        }

        public override void LoadData(PreviewQuestionModel model)
        {
            var question = (AttemptQuestionLikert)model.AttemptQuestion;

            _groupName = $"group_{model.Sequence}";

            LikertColumnRepeater.DataSource = question.Questions[0].Options;
            LikertColumnRepeater.DataBind();

            LikertRowRepeater.DataSource = question.Questions;
            LikertRowRepeater.DataBind();
        }

        protected bool IsOptionChecked()
        {
            var option = (AttemptOption)Page.GetDataItem();
            return option.Key == _selectedOptionKey;
        }

        protected string GetOptionGroup()
        {
            var option = (AttemptOption)Page.GetDataItem();

            return _groupName + "_" + option.Key;
        }
    }
}