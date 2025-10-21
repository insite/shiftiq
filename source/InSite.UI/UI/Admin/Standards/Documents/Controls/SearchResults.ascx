<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Standards.Documents.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="StandardIdentifier">
    <Columns>
        <asp:BoundField HeaderText="Document Type" DataField="DocumentType" />

        <asp:TemplateField HeaderText="Title">
            <ItemTemplate>
                <a href='/ui/admin/standards/documents/outline?asset=<%# Eval("StandardIdentifier") %>'><%# Eval("ContentTitle") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Level" DataField="LevelType" />

        <asp:TemplateField HeaderText="Posted">
            <ItemTemplate>
                <%# LocalizeDate(Eval("DatePosted")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Is Template?">
            <ItemTemplate>
                <%# (bool)Eval("IsTemplate") ? "Yes" : string.Empty %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Privacy Scope">
            <ItemTemplate>
                <%# GetPrivacyScope() %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Created By">
            <ItemTemplate>
                <%# GetCreatedBy() %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField ItemStyle-Width="65px" ItemStyle-Wrap="false" ItemStyle-CssClass="text-end" HeaderStyle-CssClass="text-end">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="search" ToolTip="Outline"
                    NavigateUrl='<%# Eval("StandardIdentifier", "/ui/admin/standards/documents/outline?asset={0}") %>' />
                <insite:IconLink runat="server" ID="DeleteButton" Name="trash-alt" ToolTip="Delete"
                    NavigateUrl='<%# Eval("StandardIdentifier", "/admin/standards/documents/delete?asset={0}") %>' />
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>