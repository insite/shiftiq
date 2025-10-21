<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Messages.Emails.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField HeaderText="User">
            <ItemTemplate>
                <asp:HyperLink runat="server" Text='<%# Eval("UserFullName") %>'
                    NavigateUrl='<%# Eval("UserIdentifier", "/ui/admin/accounts/users/edit?contact={0}") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="From">
            <ItemTemplate>
                <a href='<%# Eval("SenderIdentifier", "/ui/admin/accounts/senders/edit?id={0}") %>'>
                    <%# Eval("SenderName") %>
                </a>
                <div>
                    <small class="text-body-secondary"><%# Eval("SenderEmail") %></small>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="To">
            <ItemTemplate>
                <%# GetAllMailto(Eval("EmailTo")) %>
                <p>
                    <asp:HyperLink runat="server" Text='<%# Eval("UserTo") %>'
                        NavigateUrl='<%# Eval("UserToIdentifier", "/ui/admin/accounts/users/edit?contact={0}") %>' />
                </p>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Subject">
            <ItemTemplate>
                <table>
                    <tr>
                        <td>
                            <asp:HyperLink runat="server" Text='<%# Eval("EmailSubject") %>' ToolTip="Open Message Outline page"
                                NavigateUrl='<%# Eval("MessageIdentifier", "/ui/admin/messages/outline?message={0}") %>' />
                        </td>
                        <td class="align-top">
                            <insite:IconLink runat="server" Name="file-alt" Type="Regular" ToolTip="Mailout Summary"
                                NavigateUrl='<%# Eval("EmailIdentifier", "/ui/admin/messages/reports/email-summary?email={0}") %>' />
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Status">
            <ItemTemplate>
                <%# Eval("StatusMessage") %>
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:TemplateField HeaderText="Delivery Time" HeaderStyle-Width="50px" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# DateToHtml(Eval("DeliveryTime")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Status" HeaderStyle-Width="40px" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <%# ((bool)Eval("DeliverySuccessful"))?
                        ("<i class=\'far fa-thumbs-up text-success me-1\'></i>Success"):
                        ("<i class=\'far fa-thumbs-down text-danger me-1\'></i>Failure") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Action" HeaderStyle-Width="40px" ItemStyle-Wrap="false" ItemStyle-CssClass="text-end"  HeaderStyle-CssClass="text-end">
            <ItemTemplate>
                <insite:Button Visible='<%# (!(bool)Eval("DeliverySuccessful") & !(bool)Eval("ReDeliverySuccessful")) %>' Icon="fas fa-paper-plane" runat="server" CommandName="Resend" CommandArgument='<%# Eval("EmailIdentifier") %>' Style="padding: 8px;"
                    ToolTip="Resend email" ConfirmText="Send failed e-mail message again?" Text="" />
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
