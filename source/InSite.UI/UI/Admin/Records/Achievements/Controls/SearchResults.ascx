<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Achievements.Achievements.Controls.SearchResults" %>

<asp:Literal runat="server" ID="Instructions" />

<div class="search-results">
<insite:Grid runat="server" ID="Grid" DataKeyNames="AchievementIdentifier">
    <Columns>

        <asp:TemplateField ItemStyle-Wrap="false">
            <ItemTemplate>
                <asp:HyperLink runat="server" Text='<i class="fas fa-pencil"></i>' ToolTip="View Achievement Template"
                    NavigateUrl='<%# Eval("AchievementIdentifier", "/ui/admin/records/achievements/outline?id={0}") %>' />
                <asp:HyperLink runat="server" Text='<i class="fas fa-trash-alt"></i>' ToolTip="Delete Achievement Template"
                    NavigateUrl='<%# Eval("AchievementIdentifier", "/ui/admin/records/achievements/delete?id={0}") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Title"> 
            <ItemTemplate>
                <a href='/ui/admin/records/achievements/outline?<%# Eval("AchievementIdentifier", "id={0}") %>'><%# Eval("AchievementTitle") %></a>
                <i class="fas fa-lock" style="color: red;" runat="server" visible='<%# !(bool)Eval("AchievementIsEnabled") %>'></i>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Tag" ItemStyle-Wrap="False"> 
            <ItemTemplate>
                <%# Eval("AchievementLabel") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Description"> 
            <ItemTemplate>
                <span style="white-space:pre-wrap;"><%# Eval("AchievementDescription") %></span>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Expiration" ItemStyle-Wrap="False"> 
            <ItemTemplate>
                <%# GetExpirationHtml((string)Eval("ExpirationType"), (DateTimeOffset?)Eval("ExpirationFixedDate"), (int?)Eval("ExpirationLifetimeQuantity"), (string)Eval("ExpirationLifetimeUnit")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Layout"> 
            <ItemTemplate>
                <%# Eval("CertificateLayoutCode") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Achievements" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end"> 
            <ItemTemplate>
                <%# Eval("CredentialCount","{0:n0}") %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
</div>