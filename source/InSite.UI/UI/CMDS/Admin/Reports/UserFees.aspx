<%@ Page Language="C#" CodeBehind="UserFees.aspx.cs" Inherits="InSite.Cmds.Admin.Reports.Forms.UserFees" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<style>
    .table.table-striped th { border-color: #e8e8e8;}
</style>

<insite:DownloadButton runat="server" ID="DownloadButton" Text="Download Xlsx" CssClass="mb-3" Visible="false" />

<insite:Nav runat="server" ID="NavPanel">

    <insite:NavItem runat="server" ID="CriteriaSection" Title="Criteria" Icon="far fa-search" IconPosition="BeforeText">
        <section>
            <h2 class="h4 mt-4 mb-3">
                Criteria
            </h2>

            <div class="card border-0 shadow-lg">
                <div class="card-body">
                    <div class="form-group mb-3">
                        <label class="form-label">
                            Year and Month
                        </label>
                        <div>
                            <insite:NumericBox ID="ReportYear" runat="server" Width="80" NumericMode="Integer" DigitGrouping="false" CssClass="d-inline me-2" />
                            <insite:NumericBox ID="ReportMonth" runat="server" Width="80" NumericMode="Integer" CssClass="d-inline" />
                        </div>
                        <div class="form-text"></div>
                    </div>
                    <div class="form-group mb-3">
                        <label class="form-label">
                            Unit Price
                            <insite:RequiredValidator runat="server" ControlToValidate="UnitPricePerPeriodClassA" FieldName="Unit Price" ValidationGroup="Report" />
                        </label>
                        <div>
                            <insite:NumericBox ID="UnitPricePerPeriodClassA" runat="server" Width="80" DecimalPlaces="2" />
                        </div>
                        <div class="form-text"></div>
                    </div>

                    <insite:SearchButton runat="server" ID="SearchButton" />
                </div>
            </div>
        </section>
    </insite:NavItem>
    <insite:NavItem runat="server" ID="PreviewSection1" Title="SUMMARY PER CLASSIFICATION" Icon="far fa-chart-bar" IconPosition="BeforeText" Visible="false">
        <section>
            <h2 class="h4 mt-4 mb-3">
                SUMMARY PER CLASSIFICATION
            </h2>

            <div class="card border-0 shadow-lg">
                <div class="card-body">
                    <table class="table table-stripped">
                        <thead>
                            <tr>
                                <th>Classification</th>
                                <th># Users</th>
                                <th>Unit Price</th>
                                <th>Amount</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater runat="server" ID="SummaryPerClassificationRepeater">
                                <ItemTemplate>
                                    <tr>
                                        <td colspan="4" style="color:#116db6;"><strong><%# Eval("Classification") %></strong></td>
                                    </tr>

                                    <asp:Repeater runat="server" ID="GroupRepeater">
                                        <ItemTemplate>
                                            <tr>
                                                <td><strong><%# Eval("Category") %></strong></td>
                                                <td style="text-align:right;"><%# Eval("UserCount", "{0:n0}") %></td>
                                                <td style="text-align:right;"><%# Eval("UnitPrice", "{0:n2}") %></td>
                                                <td style="text-align:right;"><%# Eval("Amount", "{0:n2}") %></td>
                                            </tr>

                                            <asp:Repeater runat="server" ID="ItemRepeater">
                                                <ItemTemplate>
                                                    <tr>
                                                        <td><%# Eval("CompanyName") %></td>
                                                        <td style="text-align:right;"><%# Eval("UserCount", "{0:n0}") %></td>
                                                        <td></td>
                                                        <td style="text-align:right;"><%# Eval("Amount", "{0:n2}") %></td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </ItemTemplate>
                                    </asp:Repeater>

                                    <tr>
                                        <td></td>
                                        <td style="text-align:right; color:#116db6;"><strong><%# Eval("UserCount", "{0:n0}") %></strong></td>
                                        <td></td>
                                        <td style="text-align:right; color:#116db6;"><strong><%# Eval("Amount", "{0:n2}") %></strong></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <tr>
                                <td></td>
                                <td style="text-align:right;"><strong><asp:Literal runat="server" ID="SummaryPerClassificationUserCount" /></strong></td>
                                <td></td>
                                <td style="text-align:right;"><strong><asp:Literal runat="server" ID="SummaryPerClassificationAmount" /></strong></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </section>
    </insite:NavItem>
    <insite:NavItem runat="server" ID="PreviewSection2" Title="SUMMARY PER ORGANIZATION" Icon="far fa-chart-bar" IconPosition="BeforeText" Visible="false">
        <section>
            <h2 class="h4 mt-4 mb-3">
                SUMMARY PER ORGANIZATION
            </h2>

            <div class="card border-0 shadow-lg">
                <div class="card-body">
                    <table class="table table-stripped">
                        <thead>
                            <tr>
                                <th>Organization</th>
                                <th>Worker/Organization Allocation</th>
                                <th># Users</th>
                                <th>Unit Price</th>
                                <th>Amount</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater runat="server" ID="SummaryPerCompanyRepeater">
                                <ItemTemplate>
                                    <tr>
                                        <td colspan="5" style="color:#116db6;"><strong><%# Eval("Classification") %></strong></td>
                                    </tr>

                                    <asp:Repeater runat="server" ID="GroupRepeater">
                                        <ItemTemplate>
                                            <tr>
                                                <td><strong><%# Eval("CompanyName") %></strong></td>
                                                <td><%# Eval("Category") %></td>
                                                <td style="text-align:right;"><%# Eval("UserCount", "{0:n0}") %></td>
                                                <td style="text-align:right;"><%# Eval("UnitPrice", "{0:n2}") %></td>
                                                <td style="text-align:right;"><%# Eval("Amount", "{0:n2}") %></td>
                                            </tr>

                                            <asp:Repeater runat="server" ID="ItemRepeater">
                                                <ItemTemplate>
                                                    <tr>
                                                        <td><%# Eval("CompanyName") %></td>
                                                        <td><%# Eval("Category") %></td>
                                                        <td style="text-align:right;"><%# Eval("UserCount", "{0:n0}") %></td>
                                                        <td style="text-align:right;"><%# Eval("UnitPrice", "{0:n2}") %></td>
                                                        <td style="text-align:right;"><%# Eval("Amount", "{0:n2}") %></td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </ItemTemplate>
                                    </asp:Repeater>

                                    <tr>
                                        <td></td>
                                        <td></td>
                                        <td style="text-align:right; color:#116db6;"><strong><%# Eval("UserCount", "{0:n0}") %></strong></td>
                                        <td></td>
                                        <td style="text-align:right; color:#116db6;"><strong><%# Eval("Amount", "{0:n2}") %></strong></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <tr>
                                <td></td>
                                <td></td>
                                <td style="text-align:right;"><strong><asp:Literal runat="server" ID="SummaryPerCompanyUserCount" /></strong></td>
                                <td></td>
                                <td style="text-align:right;"><strong><asp:Literal runat="server" ID="SummaryPerCompanyAmount" /></strong></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </section>
    </insite:NavItem>

</insite:Nav>

</asp:Content>
