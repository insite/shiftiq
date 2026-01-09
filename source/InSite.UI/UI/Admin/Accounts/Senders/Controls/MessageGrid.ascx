<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MessageGrid.ascx.cs" Inherits="InSite.Admin.Accounts.Senders.Controls.MessageGrid" %>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="MessageUpdatePanel" />

<insite:UpdatePanel runat="server" ID="MessageUpdatePanel">
    <ContentTemplate>
        <insite:Grid runat="server" ID="Grid" DataKeyNames="MessageIdentifier">
            <Columns>

                <asp:TemplateField HeaderText="Subject and Name">
                    <itemtemplate>
                        <div><a title="Message Outline" href='/ui/admin/messages/outline?message=<%# Eval("MessageIdentifier") %>'><%# Eval("ContentSubject") %></a></div>
                        <div class="form-text"><%# Eval("MessageName").Equals(Eval("ContentSubject")) ? string.Empty : Eval("MessageName") %></div>
                    </itemtemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Organization">
                    <itemtemplate>
                        <div><a title="Edit Organization" href='/ui/admin/accounts/organizations/edit?organization=<%# Eval("OrganizationIdentifier") %>'><%# GetOrganizationName((Guid)Eval("OrganizationIdentifier")) %></a></div>
                        <div class="form-text"><%# GetOrganizationCode((Guid)Eval("OrganizationIdentifier")) %></div>
                    </itemtemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Mailouts" ItemStyle-Wrap="False" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                    <itemtemplate>
                        <div><a title="Mailouts Count" href='/ui/admin/messages/mailouts/search?type=<%# Eval("MessageName") %>&panel=results&organization=<%# Eval("OrganizationIdentifier") %>'><%# Eval("MailoutCount") %></a></div>
                    </itemtemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Recipients" ItemStyle-Wrap="False" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                    <itemtemplate>
                        <%# Eval("RecipientCount") %>
                    </itemtemplate>
                </asp:TemplateField>

            </Columns>
        </insite:Grid>
    </ContentTemplate>
</insite:UpdatePanel>