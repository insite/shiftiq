using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class AssetRelationshipConditionComboBox : ComboBox
    {
        #region Properties

        protected override BindingType ControlBinding => BindingType.Code;

        public override string Value
        {
            get => base.Value.NullIf("None");
            set => base.Value = value.IfNullOrEmpty("None");
        }

        public override bool AllowBlank => false;

        #endregion

        #region Select data

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            list.Add("None");
            list.Add("On Success");
            list.Add("On Failure");

            return list;
        }

        #endregion
    }
}