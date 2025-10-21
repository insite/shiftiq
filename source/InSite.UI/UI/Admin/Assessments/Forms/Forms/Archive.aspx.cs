using System;
using System.Collections.Generic;
using System.Linq;

using Humanizer;

using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Forms.Forms
{
    public partial class Archive : AdminBasePage, IHasParentLinkParameters
    {
        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid FormID => Guid.Parse(Request["form"]);

        private string CommandName => Request["command"];

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ArchiveButton.Click += ArchiveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanEdit)
                RedirectToSearch();

            if (!IsPostBack)
            {
                var bank = ServiceLocator.BankSearch.GetBankState(BankID);
                if (bank == null)
                    RedirectToSearch();

                var form = bank.FindForm(FormID);
                if (form == null)
                    RedirectToReader();

                var title = $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>";
                PageHelper.AutoBindHeader(this, null, title);

                ArchiveHeader.Text = $"{CommandName.Titleize()} Form";

                FormDetails.BindForm(form, BankID, form.Specification.Bank.IsAdvanced);

                var questions = form.GetQuestions();
                LoadQuestionStatuses(questions);
                LoadAttachmentStatuses(bank, questions);

                ArchiveCommandName.InnerText = CommandName.Titleize();
                if (CommandName == "unarchive")
                {
                    ArchiveButton.Text = "<i class='fas fa-box-open'></i> Unarchive";
                    ArchiveButton.ButtonStyle = ButtonStyle.Success;
                }
                else
                {
                    ArchiveCommandHelp.Attributes["class"] = "help-block text-danger";
                    ArchiveCommandHelp.InnerHtml = $"Please remember: archived questions can not be used in forms.";
                }
                CancelButton.NavigateUrl = $"/ui/admin/assessments/banks/outline?bank={bank.Identifier}&form={form.Identifier}";
            }
        }

        private void LoadQuestionStatuses(List<Question> questions)
        {
            var questionStatuses = questions
                .GroupBy(x => x.PublicationStatus)
                .Select(x => new { x.Key, Name = x.Key.Humanize(), Count = x.Count() })
                .ToList();

            if (questionStatuses.FirstOrDefault(x => x.Key == PublicationStatus.Archived) == null)
                questionStatuses.Add(new { Key = PublicationStatus.Archived, Name = PublicationStatus.Archived.Humanize(), Count = 0 });

            if (questionStatuses.FirstOrDefault(x => x.Key == PublicationStatus.Drafted) == null)
                questionStatuses.Add(new { Key = PublicationStatus.Drafted, Name = PublicationStatus.Drafted.Humanize(), Count = 0 });

            if (questionStatuses.FirstOrDefault(x => x.Key == PublicationStatus.Published) == null)
                questionStatuses.Add(new { Key = PublicationStatus.Published, Name = PublicationStatus.Published.Humanize(), Count = 0 });

            questionStatuses.Sort((x1, x2) => x1.Name.CompareTo(x2.Name));

            Questions.DataSource = questionStatuses;
            Questions.DataBind();
        }

        private void LoadAttachmentStatuses(BankState bank, List<Question> questions)
        {
            var attachmentIdentifiers = questions.SelectMany(x => x.AttachmentIdentifiers).ToHashSet();

            var attachmentStatuses = bank.EnumerateAllAttachments()
                .Where(x => attachmentIdentifiers.Contains(x.Identifier))
                .GroupBy(x => x.PublicationStatus)
                .Select(x => new { x.Key, Name = x.Key.Humanize(), Count = x.Count() })
                .ToList();

            if (attachmentStatuses.FirstOrDefault(x => x.Key == PublicationStatus.Archived) == null)
                attachmentStatuses.Add(new { Key = PublicationStatus.Archived, Name = PublicationStatus.Archived.Humanize(), Count = 0 });

            if (attachmentStatuses.FirstOrDefault(x => x.Key == PublicationStatus.Drafted) == null)
                attachmentStatuses.Add(new { Key = PublicationStatus.Drafted, Name = PublicationStatus.Drafted.Humanize(), Count = 0 });

            if (attachmentStatuses.FirstOrDefault(x => x.Key == PublicationStatus.Published) == null)
                attachmentStatuses.Add(new { Key = PublicationStatus.Published, Name = PublicationStatus.Published.Humanize(), Count = 0 });

            attachmentStatuses.Sort((x1, x2) => x1.Name.CompareTo(x2.Name));

            Attachments.DataSource = attachmentStatuses;
            Attachments.DataBind();
        }

        private void ArchiveButton_Click(object sender, EventArgs e)
        {
            var attachments = ArchiveOption.SelectedValue == "FormAndQuestionsAndAttachments";
            var questions = attachments || ArchiveOption.SelectedValue == "FormAndQuestions";

            if (CommandName == "archive")
                ServiceLocator.SendCommand(new ArchiveForm(BankID, FormID, questions, attachments));
            else
                ServiceLocator.SendCommand(new UnarchiveForm(BankID, FormID, questions, attachments));

            RedirectToReader(FormID);
        }

        private void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToReader(Guid? form = null)
        {
            var url = $"/ui/admin/assessments/banks/outline?bank={BankID}";

            if (form.HasValue)
                url += $"&form={form.Value}";

            HttpResponseHelper.Redirect(url, true);
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline") ? $"bank={BankID}" : null;
        }
    }
}