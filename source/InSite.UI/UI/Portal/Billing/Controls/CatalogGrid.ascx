<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CatalogGrid.ascx.cs" Inherits="InSite.UI.Portal.Billing.Controls.CatalogGrid" %>

<asp:Repeater runat="server" ID="ProductRepeater">
    <HeaderTemplate><div class="catalog-grid"></HeaderTemplate>
    <FooterTemplate></div></FooterTemplate>
    <ItemTemplate>
        <div class="product-card">

            <div
                class="product-media"
                aria-label="View details for <%# Eval("ProductName") %>"
                title='<%# Eval("ProductSummary") %>' data-bs-toggle="tooltip" data-bs-placement="top"
            >
                <img src="<%# GetProductImage(Eval("ProductImageUrl")) %>"
                    alt="Image for <%# Eval("ProductName") %>"
                    class="product-media" />
            </div>

            <div class="p-3">
                <h3 class="h6 mb-3">

                    <asp:PlaceHolder ID="DefaultTitle" runat="server" Visible="true">
                    <span class="text-primary fw-semibold">
                        <%# Eval("ProductName") %>
                    </span>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="SubscribeTitle" runat="server" Visible="false">
                    <div class="d-flex flex-column text-center">
                        <span class="text-success fw-semibold">Subscribe &amp; Choose Later</span>
                        <span class="fw-semibold text-secondary"><asp:Literal ID="SubName" runat="server" /></span>
                        <span class="text-muted"><asp:Literal ID="SubPack" runat="server" /></span>
                    </div>
                    </asp:PlaceHolder>
                </h3>

                <div class="actions-row justify-content-center">

                    <asp:PlaceHolder ID="QtyContainer" runat="server" Visible="true">
                        <div class="input-group input-group-sm qty-group w-auto">
                            <button class="btn btn-outline-secondary qty-btn btn-sm" type="button" data-step="-1">−</button>

                            <asp:TextBox runat="server" ID="QtyInput" CssClass="form-control qty-input btn-sm"
                                        Text="1" MaxLength="4" />

                            <button class="btn btn-outline-secondary qty-btn btn-sm" type="button" data-step="1">+</button>
                        </div>
                    </asp:PlaceHolder>

                    <insite:Button runat="server"
                        ID="AddButton"
                        CommandName="Add"
                        CommandArgument='<%# Eval("ProductIdentifier") %>'
                        CssClass="btn btn-success rounded-pill px-3 fw-semibold add-to-cart" />
                </div>
            </div>
        </div>
    </ItemTemplate>
</asp:Repeater>

<insite:PageFooterContent runat="server" ID="CommonScript">
    <script type="text/javascript">

        document.addEventListener('click', function (e) {
            const btn = e.target.closest('.qty-btn');
            if (!btn) return;
            const group = btn.closest('.qty-group');
            const input = group.querySelector('.qty-input');
            const step = parseInt(btn.dataset.step, 10) || 1;
            const current = parseInt((input.value || '0').replace(/\D/g, ''), 10) || 0;
            const next = Math.max(0, current + step);
            input.value = String(next);
        }, { passive: true });

        document.addEventListener('input', function (e) {
            const input = e.target.closest('.qty-input');
            if (!input) return;
            input.value = input.value.replace(/[^0-9]/g, '');
        });
    </script>
</insite:PageFooterContent>