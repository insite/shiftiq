<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InvoiceItemGrid.ascx.cs" Inherits="InSite.Admin.Invoices.Controls.InvoiceItemGrid" %>

<div class="float-end" style="padding-bottom: 15px;">
    <insite:Button runat="server" ID="AddItemLink" Text="Add Item" Icon="fas fa-plus-circle" ButtonStyle="Default" />
</div>

<div class="clearfix"></div>

<insite:Grid runat="server" ID="Grid" DataKeyNames="ItemIdentifier">
    <Columns>
        <asp:TemplateField HeaderText="#">
            <ItemTemplate>
                <%# Eval("ItemSequence", "{0:n0}") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Item" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# Eval("ItemDescription") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Quantity">
            <ItemTemplate>
                <%# Eval("ItemQuantity", "{0:n0}") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Price">
            <ItemTemplate>
                <%# Eval("ItemPrice", "{0:c2}") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Tax Total" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# FormatTax(Eval("TaxRate"), Eval("ItemPrice"), Eval("ItemQuantity")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Total">
            <ItemTemplate>
                <%# GetAmount((decimal)Eval("ItemPrice"), (int)Eval("ItemQuantity"), Eval("TaxRate")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Product">
            <ItemTemplate>
                <a href="<%# Eval("ProductIdentifier", "/ui/admin/sales/products/edit?id={0}") %>"><%# GetProductName((Guid)Eval("ProductIdentifier")) %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField ItemStyle-Wrap="false" ItemStyle-Width="40px">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="pencil" ToolTip="Revise"
                    NavigateUrl='<%# string.Format("/ui/admin/sales/invoices/items/revise?invoice={0}&item={1}", Eval("InvoiceIdentifier"), Eval("ItemIdentifier")) %>' />
                <insite:IconButton runat="server" Name="trash-alt" CommandName="Delete" ToolTip='Remove item'
                    ConfirmText='<%# string.Format("Are you sure you want to remove this item from this invoice?") %>' />
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>

</insite:Grid>

<div class="float-end pt-2">
    Total:
    <asp:Literal runat="server" ID="TotalAmount" />
</div>
