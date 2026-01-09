using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Admin.Assets.Contents.Controls.ContentEditor;
using InSite.Admin.Sites.Utilities;
using InSite.Application.Sites.Read;
using InSite.Application.Sites.Write;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Sdk.UI;
using Shift.Common.Events;

namespace InSite.Admin.Sites.Pages.Controls
{
    public partial class ContentBlocksManager : SectionBase
    {
        #region Classes

        public sealed class SectionSettings : AssetContentSection.ContentSectionViewer
        {
            public QPage ParentPage { get; set; }
            public bool ViewOnly { get; set; }

            public override string ControlPath => "~/UI/Admin/Sites/Pages/Controls/ContentBlocksManager.ascx";

            public SectionSettings(string id)
                : base(id)
            {

            }

            public SectionSettings(string id, bool viewOnly)
            : base(id)
            {
                this.ViewOnly = viewOnly;
            }
        }

        [Serializable]
        private class ParentInfo
        {
            public Guid PageId { get; }
            public Guid? SiteId { get; }

            public ParentInfo(QPage page)
            {
                PageId = page.PageIdentifier;
                SiteId = page.SiteIdentifier;
            }
        }

        [Serializable]
        private class SectionInfo
        {
            public Guid ID { get; }

            public SectionInfo(QPage page)
            {
                if (page == null)
                    return;

                ID = page.PageIdentifier;
            }
        }

        #endregion

        #region Events

        public event AlertHandler StatusUpdated;
        private void OnAlert(AlertArgs args) => StatusUpdated?.Invoke(this, args);

        public event EventHandler BlockInserted;
        protected void OnBlockInserted() => BlockInserted?.Invoke(this, EventArgs.Empty);

        public event EventHandler BlockDeleted;
        protected void OnBlockDeleted() => BlockDeleted?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Properties

        private ParentInfo ParentPage
        {
            get => (ParentInfo)ViewState[nameof(ParentPage)];
            set => ViewState[nameof(ParentPage)] = value;
        }

        private List<SectionInfo> SectionsData
        {
            get => (List<SectionInfo>)ViewState[nameof(SectionsData)];
            set => ViewState[nameof(SectionsData)] = value;
        }

        private int SectionsCount
        {
            get => (int)(ViewState[nameof(SectionsCount)] ?? 0);
            set => ViewState[nameof(SectionsCount)] = value;
        }

        private string ValidationGroup
        {
            get => (string)ViewState[nameof(ValidationGroup)];
            set => ViewState[nameof(ValidationGroup)] = value;
        }

        #endregion

        #region Initialization and Loading

        protected override void CreateChildControls()
        {
            if (Navigation.ItemsCount == 0)
            {
                for (var i = 0; i < SectionsCount; i++)
                    AddNavItem(out _, out _);
            }

            base.CreateChildControls();
        }

        public string GetNavTitle()
        {
            if (Navigation.ItemsCount == 0)
                return "";

            foreach (var item in Navigation.GetItems())
            {
                if (item.IsSelected)
                    return item.Title;
            }

            return "";
        }

        protected override void OnPreRender(EventArgs e)
        {
            EnumerateSectionItems((item, editor, index) =>
            {
                editor.ValidationGroup = ValidationGroup;

                return true;
            });

            base.OnPreRender(e);
        }

        #endregion

        #region Event handlers

        private void EditorContainer_ControlAdded(object sender, EventArgs e)
        {
            var container = (DynamicControl)sender;
            var editor = (ContentBlocksEditorBase)container.GetControl();

            InitSectionControl(editor);
        }

        private void BlockEditor_Delete(object sender, EventArgs e)
        {
            var editor = (ContentBlocksEditor)sender;
            var container = (DynamicControl)editor.NamingContainer;
            var item = (NavItem)container.BindingContainer;
            var index = Navigation.GetIndex(item);
            var curSection = SectionsData[index];

            var nextSection = SectionsData[index + 1];
            if (nextSection.ID == Guid.Empty && index > 0)
                nextSection = SectionsData[index - 1];

            var commands = new PageCommandGenerator().
                DeletePageWithChildren(ServiceLocator.PageSearch.GetPageChildrenIds(curSection.ID));

            foreach (var command in commands)
                ServiceLocator.SendCommand(command);

            LoadData(true);
            OpenSectionItem(nextSection.ID);

            OnBlockDeleted();
        }

