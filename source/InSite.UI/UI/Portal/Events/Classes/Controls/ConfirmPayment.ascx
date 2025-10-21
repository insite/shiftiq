<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConfirmPayment.ascx.cs" Inherits="InSite.UI.Portal.Events.Classes.Controls.ConfirmPayment" %>

<%@ Register Src="CardDetailConfirm.ascx" TagName="CardDetailConfirm" TagPrefix="uc" %>

<div class="row">
    <div class="col-md-12">

        <div class="card border-0 shadow">
            <div class="card-body">
                <h5 class="card-title" runat="server"><asp:Literal runat="server" ID="ConfirmClassName" /></h5>
                <ul class="list-unstyled mb-0">
                    <li class="d-flex pt-2">
                        <i class="far fa-calendar-alt fs-lg mt-2 mb-0 text-primary"></i>
                        <div class="ps-3">
                            <span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="When" /></span>
                            <asp:Literal runat="server" ID="ConfirmClassDates" />
                        </div>
                    </li>
                    <li class="d-flex pt-2 border-top">
                        <i class="far fa-location fs-lg mt-2 mb-0 text-primary"></i>
                        <div class="ps-3">
                            <span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Where" /></span>
                            <asp:Literal runat="server" ID="ConfirmClassVenue" />
                        </div>
                    </li>
                    <li id="Li1" runat="server"  class="d-flex pt-2 border-top">
                        <i class="far fa-map-marker-alt fs-lg mt-2 mb-0 text-primary"></i>
                        <div class="ps-3">
                            <span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Address" /></span>
                            <asp:Literal runat="server" ID="ConfirmClassVenueAddress" />
                        </div>
                    </li>
                </ul>
            </div>
        </div>

    </div>
</div>

<div runat="server" id="ConfirmClassRefundPanel" class="row">
    <div class="col-md-12">
        <div class="card border-0 shadow"  style="margin-top:15px;">
            <div class="card-body">
                <h5 class="card-title"><insite:Literal runat="server" Text="Cancellation &amp; Refund Policy" /></h5>

                <div class="mb-3">
                    <div><asp:Literal runat="server" ID="ConfirmClassRefundContent" /></div>
                </div>
            </div>
        </div>
    </div>
</div>

<div class="row mt-4">
    <div class="col-md-4">

        <div class="card border-0 shadow">
            <div class="card-body">
                <asp:Repeater runat="server" ID="ParticipantRepeater">
                    <ItemTemplate>
                        <div>
                            <%# Eval("FirstName") %> <%# Eval("LastName") %>
                        </div>
                        <div class="fs-ms text-body-secondary pb-2">
                            <%# Eval("Email") %>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

        <div class="card border-0 shadow mt-4">
            <div class="card-body">
                <h5 class="card-title" runat="server"><asp:Literal runat="server" ID="ConfirmEmployerName" /></h5>
                <ul class="list-unstyled mb-0">
                    <li id="ComfirmEmployerEmailDiv" runat="server" class="d-flex pt-2">
                        <i class="far fa-envelope fs-lg mt-2 mb-0 text-primary"></i>
                        <div class="ps-3">
                            <span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Email" /></span>
                            <asp:Literal ID="ConfirmEmployerEmail" runat="server" />
                        </div>
                    </li>
                    <li id="ConfirmEmployerAddressDiv" runat="server"  class="d-flex pt-2">
                        <i class="far fa-map-marker-alt fs-lg mt-2 mb-0 text-primary"></i>
                        <div class="ps-3">
                            <span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Mailing Address" /></span>
                            <asp:Literal ID="ConfirmEmployerAddress" runat="server" />
                        </div>
                    </li>
                    <li runat="server" id="ComfirmEmployerPhonesDiv" class="d-flex pt-2">
                        <i class="far fa-phone fs-lg mt-2 mb-0 text-primary"></i>
                        <div>
                            <div runat="server" id="ConfirmEmployerPhoneDiv" class="ps-3">
                                <span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Phone" /></span>
                                <asp:Literal ID="ConfirmEmployerPhone" runat="server" />
                            </div>
                        </div>
                        </li>
                </ul>
            </div>
        </div>

        <div runat="server" id="CreditCardPanel">

            <div class="card border-0 shadow"  style="margin-top:15px;">
                <div class="card-body" style="text-align:left;">
                    <h5 class="card-title"><insite:Literal runat="server" Text="Credit Card" /></h5>
    
                    <uc:CardDetailConfirm runat="server" ID="CardDetailConfirm" />
                </div>
            </div>
        </div>

    </div>

    <div class="col-md-8">

        <div class="card border-0 shadow">
            <div class="card-body">
                <h5 class="card-title" runat="server"><asp:Literal runat="server" ID="ConfirmSeatName" /></h5>
                <ul class="list-unstyled mb-0">
                    <li class="d-flex pt-2">
                        <i class="far fa-dollar-sign fs-lg mt-2 mb-0 text-primary"></i>
                        <div class="ps-3">
                            <span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Payment Amount ($)" /></span>
                            <asp:Literal runat="server" ID="PaymentAmountLiteral" />
                        </div>
                    </li>
                    <li class="d-flex pt-2">
                        <i class="far fa-file-contract fs-lg mt-2 mb-0 text-primary border-top"></i>
                        <div class="ps-3">
                            <asp:Literal runat="server" ID="ConfirmSeatAgreement" />
                        </div>
                    </li>
                    <li runat="server" id="BillingCustomerDiv" class="d-flex pt-2 border-top">
                        <i class="far fa-money-check-edit-alt fs-lg mt-2 mb-0 text-primary"></i>
                        <div class="ps-3">
                            <span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Tuition fee is being paid by" /></span>
                            <asp:Literal runat="server" ID="ConfirmSeatPaidBy" />
                        </div>
                    </li>
                </ul>
            </div>
        </div>

    </div>
                
</div>