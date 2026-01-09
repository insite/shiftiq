using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Commands;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Workflow.Forms.Conditions
{
    public partial class Change : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? SurveyIdentifier =>
            Guid.TryParse(Request["form"], out var value) ? value : (Guid?)null;

        private Guid? OptionItemIdentifier =>
            Guid.TryParse(Request["option"], out var value) ? value : (Guid?)null;

        private string ReturnPanel => Request["returnpanel"] as string;

        private string ReturnTab => Request["returntab"] as string;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        public override void ApplyAccessControl()
        {
            if (!CanEdit) SaveButton.Visible = false;
            base.ApplyAccessControl();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var survey = ServiceLocator.SurveySearch.GetSurveyState(SurveyIdentifier.Value);
            var optionItem = survey.Form.FindOptionItem(OptionItemIdentifier.Value);

            var result = Detail.GetInputValues();

            var commands = new List<Command>();

            if (result.MaskingItem != OptionItemIdentifier)
            {
                if (optionItem.MaskedQuestionIdentifiers.Count > 0)
                    commands.Add(new Application.Surveys.Write.DeleteSurveyCondition(SurveyIdentifier.Value, optionItem.Identifier, optionItem.MaskedQuestionIdentifiers.ToArray()));

                commands.Add(new Application.Surveys.Write.AddSurveyCondition(SurveyIdentifier.Value, result.MaskingItem, result.MaskedQuestions));
            }
            else
            {
                var addMaskedQuestions = result.MaskedQuestions.Except(optionItem.MaskedQuestionIdentifiers).ToArray();
                var removeMaskedQuestions = optionItem.MaskedQuestionIdentifiers.Except(result.MaskedQuestions).ToArray();

                if (addMaskedQuestions.Length > 0)
                    commands.Add(new Application.Surveys.Write.AddSurveyCondition(SurveyIdentifier.Value, optionItem.Identifier, addMaskedQuestions));

                if (removeMaskedQuestions.Length > 0)
                    commands.Add(new Application.Surveys.Write.DeleteSurveyCondition(SurveyIdentifier.Value, optionItem.Identifier, removeMaskedQuestions));
            }

            foreach (var command in commands)
                ServiceLocator.SendCommand(command);

            HttpResponseHelper.Redirect($"/ui/admin/workflow/forms/outline?form={SurveyIdentifier}&panel={ReturnPanel}&tab={ReturnTab}", true);
        }

        private void Open()
        {
            var survey = SurveyIdentifier.HasValue ? ServiceLocator.SurveySearch.GetSurveyState(SurveyIdentifier.Value) : null;
            var optionItem = survey != null && OptionItemIdentifier.HasValue ? survey.Form.FindOptionItem(OptionItemIdentifier.Value) : null;

            if (optionItem == null
                || survey.Form.Tenant != Organization.Identifier
                || survey.Form.Locked.HasValue
                || !CurrentSessionState.Identity.IsGranted(Route.ToolkitName, PermissionOperation.Write)
                )
            {
                HttpResponseHelper.Redirect($"/ui/admin/workflow/forms/search", true);
            }

            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{survey.Form.Name} <span class='form-text'>Form #{survey.Form.Asset}</span>");

            CancelButton.NavigateUrl = $"/ui/admin/workflow/forms/outline?form={SurveyIdentifier}&panel={ReturnPanel}&tab={ReturnTab}";

            Detail.SetInputValues(optionItem, survey, User.TimeZone);
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"form={SurveyIdentifier}"
                : null;
        }
    }
}
