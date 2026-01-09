<%@ Page Language="C#" CodeBehind="TrainingExpiryDates.aspx.cs" Inherits="InSite.Cmds.Actions.Reporting.Report.TrainingExpiryDates" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/CMDS/Common/Controls/User/AchievementCriteriaSelector.ascx" TagName="AchievementCriteriaSelector" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
        .table-report-container {
            overflow: auto;
            max-height: 85vh;
        }

            .table-report-container table.table-report {
                border-collapse: separate;
                border-spacing: 0;
            }

                .table-report-container table.table-report thead {
                }

                    .table-report-container table.table-report > thead > tr > th {
                        position: -webkit-sticky;
                        position: sticky;
                        top: 0;
                        z-index: 2;
                        background-color: #fff;
                    }

                        .table-report-container table.table-report > thead > tr > td.first-cell {
                            position: -webkit-sticky;
                            position: sticky;
                            left: 0;
                            z-index: 3;
                        }


                        .table-report-container table.table-report > thead > tr > th.name-cell {
                            position: -webkit-sticky;
                            position: sticky;
                            left: 0;
                            z-index: 3;
                            vertical-align: middle;
                            text-align: center;
                        }

                        .table-report-container table.table-report > thead > tr > th.dept-cell {
                            position: -webkit-sticky;
                            position: sticky;
                            left: 160px;
                            z-index: 3;
                            vertical-align: middle;
                            text-align: center;
                        }


                    .table-report-container table.table-report > thead > tr > td.group-header {
                        background-color: #696969;
                        color: #fff;
                        font-weight: bold;
                    }

                    .table-report-container table.table-report > thead > tr > th.resource-cell {
                        vertical-align: middle;
                        text-align: center;
                    }

                        .table-report-container table.table-report > thead > tr > th.resource-cell > div {
                            min-width: 6.4em;
                        }

                .table-report-container table.table-report > tbody > tr > td.name-cell {
                    position: -webkit-sticky;
                    position: sticky;
                    left: 0;
                    z-index: 1;
                }

                    .table-report-container table.table-report > tbody > tr > td.name-cell > div {
                        width: 144px;
                    }

                .table-report-container table.table-report > tbody > tr > td.dept-cell {
                    position: -webkit-sticky;
                    position: sticky;
                    left: 160px;
                    z-index: 1;
                    background-color: #ffffff;
                }

                    .table-report-container table.table-report > tbody > tr > td.dept-cell > div {
                        width: 160px;
                    }

                .table-report-container table.table-report > tbody > tr:nth-child(2n+1) > td.dept-cell {
                    background-color: #f9f9f9;
                }

                .table-report-container table.table-report > tbody > tr > td.data-cell {
                    white-space: nowrap;
                    text-align: center;
                }

                .table-report-container table.table-report > tbody > tr > td.even {
                    background-color: #ffffff;
                }

                .table-report-container table.table-report > tbody > tr > td.odd {
                    background-color: #f9f9f9;
                }
    </style>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Report" />

    <insite:CustomValidator runat="server" ID="DepartmentIdentifierValidator" ErrorMessage="At least one department must be selected" Display="None" ValidationGroup="Report" />
    <insite:CustomValidator runat="server" ID="AchievementSelectorValidator" ErrorMessage="At least one achievement must be selected." Display="None" ValidationGroup="Report" />

    <insite:Nav runat="server" ID="NavPanel">
        <insite:NavItem runat="server" ID="CriteriaTab" Title="Criteria" Icon="far fa-search" IconPosition="BeforeText">

            <h2 class="h4 my-3">
                Criteria
            </h2>

            <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="ReportUpdatePanel" />
                
            <insite:UpdatePanel runat="server" ID="ReportUpdatePanel">
                <Triggers>
                    <asp:PostBackTrigger ControlID="ReportButton" />
                </Triggers>
                <ContentTemplate>
                    <div class="card border-0 shadow-lg">
                        <div class="card-body">

                            <div class="row">
                                <div class="col-lg-6">

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Departments
                                        </label>
                                        <cmds:FindDepartment runat="server" ID="DepartmentIdentifier" MaxSelectionCount="0" />
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Settings
                                        </label>
                                        <div>
                                            <asp:RadioButtonList ID="IsRequired" runat="server" RepeatLayout="Flow">
                                                <asp:ListItem Text="Required and Optional Achievements" Selected="True" />
                                                <asp:ListItem Value="True" Text="Required Achievements Only" />
                                            </asp:RadioButtonList>
                                        </div>
                                    </div>

                                </div>
                                <div class="col-lg-6">

                                    <uc:AchievementCriteriaSelector runat="server" ID="AchievementSelector" />

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
                        <insite:CloseButton runat="server" NavigateUrl="/ui/cmds/reports" />
                    </div>

                </ContentTemplate>
            </insite:UpdatePanel>

        </insite:NavItem>
        <insite:NavItem runat="server" ID="ReportTab" Title="Report" Icon="far fa-chart-bar" IconPosition="BeforeText" Visible="false">

            <h2 class="h4 my-3">
                Report
            </h2>

            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <insite:DownloadButton runat="server"
                        ID="DownloadXlsx"
                        Text="Download Excel"
                        CssClass="mb-3"
                        DisableAfterClick="true"
                        EnableAfter="10000"
                    />

                    <div class="table-report-container">
                        <table class="table table-striped table-report">
                            <thead>
                                <tr>
                                    <td colspan="2" class="group-header first-cell"></td>
                                    <asp:Repeater runat="server" ID="CompanyHeaderRepeater">
                                        <ItemTemplate>
                                            <td colspan="<%# Eval("Children.Count") %>" class="group-header">
                                                Worker Training Expiry Dates for <%# Eval("Name") %> :: <%# string.Join(", ", (IEnumerable<string>)Eval("Departments")) %>
                                            </td>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tr>
                                <tr>
                                    <th class="name-cell">Employee</th>
                                    <th class="dept-cell">Department</th>
                                    <asp:Repeater runat="server" ID="AchievementHeaderRepeater">
                                        <ItemTemplate>
                                            <th class="resource-cell"><div><%# Eval("Text") %></div></th>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater runat="server" ID="EmployeeRepeater">
                                    <ItemTemplate>
                                        <tr>
                                            <td rowspan="<%# Eval("Children.Count") %>" class="name-cell<%# (Container.ItemIndex + 1) % 2 == 0 ? " even" : " odd" %>"><div><%# Eval("Text") %></div></td>
                                            <asp:Repeater runat="server" ID="DepartmentRepeater">
                                                <ItemTemplate>
                                                    <td class="dept-cell"><div><%# Eval("Text") %></div></td>
                                                    <asp:Repeater runat="server" ID="CellRepeater">
                                                        <ItemTemplate>
                                                            <td class="data-cell" style="<%# Eval("Color") == null ? null : Eval("Color", "background-color:{0};") %>">
                                                                <%# Eval("Text") %>
                                                            </td>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </ItemTemplate>
                                                <SeparatorTemplate>
                                                    </tr><tr>
                                                </SeparatorTemplate>
                                            </asp:Repeater>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>

                </div>
            </div>

            <div class="mt-3">
                <insite:CloseButton runat="server" NavigateUrl="/ui/cmds/reports" />
            </div>

        </insite:NavItem>
    </insite:Nav>

</asp:Content>
