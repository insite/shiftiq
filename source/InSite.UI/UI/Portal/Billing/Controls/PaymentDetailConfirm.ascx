<%@ Control Language="C#" CodeBehind="PaymentDetailConfirm.ascx.cs" Inherits="InSite.UI.Portal.Billing.Controls.PaymentDetailConfirm" %>

<%@ Register Src="CardDetailConfirm.ascx" TagName="CardDetailConfirm" TagPrefix="uc" %>

<div class="row">
    <div class="col-md-12">
        <div class="settings">

            <h3><insite:Literal runat="server" Text="Credit Card" /></h3>

            <uc:CardDetailConfirm runat="server" ID="CardDetailConfirm" />

            <div runat="server" id="TaxPanel" class="form-group mb-3" visible="false">
                <div class="mb-1">
                    <label class="form-label">
                        <insite:Literal runat="server" Text="Subtotal ($)" />
                    </label>
                    <div>
                        <asp:Literal runat="server" ID="SubtotalAmount" />
                    </div>
                </div>
                <div>
                    <label class="form-label">
                        <insite:Literal runat="server" Text="Tax" /><asp:Literal runat="server" ID="TaxRateDisplay" /> ($)
                    </label>
                    <div>
                        <asp:Literal runat="server" ID="TaxAmount" />
                    </div>
                </div>
            </div>

            <div class="form-group mb-3">
                <label class="form-label">
                    <insite:Literal runat="server" Text="Payment Amount ($)" />
                </label>
                <div>
                    <asp:Literal runat="server" ID="PaymentAmount" />
                </div>
            </div>

        </div>
    </div>
</div>
