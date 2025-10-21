<%@ Page Language="C#" CodeBehind="Move.aspx.cs" Inherits="InSite.Admin.Events.Registrations.Forms.Move" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Move" />

    <div class="row my-3">
        <div class="col-lg-6">
            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <h4 class="card-title mb-3">
                        <i class="far fa-id-card me-1"></i>
                        Move Registration
                    </h4>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            From Current Event
                        </label>
                        <div>
                            <asp:Literal runat="server" ID="ClassTitle" />
                        </div>
                    </div>

                    <div runat="server" id="SeatField" class="form-group mb-3">
                        <label class="form-label">
                            Current Seat
                        </label>
                        <div>
                            <asp:Literal runat="server" ID="SeatTitle" />
                        </div>
                    </div>

                    <div runat="server" id="RegistrationFeeField" class="form-group mb-3">
                        <label class="form-label">
                            Registration Fee
                        </label>
                        <div>
                            <asp:Literal runat="server" ID="RegistrationFee" />
                        </div>
                    </div>

                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />
                    <insite:UpdatePanel runat="server" ID="UpdatePanel">
                        <ContentTemplate>
                            <div class="form-group mb-3">
                                <label class="form-label">
                                    To New Event
                                    <insite:RequiredValidator runat="server" ControlToValidate="MoveToEvent" FieldName="Move to Class" ValidationGroup="Move" Display="None" />
                                    <asp:CustomValidator runat="server"
                                        ID="UniqueRegistrationValidator"
                                        ControlToValidate="MoveToEvent"
                                        ErrorMessage="This user is already registered in this class"
                                        ValidationGroup="Move"
                                        Display="None"
                                    />
                                </label>
                                <insite:FindEvent runat="server" ID="MoveToEvent" ShowPrefix="false" />
                            </div>

                            <div runat="server" id="NewSeatField" class="form-group mb-3" visible="false">
                                <label class="form-label">
                                    New Seat
                                    <insite:RequiredValidator runat="server"
                                        ID="NewSeatValidator"
                                        ControlToValidate="MoveToSeat"
                                        FieldName="New Seat"
                                        ValidationGroup="Move"
                                        Display="None"
                                    />
                                </label>
                                <div>
                                    <insite:SeatComboBox runat="server" ID="MoveToSeat" />
                                </div>
                            </div>
                        </ContentTemplate>
                    </insite:UpdatePanel>

                </div>
            </div>
        </div>
    </div>
        
    <div>
        <insite:SaveButton runat="server" ID="SaveButton" Text="Move" Icon="fas fa-copy" ValidationGroup="Move" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>
