<%@ Page Language="C#" CodeBehind="Invoice.aspx.cs" Inherits="InSite.UI.Portal.Billing.Invoice" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="~/UI/Portal/Home/Management/Controls/DashboardNavigation.ascx" TagName="DashboardNavigation" TagPrefix="uc" %>
<%@ Register Src="Controls/PaymentDetail.ascx" TagName="PaymentDetail" TagPrefix="uc" %>
<%@ Register Src="Controls/PaymentDetailConfirm.ascx" TagName="PaymentDetailConfirm" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">
    <uc:DashboardNavigation runat="server" ID="DashboardNavigation" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="PaymentStatus" />

    <insite:Accordion runat="server" ID="MainAccordion">
        <insite:AccordionPanel runat="server" ID="InvoiceSection" Title="1. Invoice" Icon="fas fa-file-invoice-dollar">

            <asp:Repeater runat="server" ID="InvoiceItemRepeater">
                <HeaderTemplate>
                    <div class="table-responsive">
                        <table class="table table-striped table-hover">
                            <thead>
                                <tr>
                                    <th><%# Translate("Item") %></th>
                                    <th style="width: 100px; text-align: right;"><%# Translate("Price") %></th>
                                    <th style="width: 60px; text-align: right;"><%# Translate("Quantity") %></th>
                                </tr>
                            </thead>
                            <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <%# Eval("ItemDescription") %>
                        </td>
                        <td align="right">
                            <%# Eval("ItemPrice", "{0:c2}") %>
                        </td>
                        <td align="right">
                            <%# Eval("ItemQuantity", "{0:n0}") %>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                            </tbody>
                        </table>
                    </div>
                </FooterTemplate>
            </asp:Repeater>

            <div class="float-end">
                <asp:Label runat="server" ID="InvoiceStatus" />
            </div>

            <div style="margin-top: 15px;">
                <insite:NextButton runat="server" ID="SubmitPaymentButton" />
            </div>

        </insite:AccordionPanel>

        <insite:AccordionPanel runat="server" ID="PaymentSection" Title="2. Payment" Icon="fas fa-credit-card-front" Visible="false">

            <insite:Alert runat="server" ID="PaymentAlert" />

            <uc:PaymentDetail runat="server" ID="PaymentDetail" />

            <insite:NextButton runat="server" ID="NextButton" />

        </insite:AccordionPanel>

        <insite:AccordionPanel runat="server" ID="ConfirmPaymentSection" Title="3. Confirm Payment" Icon="fas fa-file-check" Visible="false">

            <uc:PaymentDetailConfirm runat="server" ID="PaymentDetailConfirm" />

            <insite:Button runat="server" ID="ConfirmPaymentButton" Icon="fas fa-cloud-upload" Text="Confirm Payment" ButtonStyle="Success" />

        </insite:AccordionPanel>

    </insite:Accordion>

</asp:Content>
