<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SeatDetail.ascx.cs" Inherits="InSite.UI.Portal.Events.Classes.Controls.SeatDetail" %>

<div class="row">
    <div class="col-md-4">
        <div class="card border-0 shadow">
            <div class="card-body">
                <h5 class="card-title"><insite:Literal runat="server" Text="Select your registration below" /></h5>

                <div class="mb-3">
                    <asp:Repeater runat="server" ID="SeatRepeater">
                        <ItemTemplate>
                            <input type="radio"
                                value='<%# Eval("SeatIdentifier") %>'
                                id='<%# Eval("SeatIdentifier") %>'
                                name="Seats"
                                onclick="onSeatChanged();"
                                <%# Eval("SeatIdentifier").ToString() == SelectedSeat.Value ? "checked" : "" %>
                            />
                            <label for='<%# Eval("SeatIdentifier") %>'><b><%# Eval("SeatTitle") %></b></label>
                            <p><%# GetDescription(Container.DataItem) %></p>
                        </ItemTemplate>
                    </asp:Repeater>

                    <asp:HiddenField runat="server" ID="SelectedSeat" />
                    <asp:Button runat="server" ID="ChangeSeatButton" style="display:none;" />
                </div>
            </div>
        </div>

        <div runat="server" id="PriceArea" visible="false" class="card border-0 shadow mt-4">
            <div class="card-body">
                <h5 class="card-title"><insite:Literal runat="server" Text="Price options" /></h5>

                <div class="mb-3">
                    <b><insite:Literal ID="FreePrice" runat="server" Text="Free" /></b>
                    <b><asp:Literal runat="server" ID="SinglePrice" Visible="false" /></b>

                    <b><asp:RadioButtonList runat="server" ID="MultiplePrice" Visible="false" /></b>
                </div>
            </div>
        </div>

    </div>
    <div class="col-md-8" runat="server" id="Agreement" visible="false">

        <div class="card border-0 shadow">
            <div class="card-body">
                <h5 class="card-title"><insite:Literal runat="server" Text="Agreement" /></h5>

                <ul class="list-unstyled mb-0">
                        <li class="d-flex pt-2">
                        <i class="far fa-file-contract fs-lg mt-2 mb-0 text-primary border-top"></i>
                        <div class="ps-3">
                            <asp:Literal runat="server" ID="AgreementText" />
                            <div class="pt-2">
                                <insite:CheckBox runat="server" ID="Agreed" Text="I Agree" Visible="false" />
                                <insite:Button runat="server" ID="IAgreeButton" Text="I Agree" ButtonStyle="Success" Icon="fas fa-check" CausesValidation="true" />
                            </div>
                        </div>
                    </li>
                    <li runat="server" id="BillingCustomerField" class="d-flex pt-2">
                        <i class="far fa-money-check-edit-alt fs-lg mt-2 mb-0 text-primary border-top"></i>
                        <span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Billing To" />
                            <insite:RequiredValidator runat="server" ControlToValidate="BillingCustomerSelector" Display="None" />
                        </span>
                        <div class="ps-3">
                            <insite:ComboBox runat="server" ID="BillingCustomerSelector" Width="350px" />
                        </div>
                    </li>
                </ul>
            </div>
        </div>
                            
    </div>
</div>

<script>
    function onSeatChanged() {
        var value = $("input[name='Seats']:checked").val();
        $("#<%= SelectedSeat.ClientID %>").val(value);
        __doPostBack("<%= ChangeSeatButton.UniqueID %>", "");
    }
</script>