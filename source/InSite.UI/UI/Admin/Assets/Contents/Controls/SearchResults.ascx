<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Assets.Contents.Controls.SearchResults" %>

<insite:Grid runat="server" ID="Grid">
    <Columns>
        <asp:TemplateField HeaderText="Container Type" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# Eval("ContainerType") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Container Identifier" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# Eval("ContainerIdentifier") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Label" ItemStyle-Wrap="False">
            <ItemTemplate>
                <a href="/ui/admin/assets/contents/edit?id=<%# Eval("ContentIdentifier") %>"><%# Eval("ContentLabel") %></a>
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:TemplateField HeaderText="Language" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# Shift.Common.Language.GetName((string)Eval("ContentLanguage")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Content">
            <ItemTemplate>
                <%# Eval("ContentSnip") %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>