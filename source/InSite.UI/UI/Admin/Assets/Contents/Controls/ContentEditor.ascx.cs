using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

using InSite.Admin.Assets.Contents.Controls.ContentEditor;
using InSite.Common.Web;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Assets.Contents.Controls
{
    public partial class ContentEditor_ : UserControl
    {
        #region Classes

        [Serializable]
        private class PillInfo
        {
            public string ID { get; set; }
            public string Path { get; set; }
        }

        private class SectionControls
        {
            public NavItem NavItem { get; }
            public SectionBase Content { get; }

            public SectionControls(NavItem navItem, SectionBase content)
            {
                NavItem = navItem;
                Content = content;
            }
        }

        #endregion

        #region Events

        public event RedirectParametersHandler NeedRedirectParameters;

        private RedirectParametersArgs OnRedirect()
        {
            var args = new RedirectParametersArgs();

            NeedRedirectParameters?.Invoke(this, args);

            return args;
        }

        #endregion

        #region Properties

        public bool IsEmpty => Identifiers.Count == 0;

        private Dictionary<string, int> Identifiers
        {
            get => (Dictionary<string, int>)(ViewState[nameof(Identifiers)]
                ?? (ViewState[nameof(Identifiers)] = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)));
            set => ViewState[nameof(Identifiers)] = value;
        }

        private List<PillInfo> NavItems
        {
            get
            {
                if (ViewState[nameof(NavItems)] == null)
                    ViewState[nameof(NavItems)] = new List<PillInfo>();

                return (List<PillInfo>)ViewState[nameof(NavItems)];
            }
            set => ViewState[nameof(NavItems)] = value;
        }

        public string ValidationGroup
        {
            get => (string)ViewState[nameof(ValidationGroup)];
            set => ViewState[nameof(ValidationGroup)] = value;
        }

        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState(savedState);
        }

        #endregion

        #region Fields

        private string _language = "en";
        private List<SectionControls> _controls = new List<SectionControls>();

        #endregion

        #region Initialization

        protected override void OnLoad(EventArgs e)
        {
            EnsureChildControls();

            base.OnLoad(e);
        }

        protected override void CreateChildControls()
        {
            if (PillsNav.ItemsCount == 0 && NavItems.Count > 0)
            {
                for (var i = 0; i < NavItems.Count; i++)
                {
                    var item = NavItems[i];
                    AddPill(item.Path, out _, out _);
                }
            }

            base.CreateChildControls();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            var validationGroup = ValidationGroup;
            foreach (var ctrl in _controls)
                ctrl.Content.SetValidationGroup(validationGroup);
        }

        #endregion

        #region Event handlers

        private void Section_Redirect(object sender, RedirectUrlArgs redirectArgs)
        {
            var paramsArgs = OnRedirect();
            if (paramsArgs.IsCancelled)
                return;

            var url = redirectArgs.Url;
            if (paramsArgs.Parameters.Count > 0)
                url = HttpResponseHelper.BuildUrl(
                    url,
                    string.Join(
                        "&",
                        paramsArgs.Parameters.AllKeys.Select(key => key + "=" + paramsArgs.Parameters[key])));

            HttpResponseHelper.Redirect(url, true);
        }

        #endregion

        #region Methods (section management)

        public void Clear()
        {
            PillsNav.ClearItems();
            Identifiers.Clear();
            NavItems.Clear();

            _controls.Clear();
        }

        public void Add(ContentSectionDefault id, MultilingualDictionary content, bool? isRequired = null)
        {
            var options = AssetContentSection.Create(id, content, isRequired);

            Add(options);
        }

        public void Add(AssetContentSection options)
        {
            AddPill(options.Id, options.ControlPath, out var navItem, out var section);

            if (navItem != null)
                navItem.Title = options.Title;

            if (section != null)
                section.SetOptions(options);
        }

        private void AddPill(string id, string sectionPath, out NavItem navItem, out SectionBase section)
        {
            if (Identifiers.ContainsKey(id))
            {
                ErrorPanel.Visible = true;
                ErrorAlert.Text = $"<strong>Error:</strong> The label <strong>{id}</strong> is duplicated for this page.";
                navItem = null;
                section = null;
                return;
            }

            AddPill(sectionPath, out navItem, out section);

            Identifiers.Add(id, NavItems.Count);
            NavItems.Add(new PillInfo
            {
                ID = id,
                Path = sectionPath
            });
        }

        private void AddPill(string sectionPath, out NavItem navItem, out SectionBase section)
        {
            PillsNav.AddItem(navItem = new NavItem());
            navItem.Controls.Add(section = (SectionBase)LoadControl(sectionPath));

            section.Redirect += Section_Redirect;
            section.SetValidationGroup(ValidationGroup);

            if (!string.IsNullOrEmpty(_language))
                section.SetLanguage(_language);

            _controls.Add(new SectionControls(navItem, section));
        }

        public void OpenTab(string id)
        {
            if (!string.IsNullOrEmpty(id) && Identifiers.TryGetValue(id, out var index))
                _controls[index].NavItem.IsSelected = true;
        }

        public void OpenTab(string id1, string id2)
        {
            if (string.IsNullOrEmpty(id1) || !Identifiers.TryGetValue(id1, out var index))
                return;

            var ctrl = _controls[index];
            ctrl.NavItem.IsSelected = true;
            ctrl.Content.OpenTab(id2);
        }

        public void HideTabs()
        {
            foreach (var ctrl in _controls)
                ctrl.NavItem.Visible = false;
        }

        public void HideTabs(string[] ids)
        {
            foreach (var id in ids)
            {
                if (!string.IsNullOrEmpty(id) && Identifiers.TryGetValue(id, out var index))
                    _controls[index].NavItem.Visible = false;
            }
        }

        public void ShowTabs()
        {
            foreach (var ctrl in _controls)
                ctrl.NavItem.Visible = true;
        }

        public void ShowTabs(string[] ids)
        {
            foreach (var id in ids)
            {
                if (!string.IsNullOrEmpty(id) && Identifiers.TryGetValue(id, out var index))
                    _controls[index].NavItem.Visible = true;
            }
        }

        public string GetCurrentTab()
        {
            if (PillsNav.ItemsCount == 0)
                return null;

            var index = PillsNav.SelectedIndex;

            return Identifiers.Where(kv => kv.Value == index).Select(x => x.Key).FirstOrDefault();
        }

        public string GetCurrentSubTab()
        {
            if (PillsNav.ItemsCount == 0)
                return null;

            var index = PillsNav.SelectedIndex;

            var subTabs = _controls[index].Content as SectionTabs;

            if (subTabs != null)
            {
                return subTabs.GetCurrentTab();
            }

            return null;
        }

        #endregion

        #region Methods (get value)

        public MultilingualString GetValue(Enum id) => GetValue(id.GetName());

        public MultilingualString GetValue(string id) => GetSection(id).GetValue();

        public MultilingualString GetValue(Enum id1, Enum id2) => GetValue(id1.GetName(), id2.GetName());

        public MultilingualString GetValue(string id1, Enum id2) => GetValue(id1, id2.GetName());

        public MultilingualString GetValue(string id1, string id2) => GetSection(id1).GetValue(id2);

        public IEnumerable<MultilingualString> GetValues(Enum id) => GetValues(id.GetName());

        public IEnumerable<MultilingualString> GetValues(string id) => GetSection(id).GetValues();

        public void GetValues(Enum id, MultilingualDictionary dictionary) => GetValues(id.GetName(), dictionary);

        public void GetValues(string id, MultilingualDictionary dictionary) => GetSection(id).GetValues(dictionary);

        public SectionBase GetSection(string id, bool safeSearch = false)
        {
            if (!string.IsNullOrEmpty(id) && Identifiers.TryGetValue(id, out var index))
                return _controls[index].Content;
            else if (safeSearch)
                return null;

            throw ApplicationError.Create("Section not found: " + id);
        }

        #endregion

        #region Methods (other)

        public void SetLanguage(string lang)
        {
            _language = lang;
            foreach (var ctrl in _controls)
                ctrl.Content.SetLanguage(_language);
        }

        #endregion
    }
}