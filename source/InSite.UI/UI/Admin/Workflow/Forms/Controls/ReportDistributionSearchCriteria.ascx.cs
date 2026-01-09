using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Surveys.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;

namespace InSite.Admin.Workflow.Forms.Controls
{
    public partial class ReportDistributionSearchCriteria : UserControl
    {
        #region Events

        public event EventHandler Searching;

        private void OnSearching() => Searching?.Invoke(this, EventArgs.Empty);

        public event EventHandler Clearing;

        private void OnClearing() => Clearing?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Classes

        [Serializable]
        private class AnswerFilterItem : QResponseAnalysisFilter.Answer
        {
            public string QuestionOutputText { get; set; }

            public string ComparisonName => ComparisonType.GetDescription();

            public string AnswerOutputText { get; set; }

            public AnswerFilterItem(Guid questionId, ComparisonType comparison, string answerText)
                : base(questionId, comparison, answerText)
            {

            }
        }

        [Serializable]
        private class AnswerFilterItemCollection : IEnumerable<AnswerFilterItem>
        {
            #region Properties

            public bool IsChanged => _isChanged;

            public int Count => _items.Count;

            #endregion

            #region Fields

            private List<AnswerFilterItem> _items;

            [NonSerialized]
            private bool _isChanged = false;

            #endregion

            #region Construction

            public AnswerFilterItemCollection()
            {
                _items = new List<AnswerFilterItem>();
            }

            public AnswerFilterItemCollection(IEnumerable<AnswerFilterItem> items)
            {
                _items = new List<AnswerFilterItem>(items);
            }

            #endregion

            #region Methods

            internal void Clear()
            {
                if (_items.Count == 0)
                    return;

                _items.Clear();
                _isChanged = true;
            }

            internal void Remove(int index)
            {
                if (_items.Count == 0 || index < 0 || index >= _items.Count)
                    return;

                _items.RemoveAt(index);
                _isChanged = true;
            }

            internal void Add(AnswerFilterItem item)
            {
                _items.Add(item);

                _isChanged = true;
            }

            internal bool Contains(Guid questionId, string answerText)
            {
                foreach (var item in _items)
                {
                    if (item.QuestionIdentifier == questionId && string.Compare(item.AnswerText, answerText) == 0)
                        return true;
                }

                return false;
            }

            #endregion

            #region IEnumerable

            public IEnumerator<AnswerFilterItem> GetEnumerator() => _items.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            #endregion
        }

        #endregion

        #region Properties

        public Guid? SurveyID
        {
            get => (Guid?)ViewState[nameof(SurveyID)];
            set
            {
                ViewState[nameof(SurveyID)] = value;

                AnswerFieldFilterButtons.Visible = value.HasValue;

                AnswerFilter.Clear();
            }
        }

        private AnswerFilterItemCollection AnswerFilter
        {
            get => (AnswerFilterItemCollection)(ViewState[nameof(AnswerFilter)]
                ?? (ViewState[nameof(AnswerFilter)] = new AnswerFilterItemCollection()));
            set => ViewState[nameof(AnswerFilter)] = value;
        }

        private bool AnswerFilterRepeaterInited
        {
            get => ViewState[nameof(AnswerFilterRepeaterInited)] != null && (bool)ViewState[nameof(AnswerFilterRepeaterInited)];
            set => ViewState[nameof(AnswerFilterRepeaterInited)] = value;
        }

        public QResponseAnalysisFilter Filter
        {
            get
            {
                var filter = new QResponseAnalysisFilter
                {
                    SurveyFormIdentifier = SurveyID,
                    ShowSelections = ShowSelections.Checked,
                    ShowNumbers = ShowNumbers.Checked,
                    ShowComments = ShowText.Checked,
                    ShowText = ShowText.Checked,
                    EnableQuestionFilter = EnableQuestionFilter.Checked,
                };

                filter.AnswerFilter.Clear();
                filter.AnswerFilter.AddRange(AnswerFilter);
                filter.AnswerFilterOperator = FilterOperator.SelectedValue;

                return filter;
            }
            set
            {
                if (value?.SurveyFormIdentifier == SurveyID)
                {
                    ShowSelections.Checked = value.ShowSelections;
                    ShowNumbers.Checked = value.ShowNumbers;
                    ShowText.Checked = value.ShowText;
                    EnableQuestionFilter.Checked = value.EnableQuestionFilter;

                    if (value.AnswerFilter != null)
                        AnswerFilter = new AnswerFilterItemCollection(value.AnswerFilter.Cast<AnswerFilterItem>());
                }
                else
                {
                    Clear();
                }
            }
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AnswerFilterRepeater.DataBinding += AnswerFilterRepeater_DataBinding;
            AnswerFilterRepeater.ItemCommand += AnswerFilterRepeater_ItemCommand;

            AnswerFilterWindowUpdatePanel.Request += AnswerFilterWindowUpdatePanel_Request;

            AnswerFilterQuestionID.AutoPostBack = true;
            AnswerFilterQuestionID.ValueChanged += AnswerFilterQuestionID_ValueChanged;

            AddAnswerConditionButton.Click += AddAnswerConditionButton_Click;
            AddConditionButton.Click += AddConditionButton_Click;
            ClearAnswersFilterButton.Click += ClearAnswersFilterButton_Click;

            SearchButton.Click += SearchButton_Click;
            ClearButton.Click += ClearButton_Click;
        }

