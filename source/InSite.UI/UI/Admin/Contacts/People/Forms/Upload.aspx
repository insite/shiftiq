<%@ Page Language="C#" CodeBehind="Upload.aspx.cs" Inherits="InSite.Admin.Contacts.People.Forms.Upload" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/PersonGrid.ascx" TagName="PersonGrid" TagPrefix="uc" %>
<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="ScreenStatusView" />
    <insite:ValidationSummary runat="server" ValidationGroup="File" />
    <insite:ValidationSummary runat="server" ValidationGroup="Fields" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="FileTab" Title="File" Icon="far fa-upload" IconPosition="BeforeText">

            <div class="row mt-3">
                <div class="col-md-6 col-lg-5 col-xl-4 mb-3 mb-md-0">
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">

                            <h3>Select and Upload File</h3>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Encoding
                                </label>
                                <insite:ComboBox runat="server" ID="FileEncoding" >
                                    <Items>
                                        <insite:ComboBoxOption Text="ASCII" Value="us-ascii" />
                                        <insite:ComboBoxOption Text="UTF 7" Value="utf-7" />
                                        <insite:ComboBoxOption Text="UTF 8 (recommended)" Value="utf-8" Selected="true" />
                                        <insite:ComboBoxOption Text="UTF 16" Value="utf-16" />
                                        <insite:ComboBoxOption Text="UTF 32" Value="utf-32" />
                                    </Items>
                                </insite:ComboBox>
                                <div class="form-text">
                                    Select the character encoding that was used to create the CSV file you have selected.
                                    Most computer programs use 
                                    <a target="_blank" href="https://en.wikipedia.org/wiki/UTF-8">UTF-8</a>
                                    so you should select a different encoding only if you are certain of the encoding that was used.
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Select CSV File
                                    <insite:RequiredValidator runat="server" ControlToValidate="ImportFile" FieldName="CSV File" ValidationGroup="File" Display="Dynamic" />
                                </label>
                                <insite:FileUploadV1 runat="server" ID="ImportFile" AllowedExtensions=".txt,.csv" LabelText="" FileUploadType="Unlimited" />
                                <div class="form-text">
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Attach contacts to group
                                </label>
                                <insite:MultiField runat="server">

                                    <insite:MultiFieldView runat="server" ID="GroupSelectorView">
                                        <span class="multi-field-input">
                                            <insite:FindGroup runat="server" ID="GroupSelector" />
                                        </span>
                                        <insite:Button runat="server" OnClientClick='inSite.common.multiField.nextView(this); return false;'
                                            ButtonStyle="OutlineSecondary" Icon="far fa-keyboard" ToolTip="Enter a value manually" />
                                    </insite:MultiFieldView>

                                    <insite:MultiFieldView runat="server" ID="GroupTextView">
                                        <span class="multi-field-input">
                                            <insite:TextBox runat="server" ID="GroupText" MaxLength="90" />
                                        </span>
                                        <insite:Button runat="server" OnClientClick='inSite.common.multiField.nextView(this); return false;'
                                            ButtonStyle="OutlineSecondary" Icon="far fa-list-ul" ToolTip="Select an option from the list" />
                                    </insite:MultiFieldView>

                                </insite:MultiField>
                                <div class="form-text">
                                    Optional: Select an existing group or input the name of a new group.
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <insite:CheckBox runat="server" ID="EnableLoginCredentials" Text="Approve login credentials for all uploaded contacts" />
                            </div>

                        </div>
                    </div>
                </div>
                <div class="col-md-6 col-lg-7 col-xl-8">
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">

                            <h3>Instructions</h3>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Follow these steps to import your contacts.
                                </label>
                                <ul>
                                    <li>Your upload file must be a spreadsheet saved as a <strong>CSV UTF-8 (comma delimited) (*.csv)</strong> file.</li>
                                    <li>Click here to <a href="/UI/Admin/contacts/people/content/upload-contact-people.csv">download a template</a>.</li>
                                    <li>Your spreadsheet must contain a title for every column.</li>
                                </ul>
                                <p>Here is a simple example:</p>
                                <table class="table table-striped table-bordered">
                                    <tr>
                                        <th>Email</th>
                                        <th>FirstName</th>
                                        <th>LastName</th>
                                        <th>JobTitle</th>
                                        <th>Number</th>
                                    </tr>
                                    <tr>
                                        <td>jane.austen@famousauthors.org</td>
                                        <td>Jane</td>
                                        <td>Austen</td>
                                        <td>Novelist</td>
                                        <td>AINJANE</td>
                                    </tr>
                                    <tr>
                                        <td>mark.twain@famousauthors.org</td>
                                        <td>Mark</td>
                                        <td>Twain</td>
                                        <td>Writer</td>
                                        <td>AINTWAIN</td>
                                    </tr>
                                    <tr>
                                        <td>charles.dickens@famousauthors.org</td>
                                        <td>Charles</td>
                                        <td>Dickens</td>
                                        <td>Social Critic</td>
                                        <td>AINCHARLES</td>
                                    </tr>
                                </table>
                            </div>

                        </div>
                    </div>
                </div>
            </div>

            <div class="mt-3">
                <insite:NextButton runat="server" ID="FileNextButton" ValidationGroup="File" />
                <insite:CancelButton runat="server" ID="FileCancelButton" />
            </div>

        </insite:NavItem>

        <insite:NavItem runat="server" ID="FieldsTab" Title="Fields" Icon="far fa-cogs" IconPosition="BeforeText" Visible="false">

            <div class="row mt-3 mb-3">
                <div class="col-md-6 mb-3 mb-md-0">
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">

                            <h3>Identifier</h3>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Unique Identifier Type
                                    <insite:RequiredValidator runat="server" ControlToValidate="UniqueIdentifierTypeSelector" FieldName="Unique Identifier Type" ValidationGroup="Fields" Display="Dynamic" />
                                </label>
                                <insite:ComboBox runat="server" ID="UniqueIdentifierTypeSelector">
                                    <Items>
                                        <insite:ComboBoxOption />
                                        <insite:ComboBoxOption Value="Email" Text="Email" />
                                        <insite:ComboBoxOption Value="PersonCode" Text="PersonCode" />
                                    </Items>
                                </insite:ComboBox>
                                <div class="form-text">
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Unique Identifier
                                    <insite:RequiredValidator runat="server" ControlToValidate="UniqueIdentifierSelector" FieldName="Unique Identifier" ValidationGroup="Fields" Display="Dynamic" />
                                </label>
                                <insite:ComboBox runat="server" ID="UniqueIdentifierSelector" />
                                <div class="form-text">
                                </div>
                            </div>

                            <h3>Sign In</h3>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Email
                                    <insite:RequiredValidator runat="server" ControlToValidate="EmailSelector" FieldName="Email" ValidationGroup="Fields" Display="Dynamic" />
                                </label>
                                <insite:ComboBox runat="server" ID="EmailSelector" />
                                <div class="form-text">
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Password
                                </label>
                                <insite:ComboBox runat="server" ID="PasswordSelector" />
                                <div class="form-text">
                                </div>
                            </div>

                            <h3>Person</h3>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    First Name
                                    <insite:RequiredValidator runat="server" ControlToValidate="FirstNameSelector" FieldName="First Name" ValidationGroup="Fields" Display="Dynamic" />
                                </label>
                                <insite:ComboBox runat="server" ID="FirstNameSelector" />
                                <div class="form-text">
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Last Name
                                    <insite:RequiredValidator runat="server" ControlToValidate="LastNameSelector" FieldName="Last Name" ValidationGroup="Fields" Display="Dynamic" />
                                </label>
                                <insite:ComboBox runat="server" ID="LastNameSelector" />
                                <div class="form-text">
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Alternate Email
                                </label>
                                <div>
                                    <insite:ComboBox runat="server" ID="EmailAlternate" />
                                </div>
                                <div class="form-text">
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Gender
                                </label>
                                <insite:ComboBox runat="server" ID="GenderCombo" />
                                <div class="form-text">
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Birthdate
                                </label>
                                <insite:ComboBox runat="server" ID="BirthdateSelector"  />
                                <div class="form-text">
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Employed By (use Employer Group Unique Identifier)
                                </label>
                                <insite:ComboBox ID="EmployerGroupIdentifier" runat="server" />
                                <div class="form-text">
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Function Type
                                </label>
                                <insite:ComboBox ID="FunctionType" runat="server" />
                                <div class="form-text">
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Job Title
                                </label>
                                <insite:ComboBox runat="server" ID="JobTitleSelector"  />
                                <div class="form-text">
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Emergency Contact Name
                                </label>
                                <insite:ComboBox runat="server" ID="EmergencyContactNameSelector"  />
                                <div class="form-text">
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Emergency Contact Phone
                                </label>
                                <insite:ComboBox runat="server" ID="EmergencyContactPhoneSelector"  />
                                <div class="form-text">
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Emergency Contact Relationship
                                </label>
                                <insite:ComboBox runat="server" ID="EmergencyContactRelationshipSelector"  />
                                <div class="form-text">
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">

                            <h3>Phone Numbers</h3>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Phone Preferred
                                </label>
                                <insite:ComboBox runat="server" ID="PhonePreferredSelector"  />
                                <div class="form-text">
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Phone Home
                                </label>
                                <insite:ComboBox runat="server" ID="PhoneHomeSelector"  />
                                <div class="form-text">
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Phone Mobile
                                </label>
                                <insite:ComboBox runat="server" ID="PhoneMobileSelector"  />
                                <div class="form-text">
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Phone Other
                                </label>
                                <insite:ComboBox runat="server" ID="PhoneOtherSelector"  />
                                <div class="form-text">
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Phone Work
                                </label>
                                <insite:ComboBox runat="server" ID="PhoneWorkSelector"  />
                                <div class="form-text">
                                </div>
                            </div>

                            <h3>Other</h3>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    <insite:ContentTextLiteral runat="server" ContentLabel="Person Code"/>
                                </label>
                                <insite:ComboBox runat="server" ID="PersonCodeSelector"  />
                                <div class="form-text">
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Social Insurance Number (SIN)
                                </label>
                                <insite:ComboBox runat="server" ID="SocialInsuranceNumberSelector"  />
                                <div class="form-text">
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Start Date
                                </label>
                                <insite:ComboBox runat="server" ID="StartDateSelector"  />
                                <div class="form-text">
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Archive Status
                                </label>
                                <insite:ComboBox runat="server" ID="ArchiveStatusSelector"  />
                                <div class="form-text">
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Preferred Language
                                </label>
                                <insite:ComboBox runat="server" ID="Language" />
                                <div class="form-text">
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Time Zone
                                </label>
                                <insite:ComboBox runat="server" ID="TimeZoneSelector" />
                                <div class="form-text">
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Honorific
                                </label>
                                <insite:ComboBox runat="server" ID="HonorificSelector" />
                                <div class="form-text">
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-md-6 mb-3 mb-md-0">
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">

                            <h3>Home Address</h3>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Description
                                </label>
                                <insite:ComboBox runat="server" ID="HomeAddressDescriptionSelector"  />
                                <div class="form-text">
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Street 1
                                </label>
                                <insite:ComboBox runat="server" ID="HomeAddressStreet1Selector"  />
                                <div class="form-text">
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Street 2
                                </label>
                                <insite:ComboBox runat="server" ID="HomeAddressStreet2Selector"  />
                                <div class="form-text">
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    City
                                </label>
                                <insite:ComboBox runat="server" ID="HomeAddressCitySelector"  />
                                <div class="form-text">
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Province
                                </label>
                                <insite:ComboBox runat="server" ID="HomeAddressProvinceSelector"  />
                                <div class="form-text">
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Postal Code
                                </label>
                                <insite:ComboBox runat="server" ID="HomeAddressPostalCodeSelector"  />
                                <div class="form-text">
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Country
                                </label>
                                <insite:ComboBox runat="server" ID="HomeAddressCountrySelector"  />
                                <div class="form-text">
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">

                            <h3>Work Address</h3>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Description
                                </label>
                                <insite:ComboBox runat="server" ID="WorkAddressDescriptionSelector"  />
                                <div class="form-text">
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Street 1
                                </label>
                                <insite:ComboBox runat="server" ID="WorkAddressStreet1Selector"  />
                                <div class="form-text">
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Street 2
                                </label>
                                <insite:ComboBox runat="server" ID="WorkAddressStreet2Selector"  />
                                <div class="form-text">
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    City
                                </label>
                                <insite:ComboBox runat="server" ID="WorkAddressCitySelector"  />
                                <div class="form-text">
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Province
                                </label>
                                <insite:ComboBox runat="server" ID="WorkAddressProvinceSelector"  />
                                <div class="form-text">
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Postal Code
                                </label>
                                <insite:ComboBox runat="server" ID="WorkAddressPostalCodeSelector"  />
                                <div class="form-text">
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Country
                                </label>
                                <insite:ComboBox runat="server" ID="WorkAddressCountrySelector"  />
                                <div class="form-text">
                                </div>
                            </div>


                        </div>
                    </div>
                </div>
            </div>

            <div class="mt-3">
                <insite:SaveButton runat="server" ID="FieldsSaveButton" ValidationGroup="Person" />
                <insite:CancelButton runat="server" ID="FieldsCancelButton" />
            </div>

        </insite:NavItem>

        <insite:NavItem runat="server" ID="DuplicatesTab" Title="Duplicates" Icon="far fa-exclamation-triangle" IconPosition="BeforeText" Visible="false">

            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">
                    <insite:Alert runat="server" ID="DuplicatesStatus" />
                </div>
            </div>

            <div class="mt-3">
                <insite:NextButton runat="server" ID="DuplicatesNextButton" ValidationGroup="File" />
                <insite:CancelButton runat="server" ID="DuplicatesCancelButton" />
            </div>

        </insite:NavItem>

        <insite:NavItem runat="server" ID="CompleteTab" Title="Status" Icon="far fa-check-circle" IconPosition="BeforeText" Visible="false">

            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">
                    <div class="row">
                        <div class="col-lg-6 mb-3 mb-lg-0">

                            <asp:Repeater runat="server" ID="FirstRecordsRepeater">
                                <HeaderTemplate>
                                    <h3><%= FirstRecordsTitle %></h3>
                                    <table class="table table-striped table-bordered">
                                        <tr>
                                            <th>Name</th>
                                            <th>Email</th>
                                        </tr>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <a href="/ui/admin/contacts/people/edit?contact=<%# Eval("User.UserIdentifier") %>"><%# Eval("FirstName") %> <%# Eval("LastName") %></a>
                                        </td>
                                        <td><a href="mailto:<%# Eval("Email") %>"><%# Eval("Email") %></a></td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate></table></FooterTemplate>
                            </asp:Repeater>

                        </div>
                        <div class="col-lg-6">

                            <asp:Repeater runat="server" ID="LastRecordsRepeater">
                                <HeaderTemplate>
                                    <h3>Last 5 contacts:</h3>
                                    <table class="table table-striped table-bordered">
                                        <tr>
                                            <th>Name</th>
                                            <th>Email</th>
                                        </tr>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td><a href="/ui/admin/contacts/people/edit?contact=<%# Eval("User.UserIdentifier") %>"><%# Eval("FirstName") %> <%# Eval("LastName") %></a></td>
                                        <td><a href="mailto:<%# Eval("Email") %>"><%# Eval("Email") %></a></td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate></table></FooterTemplate>
                            </asp:Repeater>

                        </div>
                    </div>
                </div>
            </div>

            <div class="mt-3">

                <insite:CancelButton runat="server" ID="CompleteCancelButton" />
            </div>

        </insite:NavItem>

    </insite:Nav>

    <insite:ProgressPanel runat="server" ID="UploadProgress" HeaderText="Upload Contacts" Cancel="Redirect">
        <Items>
            <insite:ProgressIndicator Name="Progress" Caption="Completed: {percent}%" />
            <insite:ProgressStatus Text="Status: Importing data{running_ellipsis}" />
            <insite:ProgressStatus Text="Elapsed time: {time_elapsed}s" />
        </Items>
    </insite:ProgressPanel>


    <insite:Modal runat="server" ID="OnePersonDuplicateWindow" Title="Possible Duplicate(s)" Width="800px"
        Visible="false" VisibleOnLoad="true"
        EnableStaticBackdrop="true" EnableCloseButton="false" EnalbeCloseOnEscape="false">
        <ContentTemplate>
            <div class="px-3">
                <div runat="server" id="OnePersonDuplicateMessage"></div>

                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="OnePersonDuplicateUpdatePanel" />

                <insite:UpdatePanel runat="server" ID="OnePersonDuplicateUpdatePanel">
                    <Triggers>
                        <asp:PostBackTrigger ControlID="OnePersonDuplicateCloseButton" />
                    </Triggers>
                    <ContentTemplate>
                        <div runat="server" id="OnePersonDuplicateCounter"></div>
                        <uc:PersonGrid runat="server" ID="OnePersonDuplicateGrid" />
                        <div class="alert alert-warning">
                            <p>
                                The system does not allow two different contacts to have the same email address, therefore you have the following options:
                            </p>

                            <div class="pb-3">
                                <asp:RadioButtonList runat="server" ID="OnePersonDuplicateOption" AutoPostBack="true">
                                    <asp:ListItem Value="Skip" Text="Skip (don't add a new contact person)" />
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
                                    <li><strong>Email (Login Name)</strong> =
                                        <asp:Literal runat="server" ID="OnePersonDuplicateDuplicateEmail" /></li>
                                    <li><strong>Email Alternate</strong> =
                                        <asp:Literal runat="server" ID="OnePersonDuplicateDuplicateEmailAlternate" /></li>
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

</asp:Content>
