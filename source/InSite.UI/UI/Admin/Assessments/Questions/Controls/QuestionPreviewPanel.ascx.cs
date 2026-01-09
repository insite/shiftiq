using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using InSite.Admin.Assessments.Questions.Utilities;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Questions.Controls
{
    public partial class QuestionPreviewPanel : BaseUserControl
    {
        private class LabelInfo
        {
            public string Indicator { get; }
            public string Title { get; }

            public LabelInfo(string indicator, string title)
            {
                Indicator = indicator;
                Title = title;
            }
        }

        private static readonly ReadOnlyDictionary<QuestionItemType, string> _controlMapping = new ReadOnlyDictionary<QuestionItemType, string>(
            new Dictionary<QuestionItemType, string>
            {
                { QuestionItemType.BooleanTable, "QuestionPreviewBooleanTable" },
                { QuestionItemType.ComposedEssay, "QuestionPreviewComposedEssay" },
                { QuestionItemType.ComposedVoice, "QuestionPreviewComposedVoice" },
                { QuestionItemType.Matching,"QuestionPreviewMatching" },
                { QuestionItemType.MultipleCorrect, "QuestionPreviewMultipleCorrect" },
                { QuestionItemType.SingleCorrect, "QuestionPreviewSingleCorrect" },
                { QuestionItemType.TrueOrFalse, "QuestionPreviewTrueOrFalse" },
                { QuestionItemType.Likert, "QuestionPreviewLikert" },
                { QuestionItemType.HotspotStandard, "QuestionPreviewHotspot" },
                { QuestionItemType.HotspotImageCaptcha, "QuestionPreviewHotspot" },
                { QuestionItemType.HotspotMultipleChoice, "QuestionPreviewHotspot" },
                { QuestionItemType.HotspotMultipleAnswer, "QuestionPreviewHotspot" },
                { QuestionItemType.HotspotCustom, "QuestionPreviewHotspot" },
                { QuestionItemType.Ordering, "QuestionPreviewOrdering" }
            }
        );

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CommonStyle.ContentKey = typeof(QuestionPreviewPanel).FullName;
        }

        public void LoadData(PreviewQuestionModel model)
        {
            QuestionTitle.InnerText = $"Question {model.Sequence}";
            QuestionText.InnerHtml = Markdown.ToHtml(model.AttemptQuestion.Text);

            LabelRepeater.DataSource = GetLabels(model.BankQuestion);
            LabelRepeater.DataBind();

            if (!_controlMapping.TryGetValue(model.AttemptQuestion.Type, out var controlName))
                throw new NotImplementedException("Unexpected question type: " + model.AttemptQuestion.Type.GetName());

            var controlPath = $"~/UI/Admin/Assessments/Questions/Controls/{controlName}.ascx";
            var control = (QuestionPreviewControl)QuestionContainer.LoadControl(controlPath);

            control.LoadData(model);
        }

        private IEnumerable<LabelInfo> GetLabels(Question q)
        {
            yield return new LabelInfo("primary", $"Bank Q{q.BankIndex + 1}");

            if (q.Classification.Code != null)
                yield return new LabelInfo("info", q.Classification.Code);

            if (q.Classification.LikeItemGroup != null)
                yield return new LabelInfo("warning", q.Classification.LikeItemGroup);

            if (q.Classification.Tag != null)
                yield return new LabelInfo("default", q.Classification.Tag);
        }
    }
}