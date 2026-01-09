<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditTabDetails.ascx.cs" Inherits="InSite.UI.Admin.Contacts.People.Controls.EditTabDetails" %>

<%@ Register Src="~/UI/Layout/Common/Controls/ProfilePictureUpload.ascx" TagName="ProfilePictureUpload" TagPrefix="uc" %>

<div class="float-end text-end mb-3">
    <insite:DropDownButton runat="server" ID="SendEmail" ButtonStyle="Default" DefaultAction="None" IconName="paper-plane" Text="Send Email" CssClass="d-inline-block">
        <Items>
            <insite:DropDownButtonLinkItem Name="Correspondence" Text="Correspondence" />
            <insite:DropDownButtonLinkItem Name="Welcome" Text="Welcome Email" />
        </Items>
    </insite:DropDownButton>
    <insite:DropDownButton runat="server" ID="MoreInfoButton" ButtonStyle="Default" DefaultAction="None" IconType="Regular" IconName="circle-info" Text="More Info" CssClass="d-inline-block" />
    <insite:ButtonSpacer runat="server" />
    <insite:Button runat="server" ButtonStyle="Default" OnClientClick="personEditor.onViewPersonHistoryClick(); return false;" Text="History" Icon="fas fa-history" />
</div>

<div class="clearfix"></div>


