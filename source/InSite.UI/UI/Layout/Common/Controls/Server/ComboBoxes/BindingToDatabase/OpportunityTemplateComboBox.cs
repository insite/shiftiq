using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class OpportunityTemplateComboBox : ComboBox
    {
        public override bool AllowBlank => false;

        protected override ListItemArray CreateDataSource()
        {
            return new ListItemArray
            {
                "JobConnect",
                "FAST"
            };
        }
    }
}