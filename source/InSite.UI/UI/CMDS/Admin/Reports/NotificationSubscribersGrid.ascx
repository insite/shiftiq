<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NotificationSubscribersGrid.ascx.cs" Inherits="InSite.Custom.CMDS.Admin.Reports.Controls.NotificationsGrid" %>

<asp:Repeater runat="server" ID="NotificationRepeater">
    <HeaderTemplate>
        <table class="table table-striped table-bordered">
            <thead>
                <tr>
                    <th style="width:30%;">Notification</th>
                    <th style="width:35%;">Subscriber</th>
                    <th style="width:35%;">Followers</th>
                </tr>
            </thead>
            <tbody>
    </HeaderTemplate>
    <FooterTemplate>
            </tbody>
        </table>
    </FooterTemplate>
    <ItemTemplate>
        <tr>
            <td>
                <%# Eval("MessageTitle") %>
                <div class="form-text">
                    <%# Eval("MessageName") %>
                </div>
            </td>
            <td colspan="2" class="p-0">
                <table class="table table-striped mb-0">
                    <asp:Repeater runat="server" ID="SubscriberRepeater">
                        <ItemTemplate>
                            <tr>
                                <td style="width:50%;">
                                    <%# ((EmailAddress)Eval("Recipient")).ToHtml() %>
                                </td>
                                <td style="width:50%;padding-top:0;;padding-left:0">
                                    <asp:Repeater runat="server" ID="FollowerRepeater">
                                        <HeaderTemplate><table class="table table-borderless follower-table"><tbody></HeaderTemplate>
                                        <FooterTemplate></tbody></table></FooterTemplate>
                                        <ItemTemplate>
                                            <tr><td><%# ((EmailAddress)Container.DataItem).ToHtml() %></td></tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </table>
            </td>
        </tr>
    </ItemTemplate>
</asp:Repeater>
