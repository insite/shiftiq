using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

using InSite.Admin.Assessments.Banks;
using InSite.Admin.Assessments.Forms.Controls;
using InSite.Admin.Assessments.Questions.Controls;
using InSite.Admin.Assessments.Web.UI;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Outlines.Controls
{
    public partial class FormSectionDetails : BaseUserControl
    {
        #region Enums

        public enum Tab
        {
            Fields,
            Section,
            Form,
            Addendum,
            Content
        }

        #endregion

        #region Classes

        private class QuestionStatisticInfo
        {
            public Form Form { get; set; }
            public bool HasData { get; set; }
        }

        #endregion

        #region Properties

        public Guid BankID
        {
            get => (Guid)(ViewState[nameof(BankID)] ?? Guid.Empty);
            set => ViewState[nameof(BankID)] = value;
        }

        public Guid FormID
        {
            get => (Guid)(ViewState[nameof(FormID)] ?? Guid.Empty);
            set => ViewState[nameof(FormID)] = value;
        }

        public Guid SectionID
        {
            get => (Guid)(ViewState[nameof(SectionID)] ?? Guid.Empty);
            set => ViewState[nameof(SectionID)] = value;
        }

        public bool AllowEdit
        {
            get => (bool?)ViewState[nameof(AllowEdit)] ?? false;
            set => ViewState[nameof(AllowEdit)] = value;
        }

        public bool ProcessDownload
        {
            get => (bool?)ViewState[nameof(ProcessDownload)] ?? false;
            set => ViewState[nameof(ProcessDownload)] = value;
        }
        #endregion

        #region Loading

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Page.IsPostBack && ProcessDownload)
            {
                var bank = ServiceLocator.BankSearch.GetBankState(BankID);
                var form = bank.FindForm(FormID);

                ProcessDownload = false;
                var data = QuestionStatisticsPanel.GetXlsx(form);

                Response.SendFile("form-summaries", "xlsx", data);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            LoadQuestionsButton.Click += LoadQuestionsButton_Click;

            FormDetails.ContentInitialization += FormDetails_ContentInitialization;
            FormContent.ContentInitialization += FormContent_ContentInitialization;
            AddendumDetails.ContentInitialization += AddendumDetails_ContentInitialization;
            StatisticsPanel.ContentInitialization += StatisticsPanel_ContentInitialization;
            StatisticsPanel.DuplicateInitialization += StatisticsPanel_DuplicateInitialization;
            DownloadSummariesButton.Click += DownloadSummariesButton_Click;
        }

        private void DownloadSummariesButton_Click(object sender, EventArgs e)
        {
            ProcessDownload = true;
            ScriptManager.RegisterStartupScript(this, GetType(),
                  "postback", "javascript:__doPostBack('', '');", true);
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (SectionID != Guid.Empty)
                Section.Attributes["data-section"] = SectionID.ToString();

            base.OnPreRender(e);
        }

        #endregion

        #region Event handlers

        private void LoadQuestionsButton_Click(object sender, EventArgs e)
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);

            if (SectionID != Guid.Empty)
            {
                var section = bank.FindSection(SectionID);

                ReloadQuestions(section, true);

            }
            else
            {
                var form = bank.FindForm(FormID);

                ReloadQuestions(form, true);
            }
        }

        private void FormDetails_ContentInitialization(object sender, EventArgs e)
        {
            var repeater = (ContentRepeater)sender;
            var control = (FormDetails)repeater.Control;
            var form = (Form)repeater.InitializationData;

            if (form == null)
                return;

            control.SetInputValues(form, new ReturnUrl($"bank&form={form.Identifier}"));
            control.AllowEdit = AllowEdit;

            var hiddenFields = new List<DetailsField>();

            if (!form.Specification.Bank.IsAdvanced)
            {
                hiddenFields.Add(DetailsField.Specification);
                hiddenFields.Add(DetailsField.Standard);
                hiddenFields.Add(DetailsField.Code);
                hiddenFields.Add(DetailsField.Source);
            }

            var specification = form.Specification;
            var limitAllTabs = specification.IsTabTimeLimitAllowed
                && specification.TabTimeLimit == SpecificationTabTimeLimit.AllTabs;

            if (limitAllTabs)
                hiddenFields.Add(DetailsField.TimeLimit);

            control.HiddenFields = hiddenFields.ToArray();
        }

        private void FormContent_ContentInitialization(object sender, EventArgs e)
        {
            var repeater = (ContentRepeater)sender;
            var control = (FormContent)repeater.Control;
            var form = (Form)repeater.InitializationData;

            if (form == null)
                return;

            control.SetInputValues(form, BankID, AllowEdit);
        }

        private void AddendumDetails_ContentInitialization(object sender, EventArgs e)
        {
            var repeater = (ContentRepeater)sender;
            var control = (FormAddendumDetails)repeater.Control;
            var form = (Form)repeater.InitializationData;

            if (form == null)
                return;

            control.SetInputValues(form, AllowEdit);
        }

        private void StatisticsPanel_ContentInitialization(object sender, EventArgs e)
        {
            var repeater = (ContentRepeater)sender;
            var control = (QuestionStatisticsPanel)repeater.Control;
            var data = (QuestionStatisticInfo)repeater.InitializationData;

            if (data == null)
                return;

            StatisticsTab.Visible = control.LoadData(data.Form);
        }

        private void StatisticsPanel_DuplicateInitialization(object sender, ContentRepeater.DuplicateEventArgs e)
        {
            var data = (QuestionStatisticInfo)e.ContentRepeater.InitializationData;

            if (data == null)
                return;

            StatisticsTab.Visible = data.HasData;
        }

        #endregion

        #region Setting/getting input values

        public void SetInputValues(Form form)
        {
            var specification = form.Specification;
            var bank = specification.Bank;
            var questions = specification.Type != SpecificationType.Static
                ? form.GetQuestions()
                : null;
            var hasQuestions = questions.IsNotEmpty();

            BankID = bank.Identifier;
            FormID = form.Identifier;
            QuestionsTab.Visible = hasQuestions;

            if (hasQuestions)
            {
                QuestionsTab.SetTitle("Questions", questions.Count);
                ReloadQuestions(form, null, questions, false);
            }

            SectionTab.Visible = false;
            AddendumTab.Visible = bank.IsAdvanced;
            StatisticsTab.Visible = bank.IsAdvanced;

            FormDetails.Key = $"form_details.{form.Identifier}";
            FormDetails.InitializationData = form;

            FormContent.Key = $"form_content.{form.Identifier}";
            FormContent.InitializationData = form;

            if (bank.IsAdvanced)
            {
                AddendumDetails.Key = $"addendum_details.{form.Identifier}";
                AddendumDetails.InitializationData = form;

                StatisticsPanel.Key = $"form_statistics.{form.Identifier}";
                StatisticsPanel.InitializationData = new QuestionStatisticInfo { Form = form };
            }

            OpenTab(Tab.Form);

            SetEditLinkVisibility(AllowEdit && form.Publication.Status != PublicationStatus.Published);

            DownloadSummariesButton.Visible = FormID != null;
        }

        public void SetInputValues(Section section)
        {
            var form = section.Form;
            var specification = form.Specification;
            var bank = specification.Bank;

            BankID = bank.Identifier;
            SectionID = section.Identifier;
            if (section.Form != null)
                FormID = section.Form.Identifier;
            QuestionsTab.Visible = true;
            SectionTab.Visible = true;
            AddendumTab.Visible = bank.IsAdvanced;
            StatisticsTab.Visible = bank.IsAdvanced;

            QuestionsTab.SetTitle("Questions", section.Fields.Count);

            FormDetails.Key = $"form_details.{form.Identifier}";
            FormDetails.InitializationData = form;

            ReloadQuestions(section, false);

            SectionNumber.Text = $"{section.Sequence} of {form.Sections.Count}";
            SectionContentTitle.InnerText = (section.Content.Title?.Default).IfNullOrEmpty("None");
            SectionContentSummary.InnerText = (section.Content.Summary?.Default).IfNullOrEmpty("None");

            DeleteSectionLink.NavigateUrl = $"/admin/assessments/sections/delete?bank={BankID}&section={SectionID}";
            EditSectionContentTitle.NavigateUrl = $"/ui/admin/assessments/sections/content?bank={BankID}&section={SectionID}&tab=title";
            EditSectionContentSummary.NavigateUrl = $"/ui/admin/assessments/sections/content?bank={BankID}&section={SectionID}&tab=summary";

            BindCriterion(section.Criterion);

            FormContent.Key = $"form_content.{form.Identifier}";
            FormContent.InitializationData = form;

            if (bank.IsAdvanced)
            {
                AddendumDetails.Key = $"addendum_details.{form.Identifier}";
                AddendumDetails.InitializationData = form;

                StatisticsPanel.Key = $"form_statistics.{form.Identifier}";
                StatisticsPanel.InitializationData = new QuestionStatisticInfo { Form = form };
            }

            SetEditLinkVisibility(AllowEdit && form.Publication.Status != PublicationStatus.Published);
            SetConfigurationSection(specification, section);

            DownloadSummariesButton.Visible = FormID != null;
        }

        private void SetConfigurationSection(Specification specification, Section section)
        {
            ConfigurationSection.Visible = specification.SectionsAsTabsEnabled && !specification.TabNavigationEnabled;

            ReconfigureLink.NavigateUrl = $"/ui/admin/assessments/sections/reconfigure?bank={BankID}&section={SectionID}";

            var limitSomeTabs = specification.TabTimeLimit == SpecificationTabTimeLimit.SomeTabs;
            var limitAllTabs = specification.TabTimeLimit == SpecificationTabTimeLimit.AllTabs;

            WarningOnNextTabEnabled.Text = section.WarningOnNextTabEnabled ? "Show" : "Disabled";

            BreakTimerEnabledField.Visible = limitSomeTabs || limitAllTabs;
            BreakTimerEnabled.Text = section.BreakTimerEnabled ? "Enabled" : "Disabled";

            TimeLimitField.Visible = limitAllTabs || limitSomeTabs && section.BreakTimerEnabled;
            TimeLimit.Text = section.TimeLimit <= 0 ? "None" : $"{section.TimeLimit} minute(s)";

            TimerTypeField.Visible = limitAllTabs || limitSomeTabs && section.BreakTimerEnabled;
            TimerType.Text = section.TimerType.GetDescription();
        }

        private void SetEditLinkVisibility(bool visible)
        {
            DeleteSectionLink.Visible = visible;
            EditSectionContentTitle.Visible = visible;
            EditSectionContentSummary.Visible = visible;
        }

        public void ReloadQuestions(Form form, bool showAllQuestions)
        {
            var questions = form.Specification.Type != SpecificationType.Static
                ? form.GetQuestions()
                : null;

            ReloadQuestions(form, null, questions, showAllQuestions);
        }

        public void ReloadQuestions(Section section, bool showAllQuestions)
        {
            ReloadQuestions(section.Form, section.Identifier, section.Fields.Select(x => x.Question).ToList(), showAllQuestions);
        }

        private void ReloadQuestions(Form form, Guid? sectionId, List<Question> questions, bool showAllQuestions)
        {
            var spec = form.Specification;
            var bank = spec.Bank;

            var returnData = $"bank&form={form.Identifier}&tab=fields";
            if (sectionId.HasValue)
                returnData += $"&section={sectionId.Value}";

            try
            {
                var bankAnalyzer = new BankAnalyzer(form);
                var questionIndexes = bankAnalyzer.GetQuestionIndexes(form);
                var settings = new QuestionRepeater.BindSettings
                {
                    FormIdentifier = form.Identifier,
                    AllowEdit = AllowEdit,
                    AllowAnalyse = true,
                    ShowProperties = bank.IsAdvanced
                        ? PropertiesVisibility.Advanced
                        : PropertiesVisibility.Basic,
                    GetFormIndex = q => questionIndexes.ContainsKey(q.Identifier) ? questionIndexes[q.Identifier] : -1,
                    ReturnUrl = new ReturnUrl(returnData),
                    GetChangeUrl = q => $"/ui/admin/assessments/questions/change?bank={bank.Identifier}&form={form.Identifier}&question={q.Identifier}"
                };

                if (spec.Type == SpecificationType.Static)
                    settings.GetPinModel = () => PinModel.GetModel(bank.Identifier);

                Questions.Visible = true;

                if (!showAllQuestions && questions.Count > 6)
                {
                    LoadQuestionsButton.Visible = true;

                    Questions.LoadData(questions.Take(4), settings);
                }
                else
                {
                    LoadQuestionsButton.Visible = false;

                    Questions.LoadData(questions, settings);
                }
            }
            catch (ApplicationError apperr)
            {
                Questions.Visible = false;
                LoadQuestionsButton.Visible = false;

                QuestionsAlert.AddMessage(AlertType.Error, apperr.Message);
            }
        }

        #endregion

        #region Methods (binding)

        private void BindCriterion(Criterion criterion)
        {
            SetRepeater.DataSource = criterion.Sets;
            SetRepeater.DataBind();

            SetWeight.Text = criterion.SetWeight.ToString("n2");

            QuestionLimitField.Visible = criterion.FilterType != CriterionFilterType.Pivot;
            QuestionLimit.Text = criterion.QuestionLimit.ToString("n0") + " of " + criterion.Sets.SelectMany(x => x.Questions).Count().ToString("n0");

            var hasBasicFilter = !string.IsNullOrEmpty(criterion.TagFilter);
            var hasAdvancedFilter = !hasBasicFilter && criterion.PivotFilter != null && !criterion.PivotFilter.IsEmpty;

            FilterType.Text = hasBasicFilter ? "Question Tag Filter" : hasAdvancedFilter ? "Pivot Table Filter" : "Include All Questions";

            TagFilterField.Visible = hasBasicFilter;
            TagFilter.Text = criterion.TagFilter;
        }

        #endregion

        #region Methods (helpers)

        public void OpenTab(Tab t)
        {
            if (t == Tab.Form)
                FormTab.IsSelected = true;
            else if (t == Tab.Addendum)
                AddendumTab.IsSelected = true;
            else if (t == Tab.Section)
                SectionTab.IsSelected = true;
            else if (t == Tab.Content)
                ContentTab.IsSelected = true;
            else
                QuestionsTab.IsSelected = true;
        }

        #endregion
    }
}