using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Assets.Contents.Controls.ContentEditor;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Records.Programs
{
    public partial class ModifyContent : AdminBasePage, IHasParentLinkParameters
    {
        public const string NavigateUrl = "/ui/admin/learning/programs/modify-content";

        public static string GetNavigateUrl(Guid programId) => NavigateUrl + "?id=" + programId;

        public static void Redirect(Guid programId) => HttpResponseHelper.Redirect(GetNavigateUrl(programId));

        private Guid? ProgramId => Guid.TryParse(Request["id"], out var result) ? result : (Guid?)null;

        private Dictionary<string, int> ContentIdentifiers
        {
            get => (Dictionary<string, int>)ViewState[nameof(ContentIdentifiers)];
            set => ViewState[nameof(ContentIdentifiers)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ContentRepeater.DataBinding += ContentRepeater_DataBinding;
            ContentRepeater.ItemDataBound += ContentRepeater_ItemDataBound;

            SaveButton.Click += (s, a) => Save();
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

            ContentIdentifiers.Add(dataItem.Id, e.Item.ItemIndex);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        #region Methods (open)

        private void Open()
        {
            var program = ProgramId.HasValue ? ProgramSearch.GetProgram(ProgramId.Value) : null;
            if (program == null)
                Search.Redirect();

            PageHelper.AutoBindHeader(this, null, program.ProgramName);

            var content = ServiceLocator.ContentSearch.GetBlock(program.ProgramIdentifier);
            if (content == null)
                content = InsertContent(program);

            BindModelToControls(program, content);

            CancelButton.NavigateUrl = Outline.GetNavigateUrl(ProgramId.Value, tab: "content");
        }

        protected void BindModelToControls(TProgram program, ContentContainer content)
        {
            ContentRepeater.DataSource = GetContentSections();
            ContentRepeater.DataBind();

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

        #endregion

        #region Methods (save)

        private void Save()
        {
            if (!Page.IsValid)
                return;

            var program = ProgramId.HasValue ? ProgramSearch.GetProgram(ProgramId.Value) : null;
            if (program == null)
                return;

            try
            {
                var content = ServiceLocator.ContentSearch.GetBlock(program.ProgramIdentifier);

                BindControlsToModel(program, content);

                ServiceLocator.ContentStore.SaveContainer(
                    CurrentSessionState.Identity.Organization.OrganizationIdentifier,
                    ContentContainerType.Program,
                    program.ProgramIdentifier,
                    content);

                if (!string.IsNullOrEmpty(content.Title?.Text.Default))
                    program.ProgramName = content.Title.Text.Default;

                Outline.Redirect(program.ProgramIdentifier, tab: "content");
            }
            catch (ApplicationError apperr)
            {
                EditorStatus.AddMessage(AlertType.Error, apperr.Message);
            }
        }

        private ContentContainer InsertContent(TProgram program)
        {
            var content = new ContentContainer();
            content.Title.Text.Default = program.ProgramName;
            ServiceLocator.ContentStore.SaveContainer(program.OrganizationIdentifier, ContentContainerType.Program, program.ProgramIdentifier, content);

            return ServiceLocator.ContentSearch.GetBlock(program.ProgramIdentifier);
        }

        public virtual void BindControlsToModel(TProgram program, ContentContainer content)
        {
            content.Title.Text = GetContentValue(ContentSectionDefault.Title, null);
            content.Summary.Text = GetContentValue(ContentSectionDefault.Summary, null);

            MultilingualString GetContentValue(ContentSectionDefault id1, ContentSectionDefault? id2)
            {
                var index = ContentIdentifiers[id1.GetName()];
                var item = ContentRepeater.Items[index];
                var container = (DynamicControl)item.FindControl("Container");
                var section = (SectionBase)container.GetControl();

                return id2.HasValue ? section.GetValue(id2.Value.GetName()) : section.GetValue();
            }
        }

        #endregion

        #region IHasParentLinkParameters

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={ProgramId}"
                : null;
        }

        #endregion
    }
}