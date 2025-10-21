<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CartDetail.ascx.cs" Inherits="InSite.UI.Portal.Billing.Controls.CartDetail" %>

<insite:PageHeadContent runat="server" ID="CommonStyle">
  <style type="text/css">
      .cart-card {
          background: #fff;
          border: 1px solid rgba(0,0,0,.08);
          border-radius: .75rem;
          box-shadow: 0 .125rem .5rem rgba(0,0,0,.06);
      }
      .line-row {
          border-bottom: 1px solid rgba(0,0,0,.08);
      }
      .line-row:last-child {
          border-bottom: 0;
      }

      .qty-group {
          --q-size: 34px;
      }
      .qty-group .btn {
          width: var(--q-size);
          padding: 0;
          display: inline-flex;
          align-items: center;
          justify-content: center;
          line-height: 1;
          border-color: rgba(var(--bs-secondary-rgb,108,117,125),.35);
          max-height: 35px;
      }
          .qty-group .qty-input {
              max-width: 40px;
              padding: 0 .375rem;
              text-align: center;
              border-left-width: 0;
              border-right-width: 0;
              border-color: rgba(var(--bs-secondary-rgb,108,117,125),.35);
          }
          .qty-group .btn, .qty-group .qty-input {
              border-radius: 0;
          }
              .qty-group .btn:first-child {
                  border-top-left-radius: .375rem;
                  border-bottom-left-radius: .375rem;
              }
              .qty-group .btn:last-child {
                  border-top-right-radius: .375rem;
                  border-bottom-right-radius: .375rem;
              }
  </style>
</insite:PageHeadContent>

<h3 class="h3 mb-4 text-muted"><asp:Literal ID="CartHeader" runat="server"></asp:Literal></h3>

<asp:PlaceHolder ID="EmptyState" runat="server" Visible="false">
    <div class="alert alert-info mb-4">Your cart is empty.</div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="ModeNotSupported" runat="server" Visible="false">
    <div class="alert alert-secondary mb-4">
        This cart view currently shows only A-la-Carte items. Package / Subscribe layout coming next.
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="CartBlock" runat="server" Visible="false">
    <asp:Repeater ID="CartRepeater" runat="server">
        <HeaderTemplate><div></HeaderTemplate>
        <FooterTemplate></div></FooterTemplate>
        <ItemTemplate>
            <div class="d-flex align-items-center justify-content-between py-3 line-row">

                <div class="me-3 flex-grow-1">
                <a href='<%# Eval("Url") %>' class="fw-semibold text-decoration-underline"><%# Eval("Name") %></a>
                </div>

                <div runat="server" id="QtyWrap" class="input-group input-group-sm qty-group w-auto me-3">
                    <asp:LinkButton runat="server"
                                    CssClass="btn btn-outline-secondary btn-sm"
                                    CommandName="step"
                                    CommandArgument='<%# Eval("ProductId") + "|-1" %>'>−</asp:LinkButton>

                    <asp:TextBox runat="server" ID="QtyInput"
                                CssClass="form-control qty-input btn-sm"
                                Text='<%# Eval("Qty") %>'
                                AutoPostBack="true"
                                OnTextChanged="QtyInput_TextChanged" />
                    <asp:HiddenField runat="server" ID="ProductId" Value='<%# Eval("ProductId") %>' />

                    <asp:LinkButton runat="server"
                                    CssClass="btn btn-outline-secondary btn-sm"
                                    CommandName="step"
                                    CommandArgument='<%# Eval("ProductId") + "|+1" %>'>+</asp:LinkButton>
                </div>

                <div runat="server" id="PriceWrap" class="text-end" style="min-width:100px;">
                    <asp:Literal runat="server" ID="LinePrice" Text='<%# Eval("PriceFormatted") %>' />
                </div>

            </div>
        </ItemTemplate>
    </asp:Repeater>
</asp:PlaceHolder>

<div class="pt-5 border-top">
    <div class="d-flex justify-content-between mb-1">
        <span>Subtotal</span>
        <span><asp:Literal runat="server" ID="SubtotalLit" /></span>
    </div>
    <div class="d-flex justify-content-between mb-1">
        <span>Tax <span class="fst-italic text-muted">(Calculated at Checkout)</span></span>
        <span><asp:Literal runat="server" ID="TaxLit" />$ -</span>
    </div>
    <div class="d-flex justify-content-between fw-semibold">
        <span>Total</span>
        <span><asp:Literal runat="server" ID="TotalLit" /></span>
    </div>
</div>