using Shift.Constant;

namespace InSite.UI.CMDS.Common.Models
{
    public class SummaryDataItem
    {
        public string Title { get; set; }
        public CmdsFlagType? FlagType
        {
            get => _flagType ?? CmdsFlagType.Red;
            set => _flagType = value;
        }
        public bool HasFlagType => _flagType.HasValue;
        public string FlagTooltip { get; set; }
        public string ProgressPercent { get; set; }
        public string ProgressText { get; set; }
        public int Completed { get; internal set; }
        public int Total { get; internal set; }
        public string ProgressUrl { get; internal set; }

        private CmdsFlagType? _flagType = null;
    }
}