<div class="container-fluid px-0">
    <div class="row">
        <div class="col-12 col-md-5 d-flex flex-column mb-lg-0">
            <div class="card mb-3 flex-grow-1">
                <div class="card-body">

                    <h3>Personal</h3>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            First Name
                            <insite:RequiredValidator runat="server" ControlToValidate="FirstName" FieldName="First Name" ValidationGroup="Person" />
                        </label>
                        <insite:TextBox ID="FirstName" runat="server" MaxLength="32" />
                        <div class="form-text">
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Middle Name
                        </label>
                        <insite:TextBox ID="MiddleName" runat="server" MaxLength="32" />
                        <div class="form-text">
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Last Name
                            <insite:RequiredValidator runat="server" ControlToValidate="LastName" FieldName="Last Name" ValidationGroup="Person" />
                        </label>
                        <insite:TextBox ID="LastName" runat="server" MaxLength="32" />
                        <div class="form-text">
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Email / Login Name
                            <insite:RequiredValidator runat="server" ID="EmailRequiredValidator" ControlToValidate="Email" FieldName="Email" ValidationGroup="Person" Display="Dynamic" />
                        </label>
                        <div>
                            <insite:TextBox ID="Email" runat="server" MaxLength="128" />
                        </div>
                        <div class="mt-2">
                            <insite:CheckBox runat="server" ID="EmailDisabled" Text="Disable sending any and all email to this address" />
                        </div>
                        <div class="form-text">
                            Preferred email address for this contact person.
                            <insite:IconLink runat="server" ID="EmailCommand" CssClass="field-icon" Name="paper-plane" ToolTip="Send Email" Visible="false" />
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Alternate Email
                        </label>
                        <div>
                            <insite:TextBox ID="EmailAlternate" runat="server" MaxLength="128" />
                        </div>
                        <div class="mt-2">
                            <insite:CheckBox runat="server" ID="EmailAlternateDisabled" Text="Disable sending any and all email to this address" />
                        </div>
                        <div class="form-text">
                            Secondary email address for this contact person.
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Marketing (Newsletter) Preference
                        </label>
                        <div>
                            <insite:CheckBox runat="server" ID="MarketingEmailDisabled" Text="Disable sending marketing email to this contact, as per their request" />
                        </div>
                    </div>
                    
                    <div class="form-group mb-3">
                        <div runat="server" id="PersonType" class="float-end badge bg-custom-default" visible="false"></div>
                        <label class="form-label">
                            <insite:ContentTextLiteral runat="server" ContentLabel="Person Code"/>
                            <insite:CustomValidator runat="server"
                                ID="UniquePersonCode"
                                ControlToValidate="ContactCode"
                                ValidationGroup="Person"
                                Display="Dynamic"
                                ErrorMessage="This person code is already used by another person"
                            />
                        </label>
                        <insite:TextBox ID="ContactCode" runat="server" MaxLength="20" />
                        <div class="form-text">
                            A reference code that uniquely identifies this person
                            (e.g. employee number, membership number, account code).
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <div runat="server" id="GenderBadge" class="float-end badge bg-info" visible="false">
                        </div>
                        <label class="form-label">
                            Gender
                        </label>
                        <insite:GenderComboBox runat="server" ID="GenderCombo" AllowBlank="true" EmptyMessage="Gender" />
                        <div class="form-text">
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Birthdate
                        </label>
                        <insite:DateSelector ID="Birthdate" runat="server" Width="200px" />
                        <div class="form-text">
                        </div>
                    </div>

                </div>
            </div>
        </div>
        <div class="col-12 col-md-4 d-flex flex-column mb-lg-0">
            <div class="card mb-3">
                <div class="card-body">

                    <h3 runat="server" id="EmployerTag">Jobs and Employment</h3>

                    <div runat="server" id="EmployerField" class="form-group mb-3">
                        <div runat="server" id="EmployerBadge" class="float-end badge bg-custom-default" visible="false">
                        </div>
                        <label class="form-label">
                            Belongs to or Employed by
                            <insite:IconLink runat="server" ID="EmployerGroupLink" Name="pencil" ToolTip="Edit Group" />
                        </label>
                        <insite:FindEmployer ID="EmployerGroupIdentifier" runat="server" />
                        <div class="form-text">
                            Select the person's primary group here. <a href="/ui/admin/contacts/groups/create?type=Employer&tag=Company" target="_blank">Add a new group</a>
                        </div>
                    </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Job Title
                    </label>
                    <insite:TextBox ID="JobTitle" runat="server" MaxLength="256" />
                    <div class="form-text">
                        Current or desired primary occupation.
                    </div>
                </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Job Division
                        </label>
                        <insite:TextBox ID="JobDivision" runat="server" MaxLength="100" />
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Occupation Interest
                        </label>
                        <div>
                            <insite:OccupationListComboBox runat="server" ID="OccupationIdentifier" EmptyMessage="Select One" />
                        </div>
                        <div class="form-text">What is the occupation area of interest for this person?</div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Consent to Share Records
                        </label>
                        <div>
                            <insite:ComboBox runat="server" ID="ConsentToShare" Enabled="false">
                                <Items>
                                    <insite:ComboBoxOption />
                                    <insite:ComboBoxOption Value="Yes" Text="Consent Given" />
                                    <insite:ComboBoxOption Value="No" Text="No Consent" />
                                </Items>
                            </insite:ComboBox>
                        </div>
                        <div class="form-text">
                            Has this contact given consent to share their records with interested employers?
                        </div>
                    </div>

                </div>
            </div>
            <div class="card mb-3">
                <div class="card-body">

                    <h3>Account/Association</h3>

                    <div class="form-group mb-3">
                        <label runat="server" id="AccountStatusLabel" class="form-label">
                            Current Status
                        </label>
                        <insite:ItemIdComboBox runat="server" ID="AccountStatusId" EmptyMessage="Account Status" />
                        <div class="form-text">
                            The current status of this person's account in the organization.
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Start Date
                        </label>
                        <insite:DateSelector runat="server" ID="AssociationStartDate" />
                        <div class="form-text">
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            End Date
                        </label>
                        <insite:DateSelector runat="server" ID="AssociationEndDate" />
                        <div class="form-text">
                        </div>
                    </div>

                </div>
            </div>
            <div class="card mb-3">
                <div class="card-body">

                    <h3>Emergency Contact</h3>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Emergency Contact Name
                        </label>
                        <insite:TextBox ID="EmergencyContactName" runat="server" MaxLength="100" />
                        <div class="form-text">
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Emergency Contact Phone Number
                        </label>
                        <insite:TextBox ID="EmergencyContactPhoneNumber" runat="server" MaxLength="32" />
                        <div class="form-text">
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Emergency Contact Relationship
                        </label>
                        <insite:TextBox ID="EmergencyContactRelationship" runat="server" MaxLength="50" />
                        <div class="form-text">
                        </div>
                    </div>

                </div>
            </div>
        </div>
        <div class="col-12 col-md-3 d-flex flex-column mb-lg-0">
            <div class="card mb-3">
                <div class="card-body">
                    <uc:ProfilePictureUpload runat="server" ID="ProfilePictureUploadControl" />
                </div>
            </div>
            <div class="card mb-3 flex-grow-1">
                <div class="card-body">

                    <h3>Phone Numbers</h3>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Preferred
                    </label>
                    <insite:TextBox ID="Phone" runat="server" MaxLength="30" />
                    <div class="form-text">
                        Please call this number first
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Home
                    </label>
                    <insite:TextBox ID="PhoneHome" runat="server" MaxLength="32" />
                    <div class="form-text">
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Work
                    </label>
                    <insite:TextBox ID="PhoneWork" runat="server" MaxLength="32" />
                    <div class="form-text">
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Mobile
                    </label>
                    <insite:TextBox ID="PhoneMobile" runat="server" MaxLength="32" />
                    <div class="form-text">
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Other
                    </label>
                    <insite:TextBox ID="PhoneOther" runat="server" MaxLength="32" />
                    <div class="form-text">
                    </div>
                </div>

                </div>
            </div>
        </div>
    </div>
</div>
