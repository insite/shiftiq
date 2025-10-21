<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Invoices.Controls.SearchResults" %>

<insite:Grid runat="server" ID="Grid" DataKeyNames="InvoiceIdentifier">
    <Columns>

        <asp:TemplateField HeaderText="Customer Employer">
            <ItemTemplate>
                <%# Eval("CustomerEmployer") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Person Code">
            <ItemTemplate>
                <a href='/ui/admin/contacts/people/edit?<%# Eval("CustomerIdentifier", "contact={0}") %>'>
                    <%# Eval("CustomerPersonCode") %>
                </a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Employer Contact Name">
            <ItemTemplate>
                <a href='/ui/admin/contacts/people/edit?<%# Eval("CustomerIdentifier", "contact={0}") %>'>
                    <%# Eval("CustomerFullName") %>
                </a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Customer Email">
            <ItemTemplate>
                <a href='mailto:<%# Eval("CustomerEmail") %>'>
                    <%# Eval("CustomerEmail") %>
                </a>
            </ItemTemplate>
        </asp:TemplateField>


        <asp:TemplateField HeaderText="Invoice Number">
            <ItemTemplate>
                <a href='/ui/admin/sales/invoices/outline?<%# Eval("InvoiceIdentifier", "id={0}") %>'>
                    <%# Eval("InvoiceNumber") %>
                </a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Invoice Status">
            <ItemTemplate>
                <%# GetInvoiceStatus(Eval("InvoiceStatus")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Products">
            <ItemTemplate>
                <asp:Repeater runat="server" ID="ProductRepeater">
                    <ItemTemplate>
                        <%# Container.ItemIndex > 0 ? ", " : "" %>
                        <%# Eval("ProductName") %>
                    </ItemTemplate>
                </asp:Repeater>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Invoice Drafted">
            <ItemTemplate>
                <%# GetLocalTime(Eval("InvoiceDrafted")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Invoice Submitted">
            <ItemTemplate>
                <%# GetLocalTime(Eval("InvoiceSubmitted")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Invoice Paid">
            <ItemTemplate>
                <%# GetLocalTime(Eval("InvoicePaid")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Transaction ID">
            <ItemTemplate>
                <%# GetTransactionCode(Eval("Payments")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField ItemStyle-Width="50px" ItemStyle-Wrap="false">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="search" ToolTip="Outline"
                    NavigateUrl='<%# Eval("InvoiceIdentifier", "/ui/admin/sales/invoices/outline?id={0}") %>' />
                <insite:IconLink runat="server" Name="pencil" ToolTip="Edit Invoice Details"
                    NavigateUrl='<%# Eval("InvoiceIdentifier", "/ui/admin/sales/invoices/change-invoice-details?invoice={0}") %>' />
                <insite:IconLink runat="server" ID="DeleteButton" Name="trash-alt" ToolTip="Delete"
                    NavigateUrl='<%# Eval("InvoiceIdentifier", "/admin/sales/invoices/delete?id={0}") %>' />
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
