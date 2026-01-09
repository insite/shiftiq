using System;
using System.Text;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI.Chart
{
    public class LineChart : BaseChart
    {
        #region Properties

        public override ChartType ChartType => ChartType.Line;

        public LineChatDatasetType DatasetType { get; set; }

        public string OnClientPreInit
        {
            get => (string)ViewState[nameof(OnClientPreInit)];
            set => ViewState[nameof(OnClientPreInit)] = value;
        }

        public bool LegendVisible
        {
            get => Configuration.Options.Plugins.Legend.Visible;
            set => Configuration.Options.Plugins.Legend.Visible = value;
        }

        public ChartPosition LegendPosition
        {
            get => Configuration.Options.Plugins.Legend.Position;
            set => Configuration.Options.Plugins.Legend.Position = value;
        }

        public bool ToolTipIntersect
        {
            get => Configuration.Options.Plugins.Tooltip.Intersect;
            set => Configuration.Options.Plugins.Tooltip.Intersect = value;
        }

        public bool MaintainApectRatio
        {
            get => Configuration.Options.MaintainAspectRatio;
            set => Configuration.Options.MaintainAspectRatio = value;
        }

        public ChartOptions Options => Configuration.Options;

        #endregion

        #region Methods

        protected override ChartConfiguration CreateChartConfiguration()
        {
            IChartData data;

            if (DatasetType == LineChatDatasetType.Default)
                data = new LineChartData();
            else if (DatasetType == LineChatDatasetType.DateTime)
                data = new DateTimeChartData();
            else
                throw new NotImplementedException($"Unexpected dataset type: {DatasetType.GetName()}");

            return new ChartConfiguration(ChartType) { Data = data };
        }

        public override void BuildInitializationScript(StringBuilder builder)
        {
            builder
                .Append("(function(){")
                .AppendFormat("var canvas = document.getElementById('{0}');", ClientID)
                .AppendFormat("var config = {0};", JsonHelper.SerializeJsObject(Configuration))
                ;

            if (!string.IsNullOrEmpty(OnClientPreInit))
                builder.AppendFormat("inSite.common.execFuncByName('{0}', canvas, config);", OnClientPreInit);

            builder
                .Append("inSite.common.chart.init(canvas,config);")
                .Append("})();")
                ;
        }

        #endregion
    }
}
