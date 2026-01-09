<%@ Page Language="C#" CodeBehind="IndividualProfileAssignment.aspx.cs" Inherits="InSite.Cmds.Actions.Reporting.Report.IndividualProfileAssignment" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <insite:ValidationSummary runat="server" ValidationGroup="Report" />

    <insite:Nav runat="server" ID="NavPanel">
        <insite:NavItem runat="server" ID="CriteriaTab" Title="Criteria" Icon="far fa-search" IconPosition="BeforeText">

            <h2 class="h4 my-3">
                Criteria
            </h2>

            <div class="row">
                <div class="col-lg-6">

                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="ReportUpdatePanel" />
                
                    <insite:UpdatePanel runat="server" ID="ReportUpdatePanel">
                        <Triggers>
                            <asp:PostBackTrigger ControlID="ReportButton" />
                        </Triggers>
                        <ContentTemplate>

                            <div class="card border-0 shadow-lg">
                                <div class="card-body">

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Department
                                            <insite:RequiredValidator runat="server" ControlToValidate="Department" FieldName="Department" ValidationGroup="Report" />
                                        </label>
                                        <div>
                                            <cmds:FindDepartment ID="Department" runat="server" />
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Membership
                                        </label>
                                        <div>
                                            <asp:CheckBoxList ID="RoleTypeSelector" runat="server">
                                                <asp:ListItem Value="Organization" Text="Organization Employment" />
    	                                        <asp:ListItem Value="Department" Text="Department Employment" Selected="True" />
                                            </asp:CheckBoxList>
                                        </div>
                                    </div>

                                </div>
                            </div>

                            <div class="mt-3">
                                <insite:SearchButton runat="server" ID="ReportButton" Text="Report" Icon="fas fa-chart-bar" ValidationGroup="Report" CausesValidation="true" />
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

                    <insite:DownloadButton runat="server" ID="DownloadButton" Text="Download XLSX" CssClass="mb-3" />

                    <table class="table table-striped">
                        <thead>
                            <tr>
                                <td colspan="7" class="fw-bold text-white" style="background-color:#696969;">
                                    <asp:Literal runat="server" ID="CompanyName" /> :: <asp:Literal runat="server" ID="DepartmentName" />
                                </td>
                            </tr>
                            <tr>
                                <th class="align-middle">Employee Name</th>
                                <th class="align-middle">Primary Profile</th>
                                <th class="align-middle">Secondary Required for Compliance Profile(s)</th>
                                <th class="align-middle">Secondary Not Required for Compliance Profile(s)</th>
                                <th class="align-middle">Manager(s)</th>
                                <th class="align-middle">Supervisor(s)</th>
                                <th class="align-middle">Validator(s)</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater runat="server" ID="DataRepeater">
                                <ItemTemplate>
                                    <tr>
                                        <td style="width: 13%;"><%# Eval("PersonName") %></td>
                                        <td style="width: 12%;"><%# Eval("PrimaryProfileName") %></td>
                                        <td style="width: 15%;"><%# Eval("SecondaryRequiredProfiles") %></td>
                                        <td style="width: 15%;"><%# Eval("SecondaryProfiles") %></td>
                                        <td style="width: 15%;"><%# Eval("Managers") %></td>
                                        <td style="width: 15%;"><%# Eval("Supervisors") %></td>
                                        <td style="width: 15%;"><%# Eval("Validators") %></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>

                </div>
            </div>

        </insite:NavItem>
    </insite:Nav>

</asp:Content>