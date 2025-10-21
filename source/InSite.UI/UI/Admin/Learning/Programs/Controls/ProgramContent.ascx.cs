using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Assets.Contents.Controls.ContentEditor;
using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Records.Programs.Controls
{
    public partial class ProgramContent : UserControl
    {
        #region Properties

        public Guid ProgramIdentifier
        {
            get => (Guid?)ViewState[nameof(ProgramIdentifier)] ?? Guid.Empty;
            set => ViewState[nameof(ProgramIdentifier)] = value;
        }

        private Dictionary<string, int> ContentIdentifiers
        {
            get => (Dictionary<string, int>)ViewState[nameof(ContentIdentifiers)];
            set => ViewState[nameof(ContentIdentifiers)] = value;
        }

        #endregion

        #region Fields

        private ComboBox _language;
        private Repeater _repeater;

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            BindControlsToHandlers(Language, ContentRepeater, ProgramContentSaveButton, ProgramContentCancelButton);
        }

        #endregion

        #region Methods (binding)

        protected void BindModelToControls(TProgram program)
        {
            var content = ServiceLocator.ContentSearch.GetBlock(program.ProgramIdentifier);
            if (content == null)
                content = InsertContent(program);

            BindModelToControls(program, content);
        }

        protected void BindModelToControls(TProgram program, ContentContainer content)
        {
            _language.LoadItems(CurrentSessionState.Identity.Organization.Languages, "TwoLetterISOLanguageName", "EnglishName");

            var sections = GetContentSections().ToList();
            _repeater.DataSource = sections;
            _repeater.DataBind();

            OnLanguageChanged();

            IEnumerable<AssetContentSection> GetContentSections()
            {
                {
                    var section = (AssetContentSection.SingleLine)AssetContentSection.Create(ContentSectionDefault.Title, content.Title.Text);
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
                    section.UploadFolderPath = $"/record/{program.ProgramIdentifier}";
                    yield return section;
                }
            }
        }

        public virtual void BindControlsToModel(TProgram program, ContentContainer content)
        {
            content.Title.Text = GetContentValue(ContentSectionDefault.Title, null);
            content.Summary.Text = GetContentValue(ContentSectionDefault.Summary, null);

            MultilingualString GetContentValue(ContentSectionDefault id1, ContentSectionDefault? id2)
            {
                var index = ContentIdentifiers[id1.GetName()];
                var item = _repeater.Items[index];
                var container = (DynamicControl)item.FindControl("Container");
                var section = (SectionBase)container.GetControl();

                return id2.HasValue ? section.GetValue(id2.Value.GetName()) : section.GetValue();
            }
        }

        protected void BindControlsToHandlers(ComboBox language, Repeater repeater, Common.Web.UI.Button save, Common.Web.UI.Button cancel)
        {
            _language = language;
            _repeater = repeater;

            _language.AutoPostBack = true;
            _language.ValueChanged += (x, y) => OnLanguageChanged();

            _repeater.DataBinding += ContentRepeater_DataBinding;
            _repeater.ItemDataBound += ContentRepeater_ItemDataBound;

            save.Click += (x, y) => Save();
            cancel.NavigateUrl = Request.RawUrl;
        }

        #endregion

        #region Event handlers

        private void ContentRepeater_DataBinding(object sender, EventArgs e)
        {
            ContentIdentifiers = new Dictionary<string, int>();
        }

        private void ContentRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!(e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem))
                return;

            var dataItem = (AssetContentSection)e.Item.DataItem;
            if (ContentIdentifiers.ContainsKey(dataItem.Id))
                throw new Exception("Invalid section ID: " + dataItem.Id);

            var container = (DynamicControl)e.Item.FindControl("Container");
            var section = (SectionBase)container.LoadControl(dataItem.ControlPath);
            section.SetOptions(dataItem);

            ContentIdentifiers.Add(dataItem.Id, e.Item.ItemIndex);
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

        #endregion

        #region Public Methods

        public void LoadData(TProgram program)
        {
            if (program == null) return;

            ProgramIdentifier = program.ProgramIdentifier;
            BindModelToControls(program);
        }

        #endregion

        #region Private Methods 

        private ContentContainer InsertContent(TProgram program)
        {
            var content = new ContentContainer();
            content.Title.Text.Default = program.ProgramName;
            ServiceLocator.ContentStore.SaveContainer(program.OrganizationIdentifier, ContentContainerType.Program, program.ProgramIdentifier, content);

            return ServiceLocator.ContentSearch.GetBlock(program.ProgramIdentifier);
        }

        private void Save()
        {
            if (!Page.IsValid)
                return;

            try
            {
                var program = ProgramSearch.GetProgram(ProgramIdentifier);
                var content = ServiceLocator.ContentSearch.GetBlock(ProgramIdentifier);

                BindControlsToModel(program, content);

                ServiceLocator.ContentStore.SaveContainer(
                    CurrentSessionState.Identity.Organization.OrganizationIdentifier,
                    ContentContainerType.Program,
                    ProgramIdentifier,
                    content);

                if (!string.IsNullOrEmpty(content.Title?.Text.Default))
                    program.ProgramName = content.Title.Text.Default;

                Common.Web.HttpResponseHelper.Redirect($"/ui/admin/learning/programs/outline?id={ProgramIdentifier}");
            }
            catch (ApplicationError apperr)
            {
                ScreenStatus.AddMessage(AlertType.Error, apperr.Message);
            }
        }

        #endregion


    }
}