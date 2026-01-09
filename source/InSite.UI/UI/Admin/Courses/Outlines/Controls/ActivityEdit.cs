using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Admin.Assets.Contents.Controls.ContentEditor;
using InSite.Application.Courses.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Courses.Outlines.Controls
{
    public abstract class BaseActivityEdit : BaseUserControl, IActivityEdit
    {
        public Guid ActivityIdentifier
        {
            get => (Guid?)ViewState[nameof(ActivityIdentifier)] ?? Guid.Empty;
            set => ViewState[nameof(ActivityIdentifier)] = value;
        }

        public Guid CourseIdentifier
        {
            get => (Guid?)ViewState[nameof(CourseIdentifier)] ?? Guid.Empty;
            set => ViewState[nameof(CourseIdentifier)] = value;
        }

        private Dictionary<string, int> ContentIdentifiers
        {
            get => (Dictionary<string, int>)ViewState[nameof(ContentIdentifiers)];
            set => ViewState[nameof(ContentIdentifiers)] = value;
        }

        private ActivitySetupTab _setup;
        private ComboBox _language;
        private Repeater _repeater;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            BindModelToControls();
        }

        protected void BindModelToControls()
        {
            var activity = Persistence.CourseSearch.SelectActivity(ActivityIdentifier);
            BindModelToControls(activity);

            var content = ServiceLocator.ContentSearch.GetBlock(activity.ActivityIdentifier);
            BindModelsToControls(activity, content);
        }

        protected void BindModelsToControls(QActivity activity, Shift.Common.ContentContainer content)
        {
            _setup.BindModelToControls(activity, "CourseConfig");

            _language.LoadItems(CurrentSessionState.Identity.Organization.Languages, "TwoLetterISOLanguageName", "EnglishName");

            var title = !content.Title.Text.IsEmpty
                ? content.Title.Text
                : new MultilingualString(activity.ActivityName);

            var sections = GetContentSections().ToList();
            _repeater.DataSource = sections;
            _repeater.DataBind();

            OnLanguageChanged();

            IEnumerable<AssetContentSection> GetContentSections()
            {
                {
                    var section = (AssetContentSection.SingleLine)AssetContentSection.Create(ContentSectionDefault.Title, title);
                    section.Title = "Title";
                    section.Label = "Title";
                    section.IsRequired = true;
                    yield return section;
                }

                {
                    var section = (AssetContentSection.Markdown)AssetContentSection.Create(ContentSectionDefault.Summary, content.Summary.Text);
                    section.Title = "Summary";
                    section.Label = "Summary";
                    section.AllowUpload = true;
                    section.UploadFolderPath = $"/assets/{activity.ActivityAsset}";
                    yield return section;
                }

                {
                    var section = (AssetContentSection.MarkdownAndHtml)AssetContentSection.Create(ContentSectionDefault.Body, content.Body.Text, content.Body.Html);
                    section.Title = "Body";
                    section.MarkdownLabel = "Body Text (Markdown)";
                    section.HtmlLabel = "Body HTML";
                    section.AllowUpload = true;
                    section.UploadFolderPath = $"/assets/{activity.ActivityAsset}";
                    section.EnableHtmlBuilder = true;
                    yield return section;
                }
            }
        }

        public void BindControlsToModel(QActivity activity, ContentContainer content, string activityName)
        {
            _setup.BindControlsToModel(activity);

            content.Title.Text = ((NavItem)_setup.Parent).IsSelected
                ? new MultilingualString { Default = activityName }
                : GetContentValue(ContentSectionDefault.Title, null);

            content.Summary.Text = GetContentValue(ContentSectionDefault.Summary, null);
            content.Body.Text = GetContentValue(ContentSectionDefault.Body, ContentSectionDefault.BodyText);
            content.Body.Html = GetContentValue(ContentSectionDefault.Body, ContentSectionDefault.BodyHtml);

            MultilingualString GetContentValue(ContentSectionDefault id1, ContentSectionDefault? id2)
            {
                var index = ContentIdentifiers[id1.GetName()];
                var item = _repeater.Items[index];
                var container = (DynamicControl)item.FindControl("Container");
                var section = (SectionBase)container.GetControl();

                return id2.HasValue ? section.GetValue(id2.Value.GetName()) : section.GetValue();
            }
        }

        protected void BindControlsToHandlers(ActivitySetupTab setup, ComboBox language, Repeater repeater, Common.Web.UI.Button save, Common.Web.UI.Button cancel)
        {
            _setup = setup;

            _language = language;
            _repeater = repeater;

            _language.AutoPostBack = true;
            _language.ValueChanged += (x, y) => OnLanguageChanged();

            _repeater.DataBinding += ContentRepeater_DataBinding;
            _repeater.ItemDataBound += ContentRepeater_ItemDataBound;

            save.Click += (x, y) => Save();
            cancel.NavigateUrl = Request.RawUrl;
        }

        protected void OnLanguageChanged()
        {
            var lang = _language.Value.IfNullOrEmpty("en");

            foreach (RepeaterItem item in _repeater.Items)
            {
                var container = (DynamicControl)item.FindControl("Container");
                var section = (SectionBase)container.GetControl();
                if (section != null)
                    section.SetLanguage(lang);
            }
        }

        private void ContentRepeater_DataBinding(object sender, EventArgs e)
        {
            ContentIdentifiers = new Dictionary<string, int>();
        }

        private void ContentRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var dataItem = (AssetContentSection)e.Item.DataItem;
            if (ContentIdentifiers.ContainsKey(dataItem.Id))
                throw new Exception("Invalid section ID: " + dataItem.Id);

            var container = (DynamicControl)e.Item.FindControl("Container");
            var section = (SectionBase)container.LoadControl(dataItem.ControlPath);
            section.SetOptions(dataItem);
            section.SetValidationGroup("CourseConfig");

            ContentIdentifiers.Add(dataItem.Id, e.Item.ItemIndex);
        }

        private void Save()
        {
            if (!Page.IsValid)
                return;

            try
            {
                var activity = ServiceLocator.CourseSearch.GetActivity(ActivityIdentifier);
                var content = ServiceLocator.ContentSearch.GetBlock(ActivityIdentifier);

                BindControlsToModel(activity, content, _setup.ActivityNameInput);
                BindControlsToModel(activity);

                if (!string.IsNullOrEmpty(content.Title?.Text.Default))
                    activity.ActivityName = content.Title.Text.Default.MaxLength(200, true);

                Persistence.Course2Store.UpdateActivity(CourseIdentifier, activity, content);

                Common.Web.HttpResponseHelper.Redirect($"/ui/admin/courses/manage?course={CourseIdentifier}&activity={ActivityIdentifier}");
            }
            catch (ApplicationError apperr)
            {
                OnAlert(AlertType.Error, apperr.Message);
            }
        }

        protected abstract void BindControlsToModel(QActivity activity);
        protected abstract void BindModelToControls(QActivity activity);
        protected abstract void OnAlert(AlertType type, string message);
    }
}