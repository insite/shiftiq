<%@ Page Language="C#" CodeBehind="Invoices.aspx.cs" Inherits="InSite.UI.Portal.Billing.Invoices" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="~/UI/Portal/Home/Management/Controls/DashboardNavigation.ascx" TagName="DashboardNavigation" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">
    <uc:DashboardNavigation runat="server" ID="DashboardNavigation" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    
    <insite:Alert runat="server" ID="StatusAlert" />

    <insite:Accordion runat="server" ID="MainAccordion">
        <insite:AccordionPanel runat="server" Title="Invoices" Icon="fas fa-file-invoice-dollar">

            <asp:Repeater runat="server" ID="InvoicesRepeater">
                <HeaderTemplate>
                    <div class="table-responsive">
                        <table class="table table-striped table-hover">
                            <thead>
                                <tr>
                                    <th style="white-space: nowrap;"><%# Translate("Invoice Date") %></th>
                                    <th><%# Translate("Status") %></th>
                                    <th style="width: 10px; white-space: nowrap;"><%# Translate("Amount") %></th>
                                    <th style="">
                                        <%# Translate("Items") %>
                                    </th>
                                    <th style="width: 40px;"></th>
                                </tr>
                            </thead>
                            <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <%# Eval("InvoiceDrafted") %>
                        </td>
                        <td>
                            <%# Translate(GetInvoiceStatus(Eval("InvoiceStatus"))) %>
                        </td>
                        <td align="right">
                            <%# Eval("InvoiceAmount", "{0:c2}") %>
                        </td>
                        <td>
                            <%# Eval("ItemsHtml") %>
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="InvoiceIdentifier" Visible="false" />

                            <insite:Button runat="server" ID="PayInvoiceButton" ButtonStyle="Primary" Text="Pay" Icon="far fa-file-invoice-dollar"
                                NavigateUrl='<%# Eval("InvoiceIdentifier", "/ui/portal/billing/invoice?invoice={0}") %>'
                                Visible='<%# Eval("InvoiceStatus").ToString() == "Submitted" || Eval("InvoiceStatus").ToString() == "PaymentFailed" %>' />

                            <insite:Button runat="server" ID="PrintInvoiceButton" ButtonStyle="Primary" Icon="far fa-print" Text="Print" />
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </tbody>
                        </table>
                    </div>
                </FooterTemplate>
            </asp:Repeater>

        </insite:AccordionPanel>

        <insite:AccordionPanel runat="server" Title="Receipts" Icon="fas fa-file-invoice-dollar">

            <asp:Repeater runat="server" ID="ReceiptsRepeater">
                <HeaderTemplate>
                    <div class="table-responsive">
                        <table class="table table-striped table-hover">
                            <thead>
                                <tr>
                                    <th style="white-space: nowrap;"><%# Translate("Receipt Date") %></th>
                                    <th><%# Translate("Status") %></th>
                                    <th style="white-space: nowrap;"><%# Translate("Date Paid") %></th>
                                    <th style="width: 10px; white-space: nowrap;"><%# Translate("Amount") %></th>
                                    <th style="">
                                        <%# Translate("Items") %>
                                    </th>
                                    <th style="width: 40px;"></th>
                                </tr>
                            </thead>
                            <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <%# Eval("InvoiceDrafted") %>
                        </td>
                        <td>
                            <%# Translate(GetInvoiceStatus(Eval("InvoiceStatus"))) %>
                        </td>
                        <td>
                            <%# Eval("InvoicePaid") %>
                        </td>
                        <td align="right">
                            <%# Eval("InvoiceAmount", "{0:c2}") %>
                        </td>
                        <td>
                            <%# Eval("ItemsHtml") %>
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="InvoiceIdentifier" Visible="false" />

                            <insite:Button runat="server" ID="PayInvoiceButton" ButtonStyle="Primary" Text="Pay" Icon="far fa-file-invoice-dollar"
                                NavigateUrl='<%# Eval("InvoiceIdentifier", "/ui/portal/billing/invoice?invoice={0}") %>'
                                Visible='<%# Eval("InvoiceStatus").ToString() == "Submitted" %>' />

                            <insite:Button runat="server" ID="PrintInvoiceButton" ButtonStyle="Primary" Icon="far fa-print" Text="Print" />
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </tbody>
                 </table>
             </div>
                </FooterTemplate>
            </asp:Repeater>

        </insite:AccordionPanel>
    </insite:Accordion>

</asp:Content>
