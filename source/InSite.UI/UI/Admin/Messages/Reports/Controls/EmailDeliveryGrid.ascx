<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmailDeliveryGrid.ascx.cs" Inherits="InSite.Admin.Messages.Reports.Controls.EmailDeliveryGrid" %>

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
        <asp:BoundField HeaderText="Errors" DataField="DeliveryComment" />
    </Columns>
</insite:Grid>

