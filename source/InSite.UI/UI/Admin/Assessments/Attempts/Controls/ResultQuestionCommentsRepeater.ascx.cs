using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Attempts.Read;
using InSite.Application.Banks.Read;
using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.Admin.Assessments.Attempts.Controls
{
    public partial class ResultQuestionCommentsRepeater : BaseUserControl
    {
        #region Classes

        private class ExamItem
        {
            public string Title { get; set; }

            public IEnumerable<QuestionItem> Questions { get; set; }
        }

        private class QuestionItem
        {
            public int Sequence { get; set; }
            public string Code { get; set; }
            public string Html { get; set; }

            public IEnumerable<QAttemptCommentExtended> Comments { get; set; }
        }

        #endregion

        #region Properties

        private IAttemptSearch AttemptSearch => ServiceLocator.AttemptSearch;
        private IBankSearch AssessmentSearch => ServiceLocator.BankSearch;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ExamRepeater.ItemCreated += ExamRepeater_ItemCreated;
            ExamRepeater.ItemDataBound += ExamRepeater_ItemDataBound;
        }

        #endregion

        #region Event handlers

        private void ExamRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var questionRepeater = (Repeater)e.Item.FindControl("QuestionRepeater");
            questionRepeater.ItemDataBound += QuestionRepeater_ItemDataBound;
        }

        private void ExamRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var dataItem = (ExamItem)e.Item.DataItem;

            var questionRepeater = (Repeater)e.Item.FindControl("QuestionRepeater");
            questionRepeater.DataSource = dataItem.Questions;
            questionRepeater.DataBind();
        }

        private void QuestionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var questionItem = (QuestionItem)e.Item.DataItem;

            var commentRepeater = (Repeater)e.Item.FindControl("CommentRepeater");
            commentRepeater.DataSource = questionItem.Comments;
            commentRepeater.DataBind();
        }

        #endregion

        #region Loading

        public int LoadData(QAttemptFilter filter)
        {
            var data = GetDataSource(filter);

            ExamRepeater.DataSource = data;
            ExamRepeater.DataBind();

            return data.Sum(x => x.Questions.Count());
        }

        private IReadOnlyList<ExamItem> GetDataSource(QAttemptFilter filter)
        {
            var comments = AttemptSearch.GetVAttemptComments(filter);
            var commentsByQuestionId = comments
                .GroupBy(x => x.QuestionIdentifier)
                .ToDictionary(
                    x => x.Key,
                    x => x.OrderByDescending(y => y.CommentPosted).ToArray());

            var examFilter = comments.Select(x => x.FormIdentifier).Distinct().ToArray();
            var exams = AssessmentSearch.GetForms(examFilter);

            var result = new List<ExamItem>();

            foreach (var exam in exams.OrderBy(exam => exam.FormAsset))
            {
                var form = AssessmentSearch.GetFormData(exam.FormIdentifier);

                var item = new ExamItem
                {
                    Title = form.Content.Title?.Default ?? form.Name,
                    Questions = form.GetQuestions()
                        .Where(question => commentsByQuestionId.ContainsKey(question.Identifier))
                        .Select(question => new QuestionItem
                        {
                            Sequence = question.Sequence,
                            Code = question.Classification.Code,
                            Html = Markdown.ToHtml(question.Content.Title?.Default),
                            Comments = commentsByQuestionId[question.Identifier]
                        })
                };

                result.Add(item);
            }

            return result;
        }

        #endregion
    }
}