<%@ Page Language="C#" CodeBehind="TrainingHistoryByWorker.aspx.cs" Inherits="InSite.Cmds.Actions.Reporting.Report.TrainingHistoryByWorker" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/CMDS/Common/Controls/User/TrainingReportCriteria.ascx" TagName="TrainingReportCriteria" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

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
                            <uc:TrainingReportCriteria runat="server" ID="Criteria" ValidationGroup="Report" />
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

            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <insite:DownloadButton runat="server"
                        ID="DownloadXlsx"
                        Text="Download Excel"
                        CssClass="mb-3"
                        DisableAfterClick="true"
                        EnableAfter="10000"
                    />

                    <asp:Repeater runat="server" ID="UserRepeater">
                        <ItemTemplate>
                            <table class="table table-striped">
                                <thead>
                                    <tr>
                                        <td colspan="5">
                                            <h3><%# Eval("PersonFullName") %></h3>
                                        </td>
                                    </tr>
                                    <th style="width: 40%;">Achievement Title</th>
                                    <th style="width: 30%;">Provider</th>
                                    <th class="text-center" style="width: 10%;">Completion</th>
                                    <th class="text-center" style="width: 10%;">Renewal</th>
                                    <th class="text-center" style="width: 10%;">Valid</th>
                                </thead>
                                <tbody>
                                    <asp:Repeater runat="server" ID="AchievementRepeater">
                                        <ItemTemplate>
                                            <tr>
                                                <td><%# Eval("AchievementTitle") %></td>
                                                <td><%# Eval("AccreditorName") %></td>
                                                <td class="text-center"><%# LocalizeDate(Eval("DateCompleted")) %></td>
                                                <td class="text-center"><%# LocalizeDate(Eval("ExpirationDate")) %></td>
                                                <td class="text-center">
                                                    <%# Convert.ToBoolean(Eval("IsCompetent")) ? "Yes" : "No" %>
                                                    <span class="form-text">
                                                        <%# Eval("Score","({0:p0})") %>
                                                    </span>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                        </ItemTemplate>
                    </asp:Repeater>

                </div>
            </div>

            <div class="mt-3">
                <insite:CloseButton runat="server" ID="CloseButton2" />
            </div>

        </insite:NavItem>
    </insite:Nav>

</asp:Content>
