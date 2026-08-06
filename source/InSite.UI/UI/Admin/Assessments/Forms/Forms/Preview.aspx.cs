using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Admin.Assessments.Questions.Utilities;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Attempts;
using InSite.Domain.Banks;
using InSite.UI.Admin.Assessments.Forms.Controls;
using InSite.UI.Layout.Admin;
using InSite.UI.Portal.Assessments.Attempts.Utilities;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Forms.Forms
{
    public partial class Preview : AdminBasePage, IHasParentLinkParameters
    {
        #region Classes

        private class SectionInfo
        {
            public Guid ID { get; private set; }
            public string Title { get; private set; }
            public string Summary { get; private set; }

            public string SummaryHtml => Markdown.ToHtml(Summary);
            public bool HasTitle => !string.IsNullOrEmpty(Title);
            public bool HasSummary => !string.IsNullOrEmpty(Summary);

            public List<PreviewQuestionModel> Questions { get; } = new List<PreviewQuestionModel>();

            public SectionInfo(Domain.Banks.Section section)
            {
                ID = section.Identifier;
                Title = section.Content.Title?.Default;
                Summary = section.Content.Summary?.Default;
            }

            public SectionInfo(Domain.Banks.Criterion criterion)
            {
                ID = criterion.Identifier;
                Title = (criterion.Content.Title?.Default).IfNullOrEmpty(criterion.Name);
                Summary = (criterion.Content.Summary?.Default).NullIfEmpty();
            }
        }

        #endregion

        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid FormID => Guid.TryParse(Request.QueryString["form"], out var value) ? value : Guid.Empty;

        protected Dictionary<string, AttemptImageInfo> Images
        {
            get => (Dictionary<string, AttemptImageInfo>)ViewState[nameof(Images)];
            private set => ViewState[nameof(Images)] = value;
        }

        private int? SectionNavItemsCount
        {
            get => (int?)ViewState[nameof(SectionNavItemsCount)];
            set => ViewState[nameof(SectionNavItemsCount)] = value;
        }

        #endregion

        #region Initiailization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SectionRepeater.ItemDataBound += SectionRepeater_ItemDataBound;
        }

        protected override void CreateChildControls()
        {
            if (SectionNavItemsCount.HasValue && SectionNav.ItemsCount == 0)
            {
                var count = SectionNavItemsCount.Value;
                for (var i = 0; i < count; i++)
                    AddSectionNavItem(out _, out _);
            }

            base.CreateChildControls();
        }

        private void AddSectionNavItem(out NavItem navItem, out PreviewSectionPanel sectionPanel)
        {
            SectionNav.AddItem(navItem = new NavItem());
            navItem.Controls.Add(sectionPanel = (PreviewSectionPanel)LoadControl("~/UI/Admin/Assessments/Forms/Controls/PreviewSectionPanel.ascx"));
        }

        #endregion

        #region Loading

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
            {
                ShowAlert(a =>
                {
                    if (BankID == Guid.Empty)
                        a.ShowBankMissing();
                    else
                        a.ShowBankNotFound(BankID);
                });
                return;
            }

            var form = bank.FindForm(FormID);
            if (form == null)
            {
                var bankName = bank.Name;
                ShowAlert(a =>
                {
                    if (FormID == Guid.Empty)
                        a.ShowFormMissing(BankID, bankName);
                    else
                        a.ShowFormNotFound(FormID, BankID, bankName);
                });
                return;
            }

            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>");

            FormTitle.InnerText = form.Content.Title.Default.IfNullOrEmpty(form.Name);

            LoadQuestions(form);

            CloseButton.NavigateUrl = GetReaderUrl(form.Identifier);
        }

        private void LoadQuestions(Form form)
        {
            PreviewSection.Visible = false;
            SectionNav.Visible = false;
            SectionRepeater.Visible = false;
            SectionPanel.Visible = false;

            var questions = AttemptQuestionBuilder.Build(form, false, Language.Default);
            if (questions.Length == 0)
            {
                ScreenStatus.AddMessage(
                    AlertType.Error,
                    $"The exam form assigned to this assessment " +
                    $"(Form Asset {form.Asset}: \"{(form.Content.Title?.Default).IfNullOrEmpty(form.Name)}\") " +
                    $"does not contain any questions that match the criteria specified for it.");
                return;
            }

            PreviewSection.Visible = true;

            var spec = form.Specification;
            var bank = spec.Bank;

            var instructions = form.Content.InstructionsForOnline?.Default;
            IntroductionHtml.Visible = !string.IsNullOrEmpty(instructions);
            IntroductionHtml.InnerHtml = Markdown.ToHtml(instructions);

            if (spec.Type == SpecificationType.Static)
                BindStaticForm(form, questions);
            else
                BindDynamicForm(form, questions);

            Images = AttemptImageInfo.CreateDictionary(bank.EnumerateAllAttachments());
        }

        private void BindStaticForm(Form form, AttemptQuestion[] questions)
        {
            var spec = form.Specification;
            var fieldMapping = form.Sections.SelectMany(x => x.Fields).ToDictionary(f => f.QuestionIdentifier);

            SectionInfo section = null;
            var sections = new List<SectionInfo>();

            for (var i = 0; i < questions.Length; i++)
            {
                var attemptQuestion = questions[i];
                var field = fieldMapping[attemptQuestion.Identifier];
                var model = new PreviewQuestionModel(i + 1, field.Question, attemptQuestion);

                if (section == null || section.ID != field.Section.Identifier)
                    sections.Add(section = new SectionInfo(field.Section));

                section.Questions.Add(model);
            }

            if (spec.SectionsAsTabsEnabled)
            {
                SectionNav.Visible = true;
                SectionNavItemsCount = sections.Count;

                NextTabButton.Visible = !spec.TabNavigationEnabled && !spec.SingleQuestionPerTabEnabled;
                NextQuestionButton.Visible = !spec.TabNavigationEnabled && spec.SingleQuestionPerTabEnabled;

                if (spec.SingleQuestionPerTabEnabled)
                    SectionNav.CssClass = "single-question";

                for (var i = 0; i < sections.Count; i++)
                {
                    var sectionInfo = sections[i];

                    AddSectionNavItem(out var navItem, out var sectionPanel);

                    navItem.Title = sections[i].Title.IfNullOrEmpty("(Untitled)");
                    sectionPanel.LoadData(
                        sectionInfo.HasSummary ? sectionInfo.SummaryHtml : null,
                        sectionInfo.Questions);
                }
            }
            else
            {
                SectionRepeater.Visible = true;
                SectionRepeater.DataSource = sections;
                SectionRepeater.DataBind();
            }
        }

        private void BindDynamicForm(Form form, AttemptQuestion[] questions)
        {
            var spec = form.Specification;
            var bank = spec.Bank;

            if (spec.SectionsAsTabsEnabled)
            {
                SectionInfo section = null;
                var sections = new List<SectionInfo>();
                var questionNumber = 1;

                foreach (var group in questions.GroupBy(x => x.Section))
                {
                    var cIndex = group.Key ?? -1;
                    var criterion = cIndex >= 0 && cIndex < spec.Criteria.Count
                        ? spec.Criteria[cIndex]
                        : new Criterion();

                    foreach (var aQuestion in group)
                    {
                        var bQuestion = bank.FindQuestion(aQuestion.Identifier);
                        var model = new PreviewQuestionModel(questionNumber++, bQuestion, aQuestion);

                        if (section == null || section.ID != criterion.Identifier)
                            sections.Add(section = new SectionInfo(criterion));

                        section.Questions.Add(model);
                    }
                }

                SectionNav.Visible = true;
                SectionNavItemsCount = sections.Count;

                NextTabButton.Visible = !spec.TabNavigationEnabled && !spec.SingleQuestionPerTabEnabled;
                NextQuestionButton.Visible = !spec.TabNavigationEnabled && spec.SingleQuestionPerTabEnabled;

                if (spec.SingleQuestionPerTabEnabled)
                    SectionNav.CssClass = "single-question";

                for (var i = 0; i < sections.Count; i++)
                {
                    var sectionInfo = sections[i];

                    AddSectionNavItem(out var navItem, out var sectionPanel);

                    navItem.Title = sections[i].Title.IfNullOrEmpty("(Untitled)");
                    sectionPanel.LoadData(
                        sectionInfo.HasSummary ? sectionInfo.SummaryHtml : null,
                        sectionInfo.Questions);
                }
            }
            else
            {
                SectionPanel.Visible = true;
                SectionPanel.LoadData(null, questions.Select((attemptQuestion, index) =>
                {
                    var bankQuestion = bank.FindQuestion(attemptQuestion.Identifier);
                    return new PreviewQuestionModel(index + 1, bankQuestion, attemptQuestion);
                }));
            }
        }

        #endregion

        #region Event handlers

        private void SectionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var sectionInfo = (SectionInfo)e.Item.DataItem;
            var sectionPanel = (PreviewSectionPanel)e.Item.FindControl("SectionPanel");
            sectionPanel.LoadData(
                sectionInfo.HasSummary ? sectionInfo.SummaryHtml : null,
                sectionInfo.Questions);
        }

        #endregion

        #region Methods (redirect)

        private void ShowAlert(Action<InSite.Admin.Assessments.Forms.Controls.FormPageAlert> configure)
        {
            ContentPanel.Visible = false;
            configure(PageAlert);
        }

        private string GetReaderUrl(Guid? formId = null)
        {
            var url = $"/ui/admin/assessments/banks/outline?bank={BankID}";

            if (formId.HasValue)
                url += $"&form={formId.Value}";

            return url;
        }

        private void RedirectToReader(Guid? formId = null)
        {
            var url = GetReaderUrl(formId);

            HttpResponseHelper.Redirect(url, true);
        }

        #endregion

        #region IHasParentLinkParameters

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"bank={BankID}&form={FormID}"
                : null;
        }

        #endregion
    }
}
