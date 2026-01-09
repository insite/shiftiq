using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Assessments.Questions.Controls;
using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Assessments.Sets.Forms
{
    public partial class ChangeRandomization : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid SetID => Guid.Parse(Request.QueryString["set"]);

        private List<Guid> DataKeys => (List<Guid>)(ViewState[nameof(DataKeys)]
            ?? (ViewState[nameof(DataKeys)] = new List<Guid>()));

        #endregion

        #region Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            QuestionRepeater.DataBinding += QuestionRepeater_DataBinding;
            QuestionRepeater.ItemDataBound += QuestionRepeater_ItemDataBound;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanEdit)
                RedirectToSearch();

            if (!IsPostBack)
                Open();
        }

        #endregion

        #region Event handlers

        private void QuestionRepeater_DataBinding(object sender, EventArgs e)
        {
            DataKeys.Clear();
        }

        private void QuestionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var question = (Question)e.Item.DataItem;

            DataKeys.Add(question.Identifier);

            ((ITextControl)e.Item.FindControl("BankSequence")).Text = (question.BankIndex + 1).ToString();
            ((ITextControl)e.Item.FindControl("Title")).Text = question.Content.Title.Default;
            ((RandomizationInput)e.Item.FindControl("RandomizationDetails")).SetInputValues(question);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Save();

            RedirectToReader(SetID);
        }

        #endregion

        #region Database operations

        private void Open()
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
                RedirectToSearch();

            var set = bank.FindSet(SetID);
            if (set == null)
                RedirectToReader();

            SetInputValues(set);
        }

        private void Save()
        {
            if (SetRandomizationDetails.IsChanged)
                ServiceLocator.SendCommand(new ChangeSetRandomization(BankID, SetID, SetRandomizationDetails.GetCurrentValue()));

            var itemIndex = 0;
            foreach (RepeaterItem item in QuestionRepeater.Items)
            {
                if (!IsContentItem(item))
                    continue;

                var qDetails = (RandomizationInput)item.FindControl("RandomizationDetails");
                if (qDetails.IsChanged)
                {
                    var questionId = DataKeys[itemIndex];
                    ServiceLocator.SendCommand(new ChangeQuestionRandomization(BankID, questionId, qDetails.GetCurrentValue()));
                }

                itemIndex++;
            }
        }

        #endregion

        #region Settings/getting input values

        private void SetInputValues(Set set)
        {
            var title =
                $"{(set.Bank.Content.Title?.Default).IfNullOrEmpty(set.Bank.Name)} <span class=\"form-text\">Asset #{set.Bank.Asset}</span>";

            PageHelper.AutoBindHeader(this, null, title);

            BankDetails.BindBank(set.Bank);
            SetDetails.BindSet(set, true, true, false);

            SetRandomizationDetails.SetInputValues(set);

            QuestionRandomizationSection.Visible = set.Questions.Count > 0;
            QuestionRepeater.DataSource = set.Questions;
            QuestionRepeater.DataBind();

            CancelButton.NavigateUrl = GetReaderUrl(SetID);
        }

        #endregion

        #region Methods (redirect)

        private static void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToReader(Guid? setId = null)
        {
            var url = GetReaderUrl(setId);

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetReaderUrl(Guid? setId = null)
        {
            var url = $"/ui/admin/assessments/banks/outline?bank={BankID}";

            if (setId.HasValue)
                url += $"&set={setId.Value}";

            return url;
        }

        #endregion

        #region IHasParentLinkParameters

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"bank={BankID}"
                : null;
        }

        #endregion
    }
}