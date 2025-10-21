<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonAchievements.ascx.cs" Inherits="InSite.UI.Portal.Contacts.People.Controls.PersonAchievements" %>

<div class="card">
    <div class="card-body">

        <asp:Literal runat="server" ID="NoAchievements" Text="No achievements to display." />

        <insite:Grid runat="server" id="AchievementGrid">
            <Columns>

                <asp:TemplateField HeaderText="Achievement">
                    <ItemTemplate>
                        <%# Eval("AchievementTitle") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Credential Status">
                    <ItemTemplate>
                        <%# Eval("CredentialStatus") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Credential Granted">
                    <ItemTemplate>
                        <%# LocalizeDate(Eval("CredentialGranted")) %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Credential Revoked">
                    <ItemTemplate>
                        <%# LocalizeDate(Eval("CredentialRevoked")) %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Credential Expired">
                    <ItemTemplate>
                        <%# LocalizeDate(Eval("CredentialExpired")) %>
                    </ItemTemplate>
                </asp:TemplateField>

            </Columns>
        </insite:Grid>

    </div>
</div>