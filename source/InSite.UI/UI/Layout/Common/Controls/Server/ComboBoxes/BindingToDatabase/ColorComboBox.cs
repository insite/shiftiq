using System;
using System.Collections.Generic;

using Shift.Common;

using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class ColorComboBox : ComboBox
    {
        class Mapping
        {
            public Indicator Indicator { get; }
            public FlagType Flag { get; }

            public Mapping(Indicator indicator, FlagType flag)
            {
                Indicator = indicator;
                Flag = flag;
            }
        }

        private static readonly List<Mapping> _flagMappings = new List<Mapping>
        {
            new Mapping(Indicator.Primary, FlagType.Blue),
            new Mapping(Indicator.Default, FlagType.Gray),
            new Mapping(Indicator.Success, FlagType.Green),
            new Mapping(Indicator.Danger, FlagType.Red),
            new Mapping(Indicator.Warning, FlagType.Yellow),
            new Mapping(Indicator.Info, FlagType.Cyan),
            new Mapping(Indicator.Light, FlagType.White),
            new Mapping(Indicator.Dark, FlagType.Black),
        };

        private static readonly Indicator[] _dataSource = new[]
        {
            Indicator.Primary,
            Indicator.Default,
            Indicator.Success,
            Indicator.Danger,
            Indicator.Warning,
            Indicator.Info,
            Indicator.Light,
            Indicator.Dark
        };

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            if (AllowNone)
                list.Add(CreateItem(Indicator.None));

            for (var i = 0; i < _dataSource.Length; i++)
                list.Add(CreateItem(_dataSource[i]));

            return list;
        }

        public bool AllowNone
        {
            get => (bool)(ViewState[nameof(AllowNone)] ?? false);
            set => ViewState[nameof(AllowNone)] = value;
        }

        public Indicator? EnumValue
        {
            get => base.Value.IsEmpty() ? (Indicator?)null : base.Value.ToEnum<Indicator>();
            set => base.Value = value?.GetName();
        }

        public FlagType? FlagValue
        {
            get
            {
                var enumValue = EnumValue;
                if (enumValue == null)
                    return null;

                return _flagMappings.Find(x => x.Indicator == enumValue)?.Flag
                    ?? throw new ArgumentException($"Unsupported indicator: {enumValue}");
            }
            set
            {
                if (value == null || value == FlagType.None)
                {
                    EnumValue = null;
                    return;
                }

                EnumValue = _flagMappings.Find(x => x.Flag == value)?.Indicator
                    ?? throw new ArgumentException($"Unsupported flag: {value}");
            }
        }

        private static ListItem CreateItem(Indicator indicator)
        {
            return new ListItem
            {
                Icon = indicator == Indicator.None
                    ? $"far fa-square-xmark me-2"
                    : $"fas fa-square text-{indicator.GetContextualClass()} me-2",
                Value = indicator.GetName(),
                Text = indicator.GetDescription()
            };
        }
    }
}