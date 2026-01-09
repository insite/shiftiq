<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Detail.ascx.cs" Inherits="InSite.Admin.Events.Seats.Controls.Detail" %>

<div class="card border-0 shadow-lg">
    <div class="card-body">

        <h4 class="card-title mb-3">
            <i class="far fa-money-check-alt me-1"></i>
            Seat
        </h4>

        <div class="row">
            <div class="col-lg-6">

                <div class="form-group mb-3">
                    <label class="form-label">
                        Seat Name
                        <insite:RequiredValidator runat="server" FieldName="Seat Name" ControlToValidate="SeatName" ValidationGroup="Seat" />
                    </label>
                    <insite:TextBox runat="server" ID="SeatName" MaxLength="100" />
                    <div class="form-text">
                        The descriptive title for the seat.
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Seat Description
                    </label>
                    <insite:TextBox runat="server" ID="SeatDescription"  TextMode="MultiLine" Rows="5" />
                    <div class="form-text">
                        Short description for this seat.
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Seat Availability
                    </label>
                    <div>
                        <asp:RadioButtonList runat="server" ID="SeatAvailability">
                            <asp:ListItem Value="Available" Text="Available for purchase" Selected="true" />
                            <asp:ListItem Value="Hide" Text="Hide this ticket to prevent purchase" />
                        </asp:RadioButtonList> 
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Seat Order Sequence
                    </label>
                    <insite:NumericBox runat="server" ID="SeatOrderSequence" NumericMode="Integer" DigitGrouping="false" Width="110px" MinValue="0" MaxValue="999" /> 
                    <div class="form-text"></div>
                </div>

                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="PriceUpdatePanel" />
                <insite:UpdatePanel runat="server" ID="PriceUpdatePanel" CssClass="form-group mb-3">
                    <ContentTemplate>
                        <label class="form-label">
                            Seat Price
                            <insite:RequiredValidator runat="server" ID="SinglePriceRequiredValidator" FieldName="Price" ControlToValidate="SinglePrice" ValidationGroup="Seat" />
                        </label>
                        <div>
                            <asp:RadioButtonList runat="server" ID="SeatPrice">
                                <asp:ListItem Value="Free" Text="Free" Selected="true" />
                                <asp:ListItem Value="Single" Text="Single price" />
                                <asp:ListItem Value="Multiple" Text="Multiple price levels (Example: members pay less than non-members)" />
                            </asp:RadioButtonList> 
                            <div runat="server" id="SinglePriceArea" class="mt-2">
                                <insite:NumericBox runat="server" ID="SinglePrice" DecimalPlaces="2" DigitGrouping="false" Width="110px" MinValue="0" MaxValue="99999" EmptyMessage="Price" CssClass="d-inline-block" /> $ 
                            </div>
                            <div runat="server" id="MultiplePriceArea" class="mt-2">
                                <div>
                                    <div class="d-inline-block" style="width:calc(100% - 190px)">
                                        <insite:TextBox runat="server" ID="PriceName" MaxLength="50" EmptyMessage="Price Name" CssClass="d-inline-block" />
                                    </div>
                                    <insite:RequiredValidator runat="server" FieldName="Price Name" ControlToValidate="PriceName" ValidationGroup="Price" RenderMode="Dot" />
                                    <insite:NumericBox runat="server" ID="PriceAmount" DecimalPlaces="2" DigitGrouping="false" Width="140px" MinValue="0" MaxValue="99999" EmptyMessage="Price Amount" CssClass="d-inline-block" /> $
                                    <insite:RequiredValidator runat="server" FieldName="Price Amount" ControlToValidate="PriceAmount" ValidationGroup="Price" RenderMode="Dot" />
                                </div>

                                <div class="mt-2">
                                    <insite:ItemNameComboBox runat="server" ID="GroupStatus" MaxHeight="300" EmptyMessage="Group Status" />
                                </div>

                                <div class="text-end mt-2">
                                    <insite:Button runat="server" ID="AddPriceButton" ButtonStyle="OutlineSecondary" ValidationGroup="Price" Text="Add Price" Icon="fas fa-plus-circle" />
                                    <asp:CustomValidator runat="server" ID="MultiplePriceValidator" ErrorMessage="Price is a required field" Display="None" ValidationGroup="Seat" />
                                </div>

                                <asp:Repeater runat="server" ID="PriceList">
                                    <HeaderTemplate><table class="table table-striped mt-2"></HeaderTemplate>
                                    <FooterTemplate></table></FooterTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td><%# Eval("Name") %></td>
                                            <td class="text-end"><%# Eval("Amount", "{0:c2}") %></td>
                                            <td class="text-center"><%# Eval("GroupStatus") %></td>
                                            <td class="text-end" style="width:40px;">
                                                <insite:IconButton runat="server" CommandName="Delete" CssClass="p-2" ToolTip="Delete Price" Name="trash-alt" />
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>
                    </ContentTemplate>
                </insite:UpdatePanel>

            </div>
            <div class="col-lg-6">

                <div class="form-group mb-3">
                    <label class="form-label">
                        Taxes
                    </label>
                    <div>
                        <asp:RadioButtonList  runat="server" ID="Taxes">
                            <asp:ListItem Value="Yes" Text="Yes" />
                            <asp:ListItem Value="No" Text="No" Selected="true" />
                        </asp:RadioButtonList>
                    </div>
                    <div class="form-text">
                        Choose whether taxes apply for this seat.
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Seat Agreement
                    </label>
                    <insite:TextBox runat="server" ID="SeatAgreement"  TextMode="MultiLine" Rows="5" />
                    <div class="form-text">
                        Text of user agreement.
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Billing Customer
                    </label>
                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="CustomerUpdatePanel" />
                    <insite:UpdatePanel runat="server" ID="CustomerUpdatePanel">
                        <ContentTemplate>
                            <div>
                                <insite:TextBox runat="server" ID="CustomerName" Width="200px" MaxLength="128" CssClass="d-inline-block" />
                                <insite:RequiredValidator runat="server" FieldName="Custom Name" ControlToValidate="CustomerName" Display="Static" RenderMode="Dot" ValidationGroup="Customer" />
                                <insite:Button runat="server" ID="AddCustomerButton" ButtonStyle="OutlineSecondary" ValidationGroup="Customer" Text="Add Customer" Icon="fas fa-plus-circle" />
                            </div>
                            <p class="form-text">The individual or group responsible for payment of registration fees.</p>
                            <asp:Repeater runat="server" ID="CustomerList">
                                <HeaderTemplate><table class="table table-striped"></HeaderTemplate>
                                <FooterTemplate></table></FooterTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Container.DataItem %></td>
                                        <td class="text-end" style="width:80px;">
                                            <insite:IconButton runat="server" CommandName="Delete" CssClass="p-2" ToolTip="Delete Customer" Name="trash-alt" />
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ContentTemplate>
                    </insite:UpdatePanel>
                </div>

            </div>
        </div>

    </div>
</div>