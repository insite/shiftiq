using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI;

using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Contract;
using Shift.Sdk.UI;

using LabelSearch = InSite.Persistence.LabelSearch;
using OrganizationIdentifiers = Shift.Constant.OrganizationIdentifiers;
using WebControl = System.Web.UI.WebControls.WebControl;

namespace InSite.Admin.Assets.Contents.Utilities
{
    public class InputTranslator : ITranslator
    {
        private readonly StringDictionary _from;
        private readonly StringDictionary _to;
        private readonly List<Control> _list;
        private readonly Guid _organization;
        private readonly string _language;
        private readonly HashSet<Control> _excludedControls;

        public string Language => _language;

        public InputTranslator(Guid organization, string language)
        {
            _from = new StringDictionary();
            _to = new StringDictionary();
            _list = new List<Control>();
            _organization = organization;
            _language = language;
            _excludedControls = new HashSet<Control>();
        }

        public void ExcludeFromAutoTranslate(Control control)
        {
            _excludedControls.Add(control);
        }

        public string Translate(string source) => Translate(source, source);

        public string Translate(string from, string to, string item)
        {
            return ServiceLocator.TranslationClient.Translate(from, to, item);
        }

        public string Translate(string from, string to, MultilingualString item)
        {
            ServiceLocator.TranslationClient.Translate(from, to, item);
            return item.Get(to);
        }

        public void Translate(string from, string to, IEnumerable<MultilingualString> list)
        {
            ServiceLocator.TranslationClient.Translate(from, to, list);
        }

        private string GetTranslationWithFallback(string label, string language, params Guid[] scopes)
        {
            foreach (var scope in scopes)
            {
                var translation = LabelSearch.GetTranslation(label, language, scope, true, false);
                if (translation != null)
                    return translation;
            }
            return null;
        }

        public string Translate(string label, string source)
        {
            if (source.HasNoValue())
                return source;

            try
            {
                var partition = ServiceLocator.Partition.Identifier;

                var originalGlobalOrganizationId = Guid.Parse("0C071B03-6FE1-400F-82F4-78FF6F751AE7");

                var target = GetTranslationWithFallback(label, _language, _organization, partition, originalGlobalOrganizationId);

                if (target.IsEmpty())
                {
                    if (_language == "en")
                        return source;

                    target = ServiceLocator.TranslationClient.Translate("en", _language, source);

                    SaveTranslation(label, source, target);

                    LabelSearch.Refresh();
                }
                return target;
            }
            catch (Exception ex)
            {
                AppSentry.SentryError(ex);
                return source;
            }
        }

        public void Translate(ControlCollection controls)
        {
            foreach (Control control in controls)
                Translate(control);
        }

        public void Translate(ComboBox cb)
        {
            cb.EmptyMessage = Translate(cb.EmptyMessage);

            foreach (var item in cb.Items)
                item.Text = Translate(item.Text);
        }

        public void Translate(Control control)
        {
            var controlList = _list;
            GetControls(control);

            if (_language != "en")
            {
                var itemsPendingGoogleTranslate = new List<string>();

                foreach (string key in _from.Keys)
                {
                    var source = _from[key];
                    if (source.HasValue() && !_to[key].HasValue())
                        itemsPendingGoogleTranslate.Add(source);
                }

                if (itemsPendingGoogleTranslate.Count > 0)
                {
                    var itemsTranslatedByGoogle = ServiceLocator.TranslationClient.Translate("en", _language, itemsPendingGoogleTranslate.ToArray());

                    for (var i = 0; i < itemsPendingGoogleTranslate.Count; i++)
                    {
                        var source = itemsPendingGoogleTranslate[i];
                        var target = itemsTranslatedByGoogle[i];
                        _to[source] = target;

                        SaveTranslation(source, source, target);
                    }

                    LabelSearch.Refresh();
                }
            }

            foreach (var controlItem in controlList)
            {
                if (controlItem is IMultilingualDisabled multilingualDisabled && multilingualDisabled.DisableTranslation)
                    continue;

                if (controlItem is IHasText)
                    Translate((IHasText)controlItem);

                else if (controlItem is System.Web.UI.WebControls.Button)
                    Translate((System.Web.UI.WebControls.Button)controlItem);

                else if (controlItem is System.Web.UI.WebControls.CheckBox chk)
                    Translate(chk);

                if (controlItem is IHasConfirmText confirmText)
                    Translate(confirmText);

                if (controlItem is IHasEmptyMessage)
                    Translate((IHasEmptyMessage)controlItem);

                if (controlItem is IHasToolTip)
                    Translate((IHasToolTip)controlItem);

                else if (controlItem is WebControl)
                    Translate((WebControl)controlItem);

                if (controlItem is InputFilter inputFilter)
                    Translate(inputFilter);
            }
        }

        private void AddToBuffer(Control control, string source)
        {
            if (source.IsNotEmpty())
                source = source.Trim();

            if (source.HasValue() && !_from.ContainsKey(source))
            {
                var target = LabelSearch.GetTranslation(source, _language, _organization, true, false);

                _from.Add(source, source);

                if (_language == "en")
                    _to.Add(source, target ?? source);
                else
                    _to.Add(source, target);
            }

            if (!_list.Contains(control))
                _list.Add(control);
        }

