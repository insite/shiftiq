<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MailoutDeliveryGrid.ascx.cs" Inherits="InSite.Admin.Messages.Reports.Controls.MailoutDeliveryGrid" %>

<div runat="server" id="FilterPanel" class="mb-3">
    <insite:TextBox runat="server" ID="FilterText" Width="300" EmptyMessage="Filter" CssClass="d-inline-block" />
    <insite:IconButton runat="server" ID="FilterButton" Name="filter" ToolTip="Filter" CssClass="p-2" />

    <insite:PageFooterContent runat="server">
        <script type="text/javascript"> 
            (function () {
                Sys.Application.add_load(function () {
                    $('#<%= FilterText.ClientID %>')
                        .off('keydown', onKeyDown)
                        .on('keydown', onKeyDown);
                });

                function onKeyDown(e) {
                    if (e.which === 13) {
                        e.preventDefault();
                        $('#<%= FilterButton.ClientID %>')[0].click();
                    }
                }
            })();
        </script>
    </insite:PageFooterContent>
</div>

<insite:Grid runat="server" ID="Grid">
    <Columns>
        <asp:BoundField HeaderText="Date and Time" DataField="FormattedStatusDate" ItemStyle-Wrap="false" />
        <asp:BoundField HeaderText="Recipient Name" DataField="RecipientName" ItemStyle-Wrap="false" />
        <asp:BoundField HeaderText="Recipient Email" DataField="RecipientAddress" ItemStyle-Wrap="false" />
        <asp:TemplateField HeaderText="Carbon Copies">
            <itemtemplate>
                <%# Eval("CarbonCopies") %>
            </itemtemplate>
        </asp:TemplateField>
        <asp:BoundField HeaderText="Errors" DataField="DeliveryComment" />

        <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-Width="30px">
            <itemtemplate>
                <a href='<%# string.Format("/ui/admin/messages/deliveries/view?mailout={0}&recipient={1}", MailoutIdentifier, HttpUtility.UrlEncode((string)Eval("RecipientAddress"))) %>' title="View Email"><i class="icon far fa-search x20"></i></a>
            </itemtemplate>
        </asp:TemplateField>
    </Columns>
</insite:Grid>
