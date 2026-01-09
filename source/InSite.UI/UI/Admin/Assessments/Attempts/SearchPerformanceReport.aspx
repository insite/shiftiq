<%@ Page Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" AutoEventWireup="false" CodeBehind="SearchPerformanceReport.aspx.cs" Inherits="InSite.UI.Admin.Assessments.Attempts.SearchPerformanceReport" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <insite:Alert runat="server" ID="CommandAlert" />

    <asp:Repeater runat="server" ID="ReportRepeater">
        <HeaderTemplate>
            <table class="table table-striped">
                <thead>
                    <tr>
                        <th></th>
                        <th>Report</th>
                        <th>Language</th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td style="width:70px;">
                    <asp:HyperLink runat="server"
                        Visible='<%# CanEdit && (bool)Eval("CanEdit") %>'
                        Text='<i class="fas fa-pencil"></i>'
                        ToolTip="Edit Report"
                        NavigateUrl='<%# Eval("ReportId", "/ui/admin/assessments/attempts/edit-performance-report?id={0}") %>'
                    />
                    <insite:IconButton runat="server"
                        CommandName="Delete"
                        CommandArgument='<%# Eval("ReportId") %>'
                        Visible='<%# Eval("CanDelete") %>'
                        Name="fas fa-trash-alt"
                        ToolTip="Delete Report"
                        OnClientClick='<%# Eval("Name", "return confirm(\"Are you sure to delete the report `{0}` ?\")") %>'
                    />
                </td>
                <td>
                    <%# Eval("Name") %>
                    <asp:Repeater runat="server" ID="RoleRepeater">
                        <ItemTemplate>
                            <%# Eval("Name") %>
                            <%# Eval("Weight", "{0:p0}") %>
                        </ItemTemplate>
                    </asp:Repeater>
                </td>
                <td>
                    <%# Eval("Language") %>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </tbody></table>
        </FooterTemplate>
    </asp:Repeater>

    <insite:CancelButton runat="server" ID="BackButton" Text="Back to Report Builder" Icon="far fa-arrow-left" />

</asp:Content>
