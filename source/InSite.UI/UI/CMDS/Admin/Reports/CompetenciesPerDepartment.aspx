<%@ Page Language="C#" CodeBehind="CompetencyListingPerDepartment.ascx.cs" Inherits="InSite.Cmds.Actions.Reports.CompetencyListingPerDepartment" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

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
                                        <cmds:FindDepartment ID="Department" runat="server" />
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Profile
                                        </label>
                                        <cmds:FindProfile ID="CurrentProfile" runat="server" EmptyMessage="All Profiles" />
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Time-Sensitivity
                                        </label>
                                        <div>
                                            <asp:RadioButtonList ID="TimeSensitive" runat="server" RepeatLayout="Flow">
                                                <asp:ListItem Value="" Text="All" Selected="True" />
                                                <asp:ListItem Value="True" Text="Include only time-sensitive" />
                                                <asp:ListItem Value="False" Text="Exclude time-sensitive" />
                                            </asp:RadioButtonList>
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Priority
                                        </label>
                                        <cmds:CompetencyPrioritySelector ID="Priority" runat="server" EmptyMessage="All Priorities" />
                                    </div>

                                    <div runat="server" id="DepartmentCompetenciesField" visible="false" class="form-group mb-3">
                                        <label class="form-label">
                                            Competencies
                                        </label>
                                        <div>
                                            <asp:RadioButtonList ID="DepartmentCompetencies" runat="server" RepeatLayout="Flow">
                                                <asp:ListItem Value="False" Text="Included competencies" Selected="True" />
                                                <asp:ListItem Value="True" Text="Excluded competencies" />
                                            </asp:RadioButtonList>
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

                    <h2 runat="server" ID="ReportTitle"></h2>
                    <div runat="server" ID="ReportSubtitle"></div>

                    <asp:ListView runat="server" ID="ReportList">
                        <ItemTemplate>
                        
                            <div class="mt-4 pt-3 border-top">

                                <h3>
                                    Competency <%# Eval("CompetencyNumber") %>
                                </h3>

                                <div>
                                    <%# Eval("CompetencySummary") %>
                                </div>

                                <asp:ListView runat="server" ID="ProfileList">
                                    <LayoutTemplate>
                                        <ul>
                                            <asp:PlaceHolder runat="server" id="itemPlaceholder" />
                                        </ul>
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <li>
                                            Profile <%# Eval("ProfileNumber") %>:
                                            <%# Eval("ProfileTitle") %>
                                            <span class="d-inline-block ms-2">
                                                <%# (bool)Eval("CompetencyIsCritical") ? "<span class='badge bg-danger'><i class='fas fa-exclamation'></i> Critical</span>" : "" %>
                                                <%# (bool)Eval("CompetencyIsTimeSensitive") ? "<span class='badge bg-info'><i class='fas fa-alarm-clock'></i> Time-Sensitive</span>" : "" %>
                                                <%# Eval("CompetencyLifetimeHtml") %>
                                            </span>
                                        </li>
                                    </ItemTemplate>
                                </asp:ListView>
                        
                            </div>

                        </ItemTemplate>
                    </asp:ListView>

                </div>
            </div>

        </insite:NavItem>
    </insite:Nav>
</asp:Content>