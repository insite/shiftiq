using System;

using InSite.Common.Web.UI;
using InSite.Domain.Banks;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Forms.Controls
{
    public partial class FormContent : BaseUserControl
    {
        private Guid BankId
        {
            get
            {
                var bankParam = Request.QueryString["bank"];
                if (Guid.TryParse(bankParam, out var id))
                    return id;

                throw new InvalidOperationException("Invalid or missing bank identifier in query string.");
            }
        }

        public void SetInputValues(Form form, Guid? bankID, bool canWrite)
        {
            ContentTitle.Text = (form.Content.Title?.Default).IfNullOrEmpty("None");
            ContentSummary.Text = (form.Content.Summary?.Default).IfNullOrEmpty("None");
            HasDiagrams.Text = form.HasDiagrams ? "Yes" : "No";
            HasReferenceMaterials.Text = form.HasReferenceMaterials != ReferenceMaterialType.None
                ? form.HasReferenceMaterials.GetDescription()
                : "No";
            MaterialsForDistribution.Text = Markdown.ToHtml(form.Content.MaterialsForDistribution.Default.IfNullOrEmpty("None"));
            MaterialsForParticipation.Text = Markdown.ToHtml(form.Content.MaterialsForParticipation.Default.IfNullOrEmpty("None"));
            InstructionsForOnline.Text = Markdown.ToHtml(form.Content.InstructionsForOnline.Default.IfNullOrEmpty("None"));
            InstructionsForPaper.Text = Markdown.ToHtml(form.Content.InstructionsForPaper.Default.IfNullOrEmpty("None"));

            SetupEditLinks(form, canWrite);

            if (bankID == null)
                return;

            var bank = ServiceLocator.BankSearch.GetBank(bankID.Value);

            if (bank == null)
                return;

            AttemptStartedMessage.Text = GetAttemptMessage(form.WhenAttemptStartedNotifyAdminMessageIdentifier);
            AttemptCompletedMessage.Text = GetAttemptMessage(form.WhenAttemptCompletedNotifyAdminMessageIdentifier);
        }

        private string GetAttemptMessage(Guid? messageIdentifier)
        {
            if (!messageIdentifier.HasValue)
                return "None";

            var message = ServiceLocator.MessageSearch.GetMessage(messageIdentifier.Value);

            if (message == null)
                return "None";
            
            return message.MessageTitle;
        }

        private void SetupEditLinks(Form form, bool canWrite)
        {
            var queryString = $"bank={form.Specification.Bank.Identifier}&form={form.Identifier}";

            C1.NavigateUrl = $"/ui/admin/assessments/forms/content?{queryString}&tab=title";
            C2.NavigateUrl = $"/ui/admin/assessments/forms/content?{queryString}&tab=summary";
            C3.NavigateUrl = $"/ui/admin/assessments/forms/content?{queryString}";
            C4.NavigateUrl = $"/ui/admin/assessments/forms/content?{queryString}";

            M1.NavigateUrl = $"/ui/admin/assessments/forms/content?{queryString}&tab=materials";
            M2.NavigateUrl = $"/ui/admin/assessments/forms/content?{queryString}&tab=materials";

            N1.NavigateUrl = $"/ui/admin/assessments/forms/change-form-notification?{queryString}";
            N2.NavigateUrl = $"/ui/admin/assessments/forms/change-form-notification?{queryString}";

            I1.NavigateUrl = $"/ui/admin/assessments/forms/content?{queryString}&tab=instructions";
            I2.NavigateUrl = $"/ui/admin/assessments/forms/content?{queryString}&tab=instructions";

            C1.Visible = canWrite;
            C2.Visible = canWrite;
            C3.Visible = canWrite;
            C4.Visible = canWrite;
            M1.Visible = canWrite;
            M2.Visible = canWrite;
            I1.Visible = canWrite;
            I2.Visible = canWrite;
        }
    }
}