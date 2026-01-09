<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Utilities.Collections.Controls.SearchResults" %>

<asp:Literal runat="server" ID="Instructions" />

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField ItemStyle-Width="65px">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="pencil" Type="Regular" ToolTip='Edit'
                    NavigateUrl='<%# Eval("CollectionIdentifier", "/ui/admin/assets/collections/edit?collection={0}") %>' />
                <insite:IconLink runat="server" Name="trash-alt" Type="Regular" ToolTip="Delete"
                    NavigateUrl='<%# Eval("CollectionIdentifier", "/admin/assets/collections/delete?collection={0}") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Name">
            <ItemTemplate>
                <a href='<%# Eval("CollectionIdentifier", "/ui/admin/assets/collections/edit?collection={0}") %>'>
                    <%# Eval("CollectionName") %>
                </a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Toolkit" DataField="CollectionTool" />
        <asp:BoundField HeaderText="Type" DataField="CollectionType" />
        <asp:BoundField HeaderText="Process" DataField="CollectionProcess" />
        <asp:BoundField HeaderText="# of Items" DataField="ItemsCount" />

    </Columns>
</insite:Grid>