using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.Banks.Write;
using InSite.Application.Contacts.Read;
using InSite.Application.Pages.Write;
using InSite.Application.Sites.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

using Label = System.Web.UI.WebControls.Label;

namespace InSite.Admin.Assessments.Forms.Forms
{
    public partial class Publish : AdminBasePage, IHasParentLinkParameters
    {
        #region Classes

        private class FormItem
        {
            public Form Form { get; set; }
            public string PublicationStatus { get; set; }
        }

        #endregion

        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid FormID => Guid.TryParse(Request.QueryString["form"], out var value) ? value : Guid.Empty;

        #endregion

        #region Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PublicationOption.AutoPostBack = true;
            PublicationOption.SelectedIndexChanged += PublicationOption_SelectedIndexChanged;

            PreviousVersions.ItemDataBound += PreviousVersions_ItemDataBound;

            PortalAccessStandalone.AutoPostBack = true;
            PortalAccessStandalone.CheckedChanged += PortalAccessChanged;

            PortalAccessModule.AutoPostBack = true;
            PortalAccessModule.CheckedChanged += PortalAccessChanged;

            PublishButton.Click += PublishButton_Click;
            RepublishButton.Click += RepublishButton_Click;
            UnpublishButton.Click += UnpublishButton_Click;

            AccessFieldValidator.ServerValidate += AccessFieldValidator_ServerValidate;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        #endregion

        #region Event handlers

        private void AccessFieldValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = PortalAccessModule.Checked || PortalAccessStandalone.Checked;
        }

        private void PublicationOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            RepublishButton.Visible = PublicationOption.SelectedValue == "Republish";
            UnpublishButton.Visible = PublicationOption.SelectedValue == "Unpublish";

