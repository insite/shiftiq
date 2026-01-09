using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Assessments.Banks;
using InSite.Admin.Assessments.Questions.Controls;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

using Tab = InSite.Admin.Assessments.Outlines.Controls.FormSectionDetails.Tab;

namespace InSite.Admin.Assessments.Outlines.Controls
{
    public partial class FormsSection : BaseUserControl
    {
        #region Properties

        protected Guid BankID
        {
            get => (Guid)ViewState[nameof(BankID)];
            set => ViewState[nameof(BankID)] = value;
        }

        private bool AllowEdit
        {
            get => (bool)ViewState[nameof(AllowEdit)];
            set => ViewState[nameof(AllowEdit)] = value;
        }

        private int SectionsCount
        {
            get => (int)(ViewState[nameof(SectionsCount)] ?? 0);
            set => ViewState[nameof(SectionsCount)] = value;
        }

        protected static string UnpinText => QuestionRepeater.UnpinText;

        protected static string PinText => QuestionRepeater.PinText;

        protected static string UnpinClass => QuestionRepeater.UnpinClass;

        protected static string PinClass => QuestionRepeater.PinClass;

        public Action ReloadOutline { get; internal set; }

        public Func<BankState> LoadBank { get; internal set; }

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FormSelector.AutoPostBack = true;
            FormSelector.ValueChanged += FormSelector_ValueChanged;

            ReorderFieldsButton.Click += ReorderFieldsButton_Click;
            FormWorkshopButton.Click += FormWorkshopButton_Click;

            AddButton.Click += AddButton_Click;
            ActionButton.Click += ActionButton_Click;
            ReportButton.Click += ReportButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            EnsureChildControls();

            base.OnLoad(e);
        }

        protected override void CreateChildControls()
        {
            if (FormSectionsNav.ItemsCount == 0)
            {
                for (var i = 0; i < SectionsCount; i++)
                    AddFormSectionItem(out _, out _);
            }

            base.CreateChildControls();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            var pinModel = PinModel.GetModel(BankID);
            PinSection.Style["display"] = pinModel != null && pinModel.FieldAssetNumbers.Count > 0 ? string.Empty : "none";
            SwapFieldsLink.Style["display"] = pinModel != null && pinModel.FieldAssetNumbers.Count == 2 ? string.Empty : "none";
        }

        #endregion

        #region Event handlers

