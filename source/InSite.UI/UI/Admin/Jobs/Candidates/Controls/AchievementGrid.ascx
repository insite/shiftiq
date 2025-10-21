<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AchievementGrid.ascx.cs" Inherits="InSite.UI.Admin.Jobs.Candidates.Controls.AchievementGrid" %>

<div runat="server" id="NoAchievements" class="alert alert-warning" role="alert">
    There are no achievements
</div>

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField HeaderText="Achievement">
            <ItemTemplate>
                <%# Eval("AchievementTitle") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Status">
            <ItemTemplate>
                <%# Eval("CredentialStatus") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Expiration">
            <ItemTemplate>
                <%# GetExpiration() %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Priority">
            <ItemTemplate>
                <%# Eval("CredentialPriority") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Necessity">
            <ItemTemplate>
                <%# Eval("CredentialNecessity") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Assigned">
            <ItemTemplate>
                <%# GetLocalTime("CredentialAssigned") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Granted">
            <ItemTemplate>
                <%# GetLocalTime("CredentialGranted") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Expiry">
            <ItemTemplate>
                <%# GetCredentialExpiry() %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
