namespace InSite.Persistence.Plugin.NCSHA
{
    public class ChartDataGetModel
    {
        public string[] Code { get; set; }
        public string[] Region { get; set; }
        public int? FromYear { get; set; }
        public int? ToYear { get; set; }
        public string Func { get; set; }
        public string AxisName { get; set; }
        public string AxisUnit { get; set; }
        public string DatasetType { get; set; }
    }
}