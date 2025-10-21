using System;
using System.Linq;

using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Sdk.UI;

using RurbicsCreate = InSite.UI.Admin.Records.Rurbics.Create;
using RurbicsOutline = InSite.UI.Admin.Records.Rurbics.Outline;

namespace InSite.Admin.Assessments.Questions.Controls
{
    public partial class RubricDetail : BaseUserControl
    {
        public Guid? RubricID
        {
            get => Rubric.Value;
            set => Rubric.Value = value;
        }

        private Guid BankIdentifier
        {
            get => (Guid)ViewState[nameof(BankIdentifier)];
            set => ViewState[nameof(BankIdentifier)] = value;
        }

        private QuestionItemType? QuestionType
        {
            get => (QuestionItemType?)ViewState[nameof(QuestionType)];
            set => ViewState[nameof(QuestionType)] = value;
        }

        protected bool HasAttempts
        {
            get => (bool)ViewState[nameof(HasAttempts)];
            set => ViewState[nameof(HasAttempts)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Rubric.AutoPostBack = true;
            Rubric.ValueChanged += Rubric_ValueChanged;
        }

        private void Rubric_ValueChanged(object sender, FindEntityValueChangedEventArgs e)
        {
            SetOutlineLink();
        }

        public void InitData(Guid bankId, QuestionItemType? questionType)
        {
            BankIdentifier = bankId;
            QuestionType = questionType;
            HasAttempts = false;

            SetOutlineLink();
            SetCreateLink();
        }

        public void LoadData(Guid bankId, Guid questionId, Guid? rubricId)
        {
            BankIdentifier = bankId;

            if (!rubricId.HasValue)
                rubricId = ServiceLocator.RubricSearch.GetQuestionRubric(questionId)?.RubricIdentifier;

            Rubric.Value = rubricId;

            HasAttempts = ServiceLocator.AttemptSearch.GetExistsQuestionIdentifiers(new[] { questionId }).Any();
            Rubric.AllowClear = !HasAttempts;
            ChangeWarning.Visible = HasAttempts;

            SetOutlineLink();
            SetCreateLink();
        }

        private void SetOutlineLink()
        {
            OutlineLink.Visible = Rubric.Value.HasValue;

            OutlineLink.NavigateUrl = Rubric.Value.HasValue
                ? RurbicsOutline.GetNavigateUrl(Rubric.Value.Value)
                : null;

            RubricHint.Visible = Rubric.Value == null;
        }

        private void SetCreateLink()
        {
            var returnUrl = new ReturnUrl();
            returnUrl.Remove("returnFromRubric", "rubric", "type");

            var url = RurbicsCreate.GetNavigateUrl(
                bankId: BankIdentifier,
                type: (int?)QuestionType
            );

            CreateLink.NavigateUrl = returnUrl.GetRedirectUrl(url);
        }
    }
}