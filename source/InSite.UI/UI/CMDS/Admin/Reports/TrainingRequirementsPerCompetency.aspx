<%@ Page CodeBehind="TrainingRequirementsPerCompetency.aspx.cs" Inherits="InSite.Cmds.Actions.Reporting.Report.TrainingRequirementsPerCompetency" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

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

                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="CriteriaUpdatePanel" />
                
                    <insite:UpdatePanel runat="server" ID="CriteriaUpdatePanel">
                        <Triggers>
                            <asp:PostBackTrigger ControlID="SearchButton" />
                        </Triggers>
                        <ContentTemplate>

                            <div class="card border-0 shadow-lg">
                                <div class="card-body">

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
                                            <asp:RadioButtonList ID="StatusSelector" runat="server" RepeatLayout="Flow">
                                                <asp:ListItem Value="Expired" Text="Expired" />
                                                <asp:ListItem Value="Not Completed" Text="Not Completed" />
                                                <asp:ListItem Value="Not Applicable" Text="Not Applicable" />
                                                <asp:ListItem Value="Needs Training" Text="Needs Training" Selected="True" />
                                                <asp:ListItem Value="Self-Assessed" Text="Self-Assessed" />
                                                <asp:ListItem Value="Submitted for Validation" Text="Submitted for Validation" />
                                                <asp:ListItem Value="Validated" Text="Validated" />
                                            </asp:RadioButtonList>
                                        </div>
                                    </div>

                                </div>
                            </div>

                            <div class="mt-3">
                                <insite:SearchButton runat="server" ID="SearchButton" ValidationGroup="Report" CausesValidation="true" />
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

                    <div class="row mb-3">
                        <div class="col-lg-8">
                            <div class="me-3">
                                The column on the left side of the page lists the departments in your search results.
                                <br />
                                Click a department name to view the competencies assigned to people in that department.
                            </div>
                        </div>

                        <div class="col-lg-4 text-end" runat="server" ID="DownloadCommandsPanel">
                            <insite:DownloadButton runat="server" ID="DownloadXlsx" Text="Download XLSX" />
                        </div>
                    </div>

                    <div class="row mb-3">
                        <div class="col-lg-3 col-4">
                            <ul aria-orientation="vertical" class="nav nav-pills flex-column" role="tablist">

                                <asp:Repeater runat="server" ID="DepartmentRepeater">
                                    <ItemTemplate>
                                        <li class="nav-item" role="presentation">
                                            <asp:LinkButton runat="server"
                                                CssClass='<%# "nav-link text-center" + ((Guid)Eval("Identifier") == CurrentDepartmentIdentifier ? " active" : "") %>'
                                                Text='<%# Eval("Name") + " (" + Eval("Count") + ")" %>'
                                                CommandName="Show" CommandArgument='<%# Eval("Identifier") %>'
                                            />
                                        </li>
                                    </ItemTemplate>
                                </asp:Repeater>

                            </ul>
                        </div>
                        <div class="col-lg-9 col-8">

                            <asp:Repeater runat="server" ID="CompetencyRepeater">
                                <HeaderTemplate>
                                    <table class="table table-striped">
                                        <thead>
                                            <tr>
                                                <th colspan="2">Competency</th>
                                                <th>Worker</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <FooterTemplate>
                                        </tbody>
                                    </table>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td class="text-nowrap"><%# Eval("Number") %></td>
                                        <td><%# Eval("Summary") %></td>
                                        <td class="text-nowrap">
                                            <asp:Repeater runat="server" ID="EmployeeRepeater">
                                                <ItemTemplate>
                                                    <div class="employee">
                                                        <%# Eval("FullName") %>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>

                        </div>
                    </div>

                </div>
            </div>

        </insite:NavItem>
    
    </insite:Nav>

</asp:Content>