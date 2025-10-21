<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecentList.ascx.cs" Inherits="InSite.Admin.Invoices.Reports.Controls.RecentList" %>

<asp:Repeater runat="server" ID="PaymentRepeater">
    <HeaderTemplate>
		<dl>
	</HeaderTemplate>
    <ItemTemplate>
		<dt>
			<a href='/ui/admin/sales/invoices/outline?id=<%# Eval("InvoiceIdentifier") %>'><i class="far fa-credit-card-front me-1"></i>Invoice #<%# Eval("InvoiceNumber") %> for <%# Eval("InvoiceAmount") %></a>
        </dt>
	    <dd class="ms-1 ps-3 pb-3">
            <%# Eval("InvoiceCustomerName") %> <%# Eval("PaymentStatus") %> <%# GetTimestampHtml(Eval("PaymentStarted")) %>
		</dd>
    </ItemTemplate>
	<FooterTemplate>
		</dl>
	</FooterTemplate>
</asp:Repeater>
    
      
