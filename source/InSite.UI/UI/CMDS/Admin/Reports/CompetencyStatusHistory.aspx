<%@ Page Language="C#" CodeBehind="CompetencyStatusHistoryChart.ascx.cs" Inherits="InSite.Cmds.Actions.Reporting.Report.CompetencyStatusHistoryChart" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
        #report-container {
        }

            #report-container .report-item {
            }

                #report-container .report-item + .report-item {
                    margin-top: 80px;
                }

                #report-container .report-item .title {
                    text-align: center;
                    margin-bottom: 20px;
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

    <insite:CustomValidator runat="server" ID="DepartmentIdentifierValidator" ErrorMessage="At least one department must be selected" Display="None" ValidationGroup="Report" />

    <insite:Nav runat="server" ID="NavPanel">
        <insite:NavItem runat="server" ID="CriteriaTab" Title="Criteria" Icon="far fa-search" IconPosition="BeforeText">

            <h2 class="h4 my-3">
                Criteria
            </h2>

            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="ReportUpdatePanel" />
                
                    <insite:UpdatePanel runat="server" ID="ReportUpdatePanel">
                        <Triggers>
                            <asp:PostBackTrigger ControlID="ReportButton" />
                        </Triggers>
                        <ContentTemplate>

                            <div class="row">
                                <div class="col-lg-4">

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Departments
                                        </label>
                                        <cmds:FindDepartment runat="server" ID="DepartmentIdentifier" MaxSelectionCount="0" />
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Learners
                                        </label>
                                        <cmds:FindPerson runat="server" ID="EmployeeIdentifier" />
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Since
                                            <insite:RequiredValidator runat="server" ControlToValidate="DateSince" FieldName="Since" ValidationGroup="Report" Display="Dynamic" />
                                        </label>
                                        <div>
                                            <insite:DateSelector ID="DateSince" runat="server" />
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Before
                                            <insite:RequiredValidator runat="server" ControlToValidate="DateBefore" FieldName="Before" ValidationGroup="Report" Display="Dynamic" />
                                        </label>
                                        <div>
                                            <insite:DateSelector ID="DateBefore" runat="server" />
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <asp:CheckBox ID="IsSingleChart" runat="server" Text="Aggregate competency statuses into a single chart" />
                                    </div>

                                </div>
                                <div class="col-lg-3 offset-lg-1">

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Statuses
                                        </label>
                                        <div>
                                            <asp:CheckBoxList ID="StatusVisibility" runat="server">
                                                <asp:ListItem Value="expired" Text="Expired" Selected="True" />
                                                <asp:ListItem Value="not_completed" Text="Not Completed" Selected="True" />
                                                <asp:ListItem Value="not_applicable" Text="Not Applicable" Selected="True" />
                                                <asp:ListItem Value="needs_training" Text="Needs Training" Selected="True" />
                                                <asp:ListItem Value="self_assessed" Text="Self-Assessed" Selected="True" />
                                                <asp:ListItem Value="submitted" Text="Submitted for Validation" Selected="True" />
                                                <asp:ListItem Value="validated" Text="Validated" Selected="True" />
                                            </asp:CheckBoxList>
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Profiles
                                        </label>
                                        <div>
                                            <asp:RadioButtonList ID="Option" runat="server" RepeatLayout="Flow">
                                                <asp:ListItem Value="1" Text="Only Primary Profiles" />
                                                <asp:ListItem Value="2" Text="Profiles That Require Compliance" />
                                                <asp:ListItem Value="3" Text="All Profiles" Selected="True" />
                                            </asp:RadioButtonList>
                                        </div>
                                    </div>

                                </div>
                                <div class="col-lg-3">

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Chart Type
                                        </label>
                                        <div>
                                            <asp:RadioButtonList ID="ChartType" runat="server" RepeatLayout="Flow">
                                                <asp:ListItem Value="Lines" Text="Lines" Selected="True" />
                                                <asp:ListItem Value="StackedBars" Text="Stacked Bars" />
                                                <asp:ListItem Value="StackedArea" Text="Stacked Area" />
                                            </asp:RadioButtonList>
                                        </div>
                                    </div>

                                </div>
                            </div>

                            <div class="mt-3">
                                <insite:SearchButton runat="server"
                                    ID="ReportButton"
                                    Text="Report"
                                    Icon="fas fa-chart-bar"
                                    ValidationGroup="Report"
                                    CausesValidation="true"
                                    DisableAfterClick="true"
                                />
                            </div>

                        </ContentTemplate>
                    </insite:UpdatePanel>

                </div>
            </div>

        </insite:NavItem>
        <insite:NavItem runat="server" ID="ReportTab" Title="Report" Icon="far fa-chart-bar" IconPosition="BeforeText" Visible="false">

            <h2 class="h4 my-3">
                Report
            </h2>

            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <div class="mb-3">
                        <insite:DownloadButton runat="server"
                            ID="DownloadPdf"
                            Text="Download PDF"
                            DisableAfterClick="true"
                            EnableAfter="10000"
                        />
                        <asp:HiddenField runat="server" ID="ReportHtmlContent" ViewStateMode="Disabled" />
                    </div>

                    <div id="report-container">
                        <asp:Repeater runat="server" ID="ChartRepeater">
                            <ItemTemplate>
                                <div class="report-item" style="page-break-inside:avoid;">
                                    <div class="title">
                                        <h1><%# Eval("Title") %></h1>
                                        <h2><%# Eval("Employee") %></h2>
                                    </div>

                                    <div>
                                        <asp:PlaceHolder runat="server" ID="ChartPlaceHolder"></asp:PlaceHolder>
                                    </div>
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

    <insite:PageFooterContent runat="server" ID="FooterScript">
        <script type="text/javascript">

            (function () {
                var report = window.report = window.report || {};
                var isStackedBar = <%= CurrentParameters != null && CurrentParameters.ChartType == ChartTypeEnum.StackedBars ? "true" : "false" %>;
                var isStackedArea = <%= CurrentParameters != null && CurrentParameters.ChartType == ChartTypeEnum.StackedArea ? "true" : "false" %>;

                report.onChartPreInit = function (canvas, config) {
                    inSite.common.setObjProp(config, 'options.scales.y', {
                        display: true,
                        beginAtZero: true,
                        max: 100,
                        stacked: isStackedBar || isStackedArea,
                        title: {
                            display: true,
                            text: 'Percentage of Competencies'
                        },
                        ticks: {
                            precision: 1,
                            callback: function (value, index, values) {
                                return parseFloat(value).toFixed(0) + '%';
                            }
                        }
                    });

                    var dataset = config.data.datasets[0];
                    var months = moment(dataset.data[dataset.data.length - 1].x).diff(moment(dataset.data[0].x), 'months', true);

                    inSite.common.setObjProp(config, 'options.scales.x', {
                        stacked: isStackedBar,
                        type: 'time',
                        time: {
                            unit: months > 6 ? 'month' : 'day',
                            minUnit: 'day',
                            tooltipFormat: 'MMM D, YYYY'
                        },
                        offset: isStackedBar
                    });
                };

                report.onChartTooltipLabelCallback = function (item, data) {
                    return item.dataset.label + ': ' + String(item.raw.y.toFixed(0)) + '%';
                };
            })();

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