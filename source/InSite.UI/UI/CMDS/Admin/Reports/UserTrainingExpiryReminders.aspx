<%@ Page Language="C#" CodeBehind="UserTrainingExpiryReminders.aspx.cs" Inherits="InSite.Cmds.Admin.Reports.Forms.UserTrainingExpiryReminders" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/CMDS/Common/Controls/User/AchievementCriteriaSelector.ascx" TagName="AchievementCriteriaSelector" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">

        .report-output {
        }

            .report-output > .report-item {
                padding-bottom: 20px;
                margin-top: 20px;
                border-bottom: 2px dotted #000;
            }

                .report-output > .report-item > .line-1 {
                    margin-bottom: 8px;
                }

                    .report-output > .report-item > .line-1 > .employee {
                        font-weight: bold;
                        width: 50%;
                        float: left;
                    }

                    .report-output > .report-item > .line-1 > .resource {
                        text-align: right;
                        font-weight: bold;
                        text-decoration: underline;
                        width: 50%;
                        float: left;
                    }

                    .report-output > .report-item > .line-1:after {
                        content: ' ';
                        display: block;
                        clear: both;
                        height: 1px;
                    }

                .report-output > .report-item > .line-2 {
                    margin-bottom: 8px;
                }

                .report-output > .report-item > .line-3 {
                    margin-bottom: 8px;
                    font-weight: bold;
                    font-style: italic;
                }

    </style>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Report" />

    <insite:CustomValidator runat="server" ID="DepartmentIdentifierValidator" ErrorMessage="At least one department must be selected" Display="None" ValidationGroup="Report" />
    <insite:CustomValidator runat="server" ID="AchievementSelectorValidator" ErrorMessage="At least one achievement must be selected." Display="None" ValidationGroup="Report" />

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
                    <div class="card border-0 shadow-lg">
                        <div class="card-body">

                            <div class="row">
                                <div class="col-lg-6">

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Organization
                                        </label>
                                        <cmds:FindCompany ID="OrganizationIdentifier" runat="server" Enabled="false" />
                                    </div>

                                    <div runat="server" ID="DepartmentField" class="form-group mb-3">
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

            <h2 class="h4 my-3">
                Report
            </h2>

            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <insite:DownloadButton runat="server" ID="DownloadXlsx" Text="Download Excel" CssClass="mb-3" />
                    <insite:DownloadButton runat="server" ID="DownloadPdf" Text="Download PDF" CssClass="mb-3" />

                    <div class="report-output">
                        <asp:Repeater runat="server" ID="DataRepeater">
                            <ItemTemplate>
                                <div class="report-item" style="page-break-inside:avoid;">
                                    <div class="line-1">
                                        <div class="employee"><%# Eval("Employee") %></div>
                                        <div class="resource"><%# Eval("Achievement") %></div>
                                    </div>
                                    <div class="line-2">
                                        <%# Eval("Line1") %>
                                    </div>
                                    <div class="line-3">
                                        <%# Eval("Line2") %>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>

                </div>
            </div>

            <div class="mt-3">
                <insite:CloseButton runat="server" NavigateUrl="/ui/cmds/reports" />
            </div>

        </insite:NavItem>
    </insite:Nav>

</asp:Content>
