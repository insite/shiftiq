<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PaymentDetail.ascx.cs" Inherits="InSite.UI.Portal.Events.Classes.Controls.PaymentDetail" %>

<%@ Register Src="CardDetail.ascx" TagName="CardDetail" TagPrefix="uc" %>

<div runat="server" id="PaymentAmountPanel" class="form-group mb-3">
    <label class="form-label">
        <insite:Literal runat="server" Text="Payment Amount" />
    </label>
    <div>
        <asp:Literal runat="server" ID="PaymentAmountLiteral" />
    </div>
</div>

<asp:MultiView runat="server" ID="MultiView">
    <asp:View runat="server" ID="CardView">
        <div class="row">
            <div class="col-md-3 settings">
                <h3>Credit Card</h3>
                <uc:CardDetail runat="server" ID="CardDetail" />
            </div>
        </div>
    </asp:View>

    <asp:View runat="server" ID="BillToView">
        <div class="row">
            <div class="col-md-6">
                <div class="form-group mb-3">
                    <label class="form-label">
                        <insite:Literal runat="server" ID="test" Text="Bill To" />
                        <insite:RequiredValidator runat="server" ID="BillingCodeValidator" FieldName="Bill To" ControlToValidate="BillingCode" Display="None" ValidationGroup="Payment" />
                    </label>
                    <div>
                        <insite:TextBox runat="server" ID="BillingCode" MaxLength="100" TextMode="MultiLine" Width="100%" Rows="5" />
                    </div>
                    <small class="form-text">
                        <insite:Literal runat="server" Text="Please indicate which entity should be invoiced for this registration." />
                    </small>
                </div>
            </div>
        </div>
    </asp:View>
</asp:MultiView>
