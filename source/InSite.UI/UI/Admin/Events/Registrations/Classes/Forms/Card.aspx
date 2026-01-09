<%@ Page Language="C#" CodeBehind="Card.aspx.cs" Inherits="InSite.Admin.Events.Registrations.Forms.Card" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Events/Classes/Controls/ClassSummaryInfo.ascx" TagName="ClassSummaryInfo" TagPrefix="uc" %>	
<%@ Register Src="~/UI/Admin/Events/Classes/Controls/ClassLocationInfo.ascx" TagName="ClassLocationInfo" TagPrefix="uc" %>	

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <section class="mb-3">
        <h2 class="h4 mt-4 mb-3">
            <i class="far fa-user me-1"></i>
            Participant
        </h2>

        <div class="row">
            <div class="col-md-6 col-lg-4 mb-3 mb-md-0">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h4>Personal</h4>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Full Name
                            </label>
                            <div>
                                <asp:Literal runat="server" ID="FullName" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Email
                            </label>
                            <div>
                                <asp:Literal runat="server" ID="Email" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Birthdate
                            </label>
                            <div>
                                <asp:Literal runat="server" ID="Birthdate" />
                            </div>
                        </div>

                    </div>
                </div>
            </div>
            <div class="col-md-6 col-lg-4 mb-3 mb-md-0">

                <div class="card border-0 shadow-lg mb-3">
                    <div class="card-body">

                        <h4>Contact</h4>
                    
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Phone Numbers
                            </label>
                            <div>
                                <asp:Literal ID="PhoneNumbers" runat="server" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Home Address
                            </label>
                            <div>
                                <asp:Literal runat="server" ID="HomeAddress" />
                            </div>
                        </div>

                    </div>
                </div>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <h4>Emergency contact</h4>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Emergency Contact Name
                            </label>
                            <div>
                                <asp:Literal runat="server" ID="EmergencyContactName" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Emergency Contact Phone Number
                            </label>
                            <div>
                                <asp:Literal runat="server" ID="EmergencyContactPhoneNumber" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Emergency Contact Relationship
                            </label>
                            <div>
                                <asp:Literal runat="server" ID="EmergencyContactRelationship" />
                            </div>
                        </div>

                    </div>
                </div>

            </div>
            <div class="col-md-6 col-lg-4 mb-3 mb-md-0">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h4>Work</h4>

                        <div runat="server" ID="LearnerIdNumberField" class="form-group mb-3">
                            <label class="form-label">
                                <insite:ContentTextLiteral runat="server" ContentLabel="Person Code"/>
                            </label>
                            <div>
                                <asp:Literal runat="server" ID="PersonCode" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Number of Work Based Hours to date
                            </label>
                            <div>
                                <asp:Literal runat="server" ID="NumberWorkHoursToDate" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            English Language Learner <asp:CheckBox ID="ESL" runat="server" Text="" Enabled="false" />
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </section>

    <section runat="server" id="EmployerSection" class="mb-3">
        <h2 class="h4 mt-4 mb-3">
            <i class="far fa-user-tie me-1"></i>
            Employer
        </h2>

        <div class="row">
            <div class="col-md-6 mb-3 mb-md-0">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h4>Employment</h4>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Employer
                            </label>
                            <div>
                                <asp:Literal ID="EmployerName" runat="server" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Mailing Address
                            </label>
                            <div>
                                <asp:Literal ID="EmployerMailingAddress" runat="server" />
                            </div>
                        </div>

                    </div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h4>Employer Contact</h4>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Employer Contact Name
                            </label>
                            <div>
                                <asp:Literal ID="EmployerContactName" runat="server" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Employer Contact Phone Number
                            </label>
                            <div>
                                <asp:Literal ID="EmployerContactPhoneNumber" runat="server" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Employer Contact Email
                            </label>
                            <div>
                                <asp:Literal ID="EmployerContactEmail" runat="server" />
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </section>

    <section class="mb-3">
        <h2 class="h4 mt-4 mb-3">
            <i class="far fa-calendar-alt me-1"></i>
            Class
        </h2>

        <div class="row">
            <div class="col-md-6 mb-3 mb-md-0">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h4>Summary</h4>
                        <uc:ClassSummaryInfo runat="server" ID="ClassSummaryInfo" />
                    </div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h4>Location and Schedule</h4>
                        <uc:ClassLocationInfo runat="server" ID="ClassLocationInfo" />
                    </div>
                </div>
            </div>
        </div>
    </section>

    <section runat="server" id="SeatSection" class="mb-3">
        <h2 class="h4 mt-4 mb-3">
            <i class="far fa-money-check-alt me-1"></i>
            Seat
        </h2>

        <div class="row">
            <div class="col-md-6 mb-3 mb-md-0">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h4>Details</h4>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Seat Name
                            </label>
                            <div>
                                <asp:Literal runat="server" ID="SeatName"  />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Registration Fee
                            </label>
                            <div>
                                <asp:Literal runat="server" ID="RegistrationFee"  />
                            </div>
                        </div>

                        <div runat="server" id="BillingCustomerField" class="form-group mb-3">
                            <label class="form-label">
                                Billing To
                            </label>
                            <div>
                                <asp:Literal runat="server" ID="BillingCustomer"  />
                            </div>
                        </div>

                    </div>
                </div>
            </div>
            <div runat="server" id="SeatAgreement" class="col-md-6">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h4>Agreement</h4>

                        <div class="form-group mb-3">
                            <div class="mb-3">
                                <asp:Literal runat="server" ID="AgreementText" />
                            </div>
                            <div>
                                <asp:CheckBox runat="server" ID="Agreed" Text="I Agree" Checked="true" Enabled="false" />
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </section>

    <section>
        <insite:CancelButton runat="server" ID="CancelButton" Text="Close" />
    </section>

</asp:Content>