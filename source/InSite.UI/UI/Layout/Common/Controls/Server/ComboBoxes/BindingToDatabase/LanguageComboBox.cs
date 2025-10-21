using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class LanguageComboBox : ComboBox
    {
        protected override BindingType ControlBinding => BindingType.Code;

        public override string Value
        {
            get => base.Value;
            set => base.Value = !string.IsNullOrEmpty(value) ? value.ToLowerInvariant() : value;
        }

        [PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LanguageComboBoxSettings Settings { get; }

        public LanguageComboBox()
        {
            Settings = new LanguageComboBoxSettings(nameof(Settings), ViewState);
        }

        protected override ListItemArray CreateDataSource() => Settings.CreateDataSource();
    }

    public class LanguageMultiComboBox : MultiComboBox
    {
        protected override BindingType ControlBinding => BindingType.Code;

        public override IEnumerable<string> Values
        {
            get => base.Values;
            set
            {
                if (value != null)
                    base.Values = value.Select(x => !string.IsNullOrEmpty(x) ? x.ToLowerInvariant() : x);
                else
                    base.Values = value;
            }
        }

        [PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LanguageComboBoxSettings Settings { get; }

        public LanguageMultiComboBox()
        {
            Settings = new LanguageComboBoxSettings(nameof(Settings), ViewState);
        }

        protected override ListItemArray CreateDataSource() => Settings.CreateDataSource();
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class LanguageComboBoxSettings : StateBagProxy
    {
        #region Properties

        [TypeConverter(typeof(StringArrayConverter))]
        public string[] ExcludeLanguage
        {
            get => (string[])GetValue();
            set => SetValue(value);
        }

        [TypeConverter(typeof(StringArrayConverter))]
        public string[] IncludeLanguage
        {
            get => (string[])(GetValue() ?? CurrentSessionState.Identity.Organization.Languages.Select(x => x.Name).ToArray());
            set => SetValue(value);
        }

        #endregion

        #region Construction

        public LanguageComboBoxSettings(string prefix, StateBag viewState)
            : base(prefix, viewState)
        {
        }

        #endregion

        #region Methods

        public ListItemArray CreateDataSource()
        {
            var hasExcludes = ExcludeLanguage.IsNotEmpty();
            var excludes = ExcludeLanguage;

            var hasIncludes = IncludeLanguage.IsNotEmpty();
            var includes = IncludeLanguage;

            var list = new ListItemArray();
            var languages = Language.GetAllInfo();

            foreach (var lang in languages)
            {
                if (hasExcludes && excludes.Contains(lang.Code) || hasIncludes && !includes.Contains(lang.Code))
                    continue;

                list.Add(lang.Code, lang.Name);
            }

            return list;
        }

        #endregion
    }

}