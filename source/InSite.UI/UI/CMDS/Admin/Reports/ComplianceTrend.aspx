<%@ Page CodeBehind="ComplianceTrend.aspx.cs" Inherits="InSite.Cmds.Admin.Reports.Forms.ComplianceTrendReport" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
        .report-item + .report-item {
            margin-top: 60px;
        }

        .report-item table.table-trend th {
            font-size: 0.85em;
            white-space: nowrap;
        }

        .report-item table.table-trend td.data-column {
            text-align: center;
            white-space: nowrap;
        }

        .report-item table.table-trend td.item-column {
            white-space: nowrap;
        }

        #pdf-report-container {
            width: 0;
            height: 0;
            visibility: hidden;
        }
    </style>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <insite:ValidationSummary runat="server" ValidationGroup="Report" />

    <insite:Nav runat="server" ID="NavPanel">
        <insite:NavItem runat="server" ID="CriteriaTab" Title="Criteria" Icon="far fa-search" IconPosition="BeforeText">

            <div class="row">
                <div class="col-lg-9">

                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="ReportUpdatePanel" />

                    <insite:UpdatePanel runat="server" ID="ReportUpdatePanel">
                        <Triggers>
                            <asp:PostBackTrigger ControlID="ReportButton" />
                            <asp:PostBackTrigger ControlID="DownloadXlsxFromCriteria" />
                        </Triggers>
                        <ContentTemplate>
                            <div class="card border-0 shadow-lg">
                                <div class="card-body">

                                    <div class="row">
                                        <div class="col-lg-6">

                                            <div class="form-group mb-3">
                                                <label class="form-label">
                                                    Departments
                                                    <insite:RequiredValidator runat="server" ControlToValidate="DepartmentIdentifier" FieldName="Departments" ValidationGroup="Report" />
                                                </label>
                                                <cmds:FindDepartment runat="server" ID="DepartmentIdentifier" MaxSelectionCount="20" GroupByDivision="true" />
                                            </div>

                                            <div class="form-group mb-3">
                                                <label class="form-label">
                                                    Achievement Type
                                                    <insite:RequiredValidator runat="server" ControlToValidate="AchievementType" FieldName="Achievement Type" ValidationGroup="Report" />
                                                </label>
                                                <insite:MultiComboBox runat="server" ID="AchievementType" AllowBlank="false" Multiple-ActionsBox="true" Multiple-CountAllFormat="All Achievement Types" />
                                            </div>

                                        </div>
                                        <div class="col-lg-6">

                                            <div class="form-group mb-3">
                                                <label class="form-label">
                                                    Period
                                                </label>
                                                <div>
                                                    <insite:RadioButtonList ID="MonthCount" runat="server">
                                                        <asp:ListItem Value="12" Text="Past 12 months" Selected="True" />
                                                        <asp:ListItem Value="24" Text="Past 24 months" />
                                                    </insite:RadioButtonList>
                                                </div>
                                            </div>

                                            <div class="form-group mb-3">
                                                <label class="form-label">
                                                    Profiles
                                                </label>
                                                <div>
                                                    <insite:RadioButtonList ID="Option" runat="server">
                                                        <asp:ListItem Value="2" Text="Profiles that require compliance" Selected="True" />
                                                        <asp:ListItem Value="1" Text="Only primary profiles" />
                                                    </insite:RadioButtonList>
                                                </div>
                                                <div class="form-text">
                                                    Monthly figures come from the weekly compliance snapshot, which counts department employment only, so they will not match the Compliance Summary report exactly
                                                </div>
                                            </div>

                                        </div>
                                    </div>

                                </div>
                            </div>

                            <div class="mt-4">
                                <insite:SearchButton runat="server" ID="ReportButton" Text="Report" Icon="fas fa-chart-line" ValidationGroup="Report" CausesValidation="true" />
                                <insite:DownloadButton runat="server" ID="DownloadXlsxFromCriteria" Text="Download XLSX" ValidationGroup="Report" CausesValidation="true" />
                            </div>

                        </ContentTemplate>
                    </insite:UpdatePanel>

                </div>
            </div>

        </insite:NavItem>
        <insite:NavItem runat="server" ID="ReportTab" Title="Report" Icon="far fa-chart-bar" IconPosition="BeforeText" Visible="false">

            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <asp:Panel runat="server" ID="DownloadCommandsPanel" CssClass="mb-3">
                        <insite:DownloadButton runat="server" ID="DownloadXlsxFromReport" Text="Download XLSX" />
                        <insite:DownloadButton runat="server" ID="DownloadPdf" Text="Download PDF" DisableAfterClick="true" EnableAfter="10000" />
                        <asp:HiddenField runat="server" ID="ReportHtmlContent" ViewStateMode="Disabled" />
                    </asp:Panel>

                    <div id="report-container">
                    <asp:Repeater runat="server" ID="DepartmentRepeater">
                        <ItemTemplate>
                            <div class="report-item" style="page-break-inside:avoid;">

                                <h2 class="h5 mb-1"><%# Eval("DepartmentName") %></h2>
                                <div class="text-muted mb-3"><%# Eval("CompanyName") %></div>

                                <div runat="server" id="ChartContainer" class="chart-container mb-4">
                                    <chart:LineChart runat="server" ID="DepartmentChart"
                                        DatasetType="DateTime"
                                        ToolTipIntersect="false"
                                        OnClientPreInit="report.onChartPreInit"
                                        Height="320px"
                                        MaintainApectRatio="false"
                                    />
                                </div>

                                <div runat="server" id="NoScoreNote" class="text-muted mb-4" visible="false">
                                    No compliance requirements were recorded for this department in this period
                                </div>

                                <asp:Literal runat="server" ID="ScoreTable" Mode="PassThrough" />

                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    </div>

                </div>
            </div>

        </insite:NavItem>
    </insite:Nav>

    <div id="pdf-report-container">

    </div>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">

            (function () {
                var report = window.report = window.report || {};

                report.onChartPreInit = function (canvas, config) {
                    inSite.common.setObjProp(config, 'options.scales.y', {
                        display: true,
                        beginAtZero: true,
                        max: 100,
                        title: {
                            display: true,
                            text: 'Compliance score'
                        },
                        ticks: {
                            precision: 1,
                            callback: function (value) {
                                return parseFloat(value).toFixed(0) + '%';
                            }
                        }
                    });

                    inSite.common.setObjProp(config, 'options.scales.x', {
                        type: 'time',
                        time: {
                            unit: 'month',
                            minUnit: 'month',
                            tooltipFormat: 'MMM YYYY'
                        }
                    });
                };

                report.onChartTooltipLabelCallback = function (item) {
                    return item.dataset.label + ': ' + String(item.raw.y.toFixed(0)) + '%';
                };
            })();

        </script>

        <script type="text/javascript">

            // wkhtmltopdf does not run Chart.js, so each canvas is swapped for a still image of itself before
            // the report markup is posted back for conversion.
            (function () {
                $('#<%= DownloadPdf.ClientID %>').on('click', function (e) {
                    var $pdfContainer = $('#pdf-report-container');
                    var $report = $('#report-container')
                        .clone()
                        .attr('id', null)
                        .find('canvas').each(function () {
                            $(this).parent().addClass('chart-container').data('chartId', this.id).empty();
                        }).end();

                    $pdfContainer.append($report);

                    $report.find('.chart-container').each(function () {
                        var $container = $(this);
                        var chart = inSite.common.chart.getInstance($container.data('chartId'));
                        if (!chart)
                            return;

                        const pxRatio = chart.options.devicePixelRatio;
                        const parent = chart.canvas.parentNode;

                        parent.style.height = String(parent.offsetHeight) + 'px';

                        chart.options.devicePixelRatio = 2;
                        chart.resize(1200, 600);

                        $container.append($('<img>').attr('alt', '').attr('src', chart.toBase64Image()))

                        chart.options.devicePixelRatio = pxRatio;
                        chart.resize();

                        parent.style.height = null;
                    });

                    $('#<%= ReportHtmlContent.ClientID %>').val($report.html());

                    $pdfContainer.empty();
                });
            })();

        </script>
    </insite:PageFooterContent>

</asp:Content>
