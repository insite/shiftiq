<%@ Page Language="C#" CodeBehind="NotificationSubscribers.aspx.cs" Inherits="InSite.Cmds.Admin.Reports.Forms.NotificationSubscribers" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
        table.table.follower-table {
            background-color:transparent;
            border-bottom:none;
        }
        table.table.follower-table tbody {
            background-color:transparent;
        }
        table.table.follower-table td {
            background-color:transparent;
            border-top:none;
        }
    </style>
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
                            <asp:PostBackTrigger ControlID="SearchButton" />
                        </Triggers>
                        <ContentTemplate>

                            <div class="card border-0 shadow-lg">
                                <div class="card-body">

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Department
                                        </label>
                                        <div>
                                            <cmds:FindDepartment ID="DepartmentIdentifier" runat="server" EmptyMessage="All Departments" MaxSelectionCount="0" />
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Membership
                                        </label>
                                        <div>
                                            <asp:CheckBoxList runat="server" ID="Membership" RepeatDirection="Vertical" RepeatLayout="Flow">
                                                <asp:ListItem Value="Department" Text="Department Employment" Selected="True" />
                                                <asp:ListItem Value="Organization" Text="Organization Employment" />
                                                <asp:ListItem Value="Administration" Text="Data Access" />
                                            </asp:CheckBoxList>
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

                    <insite:DownloadButton runat="server" ID="DownloadButton" Text="Download XLSX" CssClass="mb-3" />

                    <div class="row">
                        <div class="col-lg-3 col-4">
                            <insite:Nav runat="server" ID="DepartmentsNav" ItemType="Pills" ItemAlignment="Vertical" ContentRendererID="DepartmentsNavContent">

                            </insite:Nav>
                        </div>
                        <div id="departments-nav-content" class="col-lg-9 col-6">
                            <insite:NavContent runat="server" ID="DepartmentsNavContent" />
                        </div>
                    </div>

                </div>
            </div>

        </insite:NavItem>
    </insite:Nav>

</asp:Content>