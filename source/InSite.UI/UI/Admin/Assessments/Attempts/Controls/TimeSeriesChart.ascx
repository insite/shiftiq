<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimeSeriesChart.ascx.cs" Inherits="InSite.Admin.Assessments.Attempts.Controls.TimeSeriesChart" %>

<%@ Import Namespace="Newtonsoft.Json" %>

<insite:PageHeadContent runat="server">
    <style type="text/css">
        .chart-selector button {
            width: 105px;
        }

        .zoom-selector button {
            width: 130px;
        }

        .chart-date-selector > div {
            width: 400px;
        }

        .chart-date-selector .input-group a.btn {
            line-height: 2;
        }

        .chart-window {
            overflow-x: auto;
        }

            .chart-window > .chart-container {
                height: 40vh;
                position: relative;
            }

    </style>
</insite:PageHeadContent>

<div runat="server" ID="NoResponsesMessage" Visible="false">
    No responses to this form have been submitted.
</div>

<asp:PlaceHolder runat="server" ID="StatisticInfo" Visible="false">
    <div class="row">
        <div class="col-2 text-center">
            <span class="d-block d-md-inline-block"><b>Today</b>:</span>
            <asp:Literal runat="server" ID="ResponseToday" />
        </div>
        <div class="col-2 text-center">
            <span class="d-block d-md-inline-block"><b>This Week</b>:</span>
            <asp:Literal runat="server" ID="ResponseThisWeek" />
        </div>
        <div class="col-2 text-center">
            <span class="d-block d-md-inline-block"><b>Last Week</b>:</span>
            <asp:Literal runat="server" ID="ResponseLastWeek" />
        </div>
        <div class="col-2 text-center">
            <span class="d-block d-md-inline-block"><b>This Month</b>:</span>
            <asp:Literal runat="server" ID="ResponseThisMonth" />
        </div>
        <div class="col-2 text-center">
            <span class="d-block d-md-inline-block"><b>Last Month</b>:</span>
            <asp:Literal runat="server" ID="ResponseLastMonth" />
        </div>
        <div class="col-2 text-center">
            <span class="d-block d-md-inline-block"><b>Overall</b>:</span>
            <asp:Literal runat="server" ID="ResponseOverall" />
        </div>
    </div>

    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />
    <insite:UpdatePanel runat="server" ID="UpdatePanel">
        <ContentTemplate>
            <div runat="server" id="GranularityPanel" class="mt-5">
                <div class="text-center chart-selector">
                    <button type="button" class="btn btn-default btn-sm" data-action="daily">Daily</button>
                    <button type="button" class="btn btn-default btn-sm" data-action="weekly">Weekly</button>
                    <button type="button" class="btn btn-default btn-sm" data-action="monthly">Monthly</button>
                </div>

                <div class="text-center mt-3 chart-date-selector">
                    <div class="d-inline-block">
                        <div class="input-group">
                            <insite:Button runat="server" ID="PagePrevButton" Icon="fas fa-chevron-left" ButtonStyle="OutlineSecondary" />
                            <insite:TextBox runat="server" ID="PageTitle" ReadOnly="true" CssClass="text-center bg-white form-control-sm" />
                            <insite:Button runat="server" ID="PageNextButton" Icon="fas fa-chevron-right" ButtonStyle="OutlineSecondary" />
                        </div>
                    </div>
                </div>
            </div>

            <div runat="server" id="ChartContainer" class="mt-3">
                <div class="mb-2 text-center zoom-selector mb-3">
                    <button type="button" class="btn btn-outline-secondary btn-sm" data-action="zoom-in"><i class="fa fa-search-plus me-1"></i>Zoom In</button>
                    <button type="button" class="btn btn-outline-secondary btn-sm" data-action="zoom-out"><i class="fa fa-search-minus me-1"></i>Zoom Out</button>
                    <button type="button" class="btn btn-outline-secondary btn-sm" data-action="zoom-actual"><i class="fa fa-search me-1"></i>Actual Size</button>
                </div>
                <div class="chart-window">
                    <div class="chart-container" style="width:100%;">
                        <asp:HiddenField runat="server" ID="StatisticData" ViewStateMode="Disabled" />
                        <chart:BarChart runat="server" ID="StatisticChart" LegendVisible="false" MaintainApectRatio="false" ToolTipIntersect="false" ToolTipInteractionMode="X" />
                    </div>
                </div>               
            </div>
        </ContentTemplate>
    </insite:UpdatePanel>

</asp:PlaceHolder>

<insite:PageFooterContent runat="server">

    <script type="text/javascript">

        (function () {
            var resultsChart = window.resultsChart = window.resultsChart || {
                onActiveChartChanged: null
            };

            var chartId = '<%= StatisticChart.ClientID %>';
            var chartDataId = '<%= StatisticData.ClientID %>';

            var chartData = null;
            var $granularityPanel = null;

            Sys.Application.add_load(onLoad);

            function onLoad() {
                var $dataInput = $(document.getElementById(chartDataId));
                if ($dataInput.data('inited') == true)
                    return;

                chartData = $dataInput.val();

                if (chartData)
                    chartData = JSON.parse(chartData);
                else
                    chartData = null;

                $('#<%= ChartContainer.ClientID %> [data-action="zoom-in"]').on('click', function () {
                    zoomChart('in');
                });

                $('#<%= ChartContainer.ClientID %> [data-action="zoom-out"]').on('click', function () {
                    zoomChart('out');
                });

                $('#<%= ChartContainer.ClientID %> [data-action="zoom-actual"]').on('click', function () {
                    zoomChart('actual');
                });

                $granularityPanel = $('#<%= GranularityPanel.ClientID %>');

                $granularityPanel.find('button').on('click', function () {
                    setActiveChart($(this).data('action'));
                });

                inSite.common.chart.onInited(chartId, initActiveChart);

                $dataInput.data('inited', true);
            }

            // Chart zooming

            function zoomChart(action) {
                var chart = inSite.common.chart.getInstance(chartId);
                if (chart == null)
                    return;

                var $container = $(chart.canvas).closest('div.chart-container');
                var $window = $container.closest('div.chart-window');
                var zoom = Math.ceil($container.width() / $window.width());

                if (zoom <= 0)
                    zoom = 1;

                if (action == 'in') {
                    if (zoom < 10)
                        zoom += 1;
                } else if (action == 'out') {
                    if (zoom > 1)
                        zoom -= 1;
                } else if (action == 'actual') {
                    zoom = 1;
                }

                $container.width(String(zoom * 100) + '%');
            }

            // Granularity

            function initActiveChart() {
                var value = null;

                try {
                    value = window.localStorage.getItem('assessment.resultChart.TimeSeries');
                } catch (e) {

                }

                if (value == null || typeof value !== 'string')
                    value = 'daily';

                setActiveChart(value);
            }

            function setActiveChart(value) {
                $granularityPanel
                    .find('button.active').removeClass('active')
                    .end()
                    .find('button[data-action="' + value + '"]').addClass('active');

                try {
                    window.localStorage.setItem('assessment.resultChart.TimeSeries', value);
                } catch (e) {

                }

                if (chartData == null || !chartData.hasOwnProperty(value))
                    return;

                var chart = inSite.common.chart.getInstance(chartId);
                if (chart == null)
                    return;

                chart.data = inSite.common.cloneObj(chartData[value]);
                chart.update();

                if (typeof resultsChart.onActiveChartChanged === 'function')
                    resultsChart.onActiveChartChanged(value);
            }
        })();

    </script>

</insite:PageFooterContent>
