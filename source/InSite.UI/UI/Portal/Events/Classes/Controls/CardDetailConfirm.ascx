<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CardDetailConfirm.ascx.cs" Inherits="InSite.UI.Portal.Events.Classes.Controls.CardDetailConfirm" %>


<ul class="list-unstyled mb-0">
    <li class="d-flex py-1" style="text-align:left;">
        <i class="far fa-id-card fs-lg align-self-center text-primary"></i>
        <div class="ps-3">
            <span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Cardholder Name" /></span>
            <asp:Literal runat="server" ID="CardholderName" />
        </div>
    </li>
    
    <li class="d-flex py-1">
        <i class="far fa-money-check-alt fs-lg align-self-center text-primary"></i>
        <div class="ps-3">
            <span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Card Number" /></span>
            <asp:Literal runat="server" ID="CardNumber" />
        </div>
    </li>

    <li class="d-flex py-1">
        <i class="far fa-clock fs-lg align-self-center text-primary"></i>
        <div class="ps-3">
            <span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Expiry Date" /></span>
            <asp:Literal runat="server" ID="Expiry" />
        </div>
    </li>
</ul>