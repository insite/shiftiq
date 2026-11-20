<%@ Page CodeBehind="ComplianceSummary.aspx.cs" Inherits="InSite.Cmds.Admin.Reports.Forms.ComplianceSummaryReport" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
        .insite-findentity.fe-empty.fe-not-empty > button > div > div {
            color: #737491;
        }

        #pdf-report-container {
            width: 0;
            height: 0;
            visibility: hidden;
        }

        .report-item table > tbody > tr.odd td {
            --bs-table-accent-bg: var(--bs-table-striped-bg);
            color: var(--bs-table-striped-color);
        }

        .report-item table.table-current.table-without-status .empty-column {
            width: 215px;
        }

        .report-item table.table-current.table-without-status .data-column {
            width: 95px;
        }

        .report-item table.table-current.table-without-status.table-employee thead tr.table-header td,
        .report-item table.table-current.table-with-status.table-employee thead tr.table-header td {
            border-top: 1px solid rgba(55, 56, 78, 0.35);
            border-bottom: 1px solid rgba(55, 56, 78, 0.35);
            font-weight: bold;
            font-size: 0.9em;
        }

        .report-item table.table-current.table-with-status th.data-column {
            width: 85px;
            font-size: 0.9em;
        }

        .report-item table.table-current.table-with-status td.data-column {
            width: 85px;
        }
    </style>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <insite:ValidationSummary runat="server" ValidationGroup="Report" />
    <insite:ValidationSummary runat="server" ValidationGroup="CompanySummary" />

    <insite:CustomValidator runat="server" ID="EmployeeIdentifierValidataor" ErrorMessage="At least one employee must be selected" Display="None" ValidationGroup="Report" />

    <insite:Nav runat="server" ID="NavPanel">
        <insite:NavItem runat="server" ID="CriteriaTab" Title="Criteria" Icon="far fa-search" IconPosition="BeforeText">

            <div class="row">
                <div class="col-lg-9">

                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="ReportUpdatePanel" />
                
                    <insite:UpdatePanel runat="server" ID="ReportUpdatePanel">
                        <Triggers>
                            <asp:PostBackTrigger ControlID="ReportButton" />
                            <asp:PostBackTrigger ControlID="PreDownloadXlsx1" />
                            <asp:PostBackTrigger ControlID="PreDownloadCompanyXlsx" />
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
                                                    Membership
                                                </label>
                                                <div>
                                                    <insite:CheckBoxList runat="server" ID="MembershipType">
                                                        <asp:ListItem Value="Department" Text="Department Employment" Selected="True" />
                                                        <asp:ListItem Value="Organization" Text="Organization Employment" />
                                                        <asp:ListItem Value="Administration" Text="Data Access" />
                                                    </insite:CheckBoxList>
                                                    <asp:Button runat="server" ID="MembershipTypeChanged" CssClass="d-none" />
                                                </div>
                                            </div>

                                            <div class="form-group mb-3">
                                                <label class="form-label">
                                                    Employees
                                                </label>
                                                <cmds:FindPerson runat="server" ID="EmployeeIdentifier" MaxSelectionCount="0" />
                                            </div>

                                            <div class="form-group mb-3">
                                                <label class="form-label">
                                                    Achievement Type
                                                    <insite:RequiredValidator runat="server" ControlToValidate="AchievementType" FieldName="Achievement Type" ValidationGroup="Report" />
                                                    <insite:RequiredValidator runat="server" ControlToValidate="AchievementType" FieldName="Achievement Type" ValidationGroup="CompanySummary" RenderMode="Exclamation" />
                                                </label>
                                                <insite:MultiComboBox runat="server" ID="AchievementType" AllowBlank="false" Multiple-ActionsBox="true" Multiple-CountAllFormat="All Achievement Types" />
                                            </div>

                                        </div>
                                        <div class="col-lg-6">

                                            <div class="form-group mb-3">
                                                <label class="form-label">
                                                    Options
                                                </label>
                                                <div>
                                                    <insite:RadioButtonList ID="DisplayMode" runat="server">
                                                        <asp:ListItem Value="0" Text="Show department and worker summary" Selected="True" />
                                                        <asp:ListItem Value="1" Text="Show department summary only" />
                                                    </insite:RadioButtonList>
                                                </div>
                                                <div class="mt-2">
                                                    <insite:CheckBox runat="server" ID="IsShowStatus" Text="Show the count per validation status" />
                                                    <insite:CheckBox runat="server" ID="IsShowChart" Text="Show chart" Checked="true" />
                                                    <insite:CheckBox runat="server" ID="IsIncludeUsersWithoutProfile" Text="Show users without a profile assignment" />
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
                                                        <asp:ListItem Value="3" Text="All profiles" />
                                                    </insite:RadioButtonList>
                                                </div>
                                            </div>

                                        </div>
                                    </div>

                                </div>
                            </div>

                            <div class="mt-4">
                                <insite:SearchButton runat="server" ID="ReportButton" Text="View report" Icon="fas fa-chart-bar" ValidationGroup="Report" CausesValidation="true" />
                                <insite:DownloadButton runat="server" ID="PreDownloadXlsx1" Text="Download report" ValidationGroup="Report" CausesValidation="true" />
                                <insite:DownloadButton runat="server" ID="PreDownloadCompanyXlsx" Text="Download company summary" ValidationGroup="CompanySummary" CausesValidation="true" />
                                <div class="form-text">
                                    The company summary covers all departments you can access, whatever is selected above
                                </div>
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
                        <insite:DownloadButton runat="server" ID="PreDownloadXlsx2" Text="Download XLSX" />
                        <insite:DownloadButton runat="server" ID="DownloadPdf" Text="Download PDF" />
                        <asp:HiddenField runat="server" ID="ReportHtmlContent" ViewStateMode="Disabled" />
                    </asp:Panel>

                    <div class="row mb-3">
                        <div class="col-lg-3 col-4">
                            <ul aria-orientation="vertical" class="nav nav-pills flex-column" role="tablist">

                                <asp:Repeater runat="server" ID="DepartmentRepeater">
                                    <ItemTemplate>
                                        <li class="nav-item" role="presentation">
                                            <asp:LinkButton runat="server"
                                                CssClass='<%# "nav-link text-center" + ((Guid)Eval("DepartmentIdentifier") == CurrentDepartmentIdentifier ? " active" : "") %>'
                                                Text='<%# Eval("DepartmentName") + " (" + Eval("UserCount") + ")" %>'
                                                CommandName="Show" CommandArgument='<%# Eval("DepartmentIdentifier") %>'
                                            />
                                        </li>
                                    </ItemTemplate>
                                </asp:Repeater>

                            </ul>
                        </div>
                        <div class="col-lg-9 col-8 col-report">

                            <div runat="server" id="DepartmentNoDataAlert" class="alert alert-warning" visible="false">
                                <i class="fas fa-exclamation-triangle"></i> 
                                There is no compliance summary data for employees in this department that matches your search criteria.
                            </div>

                            <asp:Repeater runat="server" ID="CurrentDepartmentRepeaterWithoutStatuses">
                                <ItemTemplate>
                                    <div class="report-item mb-5" style="page-break-after:always;">
                                        <table class="table table-current table-without-status table-department" style="page-break-inside:avoid;">
                                            <thead>
                                                <tr><th colspan="6" style="font-size:1.8em;"></th></tr>
                                                <tr><th colspan="6"><%# ReportParameters.ReportTitle %></th></tr>
                                                <tr>
                                                    <th>Compliance Item</th>
                                                    <th class="text-center data-column">N/A</th>
                                                    <th class="text-center data-column">Submitted</th>
                                                    <th class="text-center data-column">Validated</th>
                                                    <th class="text-center data-column">Required</th>
                                                    <th class="text-center data-column">Score</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td colspan="6" style="font-weight:bold;">Department Summary for <%# Eval("DepartmentName") %></td>
                                                </tr>
                                                <asp:Repeater runat="server" ID="AchievementRepeater">
                                                    <ItemTemplate>
                                                        <tr class="<%# (Container.ItemIndex + 1) % 2 == 0 ? "even" : "odd" %>">
                                                            <td><%# Eval("Info.Name") %></td>
                                                            <td class="text-center"><%# Eval("Data.NotApplicable", "{0:n0}") %></td>
                                                            <td class="text-center"><%# Eval("Data.Submitted", "{0:n0}") %></td>
                                                            <td class="text-center"><%# Eval("Data.Satisfied", "{0:n0}") %></td>
                                                            <td class="text-center"><%# Eval("Data.Required", "{0:n0}") %></td>
                                                            <td class="text-center" style="color:<%# GetScoreHtmlColor((decimal?)Eval("Data.Score")) %>;"><%# FormatScore((decimal?)Eval("Data.Score")) %></td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tbody>
                                        </table>

                                        <div runat="server" class="chart-container mt-4 mb-5 ms-3" style="page-break-inside:avoid;">
                                            <chart:BarChart runat="server" ID="DepartmentChart" ToolTipIntersect="false" LegendVisible="false" OnClientPreInit="report.onCurrentChartPreInit" Height="300px" MaintainApectRatio="false" />
                                        </div>

                                        <asp:Repeater runat="server" ID="EmployeeRepeater">
                                            <ItemTemplate>
                                                <table class="table table-current table-without-status table-employee mt-5" style="page-break-inside:avoid;">
                                                    <thead>
                                                        <tr class="table-header">
                                                            <td></td>
                                                            <td></td>
                                                            <td class="text-center">NA</td>
                                                            <td class="text-center">SV</td>
                                                            <td class="text-center">Validated</td>
                                                            <td class="text-center">Required</td>
                                                            <td class="text-center">Score</td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="7" style="font-weight:bold;">
                                                                <%# Eval("Text") %>
                                                                <br />
                                                                <%# GetOptionText(ReportParameters, Container.DataItem) %>
                                                            </td>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <asp:Repeater runat="server" ID="AchievementRepeater">
                                                            <ItemTemplate>
                                                                <tr class="<%# (Container.ItemIndex + 1) % 2 == 0 ? "even" : "odd" %>">
                                                                    <td class="empty-column"></td>
                                                                    <td><%# Eval("Info.Name") %></td>
                                                                    <td class="text-center data-column"><%# Eval("Data.NotApplicable", "{0:n0}") %></td>
                                                                    <td class="text-center data-column"><%# Eval("Data.Submitted", "{0:n0}") %></td>
                                                                    <td class="text-center data-column"><%# Eval("Data.Satisfied", "{0:n0}") %></td>
                                                                    <td class="text-center data-column"><%# Eval("Data.Required", "{0:n0}") %></td>
                                                                    <td class="text-center data-column" style="color:<%# GetScoreHtmlColor((decimal?)Eval("Data.Score")) %>;"><%# FormatScore((decimal?)Eval("Data.Score")) %></td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>
                                                </table>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>

                            <asp:Repeater runat="server" ID="CurrentDepartmentRepeaterWithStatuses">
                                <ItemTemplate>
                                    <div class="report-item mb-5" style="page-break-after:always;">
                                        <table class="table table-current table-with-status table-department" style="page-break-inside:avoid;">
                                            <thead>
                                                <tr><th colspan="10" style="font-size:1.8em;"><%# Eval("CompanyName") %></th></tr>
                                                <tr><th colspan="10"><%# ReportParameters.ReportTitle %></th></tr>
                                                <tr>
                                                    <th></th>
                                                    <th class="text-center data-column">Expired</th>
                                                    <th class="text-center data-column">Not Completed</th>
                                                    <th class="text-center data-column">Not Applicable</th>
                                                    <th class="text-center data-column">Needs Training</th>
                                                    <th class="text-center data-column">Self Assessed</th>
                                                    <th class="text-center data-column">Submitted for Validation</th>
                                                    <th class="text-center data-column">Validated</th>
                                                    <th class="text-center data-column">Required</th>
                                                    <th class="text-center data-column">Score</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td colspan="10" style="font-weight:bold;">Department Summary for <%# Eval("DepartmentName") %></td>
                                                </tr>
                                                <asp:Repeater runat="server" ID="AchievementRepeater">
                                                    <ItemTemplate>
                                                        <tr class="<%# (Container.ItemIndex + 1) % 2 == 0 ? "even" : "odd" %>">
                                                            <td><%# Eval("Info.Name") %></td>
                                                            <td class="text-center"><%# Eval("Data.Expired", "{0:n0}") %></td>
                                                            <td class="text-center"><%# Eval("Data.NotCompleted", "{0:n0}") %></td>
                                                            <td class="text-center"><%# Eval("Data.NotApplicable", "{0:n0}") %></td>
                                                            <td class="text-center"><%# Eval("Data.NeedsTraining", "{0:n0}") %></td>
                                                            <td class="text-center"><%# Eval("Data.SelfAssessed", "{0:n0}") %></td>
                                                            <td class="text-center"><%# Eval("Data.Submitted", "{0:n0}") %></td>
                                                            <td class="text-center"><%# Eval("Data.Satisfied", "{0:n0}") %></td>
                                                            <td class="text-center"><%# Eval("Data.Required", "{0:n0}") %></td>
                                                            <td class="text-center" style="color:<%# GetScoreHtmlColor((decimal?)Eval("Data.Score")) %>;"><%# FormatScore((decimal?)Eval("Data.Score")) %></td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tbody>
                                        </table>

                                        <div runat="server" class="chart-container mt-4 mb-5 ms-3" style="page-break-inside:avoid;">
                                            <chart:BarChart runat="server" ID="DepartmentChart" ToolTipIntersect="false" LegendVisible="false" OnClientPreInit="report.onCurrentChartPreInit" Height="300px" MaintainApectRatio="false" />
                                        </div>

                                        <asp:Repeater runat="server" ID="EmployeeRepeater">
                                            <ItemTemplate>
                                                <table class="table table-current table-with-status table-employee mt-5" style="page-break-inside:avoid;">
                                                    <thead>
                                                        <tr class="table-header">
                                                            <td></td>
                                                            <td class="text-center">Expired</td>
                                                            <td class="text-center">NC</td>
                                                            <td class="text-center">NA</td>
                                                            <td class="text-center">NT</td>
                                                            <td class="text-center">SA</td>
                                                            <td class="text-center">SV</td>
                                                            <td class="text-center">Validated</td>
                                                            <td class="text-center">Required</td>
                                                            <td class="text-center">Score</td>
                                                        </tr>
                                                        <tr>
                                                            <td style="border-top:1px solid #000000; font-weight:bold;"><%# Eval("Text") %></td>
                                                            <td colspan="9" style="border-top:1px solid #000000; font-weight:bold;"><%# GetOptionText(ReportParameters, Container.DataItem) %></td>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <asp:Repeater runat="server" ID="AchievementRepeater">
                                                            <ItemTemplate>
                                                                <tr class="<%# (Container.ItemIndex + 1) % 2 == 0 ? "even" : "odd" %>">
                                                                    <td><%# Eval("Info.Name") %></td>
                                                                    <td class="text-center data-column"><%# Eval("Data.Expired", "{0:n0}") %></td>
                                                                    <td class="text-center data-column"><%# Eval("Data.NotCompleted", "{0:n0}") %></td>
                                                                    <td class="text-center data-column"><%# Eval("Data.NotApplicable", "{0:n0}") %></td>
                                                                    <td class="text-center data-column"><%# Eval("Data.NeedsTraining", "{0:n0}") %></td>
                                                                    <td class="text-center data-column"><%# Eval("Data.SelfAssessed", "{0:n0}") %></td>
                                                                    <td class="text-center data-column"><%# Eval("Data.Submitted", "{0:n0}") %></td>
                                                                    <td class="text-center data-column"><%# Eval("Data.Satisfied", "{0:n0}") %></td>
                                                                    <td class="text-center data-column"><%# Eval("Data.Required", "{0:n0}") %></td>
                                                                    <td class="text-center data-column" style="color:<%# GetScoreHtmlColor((decimal?)Eval("Data.Score")) %>;"><%# FormatScore((decimal?)Eval("Data.Score")) %></td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>
                                                </table>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>

                        </div>
                    </div>

                </div>
            </div>

        </insite:NavItem>
    </insite:Nav>
    
    <asp:Button runat="server" ID="DownloadXlsx" style="display:none;" />

    <insite:ProgressPanel runat="server" ID="DownloadProgress" HeaderText="Download Progress" Cancel="Custom">
        <Triggers>
            <insite:ProgressControlTrigger ControlID="DownloadXlsx" />
        </Triggers>
        <Items>
            <insite:ProgressIndicator Name="Progress" Caption="Completed: {percent}%" />
            <insite:ProgressStatus Text="Status: {status}{running_ellipsis}" />
            <insite:ProgressStatus Text="Elapsed time: {time_elapsed}s" />
        </Items>
    </insite:ProgressPanel>

    <div id="pdf-report-container">

    </div>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            (() => {
                Sys.Application.add_load(function () {
                    $('#<%= MembershipType.ClientID %> input[type="checkbox"]')
                        .off('change', onMembershipTypeChanged)
                        .on('change', onMembershipTypeChanged);
                });

                function onMembershipTypeChanged() {
                    document.getElementById('<%= DepartmentIdentifier.ClientID %>').disable();
                    document.getElementById('<%= EmployeeIdentifier.ClientID %>').disable();
                    $('#<%= ReportButton.ClientID %>').addClass('disabled');

                    $('#<%= MembershipTypeChanged.ClientID %>').trigger('click');
                }
            })();
        </script>

        <script type="text/javascript">
            (() => {
                var report = window.report = window.report || {};

                report.onCurrentChartPreInit = function (canvas, config) {
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
                        return item.label + ': ' + parseFloat(item.raw).toFixed(2) + ' %';
                    });
                };
            })();
        </script>

        <script type="text/javascript">

            (() => {
                $('#<%= DownloadPdf.ClientID %>').on('click', function (e) {
                    var chartContainers = [];

                    var $pdfContainer = $('#pdf-report-container');
                    var $report = $('.col-report')
                        .clone()
                        .attr('id', null)
                        .find('canvas').each(function () {
                            var $container = $('<div>');
                        
                            $(this).siblings('.chartjs-size-monitor').remove().end().replaceWith($container);

                            chartContainers.push({ $container: $container, chartId: this.id });
                        }).end();

                    $pdfContainer.append($report);
                
                    for (var i = 0; i < chartContainers.length; i++) {
                        var item = chartContainers[i];
                        var chart = inSite.common.chart.getInstance(item.chartId);
                        if (!chart)
                            return;

                        const pxRatio = chart.options.devicePixelRatio;
                        const parent = chart.canvas.parentNode;

                        parent.style.height = String(parent.offsetHeight) + 'px';

                        chart.options.devicePixelRatio = 2;
                        chart.resize(1200, chart.height);

                        item.$container.append($('<img>').attr('alt', '').attr('src', chart.toBase64Image()))

                        chart.options.devicePixelRatio = pxRatio;
                        chart.resize();

                        parent.style.height = null;
                    };

                    $('#<%= ReportHtmlContent.ClientID %>').val($report.html());

                    $pdfContainer.empty();
                });
            })();

        </script>

    </insite:PageFooterContent>

</asp:Content>
