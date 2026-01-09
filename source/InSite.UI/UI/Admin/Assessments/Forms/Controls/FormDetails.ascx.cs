using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using InSite.Application.Banks.Write;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Forms.Controls
{
    public partial class FormDetails : BaseUserControl
    {
        private class StaticQuestionOrderItem
        {
            public int Sequence { get; set; }
            public string Code { get; set; }
            public string Tag { get; set; }
            public string Text { get; set; }
        }

        #region Properties

        public IEnumerable<DetailsField> VisibleFields
        {
            get => (IEnumerable<DetailsField>)ViewState[nameof(VisibleFields)];
            set => ViewState[nameof(VisibleFields)] = value;
        }

        public IEnumerable<DetailsField> HiddenFields
        {
            get => (IEnumerable<DetailsField>)ViewState[nameof(HiddenFields)];
            set => ViewState[nameof(HiddenFields)] = value;
        }

        public List<DetailsField> HiddenFieldsInternal
        {
            get => (List<DetailsField>)(ViewState[nameof(HiddenFieldsInternal)]
                ?? (ViewState[nameof(HiddenFieldsInternal)] = new List<DetailsField>()));
            set => ViewState[nameof(HiddenFieldsInternal)] = value;
        }

        public bool AllowEdit
        {
            get => (bool)(ViewState[nameof(AllowEdit)] ?? true);
            set => ViewState[nameof(AllowEdit)] = value;
        }

        private bool IsFormPublished
        {
            get => (bool)(ViewState[nameof(IsFormPublished)] ?? true);
            set => ViewState[nameof(IsFormPublished)] = value;
        }

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FormThirdPartyAssessmentToggle.Click += FormThirdPartyAssessmentToggle_Click;
            VerifyStaticQuestionOrder.Click += VerifyStaticQuestionOrder_Click;
            FooterLiteral.ContentKey = GetType().ToString();
        }

        protected override void OnPreRender(EventArgs e)
        {
            ChangeNameLink.Visible = !IsFormPublished && AllowEdit;
            ChangeGradebookLink.Visible = !IsFormPublished && AllowEdit;
            ChangeCodeLink.Visible = !IsFormPublished && AllowEdit;
            ChangeHookLink.Visible = AllowEdit;
            DeleteFormLink.Visible = !IsFormPublished && AllowEdit;
            EditClassificationLink.Visible = !IsFormPublished && AllowEdit;
            EditProctoringLink.Visible = !IsFormPublished && AllowEdit;

            var visible = new HashSet<DetailsField>(VisibleFields ?? new DetailsField[0]);
            var hidden = new HashSet<DetailsField>(HiddenFields ?? new DetailsField[0]);

            foreach (var field in HiddenFieldsInternal)
                hidden.Add(field);

            FormSpecificationField.Visible = IsFieldVisible(DetailsField.Specification);
            FormStandardField.Visible = IsFieldVisible(DetailsField.Standard);
            FormNameField.Visible = IsFieldVisible(DetailsField.Name);
            FormAssetField.Visible = IsFieldVisible(DetailsField.Asset);
            GradebookField.Visible = IsFieldVisible(DetailsField.Gradebook);
            CodeField.Visible = IsFieldVisible(DetailsField.Code);
            SourceField.Visible = IsFieldVisible(DetailsField.Source);
            FormIdentifierField.Visible = IsFieldVisible(DetailsField.Identifier);
            PublicationColumn.Visible = IsAnyFieldVisible(DetailsField.PublicationStatus, DetailsField.Feedback, DetailsField.Rationale, DetailsField.Simulate);
            PublicationStatusField.Visible = IsFieldVisible(DetailsField.PublicationStatus);
            FirstPublishedField.Visible = IsFieldVisible(DetailsField.FirstPublished) && !string.IsNullOrEmpty(FirstPublished.Text);
            FeedbackField.Visible = IsFieldVisible(DetailsField.Feedback);
            RationaleField.Visible = IsFieldVisible(DetailsField.Rationale);
            SimulateField.Visible = IsFieldVisible(DetailsField.Simulate);
            ProctoringColumn.Visible = IsAnyFieldVisible(DetailsField.SafeExamBrowser, DetailsField.KioskMode, DetailsField.ScheduleOpenDate, DetailsField.ScheduleCloseDate, DetailsField.TimeLimit, DetailsField.AttemptLimit, DetailsField.AttemptLimitPerSession, DetailsField.TimeLimitPerSession, DetailsField.TimeLimitPerLockout);
            SafeExamBrowserField.Visible = IsFieldVisible(DetailsField.SafeExamBrowser);
            KioskModeField.Visible = IsFieldVisible(DetailsField.KioskMode);
            ScheduleOpenDateField.Visible = IsFieldVisible(DetailsField.ScheduleOpenDate);
            ScheduleCloseDateField.Visible = IsFieldVisible(DetailsField.ScheduleCloseDate);
            TimeLimitField.Visible = IsFieldVisible(DetailsField.TimeLimit);
            AttemptLimitField.Visible = IsFieldVisible(DetailsField.AttemptLimit);
            AttemptLimitPerSessionField.Visible = IsFieldVisible(DetailsField.AttemptLimitPerSession);
            TimeLimitPerSessionField.Visible = IsFieldVisible(DetailsField.TimeLimitPerSession);
            TimeLimitPerLockoutField.Visible = IsFieldVisible(DetailsField.TimeLimitPerLockout);
            ClassificationColumn.Visible = IsAnyFieldVisible(DetailsField.Instrument, DetailsField.Theme);
            InstrumentField.Visible = IsFieldVisible(DetailsField.Instrument);
            ThemeField.Visible = IsFieldVisible(DetailsField.Theme);

            base.OnPreRender(e);

            bool IsFieldVisible(DetailsField field)
            {
                return (visible.Count == 0 || visible.Contains(field))
                    && (hidden.Count == 0 || !hidden.Contains(field));
            }

            bool IsAnyFieldVisible(params DetailsField[] fields)
            {
                foreach (var f in fields)
                {
                    if (IsFieldVisible(f))
                        return true;
                }

                return false;
            }
        }

        public bool SetInputValues(Form form, ReturnUrl returnUrl)
        {
            var bank = form.Specification.Bank;

            // Identification

            var gradebook = form.Gradebook.HasValue ? ServiceLocator.RecordSearch.GetGradebook(form.Gradebook.Value) : null;

            var gradebookText = gradebook != null
                ? $"<a target='_blank' href='/ui/admin/records/gradebooks/outline?id={form.Gradebook}'>{HttpUtility.HtmlEncode(gradebook.GradebookTitle)}</a>"
                : "None";

            FormSpecification.Text = form.Specification.Name;
            FormStandard.AssetID = bank.Standard;
            Name.Text = form.Name;
            Version.Text = $"{form.Asset}.{form.AssetVersion}";
            Gradebook.Text = gradebookText;
            Code.Text = form.Code ?? "None";
            Source.Text = form.Source ?? "None";
            Origin.Text = form.Origin ?? "None";
            Hook.Text = form.Hook ?? "None";
            Identifier.Text = form.Identifier.ToString();
            IsFormPublished = form.Publication.Status == Shift.Constant.PublicationStatus.Published;

            // Classification

            Instrument.Text = form.Classification.Instrument ?? "None";
            Theme.Text = form.Classification.Theme ?? "None";
            Source.Text = form.Source ?? "None";

            // Invigilation

            SafeExamBrowserState.Text = form.Invigilation.IsSafeExamBrowserRequired ? "Required" : "Optional";
            KioskModeState.Text = form.Invigilation.IsKioskModeRequired ? "Required" : "Disabled";
            ScheduleOpenDate.Text = form.Invigilation.Opened.Format(User.TimeZone, nullValue: "N/A");
            ScheduleCloseDate.Text = form.Invigilation.Closed.Format(User.TimeZone, nullValue: "N/A");
            TimeLimit.Text = form.Invigilation.TimeLimit.ToString("n0");
            AttemptLimit.Text = form.Invigilation.AttemptLimit.ToString("n0"); ;
            AttemptLimitPerSession.Text = form.Invigilation.AttemptLimitPerSession.ToString();
            TimeLimitPerSession.Text = form.Invigilation.TimeLimitPerSession.ToString();
            TimeLimitPerLockout.Text = form.Invigilation.TimeLimitPerLockout.ToString();

            if (form.Invigilation.AttemptLimit == 0)
            {
                HiddenFieldsInternal.AddRange(new[]
                {
                    DetailsField.AttemptLimitPerSession,
                    DetailsField.TimeLimitPerSession,
                    DetailsField.TimeLimitPerLockout
                });
            }

            // Publication
            var formSummary = ServiceLocator.BankSearch.GetForm(form.Identifier);
            if (formSummary != null)
                PublicationStatus.Text = formSummary.FormPublicationStatus;

            FirstPublished.Text = formSummary.FormFirstPublished.HasValue
                ? formSummary.FormFirstPublished.Value.Format(User.TimeZone, isHtml: true)
                : string.Empty;

            AllowFeedbackFromCandidates.Text = form.Publication.AllowFeedback ? "Enabled" : "Disabled";
            ShowRationale.Text =
                (form.Publication.AllowRationaleForCorrectAnswers && form.Publication.AllowRationaleForIncorrectAnswers)
                ? "Show for Correct and Incorrect Answers"
                : (form.Publication.AllowRationaleForCorrectAnswers
                  ? "Show for Correct Answers Only"
                  : (form.Publication.AllowRationaleForIncorrectAnswers
                    ? "Show for Incorrect Answers Only"
                    : "Hide for All Answers"));

            var languages = form.Languages.EmptyIfNull().Select(x => Language.GetDisplayName(x)).OrderBy(x => x).ToArray();
            LanguageField.Visible = languages.Length > 0;
            LanguageOutput.InnerText = string.Join(", ", languages);

            BindModelToControlForThirdPartyAssessment(form.ThirdPartyAssessmentIsEnabled);

            SetupEditLinks(form, returnUrl);

            BindStaticQuestionOrder(form);

            return true;
        }

        private void BindStaticQuestionOrder(Form form)
        {
            if (form.Specification.Type != SpecificationType.Static)
                return;

            StaticQuestionOrderPanel.Visible = true;
            StaticQuestionOrder.Text = form.StaticQuestionOrderVerified.HasValue
                ? $"Verified " + form.StaticQuestionOrderVerified.Value.Format(User.TimeZone, true)
                : "Not Verified";

            var verifiedQuestions = form.StaticQuestionOrder;
            if (verifiedQuestions == null)
                return;

            StaticQuestionOrderDetail.Visible = true;
            StaticQuestionOrderZoom.Attributes["data-bs-target"] = $"#{ModalStaticQuestionOrder.ClientID}";
            StaticQuestionOrderRepeater.DataSource = GetStaticQuestionOrderItems(form.Specification.Bank, verifiedQuestions);
            StaticQuestionOrderRepeater.DataBind();

            var currentQuestions = form.GetStaticFormQuestionIdentifiersInOrder();
            var isMatch = verifiedQuestions.SequenceEqual(currentQuestions);

            StaticQuestionOrderMatchSuccess.Visible = isMatch;
            StaticQuestionOrderMatchFailure.Visible = !isMatch;
        }

        private static IEnumerable<StaticQuestionOrderItem> GetStaticQuestionOrderItems(BankState bank, Guid[] questions)
        {
            if (questions == null)
                return Enumerable.Empty<StaticQuestionOrderItem>();

            var list = new List<StaticQuestionOrderItem>();
            var sequence = 0;

            foreach (var questionId in questions)
            {
                var question = bank.FindQuestion(questionId);
                if (question == null)
                    continue;

                var text = StringHelper.StripMarkdown(question.Content.Title.Default);
                text = StringHelper.StripHtml(text);
                text = StringHelper.BreakHtml(text);
                text = text.MaxLength(95);

                list.Add(new StaticQuestionOrderItem
                {
                    Sequence = ++sequence,
                    Code = question.Classification.Code,
                    Tag = question.Classification.Tag,
                    Text = text
                });
            }

            return list;
        }

        private void SetupEditLinks(Form form, ReturnUrl returnUrl)
        {
            var queryString = $"bank={form.Specification.Bank.Identifier}&form={form.Identifier}";

            DeleteFormLink.NavigateUrl = $"/admin/assessments/forms/delete?{queryString}";
            ChangeGradebookLink.NavigateUrl = $"/ui/admin/assessments/forms/change-gradebook?{queryString}";
            ChangeCodeLink.NavigateUrl = $"/ui/admin/assessments/forms/recode?{queryString}";
            ChangeHookLink.NavigateUrl = returnUrl.GetRedirectUrl($"/ui/admin/assessments/forms/recode?{queryString}&field=hook");
            ChangeNameLink.NavigateUrl = $"/ui/admin/assessments/forms/rename?{queryString}";
            EditClassificationLink.NavigateUrl = $"/ui/admin/assessments/forms/change-classification?{queryString}";
            EditProctoringLink.NavigateUrl = $"/ui/admin/assessments/forms/modify-proctoring?{queryString}";
            BuildScantronCsv.NavigateUrl = $"/ui/admin/assessments/forms/simulate?{queryString}";
        }

        public void HideLink()
        {
            FormStandard.ShowLink = false;
        }

        private void FormThirdPartyAssessmentToggle_Click(object sender, EventArgs e)
        {
            var formId = Guid.Parse(Identifier.Text);
            var form = ServiceLocator.BankSearch.GetFormData(formId);
            var bankId = form.Specification.Bank.Identifier;

            if (form.ThirdPartyAssessmentIsEnabled)
                ServiceLocator.SendCommand(new DisableThirdPartyAssessment(bankId, formId));
            else
                ServiceLocator.SendCommand(new EnableThirdPartyAssessment(bankId, formId));

            BindModelToControlForThirdPartyAssessment(!form.ThirdPartyAssessmentIsEnabled);
        }

        private void VerifyStaticQuestionOrder_Click(object sender, EventArgs e)
        {
            var formId = Guid.Parse(Identifier.Text);
            var form = ServiceLocator.BankSearch.GetFormData(formId);
            var specification = form.Specification;

            if (specification.Type != SpecificationType.Static)
                return;

            var bankId = specification.Bank.Identifier;
            var questions = form.GetStaticFormQuestionIdentifiersInOrder();

            ServiceLocator.SendCommand(new VerifyAssessmentQuestionOrder(bankId, formId, questions));

            form = ServiceLocator.BankSearch.GetFormData(formId);

            BindStaticQuestionOrder(form);
        }

        private void BindModelToControlForThirdPartyAssessment(bool enabled)
        {
            FormThirdPartyAssessment.Text = enabled ? "Enabled" : "Disabled";
            FormThirdPartyAssessmentToggle.ToolTip = $"{(enabled ? "Disable" : "Enable")} third-party assessment";
            FormThirdPartyAssessmentToggle.Text = $"<i class=\"fas fa-toggle-{(enabled ? "on" : "off")}\"></i>";
        }
    }
}