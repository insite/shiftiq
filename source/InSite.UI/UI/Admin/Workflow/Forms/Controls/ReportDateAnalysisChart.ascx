<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportDateAnalysisChart.ascx.cs" Inherits="InSite.Admin.Workflow.Forms.Controls.ReportDateAnalysisChart" %>

<%@ Import Namespace="Shift.Common" %>

<insite:PageHeadContent runat="server">
    <style type="text/css">
        .chart-row {
            margin: 15px 0;
            padding: 0 5px 0 5px;
            position: relative;
        }

            .chart-row > .chart-window {
                overflow-x: auto;
            }

                .chart-row > .chart-window > .chart-container {
                    height: 400px;
                    position: relative;
                }

            .chart-row > .zoom-panel {
                position: absolute;
                top: 6px;
                right: 11px;
                background-color: rgba(255,255,255,0.75);
                border: solid 1px #eeeeee;
                padding: 3px 5px;
                border-radius: 2px;
                width: auto;
            }

                .chart-row > .zoom-panel > button {
                    border: 0;
                    background: none;
                    padding: 0;
                    opacity: 0.5;
                }

                    .chart-row > .zoom-panel > button + button {
                        margin-left: 3px;
                    }

                .chart-row > .zoom-panel:hover > button {
                    opacity: 1;
                }
    </style>
</insite:PageHeadContent>

<div runat="server" id="NoResponsesMessage" visible="false">
    No submissions to this form have been submitted.
</div>

<asp:PlaceHolder runat="server" ID="StatisticInfo" Visible="false">

    <div class="row">

        <div class="col-lg-2 text-center">
            <span class="d-block d-md-inline-block"><b>Today</b>:</span>
            <asp:Literal runat="server" ID="ResponseToday" />
        </div>

        <div class="col-lg-2 text-center">
            <span class="d-block d-md-inline-block"><b>This Week</b>:</span>
            <asp:Literal runat="server" ID="ResponseThisWeek" />
        </div>

        <div class="col-lg-2 text-center">
            <span class="d-block d-md-inline-block"><b>Last Week</b>:</span>
            <asp:Literal runat="server" ID="ResponseLastWeek" />
        </div>

        <div class="col-lg-2 text-center">
            <span class="d-block d-md-inline-block"><b>This Month</b>:</span>
            <asp:Literal runat="server" ID="ResponseThisMonth" />
        </div>

        <div class="col-lg-2 text-center">
            <span class="d-block d-md-inline-block"><b>Last Month</b>:</span>
            <asp:Literal runat="server" ID="ResponseLastMonth" />
        </div>

        <div class="col-lg-2 text-center">
            <span class="d-block d-md-inline-block"><b>Overall</b>:</span>
            <asp:Literal runat="server" ID="ResponseOverall" />
        </div>

    </div>

    <div runat="server" id="GranularityPanel" class="row" style="margin: 15px 0;">
        <div class="col-xs-12 text-center">
            <button type="button" class="btn btn-default" data-action="daily">Daily</button>
            <button type="button" class="btn btn-default" data-action="weekly">Weekly</button>
            <button type="button" class="btn btn-default" data-action="monthly">Monthly</button>
        </div>
    </div>

    <div class="row chart-row">
        <div class="chart-window">
            <div class="chart-container" style="width: 100%;">
                <chart:BarChart runat="server" ID="StatisticChart" LegendVisible="false" MaintainApectRatio="false" ToolTipIntersect="false" ToolTipInteractionMode="X" />
            </div>
        </div>

        <div class="zoom-panel">
            <button type="button" title="Zoom In" data-action="zoom-in"><i class="fa fa-search-plus"></i></button>
            <button type="button" title="Zoom Out" data-action="zoom-out"><i class="fa fa-search-minus"></i></button>
            <button type="button" title="Actual Size" data-action="zoom-actual"><i class="fa fa-search"></i></button>
        </div>
    </div>

</asp:PlaceHolder>

<insite:PageFooterContent runat="server">

    <script type="text/javascript">

        (function () {
            var chartId = '<%= StatisticChart.ClientID %>';
            var chartData = <%= DataSource == null ? "null" : JsonHelper.SerializeJsObject(DataSource) %>;

            var resultsChart = window.resultsChart = window.resultsChart || {
                onActiveChartChanged: null
            };

            // Chart zooming

            $('[data-action="zoom-in"]').on('click', function () {
                zoomChart('in');
            });

            $('[data-action="zoom-out"]').on('click', function () {
                zoomChart('out');
            });

            $('[data-action="zoom-actual"]').on('click', function () {
                zoomChart('actual');
            });

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

            var $granularityPanel = $('#<%= GranularityPanel.ClientID %>');

            $granularityPanel.find('button').on('click', function () {
                setActiveChart($(this).data('action'));
            });

            function initActiveChart() {
                var value = null;

                try {
                    value = window.localStorage.getItem('survey.resultChart.<%= SurveyID %>');
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
                    window.localStorage.setItem('survey.resultChart.<%= SurveyID %>', value);
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

            inSite.common.chart.onInited(chartId, initActiveChart);
        })();

    </script>

</insite:PageFooterContent>
