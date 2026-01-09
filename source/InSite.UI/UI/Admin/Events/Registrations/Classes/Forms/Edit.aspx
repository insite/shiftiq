<%@ Page Language="C#" CodeBehind="Edit.aspx.cs" Inherits="InSite.Admin.Events.Registrations.Forms.Edit" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" ValidationGroup="Registration" />

    <section class="mb-3">
        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div class="clearfix">
                    <h4 class="card-title mb-3 d-sm-none">
                        <i class="far fa-id-card me-1"></i>
                        Registration
                    </h4>

                    <div class="float-sm-end mb-3">
                        <insite:Button runat="server" ID="CardButton" Icon="fas fa-id-card" Text="Card" ButtonStyle="OutlinePrimary" />
                        <insite:Button runat="server" ID="MoveButton" Icon="fas fa-copy" Text="Move" ButtonStyle="OutlinePrimary" />
                        <insite:Button runat="server" ID="HistoryButton" Icon="fas fa-history" Text="History" ButtonStyle="OutlinePrimary" />
                    </div>

                    <h4 class="card-title mb-3 d-none d-sm-block">
                        <i class="far fa-id-card me-1"></i>
                        Registration
                    </h4>
                </div>

                <div class="row mb-md-3">

                    <div class="col-md-6 mb-3 mb-md-0">
                        <div class="card h-100">
                            <div class="card-body">

                                <h4 class="card-title mb-3">
                                    Registration
                                </h4>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Registration #
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="RegistrationSequence" />
                                    </div>
                                    <div class="form-text">
                                        The position of this registration in the sequence of all registrations for the same event.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Registration Requested
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="RegistrationRequestedOn" />
                                    </div>
                                    <div class="form-text">
                                        The date and time when this registration was first initiated.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label runat="server" id="EventTitleLabel" class="form-label">
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="ClassTitle" />
                                    </div>
                                    <div class="form-text">
                                        The event for which the Participant is registering.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Participant Name
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="CandidateName" />
                                    </div>
                                    <div class="form-text">
                                        The user registering for the event.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Registered By
                                    </label>
                                    <div>
                                        <insite:FindPerson runat="server" ID="RegistrationRequestedBy" />
                                    </div>
                                </div>

                                <div runat="server" id="FormField" class="form-group mb-3">
                                    <label class="form-label">
                                        Assessment Form
                                    </label>
                                    <div>
                                        <insite:FindBankForm runat="server" ID="FormIdentifier" />
                                    </div>
                                    <div class="form-text">
                                        <asp:CheckBox runat="server" ID="IsEligible" Text="Eligible" />
                                    </div>
                                </div>

                                <div runat="server" id="RegistrationPasswordField" class="form-group mb-3">
                                    <label class="form-label">
                                        Exam Password
                                        <insite:RequiredValidator runat="server" ControlToValidate="RegistrationPassword" FieldName="Registration Password" ValidationGroup="Registration" />
                                    </label>
                                    <insite:TextBox runat="server" ID="RegistrationPassword" MaxLength="14" />
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Hours Worked to Date
                                    </label>
                                    <insite:NumericBox runat="server" ID="WorkBasedHoursToDate" NumericMode="Integer" Width="150px" MinValue="0" MaxValue="99999" />
                                    <div class="form-text">
                                        Number of hours worked at time of registration.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Approval
                                    </label>
                                    <div>
                                        <insite:ItemNameComboBox runat="server" ID="ApprovalStatus">
                                            <Settings UseCurrentOrganization="true" CollectionName="Registrations/Approval/Status" UseSequenceOrder="true" />
                                        </insite:ItemNameComboBox>
                                    </div>
                                    <div class="form-text">
                                        The current approval status of the Participant at the time of this registration.
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>

                    <div class="col-md-6 mb-3 mb-md-0">
                        <div class="card mb-3">
                            <div class="card-body">

                                <h4 class="card-title mb-3">
                                    Payment
                                </h4>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Seat
                                    </label>
                                    <insite:ComboBox runat="server" ID="Seat" />
                                </div>

                                <div runat="server" id="PriceOptionField" class="form-group mb-3">
                                    <label class="form-label">
                                        Price Option
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="PriceOption" />
                                    </div>
                                </div>

                                <insite:UpdatePanel runat="server" ID="RegistrationFeeUpdatePanel" CssClass="form-group mb-3">
                                    <ContentTemplate>
                                        <label class="form-label">
                                            Registration Fee
                                            <span runat="server" id="PaidLabel" class="badge bg-success">
                                                Paid
                                            </span>
                                            <span runat="server" id="MovedLabel" class="badge bg-warning">
                                                Moved
                                            </span>
                                        </label>
                                        <div>
                                            <insite:NumericBox runat="server" ID="RegistrationFee" DecimalPlaces="2" Width="150px" MinValue="0" MaxValue="1000000" CssClass="d-inline-block" />
                                            <div class="d-inline-block mt-2">
                                                <asp:CheckBox runat="server" ID="IncludeInT2202" Text="Include in T2202 tax form" />
                                            </div>
                                        </div>
                                        <div class="form-text">
                                            The dollar amount that must be paid by the registrant for participation in this event.
                                        </div>
                                    </ContentTemplate>
                                </insite:UpdatePanel>

                                <div runat="server" id="BillingCodePanel" class="form-group mb-3">
                                    <label class="form-label">
                                        Bill To
                                    </label>
                                    <div>
                                        <insite:TextBox runat="server" ID="BillingCode" MaxLength="100" TextMode="MultiLine" Rows="5" />
                                    </div>
                                </div>

                                <insite:UpdatePanel runat="server" CssClass="form-group mb-3">
                                    <ContentTemplate>
                                        <span runat="server" id="EmployerStatus" class="badge bg-custom-default float-end"></span>
                                        <label class="form-label">
                                            Employer
                                        </label>
                                        <insite:FindGroup runat="server" ID="Employer" />
                                    </ContentTemplate>
                                </insite:UpdatePanel>
                                <div class="form-group mb-3" runat="server" id="InvoiceNumberSection" visible="false">
                                    <label class="form-label">
                                        Invoice Number
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="InvoiceNumber" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div runat="server" id="AccommodationCard" class="card">
                            <div class="card-body">

                                <h4 class="card-title mb-3">Accommodation
                                </h4>
                                <div class="form-group mb-3">
                                    <table>
                                        <tr>
                                            <td>
                                                <insite:AccommodationTypeComboBox runat="server" ID="AccommodationTypeSelector" AllowBlank="true" EnableSearch="true" Width="225px" DropDown-Width="268px" />
                                                <insite:RequiredValidator runat="server" ControlToValidate="AccommodationTypeSelector" FieldName="Accommodation Type" ValidationGroup="Accommodation" RenderMode="Dot" Display="None" />
                                            </td>
                                            <td style="padding-left: 5px;">
                                                <insite:Button runat="server" ID="AddAccommodationButton"
                                                    ValidationGroup="Accommodation" ButtonStyle="OutlinePrimary" Icon="far fa-plus-circle" ToolTip="Add Accommodation" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>

                                <div runat="server" id="AccommodationField" class="form-group mb-3">
                                    <div>
                                        <table class="table table-stripped">
                                            <thead>
                                                <tr>
                                                    <th>Accommodation Type</th>
                                                    <th>Name</th>
                                                    <th style="text-align: right;">Time Extension</th>
                                                    <th></th>
                                                </tr>
                                            </thead>
                                            <asp:Repeater runat="server" ID="AccommodationsRepeater">
                                                <ItemTemplate>
                                                    <tr>
                                                        <td><%# Eval("AccommodationType") %></td>
                                                        <td><%# Eval("AccommodationName") %></td>
                                                        <td style="text-align: right;"><%# Eval("TimeExtension") %> minutes</td>
                                                        <td style="width: 50px;">
                                                            <insite:IconButton runat="server"
                                                                CommandName="Delete"
                                                                CommandArgument='<%# Eval("AccommodationType") %>'
                                                                Name="trash-alt"
                                                                ToolTip="Remove Accommodation"
                                                                ConfirmText="Are you sure you to remove this accommodation?" />
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>

                <div class="row">

                    <div class="col-md-6 mb-3 mb-md-0">
                        <div class="card h-100">
                            <div class="card-body">

                                <h4 class="card-title mb-3">
                                    Result
                                </h4>

                                <insite:UpdatePanel runat="server" CssClass="form-group mb-3">
                                    <ContentTemplate>
                                        <label class="form-label">
                                            Attendance
                                        </label>
                                        <div>
                                            <insite:ItemNameComboBox runat="server" ID="AttendanceStatus">
                                                <Settings UseCurrentOrganization="true" CollectionName="Registrations/Attendance/Status" UseSequenceOrder="true" />
                                            </insite:ItemNameComboBox>
                                        </div>
                                        <div class="form-text">
                                            The current status of this record in the registration process or workflow.
                                        </div>
                                    </ContentTemplate>
                                </insite:UpdatePanel>

                                <div runat="server" visible="false" class="form-group mb-3">
                                    <label class="form-label">
                                        Score
                                    </label>
                                    <insite:NumericBox runat="server" ID="Score" NumericMode="Integer" Width="150px" MinValue="0" MaxValue="100" />
                                    %
                                    <div runat="server" id="ScoreHelpBlock" class="form-text">
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Comment
                                    </label>
                                    <insite:TextBox runat="server" ID="RegistrationComment" TextMode="MultiLine" Rows="5" />
                                    <div class="form-text">
                                        Additional information that is specific to this registration.
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>

                </div>

            </div>
        </div>
    </section>

    <div>
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Registration" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>
</asp:Content>
