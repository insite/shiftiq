using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Admin.Assets.Contents.Controls.ContentEditor;
using InSite.Application.Courses.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Courses.Outlines.Controls
{
    public partial class ModuleSetup : BaseUserControl
    {
        public Guid CourseIdentifier
        {
            get => (Guid)ViewState[nameof(CourseIdentifier)];
            set => ViewState[nameof(CourseIdentifier)] = value;
        }

        public Guid ModuleIdentifier
        {
            get => (Guid)ViewState[nameof(ModuleIdentifier)];
            set => ViewState[nameof(ModuleIdentifier)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Language.AutoPostBack = true;
            Language.ValueChanged += Language_ValueChanged;

            ContentRepeater.DataBinding += ContentRepeater_DataBinding;
            ContentRepeater.ItemDataBound += ContentRepeater_ItemDataBound;

            ModuleSaveButton.Click += ModuleSaveButton_Click;
            ModuleCancelButton.NavigateUrl = Request.RawUrl;
        }

        private void ModuleSaveButton_Click(object sender, EventArgs e)
        {
            var module = ServiceLocator.CourseSearch.GetModule(ModuleIdentifier);
            var content = ServiceLocator.ContentSearch.GetBlock(ModuleIdentifier);

            BindControlsToModel(module, content);

            Course2Store.UpdateModule(CourseIdentifier, module, content);

            HttpResponseHelper.Redirect(UrlParser.BuildRelativeUrl(Request.RawUrl, "panel", "module"));
        }

        public void BindModelToControls(QUnit unit, QModule module)
        {
            Language.LoadItems(CurrentSessionState.Identity.Organization.Languages, "TwoLetterISOLanguageName", "EnglishName");

            ModuleIdentifier = module.ModuleIdentifier;
            CourseIdentifier = unit.CourseIdentifier;

            ModuleName.Text = module.ModuleName;
            ModuleCode.Text = module.ModuleCode;
            ModuleIsAdaptive.Checked = module.ModuleIsAdaptive;
            ModuleThumbprint.Text = module.ModuleIdentifier.ToString();
            ModuleAsset.Text = module.ModuleAsset.ToString();

            PrerequisiteList.BindModelToControls(CourseIdentifier, PrerequisiteObjectType.Module, ModuleIdentifier, module.PrerequisiteDeterminer, ModuleSaveButton.ValidationGroup);

            ContentRepeater.DataSource = GetContentSections();
            ContentRepeater.DataBind();

            OnLanguageChanged();

            PrivacySettingsGroups.LoadData(ModuleIdentifier, "Module");
        }

        public void BindControlsToModel(QModule module, ContentContainer content)
        {
            PrerequisiteList.SaveChanges();
            module.PrerequisiteDeterminer = PrerequisiteList.GetPrerequisiteDeterminer();

            module.ModuleName = ModuleName.Text;
            module.ModuleCode = ModuleCode.Text;
            module.ModuleIsAdaptive = ModuleIsAdaptive.Checked;

            if (string.IsNullOrWhiteSpace(module.ModuleName))
                module.ModuleName = "-";

            BindControlsToModelForContent(content, module.ModuleName);
        }

        public void BindControlsToModelForContent(ContentContainer content, string moduleName)
        {
            content.Title.Text = DetailsTab.IsSelected
                ? new MultilingualString { Default = moduleName }
                : GetContentValue(ContentSectionDefault.Title, null);

            content.Body.Text = GetContentValue(ContentSectionDefault.Body, ContentSectionDefault.BodyText);
            content.Body.Html = GetContentValue(ContentSectionDefault.Body, ContentSectionDefault.BodyHtml);

            MultilingualString GetContentValue(ContentSectionDefault id1, ContentSectionDefault? id2)
            {
                var index = ContentID[id1.GetName()];
                var item = ContentRepeater.Items[index];
                var container = (DynamicControl)item.FindControl("Container");
                var section = (SectionBase)container.GetControl();

                return id2.HasValue ? section.GetValue(id2.Value.GetName()) : section.GetValue();
            }
        }

        #region Content

        private Dictionary<string, int> ContentID
        {
            get => (Dictionary<string, int>)ViewState[nameof(ContentID)];
            set => ViewState[nameof(ContentID)] = value;
        }

        private void ContentRepeater_DataBinding(object sender, EventArgs e)
        {
            ContentID = new Dictionary<string, int>();
        }

        private void ContentRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var dataItem = (AssetContentSection)e.Item.DataItem;
            if (ContentID.ContainsKey(dataItem.Id))
                throw ApplicationError.Create("Invalid section ID: " + dataItem.Id);

            var container = (DynamicControl)e.Item.FindControl("Container");
            var section = (SectionBase)container.LoadControl(dataItem.ControlPath);
            section.SetOptions(dataItem);

            ContentID.Add(dataItem.Id, e.Item.ItemIndex);
        }

        private void Language_ValueChanged(object sender, EventArgs e) => OnLanguageChanged();

        private void OnLanguageChanged()
        {
            bool rebind = false;
            var lang = Language.Value.IfNullOrEmpty("en");

            if (lang != "en")
            {
                var from = ServiceLocator.ContentSearch.SelectContainerByLanguage(ModuleIdentifier, "en");
                var to = ServiceLocator.ContentSearch.SelectContainerByLanguage(ModuleIdentifier, lang);
                foreach (var en in from)
                {
                    var tx = to?.FirstOrDefault(x => x.ContentLanguage == lang && x.ContentLabel == en.ContentLabel);
                    if (tx?.ContentText == null)
                    {
                        rebind = true;
                        var translation = ((IHasTranslator)Page).Translator.Translate("en", lang, en.ContentText);
                        ServiceLocator.ContentStore.Save(en.ContainerType, en.ContainerIdentifier, en.ContentLabel, translation, lang, en.OrganizationIdentifier);
                    }
                }
                if (rebind)
                {
                    ContentRepeater.DataSource = GetContentSections();
                    ContentRepeater.DataBind();
                }
            }

            foreach (RepeaterItem item in ContentRepeater.Items)
            {
                var container = (DynamicControl)item.FindControl("Container");
                var section = (SectionBase)container.GetControl();

                section.SetLanguage(lang);
            }
        }

        private IEnumerable<AssetContentSection> GetContentSections()
        {
            var content = ServiceLocator.ContentSearch.GetBlock(ModuleIdentifier);

            {
                var section = (AssetContentSection.SingleLine)AssetContentSection.Create(ContentSectionDefault.Title, content.Title.Text);
                section.Title = "Title";
                section.Label = "Title";
                section.IsRequired = false;
                yield return section;
            }

            {
                var section = (AssetContentSection.MarkdownAndHtml)AssetContentSection.Create(ContentSectionDefault.Body, content.Body.Text, content.Body.Html);
                section.Title = "Body";
                section.MarkdownLabel = "Body Text (Markdown)";
                section.HtmlLabel = "Body (Html)";
                section.AllowUpload = true;
                section.UploadFolderPath = $"/courses/{CourseIdentifier}/modules/{ModuleIdentifier}";
                section.EnableHtmlBuilder = true;
                yield return section;
            }
        }

        #endregion
    }
}