<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.UI.Desktops.Design.Users.Controls.SearchResults" %>

<insite:Literal runat="server" ID="Instructions" />

<div class="table-responsive">
    <insite:Grid runat="server" ID="Grid" DataKeyNames="UserIdentifier" Translation="Header">
        <Columns>
            <asp:BoundField HeaderText="Name" DataField="FullName" />
            <asp:BoundField HeaderText="Email" DataField="Email" />
            <asp:BoundField HeaderText="Sessions" DataField="SessionCount" />

            <asp:TemplateField HeaderText="Last Authenticated">
                <ItemTemplate>
                    <%# LocalizeDate(Eval("LastAuthenticated")) %>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Action" HeaderStyle-Width="40px" ItemStyle-Wrap="false">
                <ItemTemplate>
                    <insite:Button runat="server"
                        CommandName="SendPasswordReset"
                        ToolTip='<%# Translate("Send Password Reset") %>'
                        ConfirmText="Users Confirm Password Reset"
                        Icon="fas fa-undo"
                        ButtonStyle="Default" />

                    <insite:Button runat="server"
                        CommandName="SendWelcomeEmail"
                        ToolTip='<%# Translate("Send Welcome Email") %>'
                        ConfirmText="Users Send Welcome Email"
                        Icon="fas fa-paper-plane"
                        ButtonStyle="Default" />

                    <%# GetImpersonateIcon((Guid)Eval("UserIdentifier"), (bool)Eval("IsApproved")) %>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </insite:Grid>
</div>