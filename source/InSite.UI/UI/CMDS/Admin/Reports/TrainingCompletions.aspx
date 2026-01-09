<%@ Page Language="C#" CodeBehind="TrainingCompletions.aspx.cs" Inherits="InSite.Cmds.Actions.Reporting.Report.TrainingCompletions" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/CMDS/Common/Controls/User/AchievementCriteriaSelector.ascx" TagName="AchievementCriteriaSelector" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Report" />

    <insite:CustomValidator runat="server" ID="DepartmentIdentifierValidator" ErrorMessage="At least one department must be selected" Display="None" ValidationGroup="Report" />
    <insite:CustomValidator runat="server" ID="AchievementSelectorValidator" ErrorMessage="At least one achievement must be selected." Display="None" ValidationGroup="Report" />

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
                                            Membership
                                        </label>
                                        <div>
                                            <insite:CheckBoxList ID="MembershipFunction" runat="server" />
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Credential Status
                                        </label>
                                        <insite:ComboBox runat="server" ID="CredentialStatus">
                                            <Items>
                                                <insite:ComboBoxOption />
                                                <insite:ComboBoxOption Value="Valid" Text="Valid" />
                                                <insite:ComboBoxOption Value="Pending" Text="Pending" />
                                                <insite:ComboBoxOption Value="Expired" Text="Expired" />
                                            </Items>
                                        </insite:ComboBox>
                                        <div class="form-group mt-2">
                                            <insite:CheckBox runat="server" ID="IncludeSelfDeclaredCredentials" Text="Include self-declared achievements" />
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Completed &ge;
                                        </label>
                                        <insite:DateSelector ID="CredentialGrantedStartDate" runat="server" />
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Completed &le;
                                        </label>
                                        <insite:DateSelector ID="CredentialGrantedEndDate" runat="server" />
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
                        <insite:SearchButton runat="server" ID="ReportButton" Text="Report" Icon="fas fa-chart-bar" ValidationGroup="Report" CausesValidation="true" />
                        <insite:CloseButton runat="server" NavigateUrl="/ui/cmds/reports" />
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
                                <td colspan="8" class="fw-bold text-white" style="background-color:#696969;">
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
                <insite:CloseButton runat="server" NavigateUrl="/ui/cmds/reports" />
            </div>

        </insite:NavItem>
    </insite:Nav>

</asp:Content>
