using System;
using System.Web.UI.WebControls;

using InSite.Application.Attempts.Read;
using InSite.Common.Web.UI;

using Shift.Sdk.UI;

namespace InSite.Admin.Attempts.Questions.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<QAttemptQuestionFilter>
    {
        protected override bool EnableSearchValidation => true;

        private Guid? DefaultQuestion => Guid.TryParse(Request["question"], out var value) ? value : (Guid?)null;

        public bool HasDefaultFilter => _hasDefaultValue == true;

        private bool? _hasDefaultValue = null;

        public override QAttemptQuestionFilter Filter
        {
            get
            {
                var filter = new QAttemptQuestionFilter
                {
                    BankIdentifier = BankID.Value,
                    FormIdentifier = FormID.ValueAsGuid,
                    QuestionIdentifier = BankQuestionID.Value,
                    LearnerUserIdentifier = CandidateID.Value
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                if (!IsPostBack)
                {
                    if (!_hasDefaultValue.HasValue)
                        _hasDefaultValue = TrySetDefaultValues();

                    if (_hasDefaultValue == true)
                        return;
                }

                BankID.Value = value.BankIdentifier;
                OnBankSelected(false);

                FormID.ValueAsGuid = value.FormIdentifier;
                BankQuestionID.Value = value.QuestionIdentifier;
                CandidateID.Value = value.LearnerUserIdentifier;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            BankID.AutoPostBack = true;
            BankID.ValueChanged += BankID_ValueChanged;

            FormID.ListFilter.BankIdentifier = Guid.Empty;

            CriteriaValidator.ServerValidate += CriteriaValidator_ServerValidate;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack && !_hasDefaultValue.HasValue)
                _hasDefaultValue = TrySetDefaultValues();
        }

        private bool TrySetDefaultValues()
        {
            if (DefaultQuestion == null)
                return false;

            var question = ServiceLocator.BankSearch.GetQuestion(DefaultQuestion.Value);
            if (question == null)
                return false;

            var bank = ServiceLocator.BankSearch.GetBank(question.BankIdentifier);
            if (bank.OrganizationIdentifier != Organization.Identifier)
                return false;

            ClearInternal();

            BankID.Value = question.BankIdentifier;
            OnBankSelected(false);

            BankQuestionID.Value = question.QuestionIdentifier;

            return true;
        }

        public override void Clear()
        {
            if (!TrySetDefaultValues())
                ClearInternal();
        }

        private void ClearInternal()
        {
            BankID.Value = null;
            OnBankSelected(true);

            FormID.ValueAsGuid = null;
            BankQuestionID.Value = null;
            CandidateID.Value = null;
        }

        private void BankID_ValueChanged(object sender, EventArgs e)
        {
            OnBankSelected(true);

            RightUpdatePanel.Update();
        }

        private void CriteriaValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = FormID.ValueAsGuid.HasValue || BankQuestionID.HasValue || CandidateID.HasValue;
        }

        private void OnBankSelected(bool selectFirstForm)
        {
            var bankId = BankID.Value;

            FormID.ClearSelection();
            BankQuestionID.Value = null;
            CandidateID.Value = null;

            FormID.Enabled = bankId.HasValue;
            BankQuestionID.Enabled = bankId.HasValue;
            CandidateID.Enabled = bankId.HasValue;

            if (!bankId.HasValue)
                return;

            FormID.ListFilter.BankIdentifier = bankId;
            FormID.ValueAsGuid = null;
            FormID.RefreshData();

            if (selectFirstForm && FormID.Items.Count > 1)
                ((IComboBoxOption)FormID.Items[1]).Selected = true;

            BankQuestionID.Filter.BankIdentifier = bankId;
            BankQuestionID.Value = null;

            CandidateID.Filter.AttemptBankIdentifier = bankId;
            CandidateID.Value = null;
        }
    }
}