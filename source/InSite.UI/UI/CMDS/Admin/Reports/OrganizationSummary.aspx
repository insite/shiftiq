<%@ Page Language="C#" CodeBehind="OrganizationSummary.aspx.cs" Inherits="InSite.Cmds.Admin.Reports.Forms.CompanySummary" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<insite:PageHeadContent runat="server">

    <style type="text/css">
        .table > tbody > tr > th {
            border-bottom: 1px solid #3d78d8;
            background-color: #3d78d8 !important;
            color: #fff;
            padding: 8px 15px;
            text-align: left;
        }

        .table-striped > tbody > tr:nth-child(odd) > th {
            background-color: #f1f1f1
        }

    </style>

</insite:PageHeadContent>

<div id="desktop">

    <insite:ValidationSummary runat="server" ValidationGroup="Report"/>

    <insite:Alert runat="server" ID="ReportStatus" />

    <section runat="server" ID="PreviewSection" class="mb-3">

        <asp:Panel runat="server" ID="DownloadCommandsPanel" CssClass="mb-3">
            <insite:DownloadButton runat="server" ID="DownloadXlsx" Text="Download XLSX" />
            <insite:DownloadButton runat="server" ID="DownloadPdf" Text="Download PDF" />
        </asp:Panel>

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <asp:Repeater runat="server" ID="DataRepeater">
                    <ItemTemplate>
                        <div>
                            <h2><%# Eval("Name") %></h2>
                        </div>
                        <div class="row">
                            <div class="col-md-6">
                                <table class="table table-report table-striped">
                                    <tbody>
                                        <tr>
                                            <th class="align-middle">Department</th>
                                            <th class="align-middle text-end w-100">Users</th>
                                        </tr>
                                        <tr>
                                            <td>All Departments</td>
                                            <td style="text-align: right;"><%# Eval("UserCount", "{0:n0}") %></td>
                                        </tr>
                                        <asp:Repeater runat ="server" ID="DepartmentRepeater">
                                            <ItemTemplate>
                                                <tr>
                                                    <td><%# Eval("DepartmentName") %></td>
                                                    <td style="text-align: right;"><%# Eval("UserCount", "{0:n0}") %></td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                            <div class="col-md-6">
                                <table class="table table-report table-striped">
                                    <tbody>
                                        <tr>
                                            <th>Role</th>
                                            <th style="width: 100px; text-align: right;">Users</th>
                                        </tr>
                                        <asp:Repeater runat="server" ID="RoleRepeater">
                                            <ItemTemplate>
                                                <tr>
                                                    <td><%# Eval("RoleName") %></td>
                                                    <td style="text-align: right;"><%# Eval("UserCount", "{0:n0}") %></td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </section>

</div>
</asp:Content>
