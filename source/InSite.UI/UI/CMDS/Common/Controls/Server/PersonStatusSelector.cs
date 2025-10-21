using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public class PersonStatusSelector : ComboBox
    {
        #region Constants

        public const string Approved = "Approved";
        public const string Disabled = "Disabled";

        #endregion

        #region Properties

        protected override BindingType ControlBinding => BindingType.Code;

        public bool? IsApproved
        {
            get
            {
                var value = Value;
                return value.IsEmpty() ? (bool?)null : StringHelper.Equals(value, Approved);
            }
        }

        public bool? IsDisabled
        {
            get
            {
                var value = Value;
                return value.IsEmpty() ? (bool?)null : StringHelper.Equals(value, Disabled);
            }
        }
        #endregion

        #region Select data

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            list.Add(Approved);
            list.Add(Disabled);

            return list;
        }

        #endregion
    }
}