<%@ Page Language="C#" CodeBehind="MultiorganizationUsers.aspx.cs" Inherits="InSite.Cmds.Actions.Reporting.Report.MultiorganizationUsers" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="CriteriaTab" Title="Criteria" Icon="far fa-search" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Criteria
                </h2>

                <div class="row">
                    <div class="col-lg-6">

                        <div class="card border-0 shadow-lg">
                            <div class="card-body">

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Organization
                                    </label>
                                    <cmds:FindCompany runat="server" ID="CompanyIdentifier" EmptyMessage="All Organizations"  MaxSelectionCount="0" />
                                </div>

                            </div>
                        </div>

                    </div>
                </div>

            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="UserTab" Title="Users" Icon="far fa-chart-bar" IconPosition="BeforeText" Visible="false">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Users with access to multiple organizations
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <asp:Repeater runat="server" ID="UserRepeater">
                            <HeaderTemplate>
                                <table class="table table-striped">
                                    <thead>
                                        <tr>
                                            <th class="align-middle">User</th>
                                            <th class="align-middle">Organizations</th>
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
                                    <td class="text-nowrap"><%# Eval("FullName") %></td>
                                    <td><%# Eval("PersonCompanies") %></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>

                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="OrganizationTab" Title="Organizations" Icon="far fa-city" IconPosition="BeforeText" Visible="false">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Organizations accessed by multi-organization users
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <asp:Repeater runat="server" ID="CompanyRepeater">
                            <HeaderTemplate>
                                <table class="table table-striped">
                                    <thead>
                                        <tr>
                                            <th class="align-middle">Organization</th>
                                            <th class="align-middle">Users</th>
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
                                    <td><%# Eval("CompanyName") %></td>
                                    <td runat="server" visible='<%# (int)Eval("ItemsCount") <= 1 %>'>
                                        <%# Eval("FirstItem.FullName") %>
                                    </td>
                                    <td runat="server" class="p-0" visible='<%# (int)Eval("ItemsCount") > 1 %>'>
                                        <asp:Repeater runat="server" ID="UserRepeater">
                                            <HeaderTemplate>
                                                <table class="table table-striped m-0"><tbody>
                                            </HeaderTemplate>
                                            <FooterTemplate>
                                                </tbody></table>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <tr>
                                                    <td class='<%# GetCompanyUserClass() %>'><%# Eval("FullName") %></td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>

                    </div>
                </div>
            </section>
        </insite:NavItem>

    </insite:Nav>
</asp:Content>
