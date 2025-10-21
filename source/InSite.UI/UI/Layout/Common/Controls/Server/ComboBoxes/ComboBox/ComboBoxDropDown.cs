using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ComboBoxDropDown : StateBagProxy
    {
        #region Enums

        public enum DirectionType
        {
            Auto,
            Up,
            Down
        }

        #endregion

        #region Properties

        public string Header
        {
            get => (string)(GetValue() ?? string.Empty);
            set => SetValue(value);
        }

        public string Container
        {
            get => (string)(GetValue() ?? string.Empty);
            set => SetValue(value);
        }

        public int Size
        {
            get => (int)(GetValue() ?? 0);
            set => SetValue(Number.CheckRange(value, 0));
        }

        public DirectionType Direction
        {
            get => (DirectionType)(GetValue() ?? DirectionType.Auto);
            set => SetValue(value);
        }

        public Unit Width
        {
            get => (Unit)(GetValue() ?? Unit.Empty);
            set => SetValue(value);
        }

        #endregion

        #region Construction

        public ComboBoxDropDown(string prefix, StateBag viewState)
            : base(prefix, viewState)
        {
        }

        #endregion
    }
}