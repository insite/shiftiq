using System;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Admin.Assessments.Questions.Utilities;
using InSite.Common.Web.UI;
using InSite.Domain.Attempts;

using Shift.Common;

namespace InSite.Admin.Assessments.Questions.Controls
{
    public partial class QuestionPreviewMatching : QuestionPreviewControl
    {
        private class DataItem
        {
            public string Left { get; set; }
            public string Right { get; set; }
            public string[] Options { get; set; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RowRepeater.ItemDataBound += RowRepeater_ItemDataBound;
        }

        private void RowRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var dataItem = (DataItem)e.Item.DataItem;
            var selector = (ComboBox)e.Item.FindControl("Selector");

            selector.LoadItems(dataItem.Options);
            selector.Value = dataItem.Right;
        }

        public override void LoadData(PreviewQuestionModel model)
        {
            var question = (AttemptQuestionMatch)model.AttemptQuestion;
            var options = question.Pairs.Select(x => x.RightText).Concat(question.Distractors).Distinct().ToArray();
            options.Shuffle();

            RowRepeater.DataSource = question.Pairs.Select(pair => new DataItem
            {
                Left = Markdown.ToHtml(pair.LeftText),
                Right = pair.RightText,
                Options = options
            });
            RowRepeater.DataBind();
        }
    }
}