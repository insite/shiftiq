<%@ Page Language="C#" CodeBehind="CollegeCertificates.aspx.cs" Inherits="InSite.Cmds.Actions.Reporting.Report.CollegeCertificates" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
        table.table-report tr.odd {
            --bs-table-accent-bg: var(--bs-table-striped-bg);
            color: var(--bs-table-striped-color);
        }
    </style>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:ValidationSummary runat="server" ValidationGroup="Report" />

    <insite:CustomValidator runat="server" ID="DepartmentIdentifierValidator" ErrorMessage="At least one department must be selected" Display="None" ValidationGroup="Report" />
    <insite:CustomValidator runat="server" ID="EmployeeIdentifierValidataor" ErrorMessage="At least one employee must be selected" Display="None" ValidationGroup="Report" />

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
                                            Departments
                                        </label>
                                        <cmds:FindDepartment runat="server" ID="DepartmentIdentifier" MaxSelectionCount="0" />
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Membership
                                        </label>
                                        <div>
                                            <asp:CheckBoxList runat="server" ID="MembershipType" RepeatDirection="Vertical" RepeatLayout="Flow">
                                                <asp:ListItem Value="Department" Text="Department Employment" Selected="True" />
                                                <asp:ListItem Value="Organization" Text="Organization Employment" />
                                                <asp:ListItem Value="Administration" Text="Data Access" />
                                            </asp:CheckBoxList>
                                            <asp:Button runat="server" ID="MembershipTypeChanged" CssClass="d-none" />
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Employees
                                        </label>
                                        <cmds:FindPerson runat="server" ID="EmployeeIdentifier" MaxSelectionCount="0" />
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            School
                                        </label>
                                        <cmds:SchoolSelector ID="InstitutionIdentifier" runat="server" />
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Certificate Type
                                        </label>
                                        <insite:ComboBox ID="CertificateType" runat="server">
                                            <Items>
                                                <insite:ComboBoxOption Value="Requested" Text="Requested" />
                                                <insite:ComboBoxOption Value="Submitted to College" Text="Submitted to College" />
                                                <insite:ComboBoxOption Value="Granted by College" Text="Granted by College" Selected="true" />
                                                <insite:ComboBoxOption Value="All" Text="All" />
                                            </Items>
                                        </insite:ComboBox>
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

                    <insite:DownloadButton runat="server" ID="DownloadButton" Text="Download PDF" CssClass="mb-3" />

                    <asp:Repeater runat="server" ID="DepartmentRepeater">
                        <ItemTemplate>
                            <table class="table table-report">
                                <tr>
                                    <th colspan="3" class="fs-5">
                                        <%# Eval("CompanyName") %>, <%# Eval("DepartmentName") %>
                                    </th>
                                </tr>
                                <tr>
                                    <th class="w-25">Worker</th>
                                    <th>Certificate</th>
                                    <th class="w-25">School</th>
                                </tr>

                                <asp:Repeater runat="server" ID="CertificateRepeater">
                                    <ItemTemplate>
                                        <tr>
                                            <td colspan="3">
                                                <strong><%# Eval("Name") %></strong>
                                            </td>
                                        </tr>
                                        <asp:Repeater runat="server" ID="EmployeeRepeater">
                                            <ItemTemplate>
                                                <tr class="<%# (Container.ItemIndex + 1) % 2 == 0 ? "even" : "odd" %>">
                                                    <td><%# Eval("FullName") %></td>
                                                    <td>
                                                        <%# Eval("ProfileTitle") %>
                                                    </td>
                                                    <td><%# Eval("InstitutionName") %></td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </table>
                        </ItemTemplate>
                    </asp:Repeater>

                </div>
            </div>

        </insite:NavItem>
    </insite:Nav>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            (() => {
                Sys.Application.add_load(function () {
                    $('#<%= MembershipType.ClientID %> input[type="checkbox"]')
                        .off('change', onMembershipTypeChanged)
                        .on('change', onMembershipTypeChanged);
                });

                function onMembershipTypeChanged() {
                    document.getElementById('<%= DepartmentIdentifier.ClientID %>').disable();
                    document.getElementById('<%= EmployeeIdentifier.ClientID %>').disable();
                    $('#<%= ReportButton.ClientID %>').addClass('disabled');

                    $('#<%= MembershipTypeChanged.ClientID %>').trigger('click');
                }
            })();
        </script>
    </insite:PageFooterContent>

</asp:Content>
