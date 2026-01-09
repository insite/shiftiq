using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.Surveys.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Workflow.Forms
{
    public partial class ChangeBranch : AdminBasePage, IHasParentLinkParameters
    {

        [Serializable]
        protected class SkipPatternItem
        {

            public Guid OptionID { get; private set; }
            public string OptionSequence { get; private set; }
            public string OptionText { get; private set; }

            public Guid? QuestionID { get; private set; }

            public SkipPatternItem(string language, QSurveyOptionItem option)
            {
                OptionID = option.SurveyOptionItemIdentifier;
                OptionSequence = Calculator.ToBase26(option.SurveyOptionItemSequence);
                OptionText = GetTitle(option.SurveyOptionItemIdentifier);

                QuestionID = option.BranchToQuestionIdentifier;
            }
        }

        [Serializable]
        protected class SkipPatternItemCollection : IEnumerable<SkipPatternItem>
        {

            public int Count => _list.Count;

            public SkipPatternItem this[int index] => _list[index];

            private readonly List<SkipPatternItem> _list;

            public SkipPatternItemCollection()
            {
                _list = new List<SkipPatternItem>();
            }

            public void Load(string language, QSurveyOptionList optionList)
            {
                _list.Clear();

                var options = ServiceLocator.SurveySearch.GetSurveyOptionItems(new QSurveyOptionItemFilter { SurveyOptionListIdentifier = optionList.SurveyOptionListIdentifier })
                    .OrderBy(x => x.SurveyOptionItemSequence);

                foreach (var option in options)
                {
                    var item = new SkipPatternItem(language, option);

                    _list.Add(item);
                }
            }

            public IEnumerator<SkipPatternItem> GetEnumerator()
            {
                return _list.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private Guid? OptionListIdentifier =>
            Guid.TryParse(Request["list"], out var value) ? value : (Guid?)null;

        private string ReturnPanel => Request["returnpanel"] as string;

        private string ReturnTab => Request["returntab"] as string;

        private Guid SurveyIdentifier
        {
            get => (Guid)ViewState[nameof(SurveyIdentifier)];
            set => ViewState[nameof(SurveyIdentifier)] = value;
        }

        private Guid QuestionIdentifier
        {
            get => (Guid)ViewState[nameof(QuestionIdentifier)];
            set => ViewState[nameof(QuestionIdentifier)] = value;
        }

        private SkipPatternItemCollection SkipPatternData
        {
            get
            {
                if (ViewState[nameof(SkipPatternData)] == null)
                    ViewState[nameof(SkipPatternData)] = new SkipPatternItemCollection();

                return (SkipPatternItemCollection)ViewState[nameof(SkipPatternData)];
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SkipPatternsRepeater.DataBinding += SkipPatternsRepeater_DataBinding;
            SkipPatternsRepeater.ItemDataBound += SkipPatternsRepeater_ItemDataBound;

            SaveButton.Click += SaveButton_Click;
        }

        public override void ApplyAccessControl()
        {
            SaveButton.Visible = CanEdit;
            base.ApplyAccessControl();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        private void Open()
        {
            var optionList = OptionListIdentifier.HasValue ? ServiceLocator.SurveySearch.GetSurveyOptionList(OptionListIdentifier.Value, x => x.SurveyQuestion.SurveyForm) : null;
            if (optionList == null
                || optionList.SurveyQuestion == null
                || optionList.SurveyQuestion.SurveyForm == null
                || optionList.SurveyQuestion.SurveyForm.OrganizationIdentifier != Organization.Identifier
                || optionList.SurveyQuestion.SurveyForm.SurveyFormLocked.HasValue
                )
            {
                RedirectToSearch();
            }

            SurveyIdentifier = optionList.SurveyQuestion.SurveyFormIdentifier;
            QuestionIdentifier = optionList.SurveyQuestionIdentifier;

            var surveyQuestionType = optionList.SurveyQuestion.SurveyQuestionType.ToEnum<SurveyQuestionType>();
            if (surveyQuestionType != SurveyQuestionType.RadioList && surveyQuestionType != SurveyQuestionType.Selection && surveyQuestionType != SurveyQuestionType.Likert)
                RedirectToParent();

            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{optionList.SurveyQuestion.SurveyForm.SurveyFormName} <span class='form-text'>Form #{optionList.SurveyQuestion.SurveyForm.AssetNumber}</span>");

            SetInputValues(optionList);
        }

        private void SetInputValues(QSurveyOptionList optionList)
        {
            CommonTitle.InnerText = $"Question {optionList.SurveyQuestion.SurveyQuestionSequence}, Option List {optionList.SurveyOptionListSequence}";
            QuestionTitle.InnerText = GetTitle(optionList.SurveyQuestion.SurveyQuestionIdentifier);
            OptionListTitle.InnerText = GetTitle(optionList.SurveyOptionListIdentifier);

            SkipPatternData.Load(optionList.SurveyQuestion.SurveyForm.SurveyFormLanguage, optionList);

            SkipPatternsRepeater.DataBind();

            CancelButton.NavigateUrl = $"/ui/admin/workflow/forms/outline?form={SurveyIdentifier}&panel={ReturnPanel}&tab={ReturnTab}";
        }

        private void SkipPatternsRepeater_DataBinding(Object sender, EventArgs e)
        {
            SkipPatternsRepeater.DataSource = SkipPatternData;
        }

        private void SkipPatternsRepeater_ItemDataBound(Object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var skipPattern = (SkipPatternItem)e.Item.DataItem;

            var questionID = (FormQuestionComboBox)e.Item.FindControl("QuestionID");
            questionID.SurveyIdentifier = SurveyIdentifier;
            questionID.ExcludeSpecialQuestions = true;
            questionID.ExcludeQuestionsID = new[] { QuestionIdentifier };
            questionID.RefreshData();
            questionID.ValueAsGuid = skipPattern.QuestionID;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var options = ServiceLocator.SurveySearch.GetSurveyOptionItems(new QSurveyOptionItemFilter { SurveyOptionListIdentifier = OptionListIdentifier });
            var dataIndex = 0;

            for (var i = 0; i < SkipPatternsRepeater.Items.Count; i++)
            {
                RepeaterItem item = SkipPatternsRepeater.Items[i];

                if (!IsContentItem(item))
                    continue;

                SkipPatternItem dataItem = SkipPatternData[dataIndex];
                var questionID = (FormQuestionComboBox)item.FindControl("QuestionID");

                var option = options.FirstOrDefault(x => x.SurveyOptionItemIdentifier == dataItem.OptionID);
                var skipToQuestionID = questionID.ValueAsGuid;

                if (option.BranchToQuestionIdentifier != skipToQuestionID)
                {
                    Command command = null;

                    if (skipToQuestionID.HasValue)
                    {
                        command = new Application.Surveys.Write.AddSurveyBranch(SurveyIdentifier, option.SurveyOptionItemIdentifier, skipToQuestionID.Value);
                    }
                    else
                    {
                        if (option.BranchToQuestionIdentifier.HasValue)
                            command = new Application.Surveys.Write.DeleteSurveyBranch(SurveyIdentifier, option.SurveyOptionItemIdentifier, option.BranchToQuestionIdentifier.Value);
                    }

                    if (command != null) ServiceLocator.SendCommand(command);
                }

                dataIndex++;
            }

            HttpResponseHelper.Redirect($"/ui/admin/workflow/forms/outline?form={SurveyIdentifier}&panel={ReturnPanel}&tab={ReturnTab}", true);
        }

        private void RedirectToSearch() =>
            HttpResponseHelper.Redirect($"/ui/admin/workflow/forms/search", true);

        private void RedirectToParent()
        {
            HttpResponseHelper.Redirect($"/ui/admin/workflow/forms/outline?form={SurveyIdentifier}", true);
        }

        private static string GetTitle(Guid id) =>
            ServiceLocator.ContentSearch.GetSnip(id, ContentLabel.Title);

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"form={SurveyIdentifier}"
                : null;
        }

    }
}
