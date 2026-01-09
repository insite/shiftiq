<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ScheduledGrid.ascx.cs" Inherits="InSite.Admin.Messages.Mailouts.Controls.ScheduledGrid" %>

<div class="card">
    <div class="card-body">

        <h3 class="card-title mb-3">Scheduled</h3>

        <p runat="server" id="EmptyGrid" class="help">There are no scheduled mailouts.</p>

        <insite:Grid runat="server" ID="Grid">
            <Columns>

                <asp:TemplateField ItemStyle-Width="90px" ItemStyle-HorizontalAlign="Center" ItemStyle-Wrap="false">
                    <ItemTemplate>
                        <insite:IconLink runat="server" Name="file-alt" Type="Regular" NavigateUrl='<%# Eval("MailoutIdentifier", "/ui/admin/messages/reports/mailout-summary?mailout={0}") %>' />
                        <insite:IconButton runat="server" Visible='<%# !(bool)Eval("IsStarted") %>' Name="ban" ToolTip="Cancel Mailout" CommandName="Clear" CommandArgument='<%# Eval("MailoutIdentifier") %>' OnClientClick="return confirm('Are you sure you want to cancel this mailout?')"></insite:IconButton>
                        <insite:IconButton runat="server" Visible='<%# !(bool)Eval("IsCompleted") %>' Name="flag-checkered" ToolTip="Complete Mailout" CommandName="Complete" CommandArgument='<%# Eval("MailoutIdentifier") %>' OnClientClick="return confirm('Are you sure you want to mark this mailout completed?')"></insite:IconButton>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Scheduled" HeaderStyle-Wrap="False" ItemStyle-Width="150px" ItemStyle-Wrap="false">
                    <ItemTemplate>
                        <%# LocalizeTime((DateTimeOffset?)Eval("MailoutScheduled")) %>
                        <div>
                            <label class="badge bg-<%# (bool)Eval("IsOverdue") ? "danger" : "success" %>">
                                <%# Eval("Age") %>
                            </label>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Started" HeaderStyle-Wrap="False" ItemStyle-Width="150px" ItemStyle-Wrap="false">
                    <ItemTemplate>
                        <%# LocalizeTime((DateTimeOffset?)Eval("MailoutStarted")) %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField HeaderText="Subject" DataField="ContentSubject" />

                <asp:BoundField HeaderText="Subscribers" DataField="SubscriberCount" DataFormatString="{0:n0}" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="100px" />
            
                <asp:TemplateField HeaderText="Deliveries" HeaderStyle-Wrap="False" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="100px">
                    <ItemTemplate>
                        <%# Eval("DeliveryCount") %>
                        <div class="form-text"><%# GetOneRecipientAddress(Container.DataItem) %></div>
                    </ItemTemplate>
                </asp:TemplateField>

            </Columns>
        </insite:Grid>

    </div>
</div>