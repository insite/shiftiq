<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Sites.Sites.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="SiteIdentifier">
    <Columns>
            
        <asp:TemplateField HeaderText="Title">
            <ItemTemplate>
                <a href='<%# Eval("SiteIdentifier", "/ui/admin/sites/outline?id={0}") %>'>
                    <%# HttpUtility.HtmlEncode((string)Eval("SiteTitle")) %>
                </a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Name">
            <ItemTemplate>
                    <%# Eval("SiteDomain") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Pages" HeaderStyle-Wrap="false" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="15px">
            <ItemTemplate>
                <%# Eval("PageCount") %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>