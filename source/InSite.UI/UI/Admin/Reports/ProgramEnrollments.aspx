<%@ Page Language="C#" CodeBehind="ProgramEnrollments.aspx.cs" Inherits="InSite.UI.Admin.Reports.ProgramEnrollments" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Reports/Controls/ProgramEnrollmentsCriteria.ascx" TagName="ProgramEnrollmentsCriteria" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:UpdatePanel runat="server" UpdateMode="Always">
        <ContentTemplate>
            <insite:Alert runat="server" ID="ScreenStatus" />
            <insite:ValidationSummary runat="server" ValidationGroup="Report" />
        </ContentTemplate>
    </insite:UpdatePanel>

    <insite:Nav runat="server" ID="NavPanel">
        <insite:NavItem runat="server" ID="CriteriaTab" Title="Criteria" Icon="far fa-search" IconPosition="BeforeText">

            <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="ReportUpdatePanel" />

            <insite:UpdatePanel runat="server" ID="ReportUpdatePanel">
                <Triggers>
                    <asp:PostBackTrigger ControlID="ReportButton" />
                </Triggers>
                <ContentTemplate>
                    <div class="card border-0 shadow-lg">
                        <div class="card-body">
                            <uc:ProgramEnrollmentsCriteria runat="server" ID="Criteria" ValidationGroup="Report" />
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
                        <insite:CloseButton runat="server" ID="CloseButton1" />
                    </div>

                </ContentTemplate>
            </insite:UpdatePanel>

        </insite:NavItem>
        <insite:NavItem runat="server" ID="ReportTab" Title="Report" Icon="far fa-chart-bar" IconPosition="BeforeText" Visible="false">

            <h2 class="h4 my-3">
                <asp:Literal runat="server" ID="ReportTitle" />
            </h2>

            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <div class="float-end h6">
                        Total: <asp:Literal runat="server" ID="ReportRowCount" />
                    </div>

                    <insite:DownloadButton runat="server"
                        ID="DownloadXlsx"
                        Text="Download Excel"
                        CssClass="mb-3"
                        DisableAfterClick="true"
                        EnableAfter="10000"
                    />

                    <div class="table-responsive">
                        <table class="table table-striped">
                            <thead>
                                <tr>
                                    <th>Person</th>
                                    <th>Group Type</th>
                                    <th>Group Name</th>
                                    <th class="progenrol-achname">Achievement</th>
                                    <th class="progenrol-achstatus">Status</th>
                                    <th>Program Name</th>
                                    <th class="text-end">Program Start Date</th>
                                    <th class="text-end">Program Completed Date</th>
                                    <th class="text-center">Time Taken (Days)</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater runat="server" ID="DataRepeater">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%# Eval("FullName") %></td>
                                            <td><%# Eval("GroupType") %></td>
                                            <td><div style="max-width:500px"><%# Eval("GroupName") %></div></td>
                                            <td colspan="2" class="p-0">
                                                <table class="w-100">
                                                    <asp:Repeater runat="server" ID="AchievementRepeater">
                                                        <ItemTemplate>
                                                            <tr>
                                                                <td class="progenrol-achname"><%# Eval("AchievementTitle") %></td>
                                                                <td class="progenrol-achstatus"><%# Eval("Status") %></td>
                                                            </tr>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </table>
                                            </td>
                                            <td><%# Eval("ProgramName") %></td>
                                            <td class="text-end text-nowrap"><%# Eval("ProgramStartDate") == null ? "-" : Eval("ProgramStartDate", "{0:MMM d, yyy}") %></td>
                                            <td class="text-end text-nowrap"><%# Eval("ProgramCompletedDate") == null ? "-" : Eval("ProgramCompletedDate", "{0:MMM d, yyy}") %></td>
                                            <td class="text-center"><%# Eval("TimeTakenDays") %></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>

                </div>
            </div>

            <div class="mt-3">
                <insite:CloseButton runat="server" ID="CloseButton2" />
            </div>

        </insite:NavItem>
    </insite:Nav>

    <insite:PageHeadContent runat="server">
        <style type="text/css">
            .progenrol-achname {
                width: 250px;
            }

            .progenrol-achstatus {
                width: 120px;
            }

            td.progenrol-achname,
            td.progenrol-achstatus {
                padding: 0.75rem;
                vertical-align: top;
            }

            tr + tr > td.progenrol-achname,
            tr + tr > td.progenrol-achstatus {
                padding-top: 0;
            }
        </style>
    </insite:PageHeadContent>

</asp:Content>
