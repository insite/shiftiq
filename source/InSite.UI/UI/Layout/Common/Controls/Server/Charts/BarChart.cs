using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI.Chart
{
    public class BarChart : BaseChart
    {
        #region Properties

        public override ChartType ChartType => ChartType.Bar;

        public LineChatDatasetType DatasetType { get; set; }

        public string OnClientPreInit
        {
            get => (string)ViewState[nameof(OnClientPreInit)];
            set => ViewState[nameof(OnClientPreInit)] = value;
        }

        public ChartInteractionMode ToolTipInteractionMode
        {
            get => Configuration.Options.Plugins.Tooltip.InteractionMode;
            set => Configuration.Options.Plugins.Tooltip.InteractionMode = value;
        }

        public bool ToolTipIntersect
        {
            get => Configuration.Options.Plugins.Tooltip.Intersect;
            set => Configuration.Options.Plugins.Tooltip.Intersect = value;
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
                data = new BarChartData();
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

        #region Public methods

        public void LoadData(IEnumerable<HistoryPerMonthChartItem> data)
        {
            var configData = (BarChartData)Configuration.Data;

            configData.Clear();

            var dictionary = data.ToDictionary(x => x.GetDateTime(), x => x.Count);
            if (dictionary.Count == 0)
                return;

            var sortedData = data.OrderBy(x => x.Year).ThenBy(x => x.Month).ToArray();
            var currentDate = sortedData.First().GetDateTime();
            var lastDate = sortedData.Last().GetDateTime();

            var dataset = configData.CreateDataset("sessions");
            dataset.Label = "Sessions";
            dataset.BackgroundColor = ColorTranslator.FromHtml("#86c557");

            while (currentDate <= lastDate)
            {
                var datasetItem = dataset.NewItem();
                datasetItem.Label = $"{currentDate:MMM yyyy}";
                datasetItem.Value = dictionary.ContainsKey(currentDate) ? dictionary[currentDate] : 0;

                currentDate = currentDate.AddMonths(1);
            }
        }

        #endregion
    }
}
