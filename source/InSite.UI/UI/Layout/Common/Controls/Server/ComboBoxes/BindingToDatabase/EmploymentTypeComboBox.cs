using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class EmploymentTypeComboBox : ComboBox
    {
        public bool UseAlternateList { get; set; }

        protected override BindingType ControlBinding => BindingType.Code;

        protected override ListItemArray CreateDataSource()
        {
            if (UseAlternateList)
                return new ListItemArray
                {
                    { "FullTimePermanent", "Full-time (permanent)" },
                    { "FullTimeTemporary", "Full-time (temporary)" },
                    { "PartTimePermanent", "Part-time (permanent)" },
                    { "PartTimeTemporary", "Part-time (temporary)" },
                    { "Casual", "Casual" },
                };

            return new ListItemArray
            {
                { "FullTime", "Full Time" },
                { "PartTime", "Part Time" },
                { "Contract", "Contract" },
                { "Temporary", "Temporary" },
                { "Seasonal", "Seasonal" },
                { "Flexible", "Flexible" },
                { "Other", "Other" }
            };
        }
    }
}