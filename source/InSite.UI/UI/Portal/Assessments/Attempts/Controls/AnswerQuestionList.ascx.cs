using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Attempts.Read;
using InSite.Common.Web.UI;
using InSite.UI.Portal.Assessments.Attempts.Utilities;

namespace InSite.UI.Portal.Assessments.Attempts.Controls
{
    public partial class AnswerQuestionList : BaseUserControl
    {
        private AttemptInfo _attempt;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SectionRepeater.DataBinding += SectionRepeater_DataBinding;
        }

        public void LoadData(AttemptInfo attempt)
        {
            _attempt = attempt;

            SectionRepeater.DataBind();
        }

        private void SectionRepeater_DataBinding(object sender, EventArgs e)
        {
            var sections = _attempt.GetSections() ?? new List<AttemptSectionInfo>
            {
                new AttemptSectionInfo
                {
                    Questions = _attempt.GetQuestions().ToList()
                }
            };

            SectionRepeater.ItemDataBound += SectionRepeater_ItemDataBound;
            SectionRepeater.DataSource = sections;
        }

        private void SectionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var section = (AttemptSectionInfo)e.Item.DataItem;

            var questionRepeater = (Repeater)e.Item.FindControl("QuestionRepeater");
            questionRepeater.ItemDataBound += QuestionRepeater_ItemDataBound;
            questionRepeater.DataSource = section.Questions;
            questionRepeater.DataBind();
        }

        private void QuestionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var question = (QAttemptQuestion)e.Item.DataItem;

            var output = (AnswerQuestionOutput)e.Item.FindControl("Output");
            output.LoadData(_attempt, question, false);
        }

        protected bool HasContent(string name)
        {
            var item = (AttemptSectionInfo)Page.GetDataItem();
            return item.BankSection?.Content[name]?.IsEmpty == false;
        }

        protected string GetContentText(string name)
        {
            return GetContentText((AttemptSectionInfo)Page.GetDataItem(), name);
        }

        private string GetContentText(AttemptSectionInfo item, string name)
        {
            return item.BankSection?.Content[name]?.Get(_attempt.Attempt.AttemptLanguage);
        }

        protected string GetContentHtml(string name)
        {
            var item = (AttemptSectionInfo)Page.GetDataItem();
            var text = GetContentText(item, name);
            return _attempt.GetHtml(item.Questions.FirstOrDefault(), text);
        }
    }
}