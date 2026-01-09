using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Domain.Attempts;
using InSite.UI.Portal.Assessments.Attempts.Utilities;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Questions.Utilities
{
    public abstract class QuestionPreviewControl : BaseUserControl
    {
        protected class CellInfo
        {
            public string CssClass { get; set; }
            public string Style { get; set; }
            public string Html { get; set; }
        }

        protected interface IRowDataItem
        {
            IEnumerable<CellInfo> Cells { get; set; }
        }

        public abstract void LoadData(PreviewQuestionModel model);

        protected void BindTable<TRowDataItem>(
            Repeater headerRepeater,
            Repeater rowRepeater,
            PreviewQuestionModel model,
            Func<int, int, AttemptOption, TRowDataItem> createItem) where TRowDataItem : IRowDataItem
        {
            var question = (AttemptQuestionDefault)model.AttemptQuestion;

            if (model.BankQuestion.Layout.Type == OptionLayoutType.Table)
            {
                headerRepeater.Visible = true;

                var tableData = QuestionTable.Build(
                model.BankQuestion.Layout.Columns,
                question.Options.Select(x => x.Text));
                var tableHeader = tableData.GetHeader();
                var tableBody = tableData.GetBody();

                headerRepeater.Visible = tableHeader.Any(cell => !string.IsNullOrEmpty(cell.Text));
                headerRepeater.DataSource = tableHeader.Select(cell => new CellInfo
                {
                    CssClass = ControlHelper.MergeCssClasses("text", cell.CssClass),
                    Style = $"text-align:{cell.Alignment.ToString().ToLower()}",
                    Html = Markdown.ToHtml(cell.Text)
                });
                headerRepeater.DataBind();

                rowRepeater.Visible = true;
                rowRepeater.DataSource = question.Options.Select((om, i) =>
                {
                    var row = tableBody[i];
                    var dataItem = createItem(model.Sequence, i + 1, om);

                    dataItem.Cells = row.Select(cell => new CellInfo
                    {
                        CssClass = ControlHelper.MergeCssClasses("text", cell.CssClass),
                        Style = $"text-align:{cell.Alignment.ToString().ToLower()}",
                        Html = Markdown.ToHtml(cell.Text)
                    });

                    return dataItem;
                });
            }
            else
            {
                headerRepeater.Visible = false;

                rowRepeater.DataSource = question.Options.Select((om, i) =>
                {
                    var dataItem = createItem(model.Sequence, i + 1, om);

                    dataItem.Cells = new[]
                    {
                        new CellInfo
                        {
                            CssClass = "text",
                            Style = string.Empty,
                            Html = Markdown.ToHtml(om.Text)
                        }
                    };

                    return dataItem;
                });
            }

            rowRepeater.ItemDataBound += RowRepeater_ItemDataBound;
            rowRepeater.DataBind();
        }

        private void RowRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var cellRepeater = (Repeater)e.Item.FindControl("CellRepeater");
            cellRepeater.DataSource = DataBinder.Eval(e.Item.DataItem, "Cells");
            cellRepeater.DataBind();
        }
    }
}
