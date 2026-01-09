<%@ Page Language="C#" CodeBehind="ChangeVenue.aspx.cs" Inherits="InSite.Admin.Events.Exams.Forms.ChangeVenue" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/ExamInfoSummary.ascx" TagName="ExamInfoSummary" TagPrefix="uc" %>	

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="ExamVenue" />

    <section runat="server" ID="ChangeVenueSection" class="mb-3">

        <div class="row">

            <div class="col-lg-6">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <uc:ExamInfoSummary ID="ExamInfoSummary" runat="server" />

                    </div>
                </div>
            </div>

            <div class="col-lg-6">

                <div class="card border-0 shadow-lg mb-3">
                    <div class="card-body">

                        <h3>Current Venue</h3>
                
                        <div runat="server" id="CurrentVenueOfficeField" class="form-group mb-3">
                            <div class="float-end">
                                <span runat="server" id="VenueOfficeEqualsLocation" class="badge bg-success">Office = Location</span>
                                <span runat="server" id="VenueOfficeNotEqualsLocation" class="badge bg-warning">Office &#8800; Location</span>
                            </div>
                            <label class="form-label">Current Invigilating Office</label>
                            <div>
                                <asp:Literal runat="server" ID="CurrentVenueOffice" />
                            </div>
                        </div>

                        <div runat="server" id="CurrentVenueLocationField" class="form-group mb-3">
                            <label class="form-label">Current Venue Location</label>
                            <div>
                                <asp:Literal runat="server" ID="CurrentVenueLocation" />
                            </div>
                        </div>

                        <div runat="server" id="CurrentVenueRoomField" class="form-group mb-3">
                            <label class="form-label">Current Building and Room</label>
                            <div>
                                <asp:Literal runat="server" ID="CurrentVenueRoom" />
                            </div>
                        </div>

                    </div>
                </div>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <h3>New Venue</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                New Invigilating Office
                                <insite:RequiredValidator runat="server" ControlToValidate="VenueOfficeIdentifier" FieldName="New Invigilating Office" ValidationGroup="ExamVenue" />
                            </label>
                            <div>
                                <insite:FindGroup ID="VenueOfficeIdentifier" runat="server" />
                            </div>
                            <div class="form-text">Identifies the office from which invigilators will be assigned.</div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                New Venue Location
                                <insite:RequiredValidator runat="server" ControlToValidate="VenueLocationIdentifier" FieldName="New Venue Location" ValidationGroup="ExamVenue" />
                            </label>
                            <div>
                                <insite:FindGroup ID="VenueLocationIdentifier" runat="server" />
                            </div>
                            <div class="form-text">The training provider, organization, or agency hosting the event.</div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">New Building and Room</label>
                            <div>
                                <insite:TextBox ID="NewVenueRoom" runat="server" MaxLength="200" Width="100%" />
                            </div>
                            <div class="form-text">The physical location within the venue where the event occurs.</div>
                        </div>

                    </div>
                </div>
            </div>

        </div>

    </section>

    <div>
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="ExamVenue" CausesValidation="true" />
        <insite:CancelButton runat="server" ID="BackButton" />
    </div>

</asp:Content>
