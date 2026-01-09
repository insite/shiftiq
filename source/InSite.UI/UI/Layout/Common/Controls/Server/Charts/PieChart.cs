using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using Shift.Common;
using Shift.Common.Colors;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI.Chart
{
    public class PieChart : BaseChart
    {
        #region Properties

        public override ChartType ChartType => ChartType.Pie;

        public ChartDataType DataType
        {
            get { return (ChartDataType)(ViewState[nameof(DataType)] ?? ChartDataType.Number); }
            set { ViewState[nameof(DataType)] = value; OnDataTypeChanged(); }
        }

        public bool ShowTooltipOnLegendHover
        {
            get { return (bool)(ViewState[nameof(ShowTooltipOnLegendHover)] ?? false); }
            set { ViewState[nameof(ShowTooltipOnLegendHover)] = value; OnShowTooltipOnLegendHoverChanged(); }
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

        #endregion

        #region Methods

        protected override ChartConfiguration CreateChartConfiguration()
        {
            return new ChartConfiguration(ChartType)
            {
                Data = new PieChartData()
            };
        }

        public override void BuildInitializationScript(StringBuilder builder)
        {
            builder
                .Append("(function(){")
                .AppendFormat("inSite.common.chart.init(document.getElementById('{0}'),{1});", ClientID, JsonHelper.SerializeJsObject(Configuration))
                ;

            if (ShowTooltipOnLegendHover)
            {
                builder.Append(@"
var hoverLegendItemIndex = null;

function onLegendClick (e,i) {
    Chart.defaults.pie.legend.onClick.call(this,e,i);

    if (!i.hidden) {
        if (hoverLegendItemIndex != null && hoverLegendItemIndex === i.index) {
            inSite.common.chart.hideTooltips(this.chart, hoverLegendItemIndex, false);
            hoverLegendItemIndex = null;
        }
    } else {
        inSite.common.chart.showTooltips(this.chart, i.index, false);
        hoverLegendItemIndex = i.index;
    }
}

function onLegendHover (e,i) {
    if (hoverLegendItemIndex == i.index)
        return;

    if (i.hidden)
        inSite.common.chart.hideTooltips(this.chart, i.index);
    else
        inSite.common.chart.showTooltips(this.chart, i.index);

    hoverLegendItemIndex = i.index;
}

function onChartHover (e,i) {
    if (hoverLegendItemIndex == null)
        return;

    var item = inSite.common.chart.getLegendItemByXY(this.chart, e.offsetX, e.offsetY);
    if (item != null && hoverLegendItemIndex === item.index)
        return;

    inSite.common.chart.hideTooltips(this.chart, hoverLegendItemIndex);
    hoverLegendItemIndex = null;
}");
            }

            builder.Append("})();");
        }

        #endregion

        #region Event handlers

        private void OnDataTypeChanged()
        {
            string func = null;

            if (DataType == ChartDataType.Percent)
                func = @"function (item, data) { return data.labels[item.index] + ': ' + data.datasets[item.datasetIndex].data[item.index].toFixed(2) + ' %'; }";

            Configuration.Options.Plugins.Tooltip.Callbacks.LabelJsFunction = func;
        }

        private void OnShowTooltipOnLegendHoverChanged()
        {
            if (ShowTooltipOnLegendHover)
            {
                Configuration.Options.Plugins.Legend.OnClickJsFunction = "onLegendClick";
                Configuration.Options.Plugins.Legend.OnHoverJsFunction = "onLegendHover";
                Configuration.Options.OnHoverJsFunction = "onChartHover";
            }
            else
            {
                Configuration.Options.Plugins.Legend.OnHoverJsFunction = null;
                Configuration.Options.OnHoverJsFunction = null;
            }
        }

        #endregion

        #region Public methods

        public void LoadData(IEnumerable<StatusChartItem> data, bool autoColors = false)
        {
            DataType = ChartDataType.Percent;

            var pieData = (PieChartData)Configuration.Data;

            pieData.Clear();

            var sortedData = data.OrderBy(x => x.Sequence).ToArray();
            var colorPalette = Palette.GenerateColorPalette(sortedData.Length);
            var totalCount = sortedData.Sum(x => x.Count);
            var dataset = pieData.CreateDataset("Dataset1");

            for (var i = 0; i < sortedData.Length; i++)
            {
                var dataItem = sortedData[i];
                var value = totalCount != 0 ? ((double)dataItem.Count / totalCount) * 100 : 0;
                if (value == 0)
                    continue;

                var datasetItem = dataset.NewItem();
                datasetItem.Label = dataItem.Status;
                datasetItem.Value = value;
                datasetItem.BackgroundColor = autoColors || string.IsNullOrEmpty(dataItem.Color) ? colorPalette[i] : ColorTranslator.FromHtml(dataItem.Color);
            }
        }

        #endregion
    }
}