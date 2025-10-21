using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

using InSite.Admin.Assessments.Questions.Controls;
using InSite.Application.Banks.Write;
using InSite.Application.Standards.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Domain.Standards;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;
using Shift.Common.Events;

namespace InSite.UI.Admin.Standards.Standards.Controls
{
    public partial class ScenarioQuestions : BaseUserControl
    {
        #region Properties

        protected Guid? StandardIdentifier
        {
            get => (Guid?)ViewState[nameof(StandardIdentifier)];
            set => ViewState[nameof(StandardIdentifier)] = value;
        }

        protected Guid? BankIdentifier
        {
            get => (Guid?)ViewState[nameof(BankIdentifier)];
            set => ViewState[nameof(BankIdentifier)] = value;
        }

        protected Guid? BankSetIdentifier
        {
            get => (Guid?)ViewState[nameof(BankSetIdentifier)];
            set => ViewState[nameof(BankSetIdentifier)] = value;
        }

        private bool CanEdit
        {
            get => (bool)(ViewState[nameof(CanEdit)] ?? false);
            set => ViewState[nameof(CanEdit)] = value;
        }

        public int QuestionsCount
        {
            get => (int)(ViewState[nameof(QuestionsCount)] ?? 0);
            set => ViewState[nameof(QuestionsCount)] = value;
        }

        private string ReturnQuery
        {
            get => (string)ViewState[nameof(ReturnQuery)];
            set => ViewState[nameof(ReturnQuery)] = value;
        }

        private bool IsLoadAllQuestions
        {
            get => (bool)(ViewState[nameof(IsLoadAllQuestions)] ?? false);
            set => ViewState[nameof(IsLoadAllQuestions)] = value;
        }

        #endregion

        #region Events

        public event AlertHandler Alert;

        private void OnAlert(AlertType type, string message) =>
            Alert?.Invoke(this, new AlertArgs(type, message));

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FilterQuestionsButton.Click += FilterQuestionsButton_Click;
            AddQuestionButton.Click += AddQuestionButton_Click;
            LoadQuestionsButton.Click += LoadQuestionsButton_Click;

            CommonScript.ContentKey = typeof(ScenarioQuestions).FullName;
        }

        private void FilterQuestionsButton_Click(object sender, EventArgs e)
        {
            ReloadQuestions();
        }

        private void AddQuestionButton_Click(object sender, EventArgs e)
        {
            var scenario = StandardSearch.SelectFirst(x => x.StandardIdentifier == StandardIdentifier.Value, x => x.Parent);
            if (!CanHaveBankSet(scenario))
                return;

            var bank = EnsureBankAssigned(scenario.Parent);
            var set = EnsureSetAssigned(scenario, bank);

            BankIdentifier = bank.Identifier;
            BankSetIdentifier = set.Identifier;

            var url = GetAddQuestionUrl();

            HttpResponseHelper.Redirect(url);
        }

        private void LoadQuestionsButton_Click(object sender, EventArgs e)
        {
            IsLoadAllQuestions = true;

            ReloadQuestions();
        }

        private string GetAddQuestionUrl()
        {
            return BankIdentifier.HasValue && BankSetIdentifier.HasValue
                ? new ReturnUrl(ReturnQuery)
                    .GetRedirectUrl($"/ui/admin/assessments/questions/add?bank={BankIdentifier.Value}&set={BankSetIdentifier.Value}")
                : null;
        }

        private void ScrollToQuestion(Guid id)
        {
            ScriptManager.RegisterStartupScript(
                Page,
                GetType(),
                "scrollto_question",
                $"$(document).ready(function () {{ scenarioQuestions.scrollToQuestion('{id}'); }});",
                true);
        }

        #region Methods (question list)

        public void SetInputValues(Standard standard, bool canEdit, string returnQuery, Guid? selectedQuestionId)
        {
            StandardIdentifier = standard.StandardIdentifier;
            CanEdit = canEdit;
            ReturnQuery = returnQuery;

            AddQuestionButton.Visible = canEdit;
            AddQuestionButton.PostBackEnabled = true;

            var hasQuestions = false;

            var set = LoadBankSet(standard.Parent.BankIdentifier, standard.BankSetIdentifier);
            if (set != null)
            {
                BankIdentifier = set.Bank.Identifier;
                BankSetIdentifier = set.Identifier;

                AddQuestionButton.PostBackEnabled = false;
                AddQuestionButton.NavigateUrl = GetAddQuestionUrl();

                if (hasQuestions = set.Questions.Count > 0)
                    LoadQuestions(set, selectedQuestionId);
            }

            FilterQuestionsField.Visible = hasQuestions;
        }

