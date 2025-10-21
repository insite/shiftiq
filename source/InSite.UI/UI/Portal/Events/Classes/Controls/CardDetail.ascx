<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CardDetail.ascx.cs" Inherits="InSite.UI.Portal.Events.Classes.Controls.CardDetail" %>

<div class="form-group mb-3">
    <label class="form-label">
        <insite:Literal runat="server" Text="Cardholder Name" />
        <insite:RequiredValidator runat="server" ControlToValidate="CardholderName" Display="None" ValidationGroup="Payment" />
    </label>
    <insite:TextBox runat="server" ID="CardholderName" MaxLength="32"
        ClientEvents-OnKeyDown="return cardDetail_onKeyDown(event);" />
</div>

<div class="form-group mb-3">
    <label class="form-label">
        <insite:Literal runat="server" Text="Card Number" />
        <insite:RequiredValidator runat="server" ControlToValidate="CardNumber" Display="None" ValidationGroup="Payment" />
    </label>
    <insite:TextBox runat="server" ID="CardNumber" MaxLength="19" EmptyMessage="0000 0000 0000 0000"
        data-format="{&quot;creditCard&quot;:true}"
        ClientEvents-OnKeyDown="return cardDetail_onKeyDown(event);" />
</div>

<div class="row">
    <div class="col-md-7">
        <div class="form-group mb-3">
            <label class="form-label">
                <insite:Literal runat="server" Text="Expiry Date" />
                <insite:RequiredValidator runat="server" ControlToValidate="Expiry" Display="None" ValidationGroup="Payment" />
            </label>
            <insite:TextBox runat="server" ID="Expiry" MaxLength="5" EmptyMessage="mm/yy"
                data-format="{&quot;date&quot;:true,&quot;datePattern&quot;:[&quot;m&quot;,&quot;y&quot;]}" 
                ClientEvents-OnKeyDown="return cardDetail_onKeyDown(event);" />
        </div>
    </div>

    <div class="col-md-5">
        <div class="form-group mb-3">
            <label class="form-label">
                <insite:Literal runat="server" Text="CVC" />
                <insite:RequiredValidator runat="server" ControlToValidate="SecurityCode" Display="None" ValidationGroup="Payment" />
            </label>
            <insite:TextBox runat="server" ID="SecurityCode" MaxLength="4" EmptyMessage="000" TextMode="Password" ClientEvents-OnKeyDown="return cardDetail_onKeyDown(event);" />
        </div>
    </div>
</div>

<script>
    function cardDetail_onKeyDown(event) {
        if (event.keyCode != 13)
            return true;

        onPaymentNextClicked();

        return false;
    }
</script>