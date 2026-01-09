using System;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Assessments.Questions.Controls;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;

using Shift.Common;
using Shift.Constant;

using DetailsTab = InSite.Admin.Assessments.Questions.Controls.QuestionSetDetails.Tab;

namespace InSite.Admin.Assessments.Outlines.Controls
{
    public partial class QuestionsSection : BaseUserControl
    {
        #region Properties

        private Guid BankID
        {
            get => (Guid)ViewState[nameof(BankID)];
            set => ViewState[nameof(BankID)] = value;
        }

        private Guid[] SetsInfo
        {
            get => (Guid[])ViewState[nameof(SetsInfo)];
            set => ViewState[nameof(SetsInfo)] = value;
        }

        private bool CanWrite
        {
            get => (bool)ViewState[nameof(CanWrite)];
            set => ViewState[nameof(CanWrite)] = value;
        }

        public Func<BankState> LoadBank { get; internal set; }

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FilterQuestionsButton.Click += FilterQuestionsButton_Click;

            AddButton.Click += AddButton_Click;
            SortButton.Click += SortButton_Click;
            PreviewSetButton.Click += PreviewSetButton_Click;

            DownloadButton.Click += DownloadButton_Click;
        }

        protected override void CreateChildControls()
        {
            if (SetsNav.ItemsCount == 0 && SetsInfo != null)
            {
                for (var i = 0; i < SetsInfo.Length; i++)
                    AddSetsNavItem(out _, out _);
            }

            base.CreateChildControls();
        }

        #endregion

        #region Event handlers

        private void FilterQuestionsButton_Click(object sender, EventArgs e)
        {
            var bank = LoadBank();

            LoadData(bank, CanWrite, out var _);
        }

        private void SortButton_Click(object sender, CommandEventArgs e)
        {
            if (SetsNav.SelectedItem == null)
                return;

            var setDetails = (QuestionSetDetails)SetsNav.SelectedItem.Controls[0];
            var tab = setDetails.GetTab();
            var tabText = tab == DetailsTab.Set ? "set" : "questions";
            string redirectUrl;

            if (e.CommandName == "SortQuestions")
            {
                redirectUrl = new ReturnUrl("bank")
                    .GetRedirectUrl($"/ui/admin/assessments/sets/reorder?bank={BankID}&set={setDetails.SetID}", $"set&tab={tabText}");
            }
            else if (e.CommandName == "SortSets")
            {
                redirectUrl = new ReturnUrl("bank")
                    .GetRedirectUrl($"/ui/admin/assessments/sets/reorder?bank={BankID}", $"set={setDetails.SetID}&tab={tabText}");
            }
            else
            {
                throw new ApplicationError("Unexpected command name: " + e.CommandName);
            }

            HttpResponseHelper.Redirect(redirectUrl);
        }

        private void PreviewSetButton_Click(object sender, EventArgs e)
        {
            if (SetsNav.SelectedItem == null)
                return;

            var setDetails = (QuestionSetDetails)SetsNav.SelectedItem.Controls[0];

            HttpResponseHelper.Redirect($"/ui/admin/assessments/sets/preview?bank={BankID}&set={setDetails.SetID}");
        }

