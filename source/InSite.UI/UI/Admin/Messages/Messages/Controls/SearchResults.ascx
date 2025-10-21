<%@ Control AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Messages.Messages.Controls.SearchResults" Language="C#" %>

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField HeaderText="Subject">
            <ItemTemplate>
                <div class="float-end">
                    <%# (bool)Eval("IsDisabled") ? "<span class='badge bg-danger'>Disabled</span>" : "" %>
                </div>
                <%# GetMessageSubjectHtml(Container.DataItem) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Sender" ItemStyle-Wrap="False">
            <ItemTemplate>
                <insite:Container runat="server" Visible='<%# Eval("SenderEmail") != null %>'>
                    <div class="float-end">
                         <%# (!(bool)(((bool?)Eval("SenderEnabled"))??false) ? "<span class='badge bg-danger'>Disabled</span>" : "") %>
                    </div>
                    <%# Eval("SenderName") %>
                    <div class="form-text">
                        <a href='mailto:<%# Eval("SenderEmail") %>'><%# Eval("SenderEmail") %></a>
                    </div>
                </insite:Container>
                <insite:Container runat="server" Visible='<%# Eval("SenderEmail") == null %>'>
                    <div class="float-end">
                         <span class='badge bg-danger'>Deleted</span>
                    </div>
                    <i>Sender Not Found</i>
                </insite:Container>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="System Mailbox" ItemStyle-Wrap="False">
            <ItemTemplate>
                <insite:Container runat="server" Visible='<%# Eval("SenderEmail") != null %>'>
                    <%# Eval("SenderNickname") %>
                    <div class="form-text">
                        <%# Eval("SystemMailbox") %>
                    </div>
                </insite:Container>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Last Modified">
            <ItemTemplate>
                <%# GetMessageLastChange(Container.DataItem) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Subscribers" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end" ItemStyle-Wrap="False">
            <ItemTemplate>
                <div title='<%# Eval("SubscriberMembershipCountText") %>'><%# Eval("SubscriberGroupCountText") %></div>
                <div><%# Eval("SubscriberUserCountText") %></div>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField HeaderText="Links" DataField="LinkCount" DataFormatString="{0:n0}" ItemStyle-CssClass="text-end" ItemStyle-Width="80px" HeaderStyle-CssClass="text-end" />
        <asp:BoundField HeaderText="Mailouts" DataField="MailoutCount" DataFormatString="{0:n0}" ItemStyle-CssClass="text-end" ItemStyle-Width="80px" HeaderStyle-CssClass="text-end" />

    </Columns>
</insite:Grid>