        private void BlockEditor_Insert(object sender, EventArgs e)
        {
            var creator = (ContentBlocksCreator)sender;

            var entity = SiteHelper.CreatePage("Block");

            entity.SiteIdentifier = ParentPage.SiteId;
            entity.ParentPageIdentifier = ParentPage.PageId;

            creator.GetInputValues(entity);

            var commands = new PageCommandGenerator().GetCommands(entity);

            foreach (var command in commands)
                ServiceLocator.SendCommand(command);

            LoadData(true);
            OpenSectionItem(entity.PageIdentifier);

            OnBlockInserted();
        }

        private void Editor_StatusUpdated(object sender, AlertArgs args)
        {
            OnAlert(args);
        }

        #endregion

        #region Methods (data)

        public void Save()
        {
            var entities = ServiceLocator.PageSearch
                .Bind(
                    x => x,
                    x => x.ParentPageIdentifier == ParentPage.PageId
                      && x.PageType == "Block", null, "Sequence")
                .ToDictionary(x => x.PageIdentifier);

            EnumerateSectionItems((item, ctrl, index) =>
            {
                if (!(ctrl is ContentBlocksEditor editor))
                    return false;

                var section = SectionsData[index];
                if (!entities.TryGetValue(section.ID, out var entity))
                    return true;

                var content = new Shift.Common.ContentContainer { IsLoaded = true };
                editor.GetInputValues(entity, content);

                var commands = new PageCommandGenerator().
                GetPageBlockCommands(
                    entity
                );

                foreach (var command in commands)
                    ServiceLocator.SendCommand(command);

                ServiceLocator.SendCommand(new InSite.Application.Pages.Write.ChangePageContent(entity.PageIdentifier, content));

                return true;
            });
        }

        private void LoadData(bool preserveState, bool viewOnly = false)
        {
            var entities = ServiceLocator.PageSearch.Bind(x => x, x => x.ParentPageIdentifier == ParentPage.PageId && x.PageType == "Block" && x.ContentControl != null, null, "Sequence");
            var titles = entities.Select(x => x.Title).ToArray();
            var contents = new Dictionary<Guid, Shift.Common.ContentContainer>();

            if (preserveState)
            {
                EnumerateSectionItems((item, ctrl, index) =>
                {
                    if (!(ctrl is ContentBlocksEditor editor))
                        return false;

                    var section = SectionsData[index];
                    var entity = entities.FirstOrDefault(x => x.PageIdentifier == section.ID);
                    var content = new Shift.Common.ContentContainer();

                    editor.GetInputValues(entity, content);

                    contents.Add(section.ID, content);

                    return true;
                });
            }

            {
                var containers = ServiceLocator.ContentSearch
                    .SelectContainer(entities.Select(x => x.PageIdentifier).Where(x => !contents.ContainsKey(x)))
                    .GroupBy(x => x.ContainerIdentifier)
                    .ToList();

                foreach (var container in containers)
                {
                    var records = container.ToList();
                    var data = ServiceLocator.ContentSearch.GetBlock(records);
                    contents.Add(container.Key, data);
                }
            }

            SectionsCount = 0;
            Navigation.ClearItems();
            SectionsData = new List<SectionInfo>();

            string tab = (Request["tab"] != null ? Request["tab"].ToLower() : "");
            string nav = (Request["nav"] != null ? Request["nav"].ToLower() : "");

            for (var i = 0; i < entities.Length; i++)
            {
                var e = entities[i];

                try
                {
                    var block = InSite.UI.Layout.Common.Contents.ControlPath.GetPageBlock(e.ContentControl);

                    if (!contents.TryGetValue(e.PageIdentifier, out var content))
                        content = new Shift.Common.ContentContainer();

                    var title = StringHelper.FirstValue(titles[i], block.Title);
                    if (viewOnly)
                    {
                        var viewer = (ContentBlocksViewer)AddSectionControl(title,
                            "ContentBlocksViewer", tab, nav);
                        viewer.SetInputValues(e, content, $"/ui/admin/sites/pages/content?id=" +
                            $"{ParentPage.PageId}&tab=pageblocks&nav={title.ToLower()}");

                    }
                    else
                    {
                        var editor = (ContentBlocksEditor)AddSectionControl(title, "ContentBlocksEditor",
                            tab, nav);
                        editor.SetInputValues(e, content);
                    }
                    SectionsData.Add(new SectionInfo(e));
                }
                catch (Exception ex)
                {
                    DangerPanel.InnerText = ex.Message;
                    DangerPanel.Visible = true;
                    return;
                }
            }

            if (!viewOnly)
            {
                var creator = (ContentBlocksCreator)AddSectionControl("New", "ContentBlocksCreator", tab, nav);
                creator.SetDefaultInputValues();
            }
            else if (entities.Length == 0)
            {
                InfoPanel.InnerText = "(No Content Blocks)";
                InfoPanel.Visible = true;
            }

            SectionsData.Add(new SectionInfo(null));
        }

