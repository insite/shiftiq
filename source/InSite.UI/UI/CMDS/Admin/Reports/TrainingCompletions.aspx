<%@ Page Language="C#" CodeBehind="TrainingCompletions.aspx.cs" Inherits="InSite.Cmds.Actions.Reporting.Report.TrainingCompletions" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

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
                        <insite:SearchButton runat="server" ID="ReportButton" Text="Report" Icon="fas fa-chart-bar" ValidationGroup="Report" CausesValidation="true" />
                        <insite:CloseButton runat="server" ID="CloseButton1" />
                    </div>

                </ContentTemplate>
            </insite:UpdatePanel>

        </insite:NavItem>
        <insite:NavItem runat="server" ID="ReportTab" Title="Report" Icon="far fa-chart-bar" IconPosition="BeforeText" Visible="false">

            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <insite:DownloadButton runat="server" ID="DownloadXlsx" Text="Download Excel" CssClass="mb-3" />

                    <table class="table table-striped">
                        <thead>
                            <tr>
                                <td colspan="10" class="fw-bold text-white" style="background-color: #2c2d3f;">
                                    Training Completions for <asp:Literal runat="server" ID="CompanyName" /> :: <asp:Literal runat="server" ID="DepartmentsList" />
                                </td>
                            </tr>
                            <tr>
                                <th class="align-middle">Person</th>
                                <th class="align-middle">Organization</th>
                                <th class="align-middle">Department</th>
                                <th class="align-middle">Achievement</th>
                                <th class="align-middle">Achievement Type</th>
                                <th class="text-center align-middle">Completed</th>
                                <th class="align-middle">Status</th>
                                <th class="text-center align-middle">Assigned</th>
                                <th class="text-center align-middle">Expired</th>
                                <th class="text-center align-middle">Score</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater runat="server" ID="DataRepeater">
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("FullName") %></td>
                                        <td><%# Eval("CompanyName") %></td>
                                        <td><%# Eval("DepartmentName") %></td>
                                        <td><%# Eval("AchievementTitle") %></td>
                                        <td><%# Eval("AchievementLabel") %></td>
                                        <td class="text-center text-nowrap"><%# Eval("DateCompleted", "{0:MMM d, yyy}") %></td>
                                        <td><%# Eval("CredentialStatus") %></td>
                                        <td class="text-center text-nowrap"><%# Eval("DateAssigned") == null ? "-" : Eval("DateAssigned", "{0:MMM d, yyy}") %></td>
                                        <td class="text-center text-nowrap"><%# Eval("ExpirationDate") == null ? "-" : Eval("ExpirationDate", "{0:MMM d, yyy}") %></td>
                                        <td class="text-center text-nowrap"><%# Eval("GradePercent") ==  null ? "-" : Eval("GradePercent", "{0:p0}") %></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>

                </div>
            </div>

            <div class="mt-3">
                <insite:CloseButton runat="server" ID="CloseButton2" />
            </div>

        </insite:NavItem>
    </insite:Nav>

</asp:Content>
