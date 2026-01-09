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
    public partial class UnitSetup : BaseUserControl
    {
        public Guid CourseIdentifier
        {
            get => (Guid)ViewState[nameof(CourseIdentifier)];
            set => ViewState[nameof(CourseIdentifier)] = value;
        }

        public Guid UnitIdentifier
        {
            get => (Guid)ViewState[nameof(UnitIdentifier)];
            set => ViewState[nameof(UnitIdentifier)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Language.AutoPostBack = true;
            Language.ValueChanged += Language_ValueChanged;

            ContentRepeater.DataBinding += ContentRepeater_DataBinding;
            ContentRepeater.ItemDataBound += ContentRepeater_ItemDataBound;

            UnitSaveButton.Click += UnitSaveButton_Click;
            UnitCancelButton.NavigateUrl = Request.RawUrl;
        }

        private void UnitSaveButton_Click(object sender, EventArgs e)
        {
            var unit = ServiceLocator.CourseSearch.GetUnit(UnitIdentifier);
            var content = ServiceLocator.ContentSearch.GetBlock(UnitIdentifier);

            BindControlsToModel(unit, content);

            Course2Store.UpdateUnit(unit, content);

            HttpResponseHelper.Redirect(UrlParser.BuildRelativeUrl(Request.RawUrl, "panel", "unit"));
        }

        public void BindModelToControls(QUnit unit)
        {
            Language.LoadItems(
                CurrentSessionState.Identity.Organization.Languages,
                "TwoLetterISOLanguageName",
                "EnglishName");

            UnitIdentifier = unit.UnitIdentifier;
            CourseIdentifier = unit.CourseIdentifier;

            UnitName.Text = unit.UnitName;
            UnitCode.Text = unit.UnitCode;
            UnitIsAdaptive.Checked = unit.UnitIsAdaptive;
            UnitThumbprint.Text = unit.UnitIdentifier.ToString();
            UnitAsset.Text = unit.UnitAsset.ToString();

            PrerequisiteList.BindModelToControls(CourseIdentifier, PrerequisiteObjectType.Unit, UnitIdentifier, unit.PrerequisiteDeterminer, UnitSaveButton.ValidationGroup);

            ContentRepeater.DataSource = GetContentSections();
            ContentRepeater.DataBind();

            OnLanguageChanged();

            PrivacySettingsGroups.LoadData(UnitIdentifier, "Unit");
        }

        public void BindControlsToModel(QUnit unit, ContentContainer content)
        {
            PrerequisiteList.SaveChanges();
            unit.PrerequisiteDeterminer = PrerequisiteList.GetPrerequisiteDeterminer();

            unit.UnitName = UnitName.Text;
            unit.UnitCode = UnitCode.Text;
            unit.UnitIsAdaptive = UnitIsAdaptive.Checked;

            BindControlsToModelForContent(content, UnitName.Text);
        }

        public void BindControlsToModelForContent(ContentContainer content, string unitName)
        {
            content.Title.Text = DetailsTab.IsSelected
                ? new MultilingualString { Default = unitName }
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
                var from = ServiceLocator.ContentSearch.SelectContainerByLanguage(UnitIdentifier, "en");
                var to = ServiceLocator.ContentSearch.SelectContainerByLanguage(UnitIdentifier, lang);
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
            var content = ServiceLocator.ContentSearch.GetBlock(UnitIdentifier);

            {
                var section = (AssetContentSection.SingleLine)AssetContentSection.Create(ContentSectionDefault.Title, content.Title.Text);
                section.Title = "Title";
                section.Label = "Title";
                yield return section;
            }

            {
                var section = (AssetContentSection.MarkdownAndHtml)AssetContentSection.Create(ContentSectionDefault.Body, content.Body.Text, content.Body.Html);
                section.Title = "Body";
                section.MarkdownLabel = "Body Text (Markdown)";
                section.HtmlLabel = "Body (Html)";
                section.AllowUpload = true;
                section.UploadFolderPath = $"/courses/{CourseIdentifier}/units/{UnitIdentifier}";
                section.EnableHtmlBuilder = true;
                yield return section;
            }
        }

        #endregion
    }
}