<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Sites.Pages.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="PageIdentifier">
    <Columns>
            
        <asp:TemplateField HeaderText="Page">
            <ItemTemplate>
                <a href='<%# Eval("PageIdentifier", "/ui/admin/sites/pages/outline?id={0}") %>'>
                    <%# (string)Eval("PageTitle") %>
                </a>
                <div class="form-text">
                    <%# Eval("PageSlug") %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Publication status">
            <ItemTemplate>
                <%# (string)Eval("PublicationStatus") == "Published"
                    ? "<span class='badge bg-success'>Published</span>" 
                    : "<span class='badge bg-danger'>Unpublished</span>" %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Group Permissions">
            <ItemTemplate>
                <%# Eval("GroupPermissions") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Type" DataField="PageType" />

        <asp:TemplateField HeaderText="Hook">
            <ItemTemplate>
                <%# Eval("PageHook") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Sitemap" ItemStyle-Wrap="False" ItemStyle-Width="47">
            <ItemTemplate>
                <span class="form-text">
                    <%# GetHierarchyHtml() %>
                </span>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Site">
            <ItemTemplate>
                <a href='<%# Eval("SiteIdentifier", "/ui/admin/sites/outline?id={0}") %>'>
                    <%# HttpUtility.HtmlEncode((string)Eval("SiteTitle")) %>
                </a>
                <div class="form-text">
                    <%# Eval("SiteName") %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Last Modified" HeaderStyle-Wrap="false" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# LocalizeDate(Eval("LastChangeTime")) %>
                <div class="form-text">
                    by <%# Eval("LastChangeUser") %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>