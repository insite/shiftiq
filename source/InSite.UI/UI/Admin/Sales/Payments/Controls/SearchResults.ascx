<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Payments.Controls.SearchResults" %>

<insite:Grid runat="server" ID="Grid" DataKeyNames="PaymentIdentifier">
    <Columns>

        <asp:TemplateField HeaderText="Customer Employer">
            <ItemTemplate>
                <%# Eval("CreatedInvoice.CustomerEmployer") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Customer Name">
            <ItemTemplate>
                <%# Eval("CreatedByUser.UserFullName") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Customer Email">
            <ItemTemplate>
                <%# Eval("CreatedByUser.UserEmail") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Invoice Number">
            <ItemTemplate>
                <a href='/ui/admin/sales/invoices/outline?<%# Eval("InvoiceIdentifier", "id={0}") %>'>
                    <%# Eval("CreatedInvoice.InvoiceNumber") %>
                </a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Payment Status">
            <ItemTemplate>
                <%# Eval("PaymentStatus") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Product Name">
            <ItemTemplate>
                <asp:Repeater runat="server" ID="ProductRepeater">
                    <ItemTemplate>
                        <%# Container.ItemIndex > 0 ? ", " : "" %>
                        <%# Eval("ProductName") %>
                    </ItemTemplate>
                </asp:Repeater>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Payment Amount">
            <ItemTemplate>
                <%# Eval("PaymentAmount", "{0:n2}") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Payment Aborted">
            <ItemTemplate>
                <%# GetLocalTime(Eval("PaymentAborted")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Payment Approved">
            <ItemTemplate>
                <%# GetLocalTime(Eval("PaymentApproved")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Payment Declined">
            <ItemTemplate>
                <%# GetLocalTime(Eval("PaymentDeclined")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Payment Started">
            <ItemTemplate>
                <%# GetLocalTime(Eval("PaymentStarted")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Transaction ID">
            <ItemTemplate>
                <%# Eval("TransactionId") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField ItemStyle-Width="60px" ItemStyle-Wrap="false">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="search" ToolTip="Outline"
                    NavigateUrl='<%# Eval("PaymentIdentifier", "/ui/admin/sales/payments/outline?id={0}") %>'  />
                <insite:IconLink runat="server" Name="file-invoice-dollar" ToolTip="Invoice"
                    NavigateUrl='<%# Eval("InvoiceIdentifier", "/ui/admin/sales/invoices/outline?id={0}") %>' />
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