        private Set LoadBankSet(Guid? bankId, Guid? setId)
        {
            var bank = bankId.HasValue ? ServiceLocator.BankSearch.GetBankState(bankId.Value) : null;
            if (bank == null)
                return null;

            return setId.HasValue ? bank.FindSet(setId.Value) : null;
        }

        private void LoadQuestions(Set set, Guid? selectedQuestionId)
        {
            QuestionsCount = set.Questions.Count;

            var filterKeyword = FilterQuestionsTextBox.Text;
            var hasSelectedQuestion = selectedQuestionId.HasValue;
            var questions = filterKeyword.IsNotEmpty()
                ? QuestionRepeater.ApplyFilter(set.Questions.AsQueryable(), filterKeyword).ToArray()
                : (IReadOnlyList<Question>)set.Questions;

            var isTruncate = !IsLoadAllQuestions && questions.Count > 6;
            if (isTruncate)
            {
                IReadOnlyList<Question> truncatedQuestions = questions.Take(4).ToArray();

                var isNeedAllQuestions = hasSelectedQuestion
                    && (hasSelectedQuestion = questions.Any(q => q.Identifier == selectedQuestionId.Value))
                    && !truncatedQuestions.Any(q => q.Identifier == selectedQuestionId.Value);

                if (isNeedAllQuestions)
                {
                    IsLoadAllQuestions = true;
                    isTruncate = false;
                    truncatedQuestions = questions;
                }

                questions = truncatedQuestions;
            }

            if (hasSelectedQuestion)
                ScrollToQuestion(selectedQuestionId.Value);

            LoadQuestionsButton.Visible = isTruncate;

            QuestionRepeater.LoadData(questions, new QuestionRepeater.BindSettings
            {
                AllowEdit = CanEdit,
                AllowAnalyse = false,
                ShowProperties = set.Bank.IsAdvanced
                        ? PropertiesVisibility.Advanced
                        : PropertiesVisibility.Basic,
                ReturnUrl = new ReturnUrl(ReturnQuery)
            });
        }

        private void ReloadQuestions()
        {
            var set = LoadBankSet(BankIdentifier, BankSetIdentifier);
            if (set == null)
            {
                OnAlert(AlertType.Error, "The set of questions related to the scenario was not found.");
                return;
            }

            LoadQuestions(set, null);
        }

        #endregion

        #region Methods (create set)

        private bool CanHaveBankSet(Standard standard)
        {
            if (standard == null)
            {
                OnAlert(AlertType.Error, "Standard was not found.");
                return false;
            }

            if (standard.Parent == null)
            {
                OnAlert(AlertType.Error, "Standard parent was not found.");
                return false;
            }

            if (standard.StandardType != StandardType.Scenario)
            {
                OnAlert(AlertType.Error, "Invalid standard type: " + standard.StandardType);
                return false;
            }

            if (standard.Parent.StandardType != StandardType.Blueprint)
            {
                OnAlert(AlertType.Error, "Invalid parent standard type: " + standard.StandardType);
                return false;
            }

            return true;
        }

        private BankState EnsureBankAssigned(Standard blueprint)
        {
            var bankId = blueprint.BankIdentifier;
            var bank = bankId.HasValue ? ServiceLocator.BankSearch.GetBankState(bankId.Value) : null;
            if (bank != null)
                return bank;

            var today = DateTime.Today;

            bank = new BankState();
            {
                bank.Identifier = UniqueIdentifier.Create();
                bank.Name = blueprint.ContentTitle.IfNullOrEmpty($"Blueprint: {blueprint.StandardIdentifier}");
                bank.Content.Title.Default = bank.Name;
                bank.Tenant = Organization.OrganizationIdentifier;
                bank.Edition = new Edition(today.Year, today.Month);
            }

            BankHelper.AssignAssetNumbers(bank, count => Sequence.IncrementMany(bank.Tenant, SequenceType.Asset, count));

            ServiceLocator.SendCommand(new OpenBank(bank));
            ServiceLocator.SendCommand(new ModifyStandardFieldGuid(blueprint.StandardIdentifier, StandardField.BankIdentifier, bank.Identifier));

            return bank;
        }

        private Set EnsureSetAssigned(Standard scenario, BankState bank)
        {
            var set = scenario.BankSetIdentifier.HasValue ? bank.FindSet(scenario.BankSetIdentifier.Value) : null;
            if (set != null)
                return set;

            set = new Set
            {
                Identifier = UniqueIdentifier.Create(),
                Name = scenario.ContentTitle.IfNullOrEmpty($"Scenario: {scenario.StandardIdentifier}")
            };

            ServiceLocator.SendCommand(new AddSet(bank.Identifier, set.Identifier, set.Name, set.Standard));
            ServiceLocator.SendCommand(new ModifyStandardFieldGuid(scenario.StandardIdentifier, StandardField.BankSetIdentifier, set.Identifier));

            return set;
        }

        #endregion
    }
}