        private void AddButton_Click(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "AddQuestion")
            {
                if (SetsNav.SelectedItem == null)
                    return;

                var setDetails = (QuestionSetDetails)SetsNav.SelectedItem.Controls[0];
                var redirectUrl = new ReturnUrl("bank")
                    .GetRedirectUrl($"/ui/admin/assessments/questions/add?bank={BankID}&set={setDetails.SetID}", "set&question");

                HttpResponseHelper.Redirect(redirectUrl);
            }
            else if (e.CommandName == "AddSet")
            {
                var redirectUrl = new ReturnUrl("bank")
                    .GetRedirectUrl($"/ui/admin/assessments/sets/add?bank={BankID}", "set&panel=questions");

                HttpResponseHelper.Redirect(redirectUrl);
            }
            else
            {
                throw new ApplicationError("Unexpected command name: " + e.CommandName);
            }
        }

        private void DownloadButton_Click(object sender, CommandEventArgs e)
        {
            if (e.CommandName != "SetMarkdown")
                return;

            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
                return;

            var questions = bank.Sets[SetsNav.SelectedIndex].Questions;
            if (questions.Count == 0)
            {
                ControlStatus.AddMessage(AlertType.Error, "There are no questions in this set.");
                return;
            }

            var markdown = new StringBuilder();
            var i = 0;

            foreach (var question in questions)
            {
                ++i;

                markdown.AppendLine($"# QUESTION {question.BankIndex + 1}");
                markdown.AppendLine();
                markdown.AppendLine($"{question.Content.Title.Default}");
                markdown.AppendLine();

                if (question.Options.Count > 0)
                {
                    markdown.AppendLine("## Options");
                    markdown.AppendLine();

                    foreach (var option in question.Options)
                    {
                        var optionText = option.Content.Title.Default;

                        markdown.AppendLine(string.Format("- {0}", option.HasPoints ? "*" + optionText + "*" : optionText));
                    }

                    markdown.AppendLine();
                }

                markdown.AppendLine();
            }

            var data = markdown.ToString();

            Page.Response.SendFile($"questions-{DateTime.UtcNow:yyyyMMdd}-{DateTime.UtcNow:HHmmss}", "md", Encoding.UTF8.GetBytes(data));
        }

        #endregion

        #region Data binding

        public void LoadData(BankState bank, bool canWrite, out bool isSelected)
        {
            BankID = bank.Identifier;
            CanWrite = canWrite;

            var panel = Request.QueryString["panel"];

            isSelected = false;
            if (!IsPostBack && panel == "questions")
                isSelected = true;

            var hasData = bank.Sets.IsNotEmpty();

            // Daniel - Jan 15, 2020: The Questions panel must be visible always. A bank without questions is not usable.
            // It makes sense to hide|show the filter function based on the existence of data here.

            FilterQuestionsTextBox.Visible = hasData;
            FilterQuestionsButton.Visible = hasData;
            PreviewSetButton.Visible = hasData;
            SortButton.Visible = hasData;
            AddButton.Items["AddQuestion"].Visible = hasData;
            DownloadButton.Visible = hasData;
            NoQuestionsAlert.Visible = !hasData;

            AddButton.Visible = canWrite;
            SortButton.Visible = canWrite;

            if (!hasData)
                return;

            Guid? setId = null, questionId = null;
            var isQuestionFound = false;

            if (!IsPostBack)
            {
                if (!Guid.TryParse(Request.QueryString["form"], out var formValue) || bank.FindForm(formValue) == null)
                {
                    if (Guid.TryParse(Request.QueryString["question"], out var questionValue))
                        questionId = bank.FindQuestion(questionValue)?.GetLastVersion().Identifier;

                    if (Guid.TryParse(Request.QueryString["set"], out var setValue))
                        setId = setValue;
                }

                if (setId.HasValue || questionId.HasValue)
                {
                    var filterData = Request.QueryString["filter"];
                    if (filterData.IsNotEmpty())
                    {
                        var filter = QuestionSetDetails.Filter.Deserialize(filterData);
                        if (filter != null)
                            FilterQuestionsTextBox.Text = filter.Keyword;
                    }
                }
            }

            if (!isSelected && string.IsNullOrEmpty(panel) && (setId.HasValue || questionId.HasValue))
                isSelected = true;

            DetailsTab? forcedTab = null;
            if (isSelected)
            {
                var tabValue = Request.QueryString["tab"];
                if (tabValue == "set")
                    forcedTab = DetailsTab.Set;
                else if (tabValue == "questions")
                    forcedTab = DetailsTab.Questions;
            }

            SetsInfo = new Guid[bank.Sets.Count];
            SetsNav.ClearItems();

            for (var i = 0; i < SetsInfo.Length; i++)
            {
                var set = bank.Sets[i];
                var loadAllQuestions = setId.HasValue && setId.Value == set.Identifier || questionId.HasValue && set.Questions.Any(x => x.Identifier == questionId.Value);

                AddSetsNavItem(out var navItem, out var details);

                details.DataFilter.Keyword = FilterQuestionsTextBox.Text;
                details.SetInputValues(set, loadAllQuestions, CanWrite);

                SetsInfo[i] = set.Identifier;

                var isTabVisible = FilterQuestionsTextBox.Text == string.Empty || details.QuestionsCount > 0;

                navItem.Title = set.Name;
                navItem.Visible = isTabVisible;

                if (!isTabVisible)
                    continue;

                DetailsTab? selectedTab = null;

                if (!isQuestionFound)
                {
                    if (questionId.HasValue && details.ContainsQuestion(questionId.Value))
                    {
                        SetsNav.SelectedIndex = i;
                        selectedTab = DetailsTab.Questions;
                        isQuestionFound = true;
                    }
                    else if (setId.HasValue && setId.Value == set.Identifier)
                    {
                        SetsNav.SelectedIndex = i;
                        selectedTab = DetailsTab.Set;
                    }
                }

                if (forcedTab.HasValue)
                    selectedTab = forcedTab;

                if (selectedTab.HasValue)
                    details.SetTab(selectedTab.Value);
            }

            if (isQuestionFound)
                ScrollToQuestion(questionId.Value);
        }

        public bool OpenQuestion(Question question)
        {
            var isFound = false;

            for (var i = 0; i < SetsInfo.Length; i++)
            {
                var setId = SetsInfo[i];
                if (setId == question.Set.Identifier)
                {
                    var navItem = SetsNav.GetItems()[i];
                    var details = (QuestionSetDetails)navItem.Controls[0];

                    details.DataFilter.Keyword = FilterQuestionsTextBox.Text;
                    details.SetInputValues(question.Set, true, CanWrite);

                    if (details.ContainsQuestion(question.Identifier))
                    {
                        SetsNav.SelectedIndex = i;
                        details.SetTab(DetailsTab.Questions);
                        ScrollToQuestion(question.Identifier);
                        isFound = true;
                    }

                    break;
                }
            }

            return isFound;
        }

        #endregion

        #region Helper methods

        private void AddSetsNavItem(out NavItem navItem, out QuestionSetDetails details)
        {
            SetsNav.AddItem(navItem = new NavItem());
            navItem.Controls.Add(details = (QuestionSetDetails)LoadControl("~/UI/Admin/Assessments/Questions/Controls/QuestionSetDetails.ascx"));
        }

        private void ScrollToQuestion(Guid id)
        {
            ScriptManager.RegisterStartupScript(
                Page,
                GetType(),
                "scrollto_question",
                $"$(document).ready(function() {{ bankRead.scrollToQuestion('{id}'); }});",
                true);
        }

        #endregion
    }
}