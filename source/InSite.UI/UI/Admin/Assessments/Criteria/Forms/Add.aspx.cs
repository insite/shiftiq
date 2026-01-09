using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Criteria.Forms
{
    public partial class Add : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid SpecificationID => Guid.Parse(Request["spec"]);

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            QuestionSets.AutoPostBack = true;
            QuestionSets.SelectedIndexChanged += QuestionSets_SelectedIndexChanged;

            QuestionSetRequired.ServerValidate += QuestionSetRequired_ServerValidate;

            CriterionInput.FilterTypeChanged += FilterDetails_FilterTypeChanged;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanEdit)
                RedirectToFinder();

            if (!IsPostBack)
                Open();
        }

        #endregion

        #region Event handlers

        private void QuestionSets_SelectedIndexChanged(object sender, EventArgs e) => OnFilterTypeChanged();

        private void QuestionSetRequired_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = GetSelectedQuestionSets().Count > 0;
        }

        private void FilterDetails_FilterTypeChanged(object sender, EventArgs args) => OnFilterTypeChanged();

        private void OnFilterTypeChanged()
        {
            var questionSets = GetSelectedQuestionSets();

            CriterionInput.Visible = questionSets.Count > 0;

            if (questionSets.Count == 0)
                return;

            if (CriterionInput.FilterType == CriterionFilterType.Pivot)
            {
                var bank = ServiceLocator.BankSearch.GetBankState(BankID);
                var sets = new List<Set>();

                foreach (var setId in questionSets)
                    sets.Add(bank.FindSet(setId));

                CriterionInput.SetInputValues(sets);
            }
            else
            {
                // CriterionInput.ShowTagFilter();
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Save();
        }

        #endregion

        #region Database operations

        private void Open()
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
            {
                RedirectToFinder();
                return;
            }

            var spec = bank.FindSpecification(SpecificationID);
            if (spec == null)
                RedirectToBankReader();

            SetInputValues(spec);
        }

        private void Save()
        {
            try
            {
                var sieve = new Criterion();

                GetInputValues(sieve);

                QuestionDisplayFilter.Validate(sieve.TagFilter);

                ServiceLocator.SendCommand(new AddCriterion(BankID, SpecificationID, sieve.SetIdentifiers.ToArray(), sieve.Identifier, null, sieve.SetWeight, sieve.QuestionLimit, sieve.TagFilter, CriterionInput.RequirementsTable));

                RedirectToBankReader(SpecificationID, sieve.Identifier);
            }
            catch (ApplicationError apperr)
            {
                CreatorStatus.AddMessage(AlertType.Error, apperr.Message);
            }
        }

        #endregion

        #region Settin/getting input values

        private void SetInputValues(Specification spec)
        {
            var title =
                $"{(spec.Bank.Content.Title?.Default).IfNullOrEmpty(spec.Bank.Name)} <span class=\"form-text\">Asset #{spec.Bank.Asset}</span>";

            PageHelper.AutoBindHeader(this, null, title);

            SpecificationName.Text = spec.Name;

            var hasSets = spec.Bank.Sets.Count > 0;

            QuestionSetField.Visible = hasSets;
            NoQuestionSetsMessage.Visible = !hasSets;
            SaveButton.Enabled = hasSets;

            if (hasSets)
            {
                QuestionSets.Items.Clear();

                foreach (var set in spec.Bank.Sets)
                    QuestionSets.Items.Add(new System.Web.UI.WebControls.ListItem($"{set.Sequence}. {set.Name}", set.Identifier.ToString()));
            }

            CriterionInput.SetDefaultInputValues();

            OnFilterTypeChanged();

            CancelButton.NavigateUrl = GetReaderUrl();
        }

        private void GetInputValues(Criterion sieve)
        {
            sieve.Identifier = UniqueIdentifier.Create();
            sieve.SetIdentifiers.AddRange(GetSelectedQuestionSets());

            var filter = CriterionInput.GetInputValues();
            sieve.SetWeight = filter.SetWeight;
            sieve.QuestionLimit = filter.QuestionLimit;
            sieve.TagFilter = filter.BasicFilter;
        }

        private List<Guid> GetSelectedQuestionSets()
        {
            var result = new List<Guid>();

            foreach (System.Web.UI.WebControls.ListItem item in QuestionSets.Items)
            {
                if (item.Selected)
                    result.Add(Guid.Parse(item.Value));
            }

            return result;
        }

        #endregion

        #region IHasParentLinkParameters

        private void RedirectToFinder() =>
            HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToBankReader(Guid? specificationId = null, Guid? sieveId = null)
        {
            var url = GetReaderUrl(specificationId, sieveId);

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetReaderUrl(Guid? specificationId = null, Guid? sieveId = null)
        {
            var url = $"/ui/admin/assessments/banks/outline?bank={BankID}";

            if (sieveId.HasValue)
                url += $"&sieve={sieveId.Value}";
            else if (specificationId.HasValue)
                url += $"&spec={specificationId.Value}";
            else
                url += "&panel=specifications";

            return url;
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"bank={BankID}"
                : null;
        }

        #endregion
    }
}