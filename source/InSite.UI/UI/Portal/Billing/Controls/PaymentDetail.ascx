<%@ Control Language="C#" CodeBehind="PaymentDetail.ascx.cs" Inherits="InSite.UI.Portal.Billing.Controls.PaymentDetail" %>

<%@ Register Src="CardDetail.ascx" TagName="CardDetail" TagPrefix="uc" %>

<div class="row">
    <div class="col-lg-12">
        <div class="settings">

            <div runat="server" id="TaxPanel" class="form-group mb-3" visible="false">
                <div class="mb-1">
                    <label class="form-label">
                        <insite:Literal runat="server" Text="Subtotal" />:
                    </label>
                    <asp:Literal runat="server" ID="SubtotalDisplay" />
                </div>
                <div>
                    <label class="form-label">
                        <insite:Literal runat="server" Text="Tax" /><asp:Literal runat="server" ID="TaxRateDisplay" />:
                    </label>
                    <asp:Literal runat="server" ID="TaxAmountDisplay" />
                </div>
            </div>

            <div runat="server" id="PaymentAmountPanel" class="form-group mb-3">
                <label class="form-label">
                    <insite:Literal runat="server" Text="Payment Amount" />:
                </label>
                <asp:Literal runat="server" ID="PaymentAmountDisplay" />
                <asp:HiddenField runat="server" ID="PaymentAmountValue" />
            </div>

            <uc:CardDetail runat="server" ID="CardDetail" />

        </div>
    </div>
</div>
