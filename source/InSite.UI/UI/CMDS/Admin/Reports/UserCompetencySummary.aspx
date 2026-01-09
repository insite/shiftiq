<%@ Page Language="C#" CodeBehind="UserCompetencySummary.aspx.cs" Inherits="InSite.Cmds.Actions.Reporting.Report.UserCompetencySummary" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<insite:PageHeadContent runat="server">
    <style type="text/css">

        .report-container {
            margin-top: 0px;
        }

            .report-container hr {
                border-color: #AAAAAA;
                margin: 5px 0;
            }

            .report-container label {
                margin-bottom: 0;
            }

            .report-container .normal-text label {
                font-weight: normal;
            }

            .report-container .bold-text {
                font-weight: bold;
            }

            .report-container .head-label {
                line-height: 32px;
                text-align: right;
            }

            .report-container .head-value {
                line-height: 32px;
            }

            .report-container .control-label {
                text-align: right;
            }

            .report-container .control-value {
                padding-left: 15px;
            }

        .report-container h2 { border-top: dotted #767676 1px; padding-top: 10px; margin-top: 30px; }
        .report-container h3 { color: #767676; }
        .report-container h4 { color: #3D78D8; font-size: 22px; }

    </style>
</insite:PageHeadContent>

    <insite:ValidationSummary runat="server" ValidationGroup="Report" />

    <insite:Alert runat="server" ID="ReportStatus" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="CriteriaSection" Title="Criteria" Icon="far fa-search" IconPosition="BeforeText">
            <section class="mb-3">
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
                                                Employee
                                                <insite:RequiredValidator runat="server" ControlToValidate="Employee" FieldName="Employee" ValidationGroup="Report" Display="Dynamic" />
                                            </label>
                                            <div>
                                                <cmds:FindPerson ID="Employee" runat="server" />
                                            </div>
                                            <div class="form-text"></div>
                                        </div>
                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Profile
                                                <insite:RequiredValidator ID="CurrentProfileRequired" runat="server" ControlToValidate="CurrentProfile" FieldName="Profile" ValidationGroup="Report" Enabled="false" />
                                            </label>
                                            <div>
                                                <asp:RadioButtonList ID="ProfileMode" runat="server" RepeatLayout="Flow">
                                                    <asp:ListItem Text="View primary profile only " Value="Primary" Selected="True" />
                                                    <asp:ListItem Text="View one specific profile " Value="Specific" />
                                                    <asp:ListItem Text="View all profiles " Value="All" />
                                                </asp:RadioButtonList>
                                                <div>
                                                    <cmds:FindProfile ID="CurrentProfile" runat="server" Enabled="false" />
                                                </div>
                                            </div>
                                            <div class="form-text"></div>
                                        </div>

                                    </div>
                                </div>
                            </ContentTemplate>
                        </insite:UpdatePanel>
                    </div>
                </div>
            </section>

            <insite:SearchButton runat="server" ID="CreateReportButton" Text="Create Report" Icon="far fa-chart-bar" ValidationGroup="Report" CausesValidation="true" />
        </insite:NavItem>
        <insite:NavItem runat="server" ID="PreviewSection" Title="Report" Icon="far fa-chart-bar" IconPosition="BeforeText" Visible="false">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Report
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <asp:Panel runat="server" ID="DownloadCommandsPanel">
                            <insite:Button runat="server" ID="DownloadXlsx" ButtonStyle="Primary" Text="Download XLSX" Icon="far fa-download" CssClass="mb-3" />
                        </asp:Panel>

                        <div class="report-container">
                            <asp:Repeater runat="server" ID="DataRepeater">
                                <ItemTemplate>
                                    <div class="row">
                                        <div class="col-lg-12">
                                            <h2 style="margin-bottom: 0px !important; padding-bottom: 0px;">
                                                <%# Eval("CompanyName") %>
                                            </h2>
                                            <h3 style="margin-top: 0px !important; padding-top: 0px;">
                                                <%# String.Join(", ", (IEnumerable<string>)Eval("Areas")) %>
                                            </h3>
                                        </div>
                                    </div>
                                    <asp:Repeater runat="server" ID="ProfileRepeater">
                                        <ItemTemplate>
                                            <div class="row">
                                                <div class="col-lg-12">
                                                    <h4><%# Eval("ProfileTitle") %></h4>
                                                </div>
                                            </div>
                                            <table>
                                                <tr>
                                                    <td style="width: 40%; vertical-align: top;">
                                                        <table>
                                                            <tbody>
                                                                <tr>
                                                                    <td class="control-label">
                                                                        <label class="form-label">Employee:</label>
                                                                    </td>
                                                                    <td class="control-value" style="white-space: nowrap;"><%# Eval("FullName") %></td>
                                                                </tr>
                                                                <tr>
                                                                    <td class="control-label">
                                                                        <label class="form-label"><%# (bool)Eval("IsPrimary") ? "Primary" : "Secondary" %> Profile:</label>
                                                                    </td>
                                                                    <td class="control-value"  style="white-space: nowrap;"><%# Eval("ProfileTitle") %></td>
                                                                </tr>
                                                                <tr>
                                                                    <td class="control-label" style="white-space: nowrap;">
                                                                        <label class="form-label">Validated Competencies</label>
                                                                    </td>
                                                                    <td class="control-value">
                                                                        <%# Eval("SumEmployeeCompetencies") %>
                                                                        out of
                                                                        <%# Eval("SumTotalCompetencies") %>
                                                                        =
                                                                        <%# Eval("AvgEmployeePercent") + "%" %>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <th colspan="2">
                                                                        <h5>Development Plan:</h5>
                                                                    </th>
                                                                </tr>
                                                            <tr>
                                                                <th class="text-end">Validation Status</th>
                                                                <th class="control-value"># of Competencies</th>
                                                            </tr>
                                                            <asp:Repeater runat="server" ID="StatusRepeater">
                                                                <ItemTemplate>
                                                                    <tr>
                                                                        <td class="control-label"><%# Eval("StatusName") %></td>
                                                                        <td class="control-value"><%# Eval("CompetencyCount") %></td>
                                                                    </tr>
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                        </table>
                                                    </td>
                                                    <td style="width: 10%;"></td>
                                                    <td style="vertical-align: top; padding-bottom: 15px;">
                                                        <asp:Repeater runat="server" ID="ManagerRepeater">
                                                            <ItemTemplate>
                                                                <table>
                                                                    <tr>
                                                                        <td class="control-label">
                                                                            <label class="form-label">Validator:</label>
                                                                        </td>
                                                                        <td class="control-value bold-text"><%# Eval("FullName") %></td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td class="control-label">Email:</td>
                                                                        <td class="control-value"><%# Eval("EmailWork") %></td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td class="control-label">Phone:</td>
                                                                        <td class="control-value"><%# Eval("PhoneWork") %></td>
                                                                    </tr>
                                                                </table>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td></td>
                                                    <td>
                                                        
                                                    </td>
                                                </tr>
                                            </table>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>

    </insite:Nav>

</asp:Content>
