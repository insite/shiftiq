using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Surveys.Read;
using InSite.Common.Web.UI;

using Shift.Constant;
using Shift.Common.Events;

using static InSite.Application.Surveys.Read.QResponseCorrelationAnalysisFilter;

namespace InSite.Admin.Workflow.Forms.Controls
{
    public partial class ReportCorrelationSearchCriteria : BaseUserControl
    {
        #region Events

        public event EventHandler Searching;

        private void OnSearching() => Searching?.Invoke(this, EventArgs.Empty);

        public event EventHandler Clearing;

        private void OnClearing() => Clearing?.Invoke(this, EventArgs.Empty);

        public event AlertHandler Alert;

        private void OnAlert(AlertType type, string message) => Alert.Invoke(this, new AlertArgs(type, message));

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AddXButton.Click += AddXButton_Click;
            AddYButton.Click += AddYButton_Click;

            XAxisVariablesRepeater.ItemCommand += XAxisVariablesRepeater_ItemCommand;
            YAxisVariablesRepeater.ItemCommand += YAxisVariablesRepeater_ItemCommand;

            SearchButton.Click += SearchButton_Click;
            ClearButton.Click += ClearButton_Click;
        }

        private void InitForm(Guid? surveyId)
        {
            XAxisVariables.Clear();
            YAxisVariables.Clear();

            BindXVariablesRepeater();
            BindYVariablesRepeater();

            XAxisQuestionID.ValueAsGuid = null;
            XAxisQuestionID.SurveyIdentifier = surveyId;
            XAxisQuestionID.ExcludeQuestionsID = null;
            XAxisQuestionID.RefreshData();

            YAxisQuestionID.Value = null;
            YAxisQuestionID.SurveyIdentifier = surveyId;
            YAxisQuestionID.ExcludeQuestionsID = null;
            YAxisQuestionID.RefreshData();
        }

        #endregion

        #region Clearing

        public void Clear()
        {
            InitForm(SurveyID);

            ShowFrequencies.Checked = true;
            ShowRowPercentages.Checked = true;
            ShowColumnPercentages.Checked = true;
        }

        #endregion

        #region Properties

        public Guid? SurveyID
        {
            get => (Guid?)ViewState[nameof(SurveyID)];
            set => ViewState[nameof(SurveyID)] = value;
        }

        private AxisItemCollection XAxisVariables => (AxisItemCollection)(ViewState[nameof(XAxisVariables)]
            ?? (ViewState[nameof(XAxisVariables)] = new AxisItemCollection()));

        private AxisItemCollection YAxisVariables => (AxisItemCollection)(ViewState[nameof(YAxisVariables)]
            ?? (ViewState[nameof(YAxisVariables)] = new AxisItemCollection()));

        public QResponseCorrelationAnalysisFilter Filter
        {
            get
            {
                var filter = new QResponseCorrelationAnalysisFilter
                {
                    SurveyFormIdentifier = SurveyID,
                    ShowFrequencies = ShowFrequencies.Checked,
                    ShowRowPercentages = ShowRowPercentages.Checked,
                    ShowColumnPercentages = ShowColumnPercentages.Checked
                };

                filter.XAxis.CopyFrom(XAxisVariables);
                filter.YAxis.CopyFrom(YAxisVariables);

                return filter;
            }
            set
            {
                SurveyID = value.SurveyFormIdentifier;

                InitForm(SurveyID);

                XAxisQuestionID.ValueAsGuid = null;
                XAxisTitle.Text = string.Empty;

                YAxisQuestionID.ValueAsGuid = null;
                YAxisTitle.Text = string.Empty;

                XAxisVariables.CopyFrom(value.XAxis);
                YAxisVariables.CopyFrom(value.YAxis);

                ShowFrequencies.Checked = value.ShowFrequencies;
                ShowRowPercentages.Checked = value.ShowRowPercentages;
                ShowColumnPercentages.Checked = value.ShowColumnPercentages;

                BindXVariablesRepeater();
                BindYVariablesRepeater();
            }
        }

        #endregion

        #region Loading

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                InitForm(SurveyID);
        }

        #endregion

        #region Event handlers

        private void SearchButton_Click(object sender, EventArgs e)
        {
            if (XAxisVariables.Count == 0 && YAxisVariables.Count == 0)
                OnAlert(AlertType.Warning, "Add <strong>X Variable</strong> and <strong>Y Variable</strong> to the search criteria to see result.");
            else if (XAxisVariables.Count == 0)
                OnAlert(AlertType.Warning, "Add <strong>X Variable</strong> to the search criteria to see result.");
            else if (YAxisVariables.Count == 0)
                OnAlert(AlertType.Warning, "Add <strong>Y Variable</strong> to the search criteria to see result.");
            else
                OnSearching();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            Clear();
            OnClearing();
        }

        private void AddXButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            XAxisVariables.Add(XAxisQuestionID.ValueAsGuid.Value, XAxisTitle.Text);

            XAxisQuestionID.ValueAsGuid = null;
            XAxisQuestionID.ExcludeQuestionsID = XAxisVariables.Select(x => x.QuestionIdentifier).ToArray();
            XAxisQuestionID.RefreshData();

            XAxisTitle.Text = string.Empty;

            BindXVariablesRepeater();
        }

        private void AddYButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            YAxisVariables.Add(YAxisQuestionID.ValueAsGuid.Value, YAxisTitle.Text);

            YAxisQuestionID.ValueAsGuid = null;
            YAxisQuestionID.ExcludeQuestionsID = YAxisVariables.Select(x => x.QuestionIdentifier).ToArray();
            YAxisQuestionID.RefreshData();

            YAxisTitle.Text = string.Empty;

            BindYVariablesRepeater();
        }

        private void XAxisVariablesRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "remove")
            {
                var questionId = Guid.Parse((string)e.CommandArgument);

                XAxisVariables.Remove(questionId);

                XAxisQuestionID.ExcludeQuestionsID = XAxisVariables.Select(x => x.QuestionIdentifier).ToArray();
                XAxisQuestionID.RefreshData();

                BindXVariablesRepeater();
            }
        }

        private void YAxisVariablesRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "remove")
            {
                var questionId = Guid.Parse((string)e.CommandArgument);

                YAxisVariables.Remove(questionId);

                YAxisQuestionID.ExcludeQuestionsID = YAxisVariables.Select(x => x.QuestionIdentifier).ToArray();
                YAxisQuestionID.RefreshData();

                BindYVariablesRepeater();
            }
        }

        #endregion

        #region Helper methods

        private void BindXVariablesRepeater()
        {
            XAxisVariablesRepeater.DataSource = XAxisVariables;
            XAxisVariablesRepeater.DataBind();
        }

        private void BindYVariablesRepeater()
        {
            YAxisVariablesRepeater.DataSource = YAxisVariables;
            YAxisVariablesRepeater.DataBind();
        }

        #endregion
    }
}