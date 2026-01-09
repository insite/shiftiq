<%@ Page Language="C#" CodeBehind="UnprioritizedCompetencies.aspx.cs" Inherits="InSite.Cmds.Actions.Reports.UnprioritizedCompetencies" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <style>
        .table.table-striped th { border-color: #e8e8e8;}
    </style>

    <insite:Alert runat="server" Indicator="Information">
        This report shows competencies that are not yet assigned a priority or a time-sensitivity in the profiles assigned to the departments that use them
    </insite:Alert>

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="CriteriaSection" Title="Criteria" Icon="far fa-search" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Criteria
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="ReportUpdatePanel" />
                
                        <insite:UpdatePanel runat="server" ID="ReportUpdatePanel">
                            <ContentTemplate>
                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Department
                                                <insite:RequiredValidator runat="server" ControlToValidate="Department" FieldName="Department" ValidationGroup="Report" />
                                            </label>
                                            <div>
                                                <cmds:FindDepartment ID="Department" runat="server" CssClass="w-75" />
                                            </div>
                                            <div class="form-text"></div>
                                        </div>
                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Profile
                                            </label>
                                            <div>
                                                <cmds:FindProfile ID="CurrentProfile" runat="server" CssClass="w-75" EmptyMessage="All Profiles" />
                                            </div>
                                            <div class="form-text"></div>
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
                                            <div class="form-text"></div>
                                        </div>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </insite:UpdatePanel>

                        <insite:SearchButton runat="server" ID="CreateReportButton" Text="Create Report" Icon="far fa-chart-bar" ValidationGroup="Report" CausesValidation="true" />

                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="PreviewSection" Title="Report" Icon="far fa-chart-bar" IconPosition="BeforeText" Visible="false">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Report
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <insite:Button runat="server" ID="DownloadXlsx" ButtonStyle="Primary" Text="Download XLSX" Icon="far fa-download" CssClass="mb-3" />

                        <asp:PlaceHolder runat="server" ID="place"></asp:PlaceHolder>
                    </div>
                </div>
            </section>
        </insite:NavItem>

    </insite:Nav>

</asp:Content>
