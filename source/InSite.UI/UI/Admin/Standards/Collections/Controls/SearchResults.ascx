<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Standards.Collections.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="StandardIdentifier">
    <Columns>

        <asp:BoundField HeaderText="Tag" DataField="StandardLabel" />

        <asp:TemplateField HeaderText="Title">
            <ItemTemplate>
                <a href='<%# Eval("StandardIdentifier", "/ui/admin/standards/collections/outline?asset={0}") %>'><%# Eval("ContentTitle") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-Width="60" >
            <ItemTemplate>
                <a title="Outline" href='<%# Eval("StandardIdentifier", "/ui/admin/standards/collections/outline?asset={0}") %>' runat="server"><i class="icon far fa-sitemap"></i></a>
                <a title="Delete" href='<%# Eval("StandardIdentifier", "/admin/standards/collections/delete?asset={0}") %>' runat="server"><i class="icon far fa-trash-alt"></i></a>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
