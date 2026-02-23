<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.UI.Admin.Database.Entities.SearchResults" %>

<asp:Literal runat="server" ID="Instructions" />

<insite:Grid runat="server" ID="Grid">
    <Columns>
        
        <asp:TemplateField HeaderText="Type">
            <ItemTemplate>
                <%# Eval("SubsystemType") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Subsystem">
            <ItemTemplate>
                <%# Eval("SubsystemName") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Component">
            <ItemTemplate>
                <%# Eval("SubsystemComponent") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Entity">
            <ItemTemplate>
                <%# Eval("EntityName") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Collection">
            <ItemTemplate>
                <%# Eval("CollectionSlug") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Structure">
            <ItemTemplate>
                <%# Eval("StorageStructure") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Schema">
            <ItemTemplate>
                <%# Eval("StorageSchema") %>
                <div class="text-info fs-sm"><%# Eval("StorageSchemaRename") %></div>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Table">
            <ItemTemplate>
                <%# Eval("StorageTable") %>
                <div class="text-info fs-sm"><%# Eval("StorageTableRename") %></div>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Key">
            <ItemTemplate>
                <%# Eval("StorageKey") %>
            </ItemTemplate>
        </asp:TemplateField>
        
    </Columns>
</insite:Grid>