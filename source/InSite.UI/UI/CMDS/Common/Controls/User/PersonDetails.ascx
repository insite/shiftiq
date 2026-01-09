<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonDetails.ascx.cs" Inherits="InSite.Cmds.Admin.People.Controls.PersonDetails" %>

<%@ Register Src="PersonGrid.ascx" TagName="PersonGrid" TagPrefix="uc" %>

<%@ Register Src="~/UI/Admin/Contacts/Comments/Controls/CommentRepeater.ascx" TagName="CommentRepeater" TagPrefix="uc" %>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="PersonalInformationUpdatePanel" />

<insite:UpdatePanel runat="server" ID="PersonalInformationUpdatePanel">
    <ContentTemplate>
        <insite:Nav runat="server">

            <insite:NavItem runat="server" Title="Details">
                <div class="row">
                    <div class="col-lg-4">

                        <h3>Personal</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                First Name
                                <insite:RequiredValidator runat="server" ControlToValidate="FirstName" FieldName="First Name" ValidationGroup="ContactInfo" />
                            </label>
                            <insite:TextBox runat="server" ID="FirstName" MaxLength="32" />
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Middle Initial
                            </label>
                            <insite:TextBox runat="server" ID="MiddleName" MaxLength="10" />
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Last Name
                                <insite:RequiredValidator runat="server" ControlToValidate="LastName" FieldName="Last Name" ValidationGroup="ContactInfo" />
                            </label>
                            <insite:TextBox runat="server" ID="LastName" MaxLength="32" />
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Email
                                <insite:RequiredValidator runat="server" FieldName="Email" ControlToValidate="EmailWork" ValidationGroup="ContactInfo" Display="Dynamic" />
                                <insite:EmailValidator runat="server" FieldName="Email" ControlToValidate="EmailWork" ValidationGroup="ContactInfo" Display="Dynamic" Enabled="true" />
                                <cmds:IxCustomValidator ID="ValidateEmailUnique" runat="server" ControlToValidate="EmailWork" ErrorMessage="This email already exists" ValidationGroup="ContactInfo" Display="Dynamic" />
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="EmailWork" MaxLength="128" />
                            </div>
                            <div class="form-text">
                                Enter an email that you use regularly. Remember this is a "shared" field for multi-organization users.
                            </div>
                        </div>

                        <div runat="server" id="SendEmailField" class="form-group mb-3">
                            <div>
                                <insite:Button runat="server" ID="SendEmailButton" Text="Send Welcome Email" Icon="fas fa-paper-plane" Visible="false" ButtonStyle="Default" />
                                <span runat="server" id="SendEmailDisabled" class="text-danger"><i class="far fa-exclamation-triangle"></i>Enable email notification to <strong>Send Welcome Email</strong></span>
                            </div>
                            <div class="form-text">
                                Sent Count:
                                <asp:Literal ID="SentCount" runat="server" />
                            </div>
                        </div>

                        <div runat="server" id="RowConfirmEmail" visible="false" class="form-group mb-3">
                            <label class="form-label">
                                Confirm Email
                                <insite:CompareValidator runat="server" ControlToValidate="EmailWork" ControlToCompare="ConfirmEmail" ValidationGroup="ContactInfo" ErrorMessage="Emails do not match" />
                            </label>
                            <insite:TextBox runat="server" ID="ConfirmEmail" MaxLength="256" />
                        </div>

                        <div runat="server" id="EnableEmailNotificationField" class="form-group mb-3">
                            <label class="form-label">
                                Notification
                            </label>
                            <div>
                                <asp:CheckBox runat="server" ID="EmailEnabled" Text="Enable email notification for this person" />
                            </div>
                        </div>

                        <div runat="server" id="BirthdateField" class="form-group mb-3">
                            <label class="form-label">
                                Birthdate
                            </label>
                            <div>
                                <insite:DateSelector ID="Birthdate" runat="server" />
                            </div>
                            <div class="form-text">
                                Click the calendar icon to choose a date from the calendar, or enter your 
                                birthdate manually. You can enter the date in any format. For example: 
                                <ul>
                                    <li>MMM d, yyyy</li>
                                    <li>MM d yy</li>
                                    <li>MM/d/yy</li>
                                </ul>
                                This information is required to track your progress with regard to various college certification programs.
                            </div>
                        </div>

                    </div>

                    <div class="col-lg-4">

                        <h3>Address and Phone</h3>

                        <div runat="server" id="Street1Field" class="form-group mb-3">
                            <label class="form-label">
                                Address
                            </label>
                            <insite:TextBox runat="server" ID="Street1" MaxLength="128" />
                        </div>

                        <div runat="server" id="CityField" class="form-group mb-3">
                            <label class="form-label">
                                City
                            </label>
                            <insite:TextBox runat="server" ID="City" MaxLength="256" />
                        </div>

                        <div runat="server" id="CountryField" class="form-group mb-3">
                            <label class="form-label">
                                Country
                            </label>
                            <insite:CountryComboBox ID="Country" runat="server" DropDown-Size="15" />
                        </div>

                        <div runat="server" id="ProvinceField" class="form-group mb-3">
                            <label class="form-label">
                                Province
                            </label>
                            <insite:MultiField runat="server">

                                <insite:MultiFieldView runat="server" ID="ProvinceSelectorView" Inputs="ProvinceSelector">
                                    <span class="multi-field-input">
                                        <insite:ProvinceComboBox ID="ProvinceSelector" runat="server" EnableSearch="true" CountryControl="Country" />
                                    </span>
                                    <insite:Button runat="server" OnClientClick='inSite.common.multiField.nextView(this); return false;'
                                        ButtonStyle="Default" Icon="far fa-keyboard" ToolTip="Enter a value manually" />
                                </insite:MultiFieldView>

                                <insite:MultiFieldView runat="server" ID="ProvinceTextView" Inputs="ProvinceText">
                                    <span class="multi-field-input">
                                        <insite:TextBox runat="server" ID="ProvinceText" MaxLength="64" />
                                    </span>
                                    <insite:Button runat="server" OnClientClick='inSite.common.multiField.nextView(this); return false;'
                                        ButtonStyle="Default" Icon="far fa-list-ul" ToolTip="Select an option from the list" />
                                </insite:MultiFieldView>

                            </insite:MultiField>
                        </div>

                        <div runat="server" id="PostalCodeField" class="form-group mb-3">
                            <label class="form-label">
                                Postal Code
                            </label>
                            <insite:TextBox runat="server" ID="PostalCode" MaxLength="64" />
                        </div>

                        <div runat="server" id="PhoneWorkField" class="form-group mb-3">
                            <label class="form-label">
                                Phone (work)
                            </label>
                            <insite:TextBox runat="server" ID="PhoneWork" MaxLength="32" />
                        </div>

                        <div runat="server" id="PhoneHomeField" class="form-group mb-3">
                            <label class="form-label">
                                Phone (home)
                            </label>
                            <insite:TextBox runat="server" ID="PhoneHome" MaxLength="32" />
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Phone (mobile)
                            </label>
                            <insite:TextBox runat="server" ID="PhoneMobile" MaxLength="32" />
                        </div>

                        <div runat="server" id="RowTimeZone" visible="false" class="form-group mb-3">
                            <label class="form-label">
                                Time Zone
                                <insite:RequiredValidator runat="server" FieldName="Time Zone" ControlToValidate="TimeZone" ValidationGroup="ContactInfo" Display="Dynamic" />
                            </label>
                            <insite:TimeZoneComboBox ID="TimeZone" runat="server" />
                        </div>

                    </div>
                    <div class="col-lg-4">

                        <div runat="server" id="EmploymentPanel">

                            <h3>Employment</h3>

                            <div class="form-group mb-3">
                                <label class="form-label">Person Code</label>
                                <insite:TextBox runat="server" ID="PersonCode" MaxLength="20" />
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Person Type</label>
                                <insite:TextBox runat="server" ID="PersonType" MaxLength="20" />
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Employee Type</label>
                                <insite:TextBox runat="server" ID="EmployeeType" MaxLength="16" />
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Job Title</label>
                                <insite:TextBox runat="server" ID="JobTitle" MaxLength="256" />
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Job Division</label>
                                <insite:TextBox runat="server" ID="JobDivision" MaxLength="100" />
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Age Group</label>
                                <insite:TextBox runat="server" ID="AgeGroup" MaxLength="20" />
                            </div>

                        </div>

                    </div>
                </div>
            </insite:NavItem>

            <insite:NavItem runat="server" ID="LoginTab" Title="Login">
                <div class="row">
                    <div class="col-lg-6">

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Status
                                <insite:RequiredValidator runat="server" ControlToValidate="Status" FieldName="Status" ValidationGroup="ContactInfo" />
                            </label>
                            <cmds:PersonStatusSelector ID="Status" runat="server" AllowNull="false" />
                        </div>

                        <div runat="server" id="PasswordExpiryField" class="form-group mb-3">
                            <label class="form-label">
                                Password Expiry
                            </label>
                            <insite:DateTimeOffsetSelector runat="server" ID="PasswordExpires" />
                        </div>

                        <div class="form-group mb-3">
                            <asp:Literal runat="server" ID="StatusTimestamp"></asp:Literal>
                        </div>

                    </div>
                </div>
            </insite:NavItem>

            <insite:NavItem runat="server" ID="GroupsTab" Title="Groups">
                <div class="row">
                    <div class="col-lg-4 mb-3 mb-lg-0">
                        <h3>Roles</h3>
                        <asp:CheckBoxList ID="UserRoleList" runat="server" />
                    </div>
                </div>
            </insite:NavItem>

            <insite:NavItem runat="server" ID="DuplicatesTab" Visible="false" Title="Possible Duplicates">
                <p>
                    <asp:Literal ID="WarningMessage" runat="server" />
                </p>
                <uc:PersonGrid ID="Duplicates" runat="server" />
            </insite:NavItem>

            <insite:NavItem runat="server" ID="CommentSubTab" Title="Comments">
                <uc:CommentRepeater runat="server" ID="CommentRepeater" />
            </insite:NavItem>

        </insite:Nav>
    </ContentTemplate>
</insite:UpdatePanel>
