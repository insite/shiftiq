using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Surveys.Forms.Forms
{
    public partial class DeleteSurveyCondition : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? MaskingOptionItemIdentifier => Guid.TryParse(Request["option"], out var result) ? result : (Guid?)null;

        private Guid? MaskedQuestionIdentifier => Guid.TryParse(Request["question"], out var result) ? result : (Guid?)null;

        private string ReturnPanel => Request["returnpanel"] as string;

        private string ReturnTab => Request["returntab"] as string;

        private Guid SurveyIdentifier
        {
            get => (Guid)ViewState[nameof(SurveyIdentifier)];
            set => ViewState[nameof(SurveyIdentifier)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var condition = MaskingOptionItemIdentifier.HasValue && MaskedQuestionIdentifier.HasValue
                ? ServiceLocator.SurveySearch.GetSurveyCondition(MaskingOptionItemIdentifier.Value, MaskedQuestionIdentifier.Value,
                    x => x.MaskingSurveyOptionItem.SurveyOptionList.SurveyQuestion.SurveyForm,
                    x => x.MaskedSurveyQuestion.SurveyForm)
                : null;

            if (condition == null)
            {
                HttpResponseHelper.Redirect($"/ui/admin/surveys/search", true);
            }


            var surveyForm = condition.MaskingSurveyOptionItem?.SurveyOptionList?.SurveyQuestion?.SurveyForm;

            if (surveyForm == null || surveyForm.OrganizationIdentifier != Organization.Identifier || surveyForm.SurveyFormLocked.HasValue)
            {
                HttpResponseHelper.Redirect($"/ui/admin/surveys/search", true);
            }

            SurveyIdentifier = surveyForm.SurveyFormIdentifier;
            var survey = ServiceLocator.SurveySearch.GetSurveyState(SurveyIdentifier);

            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{surveyForm.SurveyFormName} <span class='form-text'>Survey Form #{surveyForm.AssetNumber}</span>");

            SurveyLink.HRef = $"/ui/admin/surveys/forms/outline?survey={survey.Form.Identifier}";
            InternalName.Text = survey.Form.Name;

            MaskingQuestionTitle.Text = GetTitle(condition.MaskingSurveyOptionItem.SurveyOptionList.SurveyQuestionIdentifier).IfNullOrEmpty("Untitled");

            var maskingListTitle = GetTitle(condition.MaskingSurveyOptionItem.SurveyOptionListIdentifier);
            MaskingListTitleField.Visible = maskingListTitle.HasValue();
            if (MaskingListTitleField.Visible)
            {
                MaskingListTitle.Text = maskingListTitle;
            }

            MaskingOptionTitle.Text = GetTitle(condition.MaskingSurveyOptionItemIdentifier).IfNullOrEmpty("Untitled");
            MaskedQuestionTitle.Text = GetTitle(condition.MaskedSurveyQuestionIdentifier).IfNullOrEmpty("Untitled");

            CancelButton.NavigateUrl = $"/ui/admin/surveys/forms/outline?survey={SurveyIdentifier}&panel={ReturnPanel}&tab={ReturnTab}";
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            var command = new Application.Surveys.Write.DeleteSurveyCondition(SurveyIdentifier, MaskingOptionItemIdentifier.Value, new Guid[] { MaskedQuestionIdentifier.Value });

            ServiceLocator.SendCommand(command);

            HttpResponseHelper.Redirect($"/ui/admin/surveys/forms/outline?survey={SurveyIdentifier}&panel={ReturnPanel}&tab={ReturnTab}");
        }

        protected string GetTitle(Guid id) =>
            ServiceLocator.ContentSearch.GetSnip(id, ContentLabel.Title);

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"survey={SurveyIdentifier}"
                : null;
        }
    }
}