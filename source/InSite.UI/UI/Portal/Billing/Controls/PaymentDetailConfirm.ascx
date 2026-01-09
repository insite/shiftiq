<%@ Control Language="C#" CodeBehind="PaymentDetailConfirm.ascx.cs" Inherits="InSite.UI.Portal.Billing.Controls.PaymentDetailConfirm" %>

<%@ Register Src="CardDetailConfirm.ascx" TagName="CardDetailConfirm" TagPrefix="uc" %>

<div class="row">
    <div class="col-md-12">
        <div class="settings">

            <h3><insite:Literal runat="server" Text="Credit Card" /></h3>

            <uc:CardDetailConfirm runat="server" ID="CardDetailConfirm" />

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
