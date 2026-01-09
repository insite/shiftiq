<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InvoiceEventReport.ascx.cs" Inherits="InSite.Admin.Invoices.Controls.InvoiceEventReport" %>

<!DOCTYPE html>

<html>
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <title runat="server" id="PageTitle"></title>
    <style type="text/css">
        body {
            margin: 0;
            font-family: Arial;
        }

        .form-group label { 
                font-weight:bold;
            }
        tr th, tr td
            {
                text-align: left;
                vertical-align:top;
            }
        .contact-and-support p {
            padding:0;
            margin:0;
        }
    </style>
</head>
<body>

    <div>
        <div>
            <table style="width:100%;padding:0;">
                <tr>
                    <td style="padding-bottom:10px;">
                        <b>Issued By</b>
                        <br />
                        <asp:Literal runat="server" ID="IssuedBy" />
                    </td>
                    <td style="padding-bottom:10px;">
                        <b>Issued To</b><br />
                        <asp:Literal runat="server" ID="IssuedToPerson" /><br />
                        <asp:Literal runat="server" ID="IssuedToPersonEmployer" />
                    </td>
                </tr>
                <tr>
                    <td style="padding-bottom:10px;">
                        <b>Purchase Date: <asp:Literal runat="server" ID="PurchaseDate" /></b>
                    </td>
                    <td style="padding-bottom:10px;">
                        <b>Order Number: </b> <asp:Literal runat="server" ID="OrderNumber" />
                    </td>
                </tr>
            </table>

            <asp:Repeater runat="server" ID="PurchaseRepeater">
                <HeaderTemplate>
                    <table style="width:100%;padding:0 0 10px 0;">
                        <tr>
                            <th colspan="4" style="background-color:#E5E5E5;">Purchases</th>
                        </tr>
                        <tr>
                            <th></th>
                            <th style="text-align:center;">Quantity</th>
                            <th style="text-align:right;">Price</th>
                            <th style="text-align:right;">Amount</th>
                        </tr>
                </HeaderTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%# Eval("Description") %></td>
                        <td style="text-align:center;"><%# Eval("Quantity") %></td>
                        <td style="text-align:right;"><%# Eval("Price") %></td>
                        <td style="text-align:right;"><%# Eval("Amount") %></td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>

            <div runat="server" id="TaxBlock" style="width:100%; text-align:right; padding-bottom:10px;" visible="false">
              <b>Tax:</b> <asp:Literal runat="server" ID="TaxAmount" />
            </div>

            <div style="width:100%; text-align:right; padding-bottom: 10px;">
                <b>Total:</b> <asp:Literal runat="server" ID="TotalPayment" />
            </div>

            <table runat="server" ID="EventDetails" style="width:100%;padding:0 0 10px 0;">
                <tr>
                    <th style="background-color:#E5E5E5;">Event Details</th>
                </tr>
                <tr>
                    <td style="padding-top:10px;">
                        <asp:Literal runat="server" ID="EventTitle" /><br />
                        <asp:Literal runat="server" ID="EventDate" />

                        <div runat="server" id="VenuePanel" style="padding-top:10px;" visible='<%# Eval("VenueName") != null %>'>
                            <b><asp:Literal runat="server" ID="VenueName" /></b><br />
                            <asp:Literal runat="server" ID="VenueAddress" />
                        </div>
                    </td>
                </tr>
                <tr runat="server" id="ContactAndSupportRow">
                    <td style="padding-top:10px;">
                        <b>Contact &amp; Support</b>
                        <asp:Literal runat="server" ID="ContactAndSupport" />
                    </td>
                </tr>
            </table>

            <asp:Repeater runat="server" ID="PaymentRepeater">
                <HeaderTemplate>
                    <table style="width:100%;">
                        <tr>
                            <th colspan="4" style="background-color:#E5E5E5;">Payments</th>
                        </tr>
                        <tr>
                            <th>Date</th>
                            <th>Paid By</th>
                            <th>Notes</th>
                            <th style="text-align:right;">Amount</th>
                        </tr>
                </HeaderTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%# Eval("PaymentDate") %></td>
                        <td><%# Eval("PaidBy") %></td>
                        <td><%# Eval("Notes") %></td>
                        <td style="text-align:right;"><%# Eval("PaymentAmount") %></td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>

</body>
</html>
