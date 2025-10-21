using System.Collections.Generic;
using System.Linq;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    public class FlagComboBox : ComboBox
    {
        public static readonly FlagType[] Flags = {
            FlagType.None,
            FlagType.Black,
            FlagType.Blue,
            FlagType.Cyan,
            FlagType.Gray,
            FlagType.Green,
            FlagType.Red,
            FlagType.Yellow,
            FlagType.White
        };

        public FlagType? EnumValue
        {
            get => base.Value.IsEmpty() ? (FlagType?)null : base.Value.ToEnum<FlagType>();
            set => base.Value = value?.GetName();
        }

        protected override BindingType ControlBinding => BindingType.Code;

        protected override ListItemArray CreateDataSource() => ComboBoxHelper.CreateDataSource(Flags);
    }

    public class FlagMultiComboBox : MultiComboBox
    {
        public IEnumerable<FlagType> EnumValues
        {
            get => base.Values.Select(x => x.ToEnum<FlagType>());
            set => base.Values = value?.Select(x => x.GetName());
        }

        protected override BindingType ControlBinding => BindingType.Code;

        protected override ListItemArray CreateDataSource() => ComboBoxHelper.CreateDataSource(FlagComboBox.Flags);
    }
}