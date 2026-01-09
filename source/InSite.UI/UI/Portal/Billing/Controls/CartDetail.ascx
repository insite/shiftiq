<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CartDetail.ascx.cs" Inherits="InSite.UI.Portal.Billing.Controls.CartDetail" %>

<h3 ID="CartHeader" runat="server" class="h3 mb-4 text-muted" visible="false"></h3>

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
                <a href='<%# Eval("Url") %>' class="fw-semibold text-decoration-underline text-primary"
                    title='<%# Eval("Summary") %>' data-bs-toggle="tooltip" data-bs-placement="top">
                    <%# Eval("Name") %>
                </a>
                </div>

                <div runat="server" id="QtyWrap" class="input-group input-group-sm qty-group w-auto me-3 text-nowrap" style="min-width: 120px;">

                    <asp:LinkButton runat="server"
                                    CssClass="btn btn-outline-secondary btn-sm"
                                    CommandName="step"
                                    CommandArgument='<%# Eval("ProductId") + "|-1" %>'>−</asp:LinkButton>

                    <asp:TextBox runat="server" ID="QtyInput"
                                CssClass="form-control qty-input btn-sm"
                                Text='<%# Eval("Qty") %>'
                                AutoPostBack="true"
                                OnTextChanged="QtyInput_TextChanged" Width="36px" />

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