        #endregion

        #region Methods (SectionBase)

        public override void SetOptions(AssetContentSection options)
        {
            if (options is SectionSettings data)
            {
                ParentPage = new ParentInfo(data.ParentPage);

                LoadData(false, data.ViewOnly);
            }
            else
            {
                throw new NotImplementedException("Section type: " + options.GetType().FullName);
            }
        }

        public override void SetValidationGroup(string groupName)
        {
            ValidationGroup = groupName;
        }

        public override MultilingualString GetValue() => throw new NotImplementedException();

        public override MultilingualString GetValue(string id) => throw new NotImplementedException();

        public override IEnumerable<MultilingualString> GetValues() => throw new NotImplementedException();

        public override void GetValues(MultilingualDictionary dictionary) => throw new NotImplementedException();

        public override void OpenTab(string id) { }

        public override void SetLanguage(string lang) { }

        #endregion

        #region Methods (section managing)

        private ContentBlocksEditorBase AddSectionControl(string title, string path, string tab, string nav)
        {
            AddNavItem(out var navItem, out var ctrl);

            navItem.Title = title;

            if (!string.IsNullOrEmpty(tab) && !string.IsNullOrEmpty(nav))
            {
                if (tab == "pageblocks")
                {
                    if (title.ToLower() == nav)
                        navItem.IsSelected = true;
                }
            }

            var editor = (ContentBlocksEditorBase)ctrl.LoadControl($"~/UI/Admin/Sites/Pages/Controls/{path}.ascx");

            InitSectionControl(editor);

            SectionsCount++;

            return editor;
        }

        private void InitSectionControl(ContentBlocksEditorBase editor)
        {
            editor.ID = "Editor";
            editor.Insert += BlockEditor_Insert;
            editor.Delete += BlockEditor_Delete;
            editor.Alert += Editor_StatusUpdated;
        }

        public Guid? GetSectionItem()
        {
            Guid? result = null;

            EnumerateSectionItems((item, editor, index) =>
            {
                if (item.IsSelected)
                {
                    result = SectionsData[index].ID;

                    return false;
                }

                return true;
            });

            return result;
        }

        public void OpenSectionItem(Guid id)
        {
            EnumerateSectionItems((item, editor, index) =>
            {
                if (!(editor is ContentBlocksEditor))
                    return false;

                var section = SectionsData[index];
                if (section.ID != id)
                    return true;

                item.IsSelected = true;

                return false;
            });
        }

        private void AddNavItem(out NavItem navItem, out DynamicControl ctrl)
        {
            Navigation.AddItem(navItem = new NavItem());
            navItem.Controls.Add(ctrl = new DynamicControl { ID = "Container" });
            ctrl.ControlAdded += EditorContainer_ControlAdded;
        }

        private void EnumerateSectionItems(Func<NavItem, ContentBlocksEditorBase, int, bool> action)
        {
            var index = 0;

            foreach (var item in Navigation.GetItems())
            {
                var container = (DynamicControl)item.FindControl("Container");
                var ctrl = (ContentBlocksEditorBase)container.GetControl();

                if (!action(item, ctrl, index++))
                    break;
            }
        }

        #endregion
    }
}