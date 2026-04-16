<%@ Control Language="C#" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.UI.Admin.Messages.Mailouts.Failures.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField HeaderText="Subject">
            <ItemTemplate>
                <asp:HyperLink runat="server" Text='<%# Eval("MessageSubject") %>' ToolTip="Open Message Outline page"
                    NavigateUrl='<%# Eval("MessageIdentifier", "/ui/admin/messages/outline?message={0}") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Sender">
            <ItemTemplate>
                <%# Eval("SenderName") %>
                <div>
                    <small class="text-body-secondary"><%# Eval("SenderEmail") %></small>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Scheduled" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# GetLocalTime("MailoutScheduled") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Recipient">
            <ItemTemplate>
                <%# GetRecipientHtml() %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Failed" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# GetLocalTime("MailoutFailed") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Status" DataField="DeliveryStatus" />

        <asp:BoundField HeaderText="Failure Reason" DataField="DeliveryError" />

    </Columns>
</insite:Grid>
