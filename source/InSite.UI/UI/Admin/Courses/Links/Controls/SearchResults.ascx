<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Courses.Links.Controls.SearchResults" %>

<asp:Literal ID="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>
            
        <asp:TemplateField HeaderText="Publisher">
            <ItemTemplate>
                <%# Eval("Publisher") %>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Title">
            <ItemTemplate>
                <a href='<%# Eval("LinkIdentifier", "/ui/admin/courses/links/edit?link={0}") %>'><%# Eval("Title") %></a>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Location">
            <ItemTemplate>
                <%# Eval("Location") %>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Subtype">
            <ItemTemplate>
                <%# Eval("Subtype") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Code">
            <ItemTemplate>
                <%# Eval("Code") %>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-Width="40px">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="pencil" NavigateUrl='<%# Eval("LinkIdentifier", "/ui/admin/courses/links/edit?link={0}") %>' ToolTip="Edit" />
                <insite:IconLink runat="server" Name="trash-alt" NavigateUrl='<%# Eval("LinkIdentifier", "/ui/admin/courses/links/delete?link={0}") %>' ToolTip="Delete" />
                <insite:IconLink runat="server" Name="rocket" NavigateUrl='<%# Eval("AssetNumber", "/ui/portal/integrations/lti/consumer?number={0}") %>' ToolTip="Launch" />
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>