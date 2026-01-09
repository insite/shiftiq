<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Messages.Mailouts.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="MailoutIdentifier">
    <Columns>

        <asp:TemplateField HeaderText="Sender">
            <ItemTemplate>
                <%# Eval("SenderName") %>
                <div>
                    <small class="text-body-secondary"><%# Eval("SenderEmail") %></small>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Subject">
            <ItemTemplate>
                <table>
                    <tr>
                        <td>
                            <asp:HyperLink runat="server" Text='<%# Eval("ContentSubject") %>' ToolTip="Open Message Outline page"
                                NavigateUrl='<%# Eval("MessageIdentifier", "/ui/admin/messages/outline?message={0}") %>' />
                        </td>
                        <td class="align-top">
                            <insite:IconLink runat="server" Name="file-alt" Type="Regular" ToolTip="Mailout Summary"
                                NavigateUrl='<%# Eval("MailoutIdentifier", "/ui/admin/messages/reports/mailout-summary?mailout={0}") %>' />
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Scheduled" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# GetLocalTime("MailoutScheduled") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Completed" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# GetLocalTime("MailoutCompleted") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Deliveries" DataField="DeliveryCount" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
        <asp:BoundField HeaderText="Successes" DataField="DeliveryCountSuccess" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
        <asp:BoundField HeaderText="Failures" DataField="DeliveryCountFailure" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
    </Columns>
</insite:Grid>