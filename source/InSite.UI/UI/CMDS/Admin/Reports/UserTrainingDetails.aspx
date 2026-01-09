<%@ Page Language="C#" CodeBehind="UserTrainingDetail.ascx.cs" Inherits="InSite.Cmds.Actions.Reporting.Report.UserTrainingDetail" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Report" />

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
                    <div class="row">
                        <div class="col-lg-6">
                            <div class="card border-0 shadow-lg">
                                <div class="card-body">

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Department
                                            <insite:RequiredValidator runat="server" ControlToValidate="DepartmentIdentifier" FieldName="Department" ValidationGroup="Report" />
                                        </label>
                                        <cmds:FindDepartment ID="DepartmentIdentifier" runat="server" />
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Type
                                            <insite:RequiredValidator runat="server" ControlToValidate="AchievementLabel" FieldName="Type" ValidationGroup="Report" />
                                        </label>
                                        <asp:RadioButtonList runat="server" ID="AchievementLabel" />
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Achievement
                                            <insite:RequiredValidator runat="server" ControlToValidate="AchievementIdentifier" FieldName="Achievement" ValidationGroup="Report" />
                                        </label>
                                        <cmds:FindAchievement ID="AchievementIdentifier" runat="server" />
                                    </div>

                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="mt-3">
                        <insite:SearchButton runat="server" ID="ReportButton" Text="Report" Icon="fas fa-chart-bar" ValidationGroup="Report" CausesValidation="true" />
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

                    <insite:DownloadButton runat="server" ID="DownloadXlsx" Text="Download Excel" CssClass="mb-3" />

                    <table class="table table-striped">
                        <thead>
                            <tr>
                                <td colspan="6" class="fw-bold text-white" style="background-color:#696969;">
                                    Worker Training Details for <asp:Literal runat="server" ID="AchievementTitle" />
                                    <br />
                                    <asp:Literal runat="server" ID="CompanyName" /> :: <asp:Literal runat="server" ID="DepartmentName" />
                                </td>
                            </tr>
                            <tr>
                                <th class="align-middle">Person</th>
                                <th class="text-center align-middle">Date Completed</th>
                                <th class="text-center align-middle">Date Expired</th>
                                <th class="align-middle">Status</th>
                            </tr>
                        </thead>
                        <tbody>
                        <asp:Repeater runat="server" ID="DataRepeater">
                            <ItemTemplate>
                                <tr>
                                    <td><%# Eval("FullName") %></td>
                                    <td class="text-center text-nowrap"><%# Eval("DateCompleted") == null ? "--" : Eval("DateCompleted", "{0:MMM d, yyy}") %></td>
                                    <td class="text-center text-nowrap"><%# Eval("ExpirationDate") == null ? "--" : Eval("ExpirationDate", "{0:MMM d, yyy}") %></td>
                                    <td><%# Eval("Status") %></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        </tbody>
                    </table>

                </div>
            </div>

            <div class="mt-3">
                <insite:CloseButton runat="server" NavigateUrl="/ui/cmds/reports" />
            </div>

        </insite:NavItem>
    </insite:Nav>

</asp:Content>
