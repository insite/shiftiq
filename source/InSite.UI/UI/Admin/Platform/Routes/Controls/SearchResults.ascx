<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Utilities.Actions.Controls.SearchResults" %>

<asp:Literal ID="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField HeaderText="Action URL">
            <ItemTemplate>
                <a href='<%# Eval("ActionIdentifier", "/ui/admin/platform/routes/edit?id={0}") %>'><%# Eval("ActionUrl") %></a>
                <div class="text-muted fs-sm"><%# Eval("ActionType") %></div>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Name">
            <ItemTemplate>
                <%# Eval("ActionName") %>
                <div class="text-muted fs-sm"><%# GetActionIconHtml(Eval("ActionIcon")) %></div>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Navigation">
            <ItemTemplate>
                <%# Eval("NavigationParentName") %>
                <div class="text-muted fs-sm"><%# (int)Eval("NavigationChildCount") > 0 ? Eval("NavigationChildCount", "{0:n0} subactions") : "" %></div>
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:TemplateField HeaderText="Permission">
            <ItemTemplate>
                <%# Eval("PermissionParentName") %>
                <div class="text-muted fs-sm"><%# (int)Eval("PermissionChildCount") > 0 ? Eval("PermissionChildCount", "{0:n0} subactions") : "" %> <%# (int)Eval("GroupCount") > 0 ? Eval("GroupCount", "{0:n0} groups") : "" %></div>
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:TemplateField HeaderText="Help" HeaderStyle-Wrap="false">
            <ItemTemplate>
                <%# GetHelpHtml(Container.DataItem) %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>