        private void AddButton_Click(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "AddForm")
            {
                var returnQuery = "bank";

                if (FormSelector.Value.IsNotEmpty())
                {
                    var form = GetSelectedForm();
                    returnQuery += $"&form={form.Identifier}";
                }

                var returnUrl = new ReturnUrl(returnQuery);
                var redirectUrl = returnUrl.GetRedirectUrl($"/ui/admin/assessments/forms/add?bank={BankID}");

                HttpResponseHelper.Redirect(redirectUrl);
            }
            else if (e.CommandName == "AddSection")
            {
                var form = GetSelectedForm();

                HttpResponseHelper.Redirect($"/ui/admin/assessments/sections/add?bank={BankID}&form={form.Identifier}");
            }
            else if (e.CommandName == "AddFields")
            {
                if (FormSectionsNav.SelectedItem == null)
                    return;

                var sectionDetails = (FormSectionDetails)FormSectionsNav.SelectedItem.Controls[0];

                HttpResponseHelper.Redirect($"/ui/admin/assessments/fields/add?bank={BankID}&section={sectionDetails.SectionID}");
            }
            else if (e.CommandName == "AddQuestion")
            {
                if (string.IsNullOrEmpty(FormSelector.Value))
                    return;

                var form = GetSelectedForm();
                var bank = form?.Specification.Bank;

                if (bank == null || bank.IsAdvanced || bank.Sets.Count == 0)
                    return;

                var redirectUrl = $"/ui/admin/assessments/questions/add?bank={bank.Identifier}";
                var returnQuery = $"form={form.Identifier}&question";

                if (FormSectionsNav.SelectedItem != null)
                {
                    var sectionDetails = (FormSectionDetails)FormSectionsNav.SelectedItem.Controls[0];

                    redirectUrl += $"&section={sectionDetails.SectionID}";
                    returnQuery += $"&section={sectionDetails.SectionID}";
                }
                else
                {
                    redirectUrl += $"&form={form.Identifier}";
                }

                redirectUrl = new ReturnUrl("bank").GetRedirectUrl(redirectUrl, returnQuery + "&tab=fields");

                HttpResponseHelper.Redirect(redirectUrl);
            }
            else
            {
                throw new ApplicationError("Unexpected command name: " + e.CommandName);
            }
        }

        private void ActionButton_Click(object sender, CommandEventArgs e)
        {
            var form = GetSelectedForm();

            switch (e.CommandName)
            {
                case "Preview":
                    HttpResponseHelper.Redirect($"/ui/admin/assessments/forms/preview?bank={BankID}&form={form.Identifier}");
                    break;
                case "Print":
                    HttpResponseHelper.Redirect($"/ui/admin/assessments/forms/print?bank={BankID}&form={form.Identifier}");
                    break;
                case "Duplicate":
                    HttpResponseHelper.Redirect($"/ui/admin/assessments/forms/duplicate?bank={BankID}&form={form.Identifier}");
                    break;
                case "Publish":
                    HttpResponseHelper.Redirect($"/ui/admin/assessments/forms/publish?bank={BankID}&form={form.Identifier}");
                    break;
                case "Unpublish":
                    HttpResponseHelper.Redirect($"/ui/admin/assessments/forms/publish?bank={BankID}&form={form.Identifier}");
                    break;
                case "Archive":
                    HttpResponseHelper.Redirect($"/ui/admin/assessments/forms/archive?bank={BankID}&form={form.Identifier}&command=archive");
                    break;
                case "Unarchive":
                    HttpResponseHelper.Redirect($"/ui/admin/assessments/forms/archive?bank={BankID}&form={form.Identifier}&command=unarchive");
                    break;
                case "Prepublish":
                    HttpResponseHelper.Redirect($"/ui/admin/assessments/forms/prepublish?bank={BankID}&form={form.Identifier}");
                    break;
            }
        }

        private void ReportButton_Click(object sender, EventArgs e)
        {
            var form = GetSelectedForm();

            HttpResponseHelper.Redirect($"/ui/admin/assessments/attempts/report?form={form.Identifier}");
        }

        private void FormSelector_ValueChanged(object sender, EventArgs e) => OnFormSelected();

        private void OnFormSelected()
        {
            var form = GetSelectedForm();

            BindForm(form, AllowEdit);
        }

        private void ReorderFieldsButton_Click(object sender, EventArgs e)
        {
            if (FormSectionsNav.SelectedItem == null)
                return;

            var sectionDetails = (FormSectionDetails)FormSectionsNav.SelectedItem.Controls[0];

            HttpResponseHelper.Redirect($"/ui/admin/assessments/fields/reorder?bank={BankID}&section={sectionDetails.SectionID}");
        }

        private void FormWorkshopButton_Click(object sender, EventArgs e)
        {
            var form = GetSelectedForm();

            HttpResponseHelper.Redirect($"/ui/admin/assessments/forms/workshop?bank={BankID}&form={form.Identifier}");
        }

        #endregion

        #region Data binding

        public void LoadData(BankState bank, bool canWrite, out bool isSelected, out bool isVisible)
        {
            BankID = bank.Identifier;
            AllowEdit = canWrite;

            isSelected = false;
            isVisible = true;

            var forms = bank.Specifications.SelectMany(x => x.EnumerateAllForms()).OrderBy(x => x.Name).ToArray();
            var hasForms = forms.Length > 0;

            if (bank.IsAdvanced && !hasForms)
            {
                isVisible = false;
                return;
            }

            FormSelector.Visible = hasForms;
            ReorderFieldsButton.Visible = hasForms;
            ReportButton.Visible = hasForms;
            FormWorkshopButton.Visible = hasForms && bank.IsAdvanced;
            FormRow.Visible = hasForms;

            ActionButton.Visible = hasForms;

            foreach (var item in ActionButton.Items)
            {
                if (item.Name != "Preview" && item.Name != "Publish" && item.Name != "Unpublish" && item.Name != "Print")
                    item.Visible = bank.IsAdvanced;
            }

            {
                AddButton.Items["AddForm"].Visible = !bank.IsAdvanced;
                AddButton.Items["AddSection"].Visible = hasForms;

                var addFieldsItem = (DropDownButtonItem)AddButton.Items["AddFields"];
                addFieldsItem.Text = bank.IsAdvanced ? "Question" : "Existing Question";
                addFieldsItem.Visible = false;

                AddButton.Items["AddQuestion"].Visible = false;
            }

            if (!hasForms)
                return;

            SwapFieldsLink.NavigateUrl = $"/ui/admin/assessments/fields/swap?bank={bank.Identifier}";
            SwapFieldsLink.Visible = canWrite;

            Guid? formId = null, sectionId = null, questionId = null;
            Tab selectedTab = Tab.Fields;
            Tuple<Form, ComboBoxOption> selectedForm = null;

            if (!IsPostBack)
            {
                if (Guid.TryParse(Request["form"], out var formValue))
                {
                    formId = formValue;

                    if (Guid.TryParse(Request["section"], out var sectionValue))
                        sectionId = sectionValue;

                    if (Guid.TryParse(Request["question"], out var questionValue))
                        questionId = questionValue;

                    var formTab = Request["tab"];
                    if (formTab == "addendum")
                        selectedTab = Tab.Addendum;
                    else if (formTab == "section")
                        selectedTab = Tab.Section;
                    else if (formTab == "fields")
                        selectedTab = Tab.Fields;
                    else if (formTab == "content")
                        selectedTab = Tab.Content;
                    else
                        selectedTab = Tab.Form;
                }
            }

            FormSelector.Items.Clear();

            foreach (var form in forms)
            {
                var option = new ComboBoxOption(
                    $"{form.Name} [{form.Asset}.{form.AssetVersion}]",
                    form.Identifier.ToString());

                if (selectedForm == null || formId.HasValue && form.Identifier == formId.Value)
                    selectedForm = new Tuple<Form, ComboBoxOption>(form, option);

                FormSelector.Items.Add(option);
            }

            if (selectedForm != null)
                selectedForm.Item2.Selected = true;

            OnFormSelected();

            if (formId.HasValue && selectedForm.Item1.Identifier == formId.Value)
            {
                isSelected = true;

                if (selectedForm.Item1.Specification.Type == SpecificationType.Static)
                {
                    var isQuestionFound = false;

                    foreach (var item in FormSectionsNav.GetItems())
                    {
                        var details = (FormSectionDetails)item.Controls[0];

                        var section = bank.FindSection(details.SectionID);
                        if (section != null)
                        {
                            details.ReloadQuestions(section, true);
                            details.OpenTab(selectedTab);
                        }

                        if (!isQuestionFound)
                        {
                            if (questionId.HasValue && section.Fields.Any(x => x.QuestionIdentifier == questionId.Value))
                            {
                                item.IsSelected = true;
                                ScriptManager.RegisterStartupScript(Page, GetType(), "scrollto_question", $"$(document).ready(function() {{ bankRead.scrollToQuestion('{questionId}'); }});", true);
                                isQuestionFound = true;
                            }
                            else if (sectionId.HasValue && details.SectionID == sectionId.Value)
                            {
                                item.IsSelected = true;
                            }
                        }
                    }
                }
                else
                {
                    var sectionNav = FormSectionsNav.GetItems().SingleOrDefault();
                    if (sectionNav != null)
                    {
                        var form = selectedForm.Item1;

                        var details = (FormSectionDetails)sectionNav.Controls[0];
                        details.ReloadQuestions(form, true);
                        details.OpenTab(selectedTab);

                        if (questionId.HasValue && form.GetQuestions().Any(x => x.Identifier == questionId.Value))
                        {
                            sectionNav.IsSelected = true;
                            ScriptManager.RegisterStartupScript(Page, GetType(), "scrollto_question", $"$(document).ready(function() {{ bankRead.scrollToQuestion('{questionId}'); }});", true);
                        }
                    }
                }
            }

            var pinModel = PinModel.GetModel(BankID);
            if (pinModel != null)
                PinLabel.InnerText = $"{pinModel.FieldAssetNumbers.Count} {(pinModel.FieldAssetNumbers.Count > 1 ? "Pins" : "Pin")}";
        }

        private void BindForm(Form form, bool canWrite)
        {
            AllowEdit = canWrite;

            var bank = form.Specification.Bank;

            SectionsCount = 0;
            FormSectionsNav.ClearItems();

            var hasSections = form.Sections.Count > 0;
            var isArchived = form.Publication.Status == PublicationStatus.Archived;
            var isDrafted = form.Publication.Status == PublicationStatus.Drafted;
            var isPublished = form.Publication.Status == PublicationStatus.Published;
            var isUnpublished = form.Publication.Status == PublicationStatus.Unpublished;
            var isStaticSpec = form.Specification.Type == SpecificationType.Static;

            AddButton.Items["AddFields"].Visible = hasSections;
            AddButton.Items["AddQuestion"].Visible = hasSections && !bank.IsAdvanced;

            ReorderFieldsButton.Visible = isStaticSpec && hasSections && !isPublished && canWrite;

            var summary = ServiceLocator.LearnerAttemptSummarySearch.GetFormSummary(form.Identifier);

            ReportButton.Visible = summary.AttemptGradedCount > 0;

            if (isStaticSpec && hasSections)
            {
                foreach (var section in form.Sections)
                {
                    AddFormSectionItem(out var navItem, out var details);

                    navItem.Title = section.Criterion.Title;

                    details.AllowEdit = canWrite;
                    details.SetInputValues(section);

                    SectionsCount++;
                }
            }
            else
            {
                AddFormSectionItem(out var navItem, out var details);

                navItem.Title = "All Questions";

                details.AllowEdit = canWrite;
                details.SetInputValues(form);

                SectionsCount = 1;
            }

            ActionButton.Items["Publish"].Visible = canWrite && (isDrafted || isUnpublished);
            ActionButton.Items["Unpublish"].Visible = canWrite && isPublished;

            ActionButton.Items["Archive"].Visible = canWrite && bank.IsAdvanced && !isArchived;
            ActionButton.Items["Unarchive"].Visible = canWrite && bank.IsAdvanced && isArchived;

            ActionButton.Items["Prepublish"].Visible = canWrite;
            ActionButton.Items["Duplicate"].Visible = canWrite;

            AddButton.Visible = isStaticSpec && canWrite;
            FormWorkshopButton.Visible = isStaticSpec && canWrite;
        }

        #endregion

        #region Helper methods

        private void AddFormSectionItem(out NavItem navItem, out FormSectionDetails details)
        {
            navItem = new NavItem();
            details = (FormSectionDetails)LoadControl("~/UI/Admin/Assessments/Outlines/Controls/FormSectionDetails.ascx");

            navItem.Controls.Add(details);
            FormSectionsNav.AddItem(navItem);
        }

        private Form GetSelectedForm()
        {
            var formId = FormSelector.ValueAsGuid;
            if (!formId.HasValue)
                ReloadOutline();

            var bank = LoadBank();
            var form = bank.FindForm(formId.Value);

            if (form == null)
                ReloadOutline();

            return form;
        }

        #endregion
    }
}