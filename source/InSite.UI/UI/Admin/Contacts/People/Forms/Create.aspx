<%@ Page CodeBehind="Create.aspx.cs" Inherits="InSite.Admin.Contacts.People.Forms.Create" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../../Addresses/Controls/AddressCreator.ascx" TagName="AddressCreator" TagPrefix="uc" %>
<%@ Register Src="../Controls/PersonGrid.ascx" TagName="PersonGrid" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="ScreenNotification" />
    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Person" />

    <div class="row mb-3">
        <div class="col-lg-12">
            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <h4 class="card-title mb-3">
                        <i class="far fa-user me-1"></i>
                        New Person
                    </h4>

                    <div runat="server" id="CreationTypePanel" class="row">
                        <div class="col-lg-6">
                            <div class="form-group mb-3">
                                <insite:CreationTypeComboBox runat="server" ID="CreationType" TypeName="Person" />
                            </div>
                        </div>
                    </div>

                    <asp:MultiView runat="server" ID="CreateMultiView">

                        <asp:View runat="server" ID="ViewOnePerson">

                            <div class="row mb-3">
                                <div class="col-lg-6">

                                    <h6 class="mt-3">Personal Information</h6>

                                    <div class="row">
                                        <div class="col-xxl-4">
                                            <div class="form-group mb-3">
                                                <label class="form-label">
                                                    First Name
                                                    <insite:RequiredValidator runat="server" ControlToValidate="OnePersonFirstName" FieldName="First Name" ValidationGroup="Person" />
                                                </label>
                                                <insite:TextBox ID="OnePersonFirstName" runat="server" MaxLength="32" Width="100%" />
                                            </div>
                                        </div>
                                        <div class="col-xxl-4">
                                            <div class="form-group mb-3">
                                                <label class="form-label">
                                                    Middle Name
                                                </label>
                                                <insite:TextBox ID="OnePersonMiddleName" runat="server" MaxLength="32" />
                                            </div>
                                        </div>
                                        <div class="col-xxl-4">
                                            <div class="form-group mb-3">
                                                <label class="form-label">
                                                    Last Name
                                                    <insite:RequiredValidator runat="server" ControlToValidate="OnePersonLastName" FieldName="Last Name" ValidationGroup="Person" />
                                                </label>
                                                <insite:TextBox ID="OnePersonLastName" runat="server" MaxLength="32" Width="100%" />
                                            </div>
                                        </div>
                                    </div>

                                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="OnePersonEmailUpdatePanel" />

                                    <insite:UpdatePanel runat="server" ID="OnePersonEmailUpdatePanel">
                                        <ContentTemplate>
                                            <div class="form-group mb-3">
                                                <label class="form-label">
                                                    Email
                                                    <insite:EmailValidator runat="server" ControlToValidate="OnePersonEmail" FieldName="Email" ValidationGroup="Person" Display="Dynamic" />
                                                    <insite:RequiredValidator runat="server" ID="OnePersonEmailRequiredValidator" ControlToValidate="OnePersonEmail" FieldName="Email" ValidationGroup="Person" Display="Dynamic" />
                                                </label>
                                                <div>
                                                    <insite:TextBox ID="OnePersonEmail" runat="server" MaxLength="128" CssClass="d-inline-block" />
                                                    <insite:Button ID="OnePersonGenerateEmailButton" runat="server" Text="Generate Email" Icon="fas fa-sync" 
                                                        ButtonStyle="OutlinePrimary" CssClass="float-end mt-1" />
                                                </div>
                                                <div>
                                                    <div class="mt-2">
                                                        <insite:CheckBox runat="server" ID="OnePersonEmailDisabled" Text="Disable sending any and all email to this address" />
                                                    </div>
                                                </div>
                                                <div class="form-text">
                                                    Preferred email address for this contact person.
                                                    <insite:IconLink runat="server" ID="OnePersonEmailCommand" CssClass="field-icon" Name="paper-plane" ToolTip="Send Email" Visible="false" />
                                                </div>
                                            </div>
                                        </ContentTemplate>
                                    </insite:UpdatePanel>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            <insite:ContentTextLiteral runat="server" ContentLabel="Person Code"/>
                                            <insite:CustomValidator runat="server" ID="OnePersonUniquePersonCode"
                                                ControlToValidate="OnePersonCode" ValidationGroup="Person" Display="Dynamic"
                                                ErrorMessage="This person code is already used by another person"
                                            />
                                        </label>
                                        <insite:TextBox ID="OnePersonCode" runat="server" MaxLength="20" />
                                        <div class="form-text">
                                            A reference code that uniquely identifies this person
                                            (e.g. employee number, membership number, account code).
                                        </div>
                                    </div>

                                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="OnePersonEmployerUpdatePanel" />

                                    <insite:UpdatePanel runat="server" ID="OnePersonEmployerUpdatePanel">
                                        <ContentTemplate>
                                            <div class="form-group mb-3">
                                                <div runat="server" id="OnePersonEmployerBadge" class="float-end badge bg-custom-default" visible="false">
                                                </div>
                                                <label class="form-label">
                                                    Belongs to or Employed by
                                                </label>
                                                <insite:FindEmployer ID="OnePersonEmployerGroupIdentifier" runat="server" />
                                                <div class="form-text">
                                                    Select the person's primary group here. <a href="/ui/admin/contacts/groups/create?type=Employer&tag=Company" target="_blank">Add a new group</a>
                                                </div>
                                            </div>
                                        </ContentTemplate>
                                    </insite:UpdatePanel>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Job Title
                                        </label>
                                        <insite:TextBox ID="OnePersonJobTitle" runat="server" MaxLength="256" Width="100%" />
                                        <div class="form-text">
                                            Occupation at primary place of employment.
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Preferred Phone
                                        </label>
                                        <insite:TextBox ID="Phone" runat="server" MaxLength="30" />
                                        <div class="form-text">
                                            Please call this number first
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Time Zone
                                            <insite:RequiredValidator runat="server" ControlToValidate="OnePersonTimeZone" FieldName="Time Zone" ValidationGroup="Person" Display="Dynamic" />
                                        </label>
                                        <insite:TimeZoneComboBox runat="server" ID="OnePersonTimeZone" />
                                        <div class="form-text">
                                            All time zones in Canada and United States are supported. 
                                            Contact us if you need another time zone added to this list.
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Birthdate
                                        </label>
                                        <insite:DateSelector ID="OnePersonBirthdate" runat="server" Width="200px" />
                                        <div class="form-text">
                                            Date of birth for this contact person.
                                        </div>
                                    </div>

                                    <div runat="server" ID="OnePersonCheckDuplicatesField" class="form-group mb-3">
                                        <div>
                                            <insite:CheckBox runat="server" ID="OnePersonCheckDuplicates" Text="Check for potential duplicates" />
                                        </div>
                                        <div class="form-text">
                                            Search the database for contacts with the same name and/or similar pronunciation.
                                        </div>
                                    </div>

                                </div>
                                <div class="col-lg-6">

                                    <h6 class="mt-3">Login Credentials</h6>

                                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="OnePersonAccessGrantedUpdatePanel" />

                                    <insite:UpdatePanel runat="server" ID="OnePersonAccessGrantedUpdatePanel">
                                        <ContentTemplate>
                                            <div class="form-group mb-3">
                                                <label class="form-label">
                                                    Status
                                                </label>
                                                <div>
                                                    <insite:CheckBox ID="OnePersonIsUserAccessGranted" runat="server" Text="User Access Granted" />
                                                </div>
                                                <div class="form-text">
                                                    Enable or disable the login credentials for this person's user account, allowing them to sign in.
                                                </div>
                                            </div>

                                            <insite:Container runat="server" ID="OnePersonSignInFieldGroup">
                                                <div class="form-group mb-3">
                                                    <label class="form-label">
                                                        Password
                                                        <insite:RequiredValidator runat="server" ID="OnePersonPasswordValidator" ControlToValidate="OnePersonPassword" ValidationGroup="Person" FieldName="Password" />
                                                    </label>
                                                    <insite:TextBox runat="server" ID="OnePersonPassword" MaxLength="64" Width="100%" />
                                                    <div class="form-text">
                                                        Specify the password this user will sign in initially. They will be required to change this password the first time they sign in.
                                                    </div>
                                                </div>

                                                <div class="form-group mb-3">
                                                    <label class="form-label">
                                                        Permission Lists
                                                    </label>
                                                    <asp:Repeater runat="server" ID="OnePersonRolesRepeater">
                                                        <ItemTemplate>
                                                            <div>
                                                                <insite:CheckBox runat="server" ID="IsSelected" Text='<%# Eval("GroupName") %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                    <div class="form-text">
                                                        Select the permission lists to which you want to assign this new user account.
                                                    </div>
                                                </div>
                                            </insite:Container>
                                        </ContentTemplate>
                                    </insite:UpdatePanel>

                                </div>
                            </div>
                        </asp:View>

                        <asp:View runat="server" ID="ViewMultiplePerson">
                            <div class="row">
                                <div class="col-md-6">

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Email Addresses
                                            <insite:RequiredValidator runat="server" ControlToValidate="MultiplePersonEmailAddressList" ValidationGroup="Person" FieldName="Email Addresses" Display="None" />
                                        </label>
                                        <insite:TextBox runat="server" ID="MultiplePersonEmailAddressList" TextMode="MultiLine" Rows="20" Width="100%" AllowHtml="true" />
                                        <div class="form-text">
                                            Input one email address per line. The display name for each recipient must precede the
                                            address.<br />For example: <b><i>John Smith</i> john.smith@example.org</b>
                                        </div>
                                    </div>

                                </div>
                            </div>
                        </asp:View>

                    </asp:MultiView>

                </div>
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-lg-6">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Person" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

    <insite:Modal runat="server" ID="OnePersonDuplicateWindow" Title="Possible Duplicate(s)" Width="800px"
        Visible="false" VisibleOnLoad="true"
        EnableStaticBackdrop="true" EnableCloseButton="false" EnalbeCloseOnEscape="false">
        <ContentTemplate>
            <div class="px-3">

                <div runat="server" id="OnePersonDuplicateMessage"></div>

                <uc:PersonGrid runat="server" ID="OnePersonDuplicateGrid" />
                                    
                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="OnePersonDuplicateUpdatePanel" />

                <insite:UpdatePanel runat="server" ID="OnePersonDuplicateUpdatePanel">
                    <Triggers>
                        <asp:PostBackTrigger ControlID="OnePersonDuplicateCloseButton" />
                    </Triggers>
                    <ContentTemplate>
                        <div class="alert alert-warning">
                            <p>
                                The system does not allow two different contacts to have the same email address, therefore you have the following options:
                            </p>

                            <div class="pb-3">
                                <asp:RadioButtonList runat="server" ID="OnePersonDuplicateOption" AutoPostBack="true">
                                    <asp:ListItem Value="Cancel" Text="Cancel (don't add a new contact person)" />
                                    <asp:ListItem Value="Connect" Text="Connect (connect the existing person to my organization)" />
                                    <asp:ListItem Value="Create" Text="Create (proceed to add a new contact person)" />
                                </asp:RadioButtonList>
                            </div>
                                        
                            <div runat="server" id="OnePersonDuplicateCancelDescription" visible="false">
                                Clear the form and start again. Do <strong>not</strong> add a new record for this person. 
                            </div>
                            <div runat="server" id="OnePersonDuplicateConnectDescription" visible="false">
                                Do <strong>not</strong> add a new record for this person. Instead, connect the existing user to my 
                                organization's organization account. This person can use the same email address to sign in to both organizations, 
                                and their information in each organization remains separate.
                            </div>
                            <div runat="server" id="OnePersonDuplicateCreateDescription" visible="false">
                                Create a new record for this person. 
                                To ensure no two users have the same Email, the new contact record will have these values:
                                <ul>
                                    <li><strong>Email (Login Name)</strong> = <asp:Literal runat="server" ID="OnePersonDuplicateDuplicateEmail" /></li>
                                    <li><strong>Email Alternate</strong> = <asp:Literal runat="server" ID="OnePersonDuplicateDuplicateEmailAlternate" /></li>
                                </ul>
                                Please remember: If a contact person has multiple user accounts in this system then they require a 
                                different email address for each each account.
                            </div>
                        </div>

                        <div class="mb-3">
                            <insite:Button runat="server" ID="OnePersonDuplicateContinueButton" Text="Continue" ButtonStyle="Primary" Icon="fas fa-arrow-right" Visible="false" />
                            <insite:Button runat="server" ID="OnePersonDuplicateCloseButton" Text="Cancel" Icon="fas fa-times" ButtonStyle="Default" />
                        </div>
                    </ContentTemplate>
                </insite:UpdatePanel>

                <p runat="server" id="OnePersonDuplicateAddNewInstruction">
                    Alternatively, you can contact your account manager to create and configure this person's record (in order to help prevent duplication of data).
                </p>
                                    
            </div>

        </ContentTemplate>
    </insite:Modal>

    <insite:Modal runat="server" ID="MultiplePersonConfirmWindow" Title="Confirmation" Width="800px" 
        Visible="false" VisibleOnLoad="true"
        EnableStaticBackdrop="true" EnableCloseButton="false" EnalbeCloseOnEscape="false">
        <ContentTemplate>
            <div class="my-3 px-3">
                <div runat="server" id="MultiplePersonConfirmMessage"></div>

                <insite:Container runat="server" ID="MultiplePersonConfirmCommands">
                    <p>Are you sure you want to create new records for these people?</p>

                    <insite:Button runat="server" ID="MultiplePersonConfirmButton" Text="Yes - Add New Contacts" Icon="fas fa-check" ButtonStyle="Success" />
                    <insite:Button runat="server" ID="MultiplePersonCancelButton" Text="No" Icon="fas fa-times" ButtonStyle="Default" />
                </insite:Container>
                <insite:Container runat="server" ID="MultiplePersonOtherCommands">
                    <insite:Button runat="server" ID="MultiplePersonCloseButton" Text="Close" Icon="fas fa-times" ButtonStyle="Default" />
                </insite:Container>
            </div>
        </ContentTemplate>
    </insite:Modal>

</asp:Content>