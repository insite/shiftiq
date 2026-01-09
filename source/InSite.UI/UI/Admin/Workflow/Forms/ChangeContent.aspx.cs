using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

using InSite.Admin.Assets.Contents.Controls.ContentEditor;
using InSite.Application.Surveys.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;
using Shift.Sdk.UI;

using ChangeSurveyFormContentCommand = InSite.Application.Surveys.Write.ChangeSurveyFormContent;

namespace InSite.Admin.Workflow.Forms
{
    public partial class ChangeContent : AdminBasePage, IHasParentLinkParameters
    {
        #region Constants

        private const string InstructionsId = "Instructions";

        #endregion

        #region Properties

        private Guid? SurveyIdentifier =>
            Guid.TryParse(Request["form"], out var value) ? value : (Guid?)null;

        private string Tab => Request["tab"];

        private string[] Fields => Domain.Surveys.Forms.SurveyForm.ContentLabels.Select(x => x.Name).ToArray();

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ContentEditor.NeedRedirectParameters += ContentEditor_NeedRedirectParameters;

            SaveButton.Click += SaveButton_Click;
            CancelButton.Click += CancelButton_Click;
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
        #endregion

        #region Event handlers

        private void ContentEditor_NeedRedirectParameters(object sender, RedirectParametersArgs args)
        {
            args.IsCancelled = !Save();
            if (!args.IsCancelled)
                args.Parameters.Add("form", SurveyIdentifier.ToString());
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (Save())
                RedirectToParent();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            RedirectToParent();
        }

        #endregion

        #region Database operations

        private void Open()
        {
            var survey = SurveyIdentifier.HasValue ? ServiceLocator.SurveySearch.GetSurveyForm(SurveyIdentifier.Value) : null;
            if (survey == null || survey.OrganizationIdentifier != Organization.Identifier || survey.SurveyFormLocked.HasValue)
                RedirectToSearch();

            if (Fields.Length == 0)
                RedirectToParent();

            SetInputValues(survey);
        }

        private bool Save()
        {
            if (!Page.IsValid)
                return false;

            var survey = ServiceLocator.SurveySearch.GetSurveyForm(SurveyIdentifier.Value);
            if (survey == null)
                return true;

            var content = new Shift.Common.ContentContainer();

            GetInputValues(content);

            ServiceLocator.SendCommand(new ChangeSurveyFormContentCommand(survey.SurveyFormIdentifier, content));

            return true;
        }

        #endregion

        #region Settings/getting input values

        private void SetInputValues(QSurveyForm survey)
        {
            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{survey.SurveyFormName} <span class='form-text'>Form #{survey.AssetNumber}</span>");

            if (!ContentEditor.IsEmpty)
                return;

            var content = ServiceLocator.ContentSearch.GetBlock(survey.SurveyFormIdentifier, labels: Fields);

            var (nonInstructionEditors, instructionEditors) = SplitEditors();

            AddNonInstructionEditors(survey, content, nonInstructionEditors);
            AddInstructionEditors(survey, content, instructionEditors);

            ContentEditor.SetLanguage(survey.SurveyFormLanguage ?? CurrentSessionState.Identity.Language);
            ContentEditor.OpenTab(Tab);
        }

        private void AddNonInstructionEditors(QSurveyForm survey, ContentContainer content, List<string> fields)
        {
            foreach (var name in fields)
            {
                if (name == "Title")
                {
                    ContentEditor.Add(new AssetContentSection.SingleLine(name)
                    {
                        Title = name,
                        Value = content.Title.Text
                    });
                }
                else
                {
                    ContentEditor.Add(CreateMarkdownAndHtml(survey, content, name, name));
                }
            }
        }

        private void AddInstructionEditors(QSurveyForm survey, ContentContainer content, List<string> fields)
        {
            var options = fields
                .Select(x => CreateMarkdownAndHtml(survey, content, x, GetLabelTitle(x)))
                .ToList();

            var tileList = new AssetContentSection.TileList(InstructionsId)
            {
                Title = InstructionsId,
                Size = TileSize.Full,
                Options = options
            };

            ContentEditor.Add(tileList);
        }

        private AssetContentSection.MarkdownAndHtml CreateMarkdownAndHtml(QSurveyForm survey, ContentContainer content, string field, string title)
        {
            var item = content[field];

            return new AssetContentSection.MarkdownAndHtml(field)
            {
                Title = title,
                HtmlValue = item.Html,
                MarkdownValue = item.Text,
                IsMultiValue = true,
                AllowUpload = true,
                UploadFolderPath = $"/forms/{survey.AssetNumber}"
            };
        }

        private (List<string> nonInstructionEditors, List<string> instructionEditors) SplitEditors()
        {
            var nonInstructionEditors = new List<string>();
            var instructionEditors = new List<string>();

            foreach (var name in Fields)
            {
                if (name.EndsWith("Instructions"))
                    instructionEditors.Add(name);
                else
                    nonInstructionEditors.Add(name);
            }

            return (nonInstructionEditors, instructionEditors);
        }

        private void GetInputValues(ContentContainer data)
        {
            var (nonInstructionEditors, instructionEditors) = SplitEditors();

            GetNonInstructionInputValues(data, nonInstructionEditors);
            GetInstructionInputValues(data, instructionEditors);
        }

        private void GetNonInstructionInputValues(ContentContainer data, List<string> fields)
        {
            foreach (var name in fields)
            {
                var item = data[name];

                if (name == "Title")
                {
                    item.Text = ContentEditor.GetValue(name);
                }
                else
                {
                    item.Html = ContentEditor.GetValue(name, ContentSectionDefault.BodyHtml);
                    item.Text = ContentEditor.GetValue(name, ContentSectionDefault.BodyText);
                }
            }
        }

        private void GetInstructionInputValues(ContentContainer data, List<string> fields)
        {
            var section = (SectionTiles)ContentEditor.GetSection(InstructionsId);

            foreach (var field in fields)
            {
                var item = data[field];

                item.Html = section.GetValue(field, ContentSectionDefault.BodyHtml);
                item.Text = section.GetValue(field, ContentSectionDefault.BodyText);
            }
        }

        #endregion

        #region Helper methods

        private void RedirectToSearch() =>
            HttpResponseHelper.Redirect($"/ui/admin/workflow/forms/search", true);

        private void RedirectToParent()
        {
            var selectedTab = ContentEditor.GetCurrentTab();
            var returnQuery = selectedTab.HasValue() && selectedTab != "Title"
                ? $"panel=content&tab={HttpUtility.UrlEncode(selectedTab)}"
                : null;
            var returnUrl = new ReturnUrl();

            var url = returnUrl.GetReturnUrl(returnQuery);
            if (!url.HasValue())
            {
                url = $"/ui/admin/workflow/forms/outline?form={SurveyIdentifier}";

                if (returnQuery.HasValue())
                    url += "&" + returnQuery;
            }

            HttpResponseHelper.Redirect(url, true);
        }

        public static string GetLabelTitle(string labelName)
        {
            return string.Equals(labelName, "Completed Instructions", StringComparison.OrdinalIgnoreCase)
                ? "Message on Submission Review"
                : labelName;
        }

        #endregion

        #region IHasParentLinkParameters

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"form={SurveyIdentifier}"
                : null;
        }

        #endregion
    }
}
