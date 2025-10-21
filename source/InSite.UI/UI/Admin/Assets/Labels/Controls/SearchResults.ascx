<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.UI.Admin.Assets.Labels.Controls.SearchResults" %>

<asp:Literal runat="server" ID="Instructions" />

<insite:Grid runat="server" ID="Grid">
    <Columns>
        <asp:TemplateField ItemStyle-Width="65px">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="pencil" ToolTip='Edit'
                    NavigateUrl='<%# string.Format("/ui/admin/assets/labels/edit?label={0}", GetLabelIdentifier(Eval("ContentLabel"))) %>' />
                <insite:IconLink runat="server" Name="trash-alt" ToolTip="Delete"
                    NavigateUrl='<%# string.Format("/ui/admin/assets/labels/delete?label={0}", GetLabelIdentifier(Eval("ContentLabel"))) %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Placeholder Name">
            <ItemTemplate>
                <a href="/ui/admin/assets/labels/edit?label=<%# GetLabelIdentifier(Eval("ContentLabel")) %>">
                    <%# Eval("ContentLabel") %>
                </a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Placeholder Value(s)">
            <ItemTemplate>
                <%# CreateHtmlTable(Eval("Languages")) %>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</insite:Grid>