        #endregion

        #region PreRender

        protected override void OnPreRender(EventArgs e)
        {
            AnswerFilterUpdatePanel.Style["display"] = EnableQuestionFilter.Checked ? string.Empty : "none";

            if (AnswerFilter.IsChanged)
            {
                AnswerFilterRepeater.DataBind();
            }
            else if (!AnswerFilterRepeaterInited)
            {
                AnswerFilterRepeater.DataBind();

                AnswerFilterRepeaterInited = true;
            }

            base.OnPreRender(e);
        }

        #endregion

        #region Clearing

        public void Clear()
        {
            ShowSelections.Checked = true;
            ShowNumbers.Checked = true;
            ShowText.Checked = true;
            EnableQuestionFilter.Checked = false;

            AnswerFilter.Clear();
        }

        #endregion

        #region Event handlers

        private void SearchButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            OnSearching();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            Clear();
            OnClearing();
        }

        private void AnswerFilterRepeater_DataBinding(object sender, EventArgs e)
        {
            var hasData = AnswerFilter.Count > 0;

            AnswerFilterNoItemMessage.Visible = !hasData;

            AnswerFilterRepeater.Visible = hasData;
            AnswerFilterRepeater.DataSource = AnswerFilter;
        }

        private void AnswerFilterRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
                AnswerFilter.Remove(e.Item.ItemIndex);
        }

        private void AddConditionButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var questionId = AnswerFilterQuestionID.ValueAsGuid.Value;
            var answerText = AnswerFilterAnswerText.Value;
            var comparison = AnswerFilterComparison.SelectedValue.ToEnum<ComparisonType>();

            if (!AnswerFilter.Contains(questionId, answerText))
                AnswerFilter.Add(new AnswerFilterItem(questionId, comparison, answerText)
                {
                    QuestionOutputText = AnswerFilterQuestionID.GetSelectedOption().Text,
                    AnswerOutputText = AnswerFilterAnswerText.GetSelectedOption().Text
                });
        }

        private void AddAnswerConditionButton_Click(object sender, EventArgs e)
        {
            AnswerFilterRepeater.DataBind();
        }

        private void ClearAnswersFilterButton_Click(object sender, EventArgs e)
        {
            AnswerFilter.Clear();
        }

        private void AnswerFilterQuestionID_ValueChanged(object sender, EventArgs e)
        {
            AnswerFilterQuestionSelected(AnswerFilterQuestionID.ValueAsGuid);
        }

        private void AnswerFilterWindowUpdatePanel_Request(object sender, StringValueArgs e)
        {
            if (e.Value == "open")
            {
                AnswerFilterQuestionID.ValueAsGuid = null;
                AnswerFilterQuestionID.SurveyIdentifier = SurveyID.Value;
                AnswerFilterQuestionID.RefreshData();

                AnswerFilterComparison.SelectedIndex = 0;

                AnswerFilterQuestionSelected(null);
            }
        }

        #endregion

        #region Helper methods

        private void AnswerFilterQuestionSelected(Guid? questionId)
        {
            AnswerFilterQuestionControlName.Text = null;

            AnswerFilterAnswerText.SurveyQuestionIdentifier = null;
            AnswerFilterAnswerText.RefreshData();

            if (!questionId.HasValue)
                return;

            var question = ServiceLocator.SurveySearch.GetSurveyQuestion(questionId.Value);
            if (question == null)
                return;

            var questionType = question.SurveyQuestionType.ToEnumNullable<SurveyQuestionType>();
            if (!questionType.HasValue)
                return;

            AnswerFilterQuestionControlName.Text = $"<span class='badge bg-{questionType.Value.GetContextualClass()}'>{questionType.Value.GetDescription()}</span>";

            AnswerFilterAnswerText.SurveyQuestionIdentifier = question.SurveyQuestionIdentifier;
            AnswerFilterAnswerText.RefreshData();
        }

        #endregion
    }
}