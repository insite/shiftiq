<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CatalogGrid.ascx.cs" Inherits="InSite.UI.Portal.Billing.Controls.CatalogGrid" %>

<insite:PageHeadContent runat="server" ID="CommonStyle">
    <style type="text/css">
        .product-card {
            background: #fff;
            border: 1px solid rgba(0,0,0,.08) !important;
            border-radius: .75rem;
            box-shadow: 0 .125rem .5rem rgba(0,0,0,.06);
            overflow: hidden;
        }

            .product-card:hover {
                box-shadow: 0 .5rem 1.25rem rgba(0,0,0,.08);
            }

        .product-media {
            height: 200px;
            object-fit: cover;
            display: block;
            width: 100%;
        }

        .product-link {
            color: var(--bs-primary);
            text-decoration: underline;
            font-weight: 600;
        }

            .product-link:hover {
                text-decoration: none;
            }

        .catalog-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(260px, 1fr));
            gap: 1.5rem;
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
                border-color: rgba(var(--bs-secondary-rgb,108,117,125), .35);
                max-height: 35px;
            }

            .qty-group .qty-input {
                max-width: 40px;
                padding: 0 .375rem;
                text-align: center;
                border-left-width: 0;
                border-right-width: 0;
                border-color: rgba(var(--bs-secondary-rgb,108,117,125), .35);
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

        .add-to-cart {
            min-width: 130px;
        }

        .actions-row {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: .5rem;
            flex-wrap: nowrap;
        }

            .actions-row .qty-group,
            .actions-row .add-to-cart {
                flex: 0 0 auto;
            }
    </style>
</insite:PageHeadContent>

<asp:Repeater runat="server" ID="ProductRepeater">
    <HeaderTemplate><div class="catalog-grid"></HeaderTemplate>
    <FooterTemplate></div></FooterTemplate>
    <ItemTemplate>
        <div class="product-card">

            <a href='<%# GetProductUrl(Eval("ProductIdentifier")) %>'
                class="d-block"
                aria-label="View details for <%# Eval("ProductName") %>">
                <img src="<%# GetProductImage(Eval("ProductImageUrl")) %>"
                    alt="Image for <%# Eval("ProductName") %>"
                    class="product-media" />
            </a>

            <div class="p-3">
                <h3 class="h6 mb-3">

                    <asp:PlaceHolder ID="DefaultTitle" runat="server" Visible="true">
                    <a class="product-link" href='<%# GetProductUrl(Eval("ProductIdentifier")) %>'>
                        <%# Eval("ProductName") %>
                    </a>
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
                                        Text="0" MaxLength="4" />

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