        private void GetControls(Control parent)
        {
            if (_excludedControls.Contains(parent))
                return;

            foreach (Control child in parent.Controls)
            {
                if (_excludedControls.Contains(child)
                    || child is IMultilingualDisabled multilingualDisabled && multilingualDisabled.DisableTranslation
                    )
                {
                    continue;
                }

                if (child is IHasText)
                    AddToBuffer((IHasText)child);
                else if (child is System.Web.UI.WebControls.Button)
                    AddToBuffer((System.Web.UI.WebControls.Button)child);
                else if (child is System.Web.UI.WebControls.CheckBox chk)
                    AddToBuffer(chk);

                if (child is IHasConfirmText confirmText)
                    AddToBuffer(confirmText);

                if (child is IHasEmptyMessage placeholderControl)
                    AddToBuffer(placeholderControl);

                if (child is IHasToolTip)
                    AddToBuffer((IHasToolTip)child);
                else if (child is WebControl)
                    AddToBuffer((WebControl)child);

                if (child is InputFilter inputFilter)
                    AddToBuffer(inputFilter);

                GetControls(child);
            }
        }

        private void Translate(IHasText control)
        {
            if (control.Text.HasValue())
                control.Text = GetToValue(control.Text);
        }

        private void Translate(IHasConfirmText control)
        {
            if (control.ConfirmText.HasValue())
                control.ConfirmText = GetToValue(control.ConfirmText);
        }

        private void Translate(IHasEmptyMessage control)
        {
            if (control.EmptyMessage.HasValue())
                control.EmptyMessage = GetToValue(control.EmptyMessage);
        }

        private void Translate(IHasToolTip control)
        {
            if (control.ToolTip.HasValue())
                control.ToolTip = GetToValue(control.ToolTip);
        }

        private void Translate(WebControl control)
        {
            if (control.ToolTip.HasValue())
                control.ToolTip = GetToValue(control.ToolTip);
        }

        private void Translate(System.Web.UI.WebControls.Button control)
        {
            if (control.Text.HasValue())
                control.Text = GetToValue(control.Text);
        }

        private void Translate(System.Web.UI.WebControls.CheckBox control)
        {
            if (control.Text.HasValue())
                control.Text = GetToValue(control.Text);
        }

        private void Translate(InputFilter control)
        {
            if (control.ClearButtonTooltip.HasValue())
                control.ClearButtonTooltip = GetToValue(control.ClearButtonTooltip);

            if (control.FilterButtonTooltip.HasValue())
                control.FilterButtonTooltip = GetToValue(control.FilterButtonTooltip);
        }

        private string GetToValue(string key)
            => _to[key.Trim()];

        private void AddToBuffer(IHasText control)
        {
            if (control.Text.HasValue())
                AddToBuffer((Control)control, control.Text);
        }

        private void AddToBuffer(IHasConfirmText control)
        {
            if (control.ConfirmText.HasValue())
                AddToBuffer((Control)control, control.ConfirmText);
        }

        private void AddToBuffer(IHasEmptyMessage control)
        {
            if (control.EmptyMessage.HasValue())
                AddToBuffer((Control)control, control.EmptyMessage);
        }

        private void AddToBuffer(IHasToolTip control)
        {
            if (control.ToolTip.HasValue())
                AddToBuffer((Control)control, control.ToolTip);
        }

        private void AddToBuffer(WebControl control)
        {
            if (control.ToolTip.HasValue())
                AddToBuffer(control, control.ToolTip);
        }

        private void AddToBuffer(System.Web.UI.WebControls.Button control)
        {
            if (control.Text.HasValue())
                AddToBuffer((Control)control, control.Text);
        }

        private void AddToBuffer(System.Web.UI.WebControls.CheckBox control)
        {
            if (control.Text.HasValue())
                AddToBuffer(control, control.Text);
        }

        private void AddToBuffer(InputFilter control)
        {
            if (control.ClearButtonTooltip.HasValue())
                AddToBuffer(control, control.ClearButtonTooltip);

            if (control.FilterButtonTooltip.HasValue())
                AddToBuffer(control, control.FilterButtonTooltip);
        }

        private void SaveTranslation(string label, string source, string target)
        {
            label = StringHelper.Snip(label, 100);

            if (!ServiceLocator.ContentSearch.Exists(x =>
                x.ContainerType == "Application"
                && x.ContainerIdentifier == LabelSearch.ContainerIdentifier
                && x.ContentLabel == label
                && x.ContentLanguage == "en"
                && x.OrganizationIdentifier == OrganizationIdentifiers.Global)
                )
            {
                ServiceLocator.ContentStore.Save("Application", LabelSearch.ContainerIdentifier, label, source, "en", OrganizationIdentifiers.Global);
            }

            ServiceLocator.ContentStore.Save("Application", LabelSearch.ContainerIdentifier, label, target, _language, OrganizationIdentifiers.Global);
        }
    }
}