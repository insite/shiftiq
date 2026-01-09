using System;

using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public class EmployeeProfileFilterSelector : ComboBox
    {
        #region Constants

        public const Int32 All = 0;
        public const Int32 SelfAssessed = 1;
        public const Int32 Submitted = 2;
        public const Int32 Validated = 3;

        #endregion

        protected override BindingType ControlBinding => BindingType.Code;

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            list.Add(All.ToString(), "All Competencies");
            list.Add(SelfAssessed.ToString(), "Self-Assessed");
            list.Add(Submitted.ToString(), "Submitted for Validation");
            list.Add(Validated.ToString(), "Validated");

            return list;
        }
    }
}
