<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompletedGrid.ascx.cs" Inherits="InSite.Admin.Messages.Mailouts.Controls.CompletedGrid" %>

<div class="card">
    <div class="card-body">

        <h3 class="card-title mb-3">Completed <small class="fs-sm text-body-secondary">in the last 90 days</small></h3>
                        
        <p runat="server" id="EmptyGrid" class="help">There are no mailouts completed in the last 90 days.</p>

        <insite:Grid runat="server" ID="Grid">
            <Columns>

                <asp:TemplateField ItemStyle-Width="32px" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <insite:IconLink runat="server" Name="file-alt" Type="Regular"
                            NavigateUrl='<%# Eval("MailoutIdentifier", "/ui/admin/messages/reports/mailout-summary?mailout={0}") %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Scheduled" ItemStyle-Wrap="False" ItemStyle-Width="150px">
                    <ItemTemplate>
                        <%# LocalizeTime((DateTimeOffset?)Eval("MailoutScheduled")) %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Completed" ItemStyle-Wrap="False" ItemStyle-Width="150px">
                    <ItemTemplate>
                        <%# LocalizeTime((DateTimeOffset?)Eval("MailoutCompleted")) %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField HeaderText="" DataField="ContentSubject" />
                <asp:TemplateField HeaderText="Subject">
                    <ItemTemplate>
                        <%# Eval("ContentSubject") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Deliveries" ItemStyle-Wrap="False" ItemStyle-Width="150px">
                    <ItemTemplate>
                        <%# Eval("DeliveryCountSuccess") %> / <%# Eval("DeliveryCount") %>
                        <div class="form-text">
                            <%# GetOneRecipientAddress() %>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>

            </Columns>
        </insite:Grid>

    </div>
</div>