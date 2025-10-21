using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Admin.Assessments.Questions.Controls;
using InSite.Admin.Assessments.Questions.Utilities;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Portal.Assessments.Attempts.Utilities;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Sets.Forms
{
    public partial class Preview : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid SetID => Guid.TryParse(Request.QueryString["set"], out var value) ? value : Guid.Empty;

        protected Dictionary<string, AttemptImageInfo> Images
        {
            get => (Dictionary<string, AttemptImageInfo>)ViewState[nameof(Images)];
            private set => ViewState[nameof(Images)] = value;
        }

        #endregion

        #region Initiailization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            QuestionRepeater.ItemDataBound += QuestionRepeater_ItemDataBound;
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
                RedirectToSearch();

            var set = bank.FindSet(SetID);
            if (set == null)
                RedirectToReader();

            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{bank.Content.Title.Default.IfNullOrEmpty(bank.Name)} <span class='form-text'>Asset #{bank.Asset}</span>");

            SetName.InnerText = set.Name;

            LoadQuestions(set);
        }

        #endregion

        #region Event handlers

        private void QuestionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var previewPanel = (QuestionPreviewPanel)e.Item.FindControl("PreviewPanel");
            previewPanel.LoadData((PreviewQuestionModel)e.Item.DataItem);
        }

        #endregion

        private void LoadQuestions(Domain.Banks.Set set)
        {
            PreviewContainer.Visible = false;
            QuestionRepeater.Visible = false;

            if (set.Questions.Count == 0)
            {
                ScreenStatus.AddMessage(AlertType.Error, $"There are no questions in this set.");
                return;
            }

            PreviewContainer.Visible = true;

            QuestionRepeater.Visible = true;
            QuestionRepeater.DataSource = set.Questions.Select((question, qIndex) =>
            {
                var aQuestion = AttemptStarter.CreateQuestion(question, false, Language.Default);
                return new PreviewQuestionModel(qIndex + 1, question, aQuestion);
            });
            QuestionRepeater.DataBind();

            Images = AttemptImageInfo.CreateDictionary(set.Bank.EnumerateAllAttachments());
        }

        #region Methods (redirect)

        private void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToReader(Guid? setId = null)
        {
            var url = $"/ui/admin/assessments/banks/outline?bank={BankID}";

            if (setId.HasValue)
                url += $"&set={setId.Value}";

            HttpResponseHelper.Redirect(url, true);
        }

        #endregion

        #region IHasParentLinkParameters

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"bank={BankID}&set={SetID}"
                : null;
        }

        #endregion
    }
}