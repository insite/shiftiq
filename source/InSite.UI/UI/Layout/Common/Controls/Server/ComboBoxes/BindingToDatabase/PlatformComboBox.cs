using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class PlatformComboBox : ComboBox
    {
        protected override BindingType ControlBinding => BindingType.Code;

        public bool InSiteOnly
        {
            get => (bool?)ViewState[nameof(InSiteOnly)] ?? false;
            set => ViewState[nameof(InSiteOnly)] = value;
        }

        protected override ListItemArray CreateDataSource()
        {
            return new ListItemArray(new[] {
                    "Blackboard",
                    "Brainshark",
                    "Canvas",
                    "CMDS",
                    "D2L",
                    "Harvard ManageMentor",
                    "InSite",
                    "Lynda",
                    "Microsoft Learn",
                    "Moodle",
                    "Pluralsight",
                    "SCORM Cloud",
                    "Shift iQ",
                    "Skills Passport",
                    "Stanford eCorner",
                    "Udacity"
                });
        }
    }
}