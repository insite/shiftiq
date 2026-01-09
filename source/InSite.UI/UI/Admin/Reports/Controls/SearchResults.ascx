<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.UI.Admin.Reports.Controls.SearchResults" %>

<insite:Literal runat="server" ID="Instructions" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="OrganizationIdentifier" Translation="Header">
    <Columns>
        <asp:TemplateField HeaderText="Report Title">
            <ItemTemplate>
                    <a runat="server" href='<%# GetTitleUrl() %>'>
                        <%# Eval("ReportTitle") %>
                    </a>
                <div><small class="text-body-secondary"><%# Eval("ReportType") %></small></div>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Report Description">
            <ItemTemplate>
                <%# GetHtml(Eval("ReportDescription")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Last Modified">
            <ItemTemplate>
                <%# GetDataTimeHtml((DateTimeOffset?)Eval("Modified"), (string)Eval("ModifiedByFullName")) %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Created">
            <ItemTemplate>
                <%# GetDataTimeHtml((DateTimeOffset?)Eval("Created"), (string)Eval("CreatedByFullName")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Action" HeaderStyle-Width="40px" ItemStyle-Wrap="false" ItemStyle-CssClass="text-end" HeaderStyle-CssClass="text-end">
            <ItemTemplate>
                <a runat="server" href='<%# Eval("ReportIdentifier", "/ui/admin/reports/edit?id={0}") %>'><i class="far fa-pencil"></i></a>
                <a runat="server" href='<%# Eval("ReportIdentifier", "/ui/admin/reports/delete?id={0}") %>' visible='<%# Eval("CanDelete") %>'><i class="far fa-trash-alt"></i></a>
                <a runat="server" href='<%# Eval("ReportIdentifier", "/ui/admin/reports/build?id={0}&execute=true") %>' visible='<%# Eval("CanExecute") %>'><i class="far fa-bolt"></i></a>
                <a runat="server" href='<%# Eval("ReportIdentifier", "/ui/admin/reports/build?id={0}") %>' visible='<%# Eval("CanConfigure") %>'><i class="far fa-cog"></i></a>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>