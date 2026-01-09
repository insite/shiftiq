<%@ Page Language="C#" CodeBehind="DepartmentProfileSummary.aspx.cs" Inherits="InSite.Cmds.Admin.Reports.Forms.DepartmentProfileSummary" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

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
                                            <insite:RequiredValidator ID="DepartmentRequired" runat="server" ControlToValidate="Department" FieldName="Department" ValidationGroup="Report" />
                                        </label>
                                        <div>
                                            <cmds:FindDepartment ID="Department" runat="server" ValidationGroup="Report" />
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Profile
                                        </label>
                                        <div>
                                            <cmds:FindProfile ID="CurrentProfile" runat="server" />
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

                    <asp:Panel runat="server" ID="DownloadCommandsPanel" CssClass="mb-3">
                       <insite:DownloadButton runat="server" ID="DownloadXlsx" Text="Download XLSX" />
                       <insite:DownloadButton runat="server" ID="DownloadPdf" Text="Download PDF" />
                    </asp:Panel>

                    <asp:Repeater runat="server" ID="DataRepeater">
                        <ItemTemplate>
                            <div style="page-break-inside:avoid;">
                                <h2><%# Eval("CompanyName") %>: <%# Eval("DepartmentName") %></h2>
                                <h3><%# Eval("ProfileTitle") %></h3>
                                <div class="mb-2">
                                    <%# Eval("CompetencyCount") %>
                                    Competencies as at
                                    <%# DateTime.Today.ToString("MMM d, yyy") %>
                                </div>
                                <asp:Repeater runat="server" ID="EmployeeRepeater">
                                    <HeaderTemplate>
                                        <table class="table table-striped">
                                            <thead>
                                                <tr>
                                                    <th class="align-middle" style="width:40%;">Name</th>
                                                    <th class="align-middle" style="width:60%;">Development Plan</th>
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
                                            <td><%# Eval("PersonFullName") %></td>
                                            <td class="p-0">
                                                <asp:Repeater runat="server" ID="EmployeeStatusRepeater">
                                                    <HeaderTemplate>
                                                        <table class="table table-striped m-0">
                                                            <tbody>
                                                    </HeaderTemplate>
                                                    <FooterTemplate>
                                                            </tbody>
                                                        </table>
                                                    </FooterTemplate>
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td class="w-50"><%# Eval("ValidationStatus") %></td>
                                                            <td class="w-50 text-end"><%# Eval("CompetencyCount") %></td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>

                </div>
            </div>

        </insite:NavItem>
    </insite:Nav>

</asp:Content>
