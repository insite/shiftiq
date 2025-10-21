<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Create.aspx.cs" Inherits="InSite.UI.Portal.Contacts.Referral.Create" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />
    <insite:ValidationSummary runat="server" ValidationGroup="Person" />

    <div class="row">
        <div class="col-md-6">

            <div class="row">
                <div class="col-md-12">

                    <div class="card mb-3">

                        <div class="card-body">
                            <h3 class="mt-3">Personal Information</h3>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    First Name
                                    <insite:RequiredValidator runat="server" ControlToValidate="PersonFirstName" FieldName="First Name" ValidationGroup="Person" />
                                </label>
                                <insite:TextBox ID="PersonFirstName" runat="server" MaxLength="40" />
                            </div>
                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Middle Name
                                </label>
                                <insite:TextBox ID="PersonMiddleName" runat="server" MaxLength="32" />
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Last Name
                                    <insite:RequiredValidator runat="server" ControlToValidate="PersonLastName" FieldName="Last Name" ValidationGroup="Person" />
                                </label>
                                <insite:TextBox ID="PersonLastName" runat="server" MaxLength="40" />
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Primary Email
                                    <insite:RequiredValidator runat="server" ControlToValidate="PersonEmail" FieldName="Primary Email" ValidationGroup="Person" />
                                </label>
                                <insite:TextBox ID="PersonEmail" runat="server" MaxLength="254" />
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Phone
                                    <insite:RequiredValidator runat="server" ControlToValidate="PersonPhone" FieldName="Phone" ValidationGroup="Person" />
                                </label>
                                <insite:TextBox ID="PersonPhone" runat="server" MaxLength="32" />
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Birthdate
                                    <insite:RequiredValidator runat="server" ControlToValidate="PersonBirthdate" FieldName="Birthdate" ValidationGroup="Person" />
                                </label>
                                <insite:DateSelector ID="PersonBirthdate" runat="server" Width="200px" />
                                <div class="form-text">
                                    Date of birth for this contact person.
                                </div>
                            </div>

                        </div>

                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-md-12">

                    <div class="card mb-3">

                        <div class="card-body">
                            <h3 class="mt-3">Other Information</h3>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Occupation
                                    <insite:RequiredValidator runat="server" ControlToValidate="OccupationIdentifier" FieldName="Occupation" ValidationGroup="Person" Enabled="false" />
                                </label>
                                <div>
                                    <insite:OccupationListComboBox runat="server" ID="OccupationIdentifier" EmptyMessage="Select One" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Reason for Referral
                                </label>
                                <div>
                                    <insite:CollectionItemComboBox runat="server" ID="ReferrerIdentifier" />
                                </div>
                            </div>

                        </div>

                    </div>
                </div>
            </div>

        </div>

        <div class="col-md-6">

            <div class="card mb-3">

                <div class="card-body">
                    <h3 class="mt-3">Home Address</h3>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Address 1
                            <insite:RequiredValidator runat="server" ControlToValidate="Street1" FieldName="Address" ValidationGroup="Person" />
                        </label>
                        <insite:TextBox ID="Street1" runat="server" MaxLength="128" />
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Address 2
                        </label>
                        <insite:TextBox ID="Street2" runat="server" MaxLength="128" />
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            City
                            <insite:RequiredValidator runat="server" ControlToValidate="City" FieldName="City" ValidationGroup="Person" />
                        </label>
                        <insite:TextBox ID="City" runat="server" MaxLength="128" />
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            State/Province
                            <insite:RequiredValidator runat="server" ControlToValidate="Province" FieldName="State/Province" ValidationGroup="Person" />
                        </label>
                        <insite:TextBox ID="Province" runat="server" MaxLength="128" />
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Postal Code
                            <insite:RequiredValidator runat="server" ControlToValidate="PostalCode" FieldName="Postal Code" ValidationGroup="Person" />
                        </label>
                        <insite:TextBox ID="PostalCode" runat="server" MaxLength="16" />
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Country
                            <insite:RequiredValidator runat="server" ControlToValidate="Country" FieldName="Country" ValidationGroup="Person" />
                        </label>
                        <insite:TextBox ID="Country" runat="server" MaxLength="128" Text="Canada" />
                    </div>


                </div>
            </div>
        </div>


    </div>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Person" />
            <insite:CancelButton runat="server" ID="CancelButton" NavigateUrl="/ui/portal/contacts/referral/search" />
        </div>
    </div>

</asp:Content>