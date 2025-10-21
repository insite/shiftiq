<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NotificationsTab.ascx.cs" Inherits="InSite.UI.CMDS.Admin.Users.Controls.NotificationsTab" %>

<section>

    <h2 class="h4 mt-4 mb-3">Notifications</h2>

    <div class="card border-0 shadow-lg">
        <div class="card-body">

            <div runat="server" ID="NoDataPanel" class="alert alert-info" role="alert">
                There are no email notifications configured for this person.
            </div>

            <insite:Nav runat="server">
                <insite:NavItem runat="server" ID="SubscriptionTab" Title="Subscriptions">
                    <insite:Grid runat="server" ID="NotificationsGrid">
                        <Columns>
        
                            <asp:TemplateField HeaderText="Notification" ItemStyle-Wrap="False">
                                <ItemTemplate>
                                    <%# Eval("MessageTitle") %>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Subscribed" ItemStyle-Wrap="False">
                                <ItemTemplate>
                                    <%# Eval("SubscribedText") %>
                                </ItemTemplate>
                            </asp:TemplateField>

                        </Columns>
                    </insite:Grid>
                </insite:NavItem>
                <insite:NavItem runat="server" ID="FollowingTab" Title="Carbon Copies">
                    <insite:Grid runat="server" ID="FollowingGrid" DataKeyNames="SubscriberIdentifier,MessageIdentifier">
                        <Columns>
                            <asp:TemplateField HeaderText="Subscriber">
                                <ItemTemplate>
                                    <%# Eval("SubscriberName") %>
                                    <div class="form-text"><%# Eval("SubscriberEmail") %></div>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Notification">
                                <ItemTemplate>
                                    <%# Eval("MessageTitle") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Subscribed" ItemStyle-Wrap="False">
                                <ItemTemplate>
                                    <%# LocalizeTime(Eval("Subscribed")) %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="15px" ItemStyle-Wrap="false">
                                <ItemTemplate>
                                    <asp:LinkButton runat="server" Text="<i class='far fa-trash-alt'></i>" ToolTip="Delete" CommandName="Delete" OnClientClick="return confirm('Delete this carbon copy?')" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </insite:Grid>
                </insite:NavItem>
            </insite:Nav>

        </div>
    </div>

</section>