using System.ComponentModel;
using System.Web.UI;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ComboBoxMultiple : StateBagProxy
    {
        #region Constants

        public const string DefaultSelectAllText = "Select All";
        public const string DefaultDeselectAllText = "Deselect All";
        public const string DefaultCountSingularFormat = "{0} item selected";
        public const string DefaultCountPluralFormat = "{0} items selected";
        public const string DefaultMaxTotalSingularFormat = "Limit reached ({n} item max)";
        public const string DefaultMaxTotalPluralFormat = "Limit reached ({n} items max)";
        public const string DefaultMaxGroupSingularFormat = "Group limit reached ({n} item max)";
        public const string DefaultMaxGroupPluralFormat = "Group limit reached ({n} items max)";

        #endregion

        #region Enums

        public enum FormatType
        {
            Values, Count, Static
        }

        #endregion

        #region Properties

        public int Max
        {
            get => (int)(GetValue() ?? 0);
            set => SetValue(Number.CheckRange(value, 0));
        }

        public bool ActionsBox
        {
            get => (bool)(GetValue() ?? false);
            set => SetValue(value);
        }

        public FormatType Format
        {
            get => (FormatType)(GetValue() ?? FormatType.Values);
            set => SetValue(value);
        }

        public int FormatCountMax
        {
            get => (int)(GetValue() ?? 0);
            set => SetValue(Number.CheckRange(value, 0));
        }

        public string SelectAllText
        {
            get => (string)GetValue() ?? DefaultSelectAllText;
            set => SetValue(value);
        }

        public string DeselectAllText
        {
            get => (string)GetValue() ?? DefaultDeselectAllText;
            set => SetValue(value);
        }

        public string CountSingularFormat
        {
            get => (string)GetValue() ?? DefaultCountSingularFormat;
            set => SetValue(value);
        }

        public string CountPluralFormat
        {
            get => (string)GetValue() ?? DefaultCountPluralFormat;
            set => SetValue(value);
        }

        public string CountAllFormat
        {
            get => (string)GetValue() ?? string.Empty;
            set => SetValue(value);
        }

        public string MaxTotalSingularFormat
        {
            get => (string)GetValue() ?? DefaultMaxTotalSingularFormat;
            set => SetValue(value);
        }

        public string MaxTotalPluralFormat
        {
            get => (string)GetValue() ?? DefaultMaxTotalPluralFormat;
            set => SetValue(value);
        }

        public string MaxGroupSingularFormat
        {
            get => (string)GetValue() ?? DefaultMaxGroupSingularFormat;
            set => SetValue(value);
        }

        public string MaxGroupPluralFormat
        {
            get => (string)GetValue() ?? DefaultMaxGroupPluralFormat;
            set => SetValue(value);
        }

        #endregion

        #region Construction

        public ComboBoxMultiple(string prefix, StateBag viewState)
            : base(prefix, viewState)
        {
        }

        #endregion
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ComboBoxGroupMultiple
    {
        public int Max { get; set; }
    }
}