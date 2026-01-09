<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportDistributionAnalysisSelection.ascx.cs" Inherits="InSite.Admin.Workflow.Forms.Controls.ReportDistributionAnalysisSelection" %>

<insite:PageHeadContent runat="server" ID="CommonStyle">
    <style type="text/css">

        .table-selection-analysis {

        }

            .table-selection-analysis > tbody .option-text {
                padding-left: 25px;
            }

                .table-selection-analysis > tbody .option-text > span {
                    position: absolute;
                    margin-left: -20px;
                }

            .table-selection-analysis > tfoot tr.total th {
                border-top: solid 1px #000;
                border-bottom: solid 1px #000;
            }

    </style>
</insite:PageHeadContent>

<div class="text-end cmd-selection-analysis" style="margin-bottom:5px;">
    <insite:Button runat="server" ID="DownloadCsv" ButtonStyle="Primary" Size="ExtraSmall" ToolTip="Download CSV" Text="CSV" Icon="fa fa-download" />
    <insite:Button runat="server" ID="DownloadPng" ButtonStyle="Primary" Size="ExtraSmall" ToolTip="Download PNG" Text="PNG" Icon="fa fa-download" />
</div>

<div>
    <chart:BarChart runat="server" ID="Chart" Width="100%" LegendVisible="false" />
</div>

<table class="table table-condensed table-selection-analysis">
    <thead>
        <tr>
            <th><asp:Literal runat="server" ID="EntityName" /></th>
            <th style="width:80px;" class="text-end">Frequency</th>
            <th style="width:80px;" class="text-end">Relative %</th>
            <th style="width:80px;" class="text-end">Valid %</th>
        </tr>
    </thead>
    <tbody>
        <asp:Repeater runat="server" ID="OptionRepeater">
            <ItemTemplate>
                <tr>
                    <td class="option-text">
                        <span><asp:Literal runat="server" ID="Label" />.</span>
                        <asp:Literal runat="server" ID="Text" Text='<%# Eval("Text") %>' />
                    </td>
                    <td class="text-end">
                        <asp:Literal runat="server" ID="Frequency" Text='<%# Eval("Frequency") %>' />
                    </td>
                    <td class="text-end">
                        <asp:Literal runat="server" ID="Relative" Text='<%# FormatPercent(Eval("Relative")) %>' />
                    </td>
                    <td class="text-end">
                        <asp:Literal runat="server" ID="Valid" Text='<%# FormatPercent(Eval("Valid")) %>' />
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
        <asp:PlaceHolder runat="server" ID="RowNoResponse" Visible="false">
            <tr>
		        <td class="option-text">No Submission</td>
                <td class="text-end"><asp:Literal runat="server" ID="RowNoResponseFrequency" /></td>
                <td class="text-end"><asp:Literal runat="server" ID="RowNoResponseRelative" /></td>
                <td class="text-end">&nbsp;</td>
            </tr>
        </asp:PlaceHolder>
    </tbody>
    <tfoot>
        <asp:PlaceHolder runat="server" ID="RowTotal" Visible="false">
            <tr class="total">
                <th>Totals</th>
                <th class="text-end"><asp:Literal runat="server" ID="RowTotalFrequency" /></th>
                <th class="text-end">100%</th>
                <th class="text-end">100%</th>
            </tr>
        </asp:PlaceHolder>

        <asp:PlaceHolder runat="server" ID="RowValid" Visible="false">
            <tr>
                <td>Valid Submissions</td>
                <td colspan="3"><asp:Literal runat="server" ID="RowValidText" /></td>
            </tr>
        </asp:PlaceHolder>

        <asp:PlaceHolder runat="server" ID="RowNumericAnalysis" Visible="false">
            <tr>
                <td>Mean</td>
                <td colspan="3"><asp:Literal runat="server" ID="RowNumericAnalysisMean" /></td>
            </tr>
            
            <tr>
                <td>Standard Deviation</td>
                <td colspan="3"><asp:Literal runat="server" ID="RowNumericAnalysisStandardDeviation" /></td>
            </tr>
        </asp:PlaceHolder>
    </tfoot>
</table>

<insite:PageFooterContent runat="server" ID="CommonScript">
    <script type="text/javascript">
        
        (function () {
            const instance = window.selectionAnalysis = window.selectionAnalysis || {};

            instance.onVerticalChartInit = function (canvas, config) {
                inSite.common.setObjProp(config, 'options.scales.y', {
                    display: true,
                    beginAtZero: true,
                    max: 100,
                    ticks: {
                        precision: 1,
                        callback: function (value, index, values) {
                            return String(value) + ' %';
                        }
                    }
                });
                inSite.common.setObjProp(config, 'options.plugins.tooltip.displayColors', false);
                inSite.common.setObjProp(config, 'options.plugins.tooltip.callbacks.title', function () {
                    return null;
                });
                inSite.common.setObjProp(config, 'options.plugins.tooltip.callbacks.label', function (item, data) {
                    return item.label + ': ' + parseFloat(item.raw).toFixed(1) + ' %';
                });
            };

            instance.onHorizontalChartInit = function (canvas, config) {
                config.options.indexAxis = 'y';
                inSite.common.setObjProp(config, 'options.scales.x', {
                    display: true,
                    beginAtZero: true,
                    max: 100,
                    ticks: {
                        precision: 1,
                        callback: function (value, index, values) {
                            return String(value) + ' %';
                        }
                    }
                });
                inSite.common.setObjProp(config, 'options.plugins.tooltip.displayColors', false);
                inSite.common.setObjProp(config, 'options.plugins.tooltip.callbacks.title', function () {
                    return null;
                });
                inSite.common.setObjProp(config, 'options.plugins.tooltip.callbacks.label', function (item, data) {
                    return item.label + ': ' + parseFloat(item.raw).toFixed(1) + ' %';
                });
            };

            instance.onDownloadPng = function (chartId) {
                const chart = inSite.common.chart.getInstance(chartId);
                const pxRatio = chart.options.devicePixelRatio;
                const parent = chart.canvas.parentNode;

                parent.style.height = String(parent.offsetHeight) + 'px';

                chart.options.devicePixelRatio = 2;
                chart.resize(600, 300);

                const $input = $('<input type="hidden" name="img-data">').val(chart.toBase64Image());

                $('form').append($input);

                setTimeout(function () { $input.remove(); }, 100, $input);

                chart.options.devicePixelRatio = pxRatio;
                chart.resize();

                parent.style.height = null;
            }
        })();

    </script>
</insite:PageFooterContent>