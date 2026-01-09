<%@ Control Language="C#" CodeBehind="PaymentDetail.ascx.cs" Inherits="InSite.UI.Portal.Billing.Controls.PaymentDetail" %>

<%@ Register Src="CardDetail.ascx" TagName="CardDetail" TagPrefix="uc" %>

<div class="row">
    <div class="col-lg-12">
        <div class="settings">

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