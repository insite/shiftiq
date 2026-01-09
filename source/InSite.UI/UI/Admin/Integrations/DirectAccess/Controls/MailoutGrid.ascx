<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MailoutGrid.ascx.cs" Inherits="InSite.UI.Desktops.Custom.SkilledTradesBC.Individuals.Controls.MailoutGrid" %>

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

            <asp:TemplateField HeaderText="External Subject">
                <ItemTemplate>
                    <%# Eval("ContentSubject") %>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Scheduled" ItemStyle-Wrap="false">
                <ItemTemplate>
                    <%# GetLocalTime((DateTimeOffset)Eval("MailoutScheduled")) %>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Completed" ItemStyle-Wrap="false">
                <ItemTemplate>
                    <%# GetLocalTime((DateTimeOffset?)Eval("MailoutCompleted")) %>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:BoundField HeaderText="Deliveries" DataField="DeliveryCount" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
            <asp:BoundField HeaderText="Successes" DataField="DeliveryCountSuccess" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
            <asp:BoundField HeaderText="Failures" DataField="DeliveryCountFailure" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
        </Columns>
</insite:Grid>