            FirstPublishedField.Visible = PublicationOption.SelectedValue == "Republish";
        }

        private void PreviousVersions_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var formItem = (FormItem)e.Item.DataItem;
            var publicationStatus = formItem.PublicationStatus;

            var label = (Label)e.Item.FindControl("PublicationStatus");

            if (string.Equals(publicationStatus, "Drafted", StringComparison.OrdinalIgnoreCase))
                label.CssClass = "badge bg-primary";
            else if (string.Equals(publicationStatus, "Published", StringComparison.OrdinalIgnoreCase))
                label.CssClass = "badge bg-success";
            else if (string.Equals(publicationStatus, "Unpublished", StringComparison.OrdinalIgnoreCase))
                label.CssClass = "badge bg-custom-default";
            else
                label.CssClass = "badge bg-info";

            label.Text = publicationStatus;
        }

        private void PortalAccessChanged(object sender, EventArgs e)
        {
            ProgramTilePanel.Visible = PortalAccessStandalone.Checked;
            PermissionListPanel.Visible = PortalAccessStandalone.Checked;
        }

        private void PublishButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Save(true);

            RedirectToReader(FormID);
        }

        private void RepublishButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Save(true);

            RedirectToReader(FormID);
        }

        private void UnpublishButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Save(false);

            RedirectToReader(FormID);
        }

        #endregion

        #region Database operations

        private void Open()
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
                RedirectToSearch();

            if (FormID == Guid.Empty)
                RedirectToReader(); 

            var form = bank.FindForm(FormID);
            if (form == null)
                RedirectToReader();

            SetInputValues(bank, form);
        }

        private void Save(bool published)
        {
            if (published)
                PublishForm();
            else
                UnpublishForm(BankID, FormID);
        }

        private void PublishForm()
        {
            var publication = new FormPublication
            {
                AllowFeedback = AllowFeedbackFromCandidates.ValueAsBoolean.Value,
                AllowRationaleForCorrectAnswers = AllowRationaleForCorrectAnswers.Checked,
                AllowDownloadAssessmentsQA = AllowDownloadAssessmentsQA.Checked,
                AllowRationaleForIncorrectAnswers = AllowRationaleForIncorrectAnswers.Checked,
                FirstPublished = FirstPublished.Value
            };

            ServiceLocator.SendCommand(new PublishForm(BankID, FormID, publication));

            if (FormTranslationsField.Visible)
            {
                var langs = FormTranslations.Items
                    .Cast<System.Web.UI.WebControls.ListItem>()
                    .Where(x => x.Selected).Select(x => x.Value).ToArray();
                ServiceLocator.SendCommand(new ModifyFormLanguages(BankID, FormID, langs));
            }

            if (!PortalAccessStandalone.Checked)
                return;

            var assessments = ServiceLocator.PageSearch.Bind(x => x, x => x.ObjectType == "Assessment" && x.ObjectIdentifier == FormID);
            if (assessments.Length > 1)
                return;

            var commands = new List<Command>();

            var id = assessments.Length == 0
                ? AddNewPage(commands)
                : UpdateExistingPage(commands, assessments[0]);

            ServiceLocator.SendCommands(commands);

            UpdatePermissions(id);
        }

        private Guid AddNewPage(List<Command> commands)
        {
            var id = UniqueIdentifier.Create();

            commands.Add(new CreatePage(id, null, null, Organization.Identifier, User.Identifier, ProgramTileLabel.Text, "Page", 0, false, false));
            commands.Add(new ChangePageContentControl(id, "Assessment"));
            commands.Add(new ChangePageAssessment(id, FormID));
            commands.Add(new ChangePageIcon(id, ProgramTileIcon.Text));

            return id;
        }

        private Guid UpdateExistingPage(List<Command> commands, QPage page)
        {
            var id = page.PageIdentifier;

            commands.Add(new ChangePageVisibility(id, false));

            if (!string.Equals(page.Title, ProgramTileLabel.Text))
                commands.Add(new ChangePageTitle(id, ProgramTileLabel.Text));

            if (!string.Equals(page.PageIcon, ProgramTileIcon.Text))
                commands.Add(new ChangePageIcon(id, ProgramTileIcon.Text));

            return id;
        }

        private void UpdatePermissions(Guid asset)
        {
            int selectionCount = 0;

            foreach (System.Web.UI.WebControls.ListItem item in PermissionList.Items)
            {
                var group = Guid.Parse(item.Value);

                if (item.Selected)
                {
                    selectionCount++;

                    var permission = TGroupPermissionSearch.Select(group, asset);

                    if (permission == null)
                    {
                        permission = new TGroupPermission
                        {
                            PermissionIdentifier = UniqueIdentifier.Create(),
                            ObjectIdentifier = asset,
                            ObjectType = "Assessment Form",
                            GroupIdentifier = group,
                            AllowRead = true
                        };
                    }

                    TGroupPermissionStore.Save(permission);
                }
                else
                {
                    var permission = TGroupPermissionSearch.Select(group, asset);
                    if (permission != null)
                        TGroupPermissionStore.Delete(permission.PermissionIdentifier);
                }
            }
        }

        private static void UnpublishForm(Guid bankId, Guid formId)
        {
            ServiceLocator.SendCommand(new UnpublishForm(bankId, formId));
            UnpublishAssessment(formId);
        }

        private static void UnpublishAssessment(Guid formId)
        {
            var assessments = ServiceLocator.PageSearch.Bind(x => x, x => x.ObjectType == "Assessment" && x.ObjectIdentifier == formId);
            foreach (var assessment in assessments)
                ServiceLocator.SendCommand(new ChangePageVisibility(assessment.PageIdentifier, true));
        }

        #endregion

        #region Settings/getting input values

        private void BindPermissionLists(Guid[] assets)
        {
            var groups = ServiceLocator.GroupSearch.GetGroups(new QGroupFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                GroupType = GroupTypes.Role
            })
                .Select(x => new { x.GroupIdentifier, x.GroupName })
                .ToList();

            PermissionList.DataSource = groups;
            PermissionList.DataBind();

            var assignments = TGroupPermissionSearch
                .Bind(x => x.GroupIdentifier,
                      x => assets.Contains(x.ObjectIdentifier) && x.Group.GroupType == GroupTypes.Role)
                .ToList();

            foreach (var assignment in assignments)
            {
                var item = PermissionList.Items.FindByValue(assignment.ToString());
                if (item != null)
                    item.Selected = true;
            }
        }

        private void SetInputValues(BankState bank, Form form)
        {
            var formPublicationStatus = ServiceLocator.BankSearch.GetForm(form.Identifier)?.FormPublicationStatus;
            var isPublished = string.Equals(formPublicationStatus, "Published", StringComparison.OrdinalIgnoreCase);
            var isDrafted = string.Equals(formPublicationStatus, "Drafted", StringComparison.OrdinalIgnoreCase);

            var subtitle = isPublished ? "Unpublish a form" : "Publish a form";

            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{subtitle} {(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>");

            FormStandard.AssetID = form.Specification.Bank.Standard;
            FormTitle.Text = (form.Content.Title?.Default).IfNullOrEmpty(form.Name);
            Version.Text = $"{form.Asset}.{form.AssetVersion}";
            ProgramTileLabel.Text = FormTitle.Text;

            AllowFeedbackFromCandidates.ValueAsBoolean = isPublished
                ? form.Publication.AllowFeedback
                : (bool?)null;
            AllowRationaleForCorrectAnswers.Checked = form.Publication.AllowRationaleForCorrectAnswers;
            AllowRationaleForIncorrectAnswers.Checked = form.Publication.AllowRationaleForIncorrectAnswers;
            AllowDownloadAssessmentsQA.Checked = form.Publication.AllowDownloadAssessmentsQA;

            FirstPublished.Value = form.Publication.FirstPublished ?? DateTimeOffset.Now;
            FirstPublishedField.Visible = !isPublished;

            var assets = ServiceLocator.PageSearch.Bind(
                x => new
                {
                    x.PageIdentifier,
                    Title = CoreFunctions.GetContentTextEn(x.PageIdentifier, ContentLabel.Title)
                },
                x => x.ObjectType == "Assessment" && x.ObjectIdentifier == form.Identifier);

            if (Identity.IsOperator)
            {
                DeveloperPanel.Visible = true;

                AssessmentRepeater.Visible = assets.Length > 0;
                AssessmentRepeater.DataSource = assets;
                AssessmentRepeater.DataBind();

                AssessmentMissing.Visible = !AssessmentRepeater.Visible;
            }

            CancelButton.NavigateUrl = GetReaderUrl(FormID);

            BindPermissionLists(assets.Select(x => x.PageIdentifier).ToArray());

            PublishButton.Visible = !isPublished;
            RepublishButton.Visible = false;
            UnpublishButton.Visible = isPublished;

            BindPrevVersions(form);

            PublicationOptionField.Visible = isPublished;

            Instruction.Text = isPublished
                ? "Are you sure you want to unpublish this exam form? Click the<strong> Unpublish</strong> button to confirm."
                : "Are you sure you want to publish this exam form? Click the<strong> Publish</strong> button to confirm. The system will create and publish a new Achievement containing this form.";

            if (isPublished)
            {
                CandidateFeedbackField.Visible = false;
                InstructorRationaleField.Visible = false;
                DownloadAssessmentResultsField.Visible = false;
                AccessField.Visible = false;
                PermissionListPanel.Visible = false;
                ProgramTileIcon.Enabled = false;
            }

            var formLanguages = form.Languages.EmptyIfNull();
            var questionLanguages = isPublished ? null : form.Sections
                .SelectMany(x => x.Fields)
                .SelectMany(x => x.Question.Content.Languages)
                .Distinct()
                .Where(x => Language.CodeExists(x))
                .Select(x => new
                {
                    Value = x.ToLower(),
                    Title = Language.GetDisplayName(x)
                })
                .OrderBy(x => x.Title)
                .ToArray();

            FormTranslationsField.Visible = questionLanguages.IsNotEmpty();
            FormTranslations.DataSource = questionLanguages;
            FormTranslations.DataValueField = "Value";
            FormTranslations.DataTextField = "Title";
            FormTranslations.DataBind();

            foreach (System.Web.UI.WebControls.ListItem item in FormTranslations.Items)
                item.Selected = formLanguages.Contains(item.Value);
        }

        private void BindPrevVersions(Form form)
        {
            PreviousVersionsPanel.Visible = form.AssetVersion > 0;

            if (!form.IsFirstVersion())
            {
                var prevForms = form.EnumeratePreviousVersions().Select(y => new FormItem
                {
                    Form = y,
                    PublicationStatus = ServiceLocator.BankSearch.GetForm(form.Identifier)?.FormPublicationStatus
                });

                PreviousVersions.DataSource = prevForms;
                PreviousVersions.DataBind();

                if (prevForms.FirstOrDefault(x => !string.Equals(x.PublicationStatus, "Archived", StringComparison.OrdinalIgnoreCase)) != null)
                    EditorStatus.AddMessage(AlertType.Warning, "The previous version of this form is not archived.");
            }

            var purgeOrSurplus = form.GetQuestions().Where(x => x.Condition == "Purge" || x.Condition == "Surplus").Count();
            if (purgeOrSurplus > 0)
            {
                var questionText = purgeOrSurplus == 1 ? "question" : "questions";
                EditorStatus.AddMessage(AlertType.Warning, $"This form contains {purgeOrSurplus} {questionText} with a status of Purge or Surplus. Please change or remove these questions before you publish this form.");
            }

            var obsoleteCount = form.Sections.SelectMany(x => x.Fields).Where(x => !x.Question.IsLastVersion()).Count();
            if (obsoleteCount > 0)
            {
                var questionText = obsoleteCount == 1 ? "question" : "questions";
                var verb = obsoleteCount == 1 ? "is" : "are";
                EditorStatus.AddMessage(AlertType.Warning, $"This form contains {obsoleteCount} {questionText} that {verb} obsolete. Please ensure every question on the form is the latest version of that question before you publish.");
            }

            var composedQuestions = form.GetQuestions()
                .Where(x => x.Type.IsComposed())
                .Select(x => x.Identifier)
                .ToArray();
            if (composedQuestions.Length > 0)
            {
                var existing = ServiceLocator.BankSearch.GetQuestionsNotConnectedToRubrics(composedQuestions);
                if (existing.Count > 0)
                {
                    var questionText = existing.Count == 1 ? "question" : "questions";
                    var verb = existing.Count == 1 ? "is" : "are";
                    EditorStatus.AddMessage(AlertType.Warning, $" This Form cannot be published, there {verb} {existing.Count} Composed Response {questionText} without a Rubric attached.");

                    PublishButton.Visible = false;
                    RepublishButton.Visible = false;
                }
            }
        }

        #endregion

        #region Methods (redirect)

        private static void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToReader(Guid? form = null)
        {
            var url = GetReaderUrl(form);

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetReaderUrl(Guid? form = null)
        {
            var url = $"/ui/admin/assessments/banks/outline?bank={BankID}";

            if (form.HasValue)
                url += $"&form={form.Value}";

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