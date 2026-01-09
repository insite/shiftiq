<%@ Page Language="C#" CodeBehind="ExpiredCredentials.aspx.cs" Inherits="InSite.Cmds.Admin.Reports.Forms.ExpiredCredentials" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

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

                    <div class="card border-0 shadow-lg">
                        <div class="card-body">

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    User Name
                                </label>
                                <insite:TextBox runat="server" ID="UserName" />
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    User Email
                                </label>
                                <insite:TextBox runat="server" ID="UserEmail" />
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Achievement Type
                                </label>
                                <cmds:AchievementTypeSelector runat="server" ID="AchievementType" />
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Asset Title
                                </label>
                                <insite:TextBox runat="server" ID="AssetTitle" />
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Expired &ge;
                                </label>
                                <insite:DateSelector ID="ExpiredSince" runat="server" />
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Expired &lt;
                                </label>
                                <insite:DateSelector ID="ExpiredBefore" runat="server" />
                            </div>

                        </div>
                    </div>

                    <div class="mt-3">
                        <insite:SearchButton runat="server" ID="ReportButton" Text="Report" Icon="fas fa-chart-bar" ValidationGroup="Report" CausesValidation="true" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>

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

                    <asp:Repeater runat="server" ID="DataRepeater">
                        <HeaderTemplate>
                            <table class="table table-striped">
                                <thead>
                                    <tr>
                                        <td colspan="2" class="text-center text-white" style="background-color:#696969; border-right:1px solid #f3f3f3;">User</td>
                                        <td colspan="4" class="text-center text-white" style="background-color:#696969; border-right:1px solid #f3f3f3;">Asset</td>
                                        <td colspan="2" class="text-center text-white" style="background-color:#696969;"></td>
                                    </tr>
                                    <tr>
                                        <th>Name</th>
                                        <th>Email</th>
                                        <th>Type</th>
                                        <th>Subtype</th>
                                        <th>Code</th>
                                        <th>Title</th>
                                        <th class="text-end">Expired</th>
                                        <th class="text-end">Days Since Expiration</th>
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
                                <td class="text-nowrap">
                                    <%# Eval("ContactName") %>
                                </td>
                                <td><%# Eval("ContactEmail") %></td>
                                <td><%# Eval("AssetType") %></td>
                                <td><%# Eval("AssetSubtype") %></td>
                                <td><%# Eval("AssetCode") %></td>
                                <td><%# Eval("AssetTitle") %></td>
                                <td class="text-nowrap text-end">
                                    <%# Eval("Expired", "{0:MMM d, yyyy}") %>
                                    <div class="form-text">
                                        <%# Eval("Expired", "{0:h:mm tt} ") %>
                                    </div>
                                </td>
                                <td class="text-end">
                                    <%# Eval("DaysSinceExpiration") %>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>

                </div>
            </div>

        </insite:NavItem>
    </insite:Nav>

</asp:Content>
