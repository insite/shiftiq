<%@ Page Language="C#" CodeBehind="Profile.aspx.cs" Inherits="InSite.UI.Portal.Profile" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="~/UI/Portal/Contacts/Locations/AddressList.ascx" TagName="AddressList" TagPrefix="uc" %>
<%@ Register Src="~/UI/Portal/Contacts/Groups/Controls/EmployerEditor.ascx" TagName="EmployerDetail" TagPrefix="uc" %>
<%@ Register Src="~/UI/Layout/Common/Controls/ProfilePictureUpload.ascx" TagName="ProfilePictureUpload" TagPrefix="uc" %>
<%@ Register Src="~/UI/Layout/Common/Controls/Navigation/ModeSwitch.ascx" TagName="ModeSwitch" TagPrefix="uc" %>
<%@ Register Src="~/UI/Layout/Common/Controls/Navigation/SidebarSwitch.ascx" TagName="SidebarSwitch" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

    <style>
        
        h2, h5 { margin: 0; }
        
        .card-body { z-index: inherit; }

        .api-key-label {
            font-weight: 600;
            color: #374151;
            margin-bottom: 0.5rem;
            font-size: 0.875rem;
            text-transform: uppercase;
            letter-spacing: 0.05em;
        }

        .api-key-display {
            display: flex;
            align-items: center;
            gap: 0.75rem;
            background: #f9fafb;
            border: 1px solid #d1d5db;
            border-radius: 6px;
            padding: 0.75rem;
        }

        .api-key-text {
            flex: 1;
            font-family: 'Monaco', 'Consolas', monospace;
            font-size: 0.875rem;
            color: #1f2937;
            word-break: break-all;
            user-select: all;
        }

        .copy-link {
            background: #3b82f6;
            color: white;
            text-decoration: none;
            border-radius: 4px;
            padding: 0.5rem 1rem;
            font-size: 0.875rem;
            font-weight: 500;
            cursor: pointer;
            transition: all 0.2s;
            white-space: nowrap;
            border: none;
            display: inline-block;
        }

        .copy-link:hover {
            background: #2563eb;
            color: white;
            text-decoration: none;
            transform: translateY(-1px);
        }

        .copy-link:active {
            transform: translateY(0);
        }

        .copy-link.copied {
            background: #10b981;
        }

        .copy-feedback {
            margin-top: 0.5rem;
            font-size: 0.75rem;
            color: #10b981;
            opacity: 0;
            transition: opacity 0.3s;
        }

        .copy-feedback.show {
            opacity: 1;
        }

    </style>

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Toast runat="server" ID="ProfileToast" Visible="false" />
    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ID="ValidationSummary" />

    <insite:Nav runat="server">
        <insite:NavItem runat="server" ID="AccountSection" Icon="far fa-user" Title="My Account">

            <div class="mb-3">
                <insite:SaveButton runat="server" ID="SaveButtonTop" />
                <insite:CancelButton runat="server" ID="CancelButtonTop" />
                <insite:Button runat="server" ID="ChangePasswordButtonTop" Text="Change Password" ButtonStyle="Default" Icon="fas fa-sync" CausesValidation="false" />
                <insite:Button runat="server" ID="MFAButtonTop" Text="Multi-Factor Authentication" ButtonStyle="Default" Icon="fas fa-shield" CausesValidation="false" />
            </div>

            <div class="row">

                <div class="col-md-3">

                    <div class="card border-0 shadow mb-4">
                        <div class="card-body">
                            <uc:ProfilePictureUpload runat="server" ID="ProfilePictureUploadControl" />
                        </div>
                    </div>

                    <div runat="server" id="PhoneNumbersGroup" class="card border-0 shadow">
                        <div class="card-body">

                            <h5 class="card-title">
                                <insite:Literal runat="server" Text="Phone Numbers" />
                            </h5>

                            <div class="form-group mb-3" runat="server" id="PhoneField">
                                <label class="form-label" for="<%# Phone.ClientID %>">
                                    <insite:Literal runat="server" Text="Preferred" />
                                    <insite:RequiredValidator runat="server" ID="PhoneRequiredValidator" ControlToValidate="Phone" Display="None" Visible="false" />
                                </label>
                                <div>
                                    <insite:TextBox ID="Phone" runat="server" MaxLength="32" Width="100%" />
                                </div>
                                <div class="form-text">
                                    <insite:Literal runat="server" Text="Call this number first" />
                                </div>
                            </div>

                            <div class="form-group mb-3" runat="server" id="PhoneHomeField">
                                <label class="form-label" for="<%# PhoneHome.ClientID %>">
                                    <insite:Literal runat="server" Text="Home" />
                                    <insite:RequiredValidator runat="server" ID="PhoneHomeRequiredValidator" ControlToValidate="PhoneHome" Display="None" Visible="false" />
                                </label>
                                <div>
                                    <insite:TextBox ID="PhoneHome" runat="server" MaxLength="32" Width="100%" />
                                </div>
                            </div>

                            <div class="form-group mb-3" runat="server" id="PhoneWorkField">
                                <label class="form-label" for="<%# PhoneWork.ClientID %>">
                                    <insite:Literal runat="server" Text="Work" />
                                    <insite:RequiredValidator runat="server" ID="PhoneWorkRequiredValidator" ControlToValidate="PhoneWork" Display="None" Visible="false" />
                                </label>
                                <div>
                                    <insite:TextBox ID="PhoneWork" runat="server" MaxLength="32" Width="100%" />
                                </div>
                            </div>

                            <div class="form-group mb-3" runat="server" id="PhoneMobileField">
                                <label class="form-label" for="<%# PhoneMobile.ClientID %>">
                                    <insite:Literal runat="server" Text="Mobile" />
                                    <insite:RequiredValidator runat="server" ID="PhoneMobileRequiredValidator" ControlToValidate="PhoneMobile" Display="None" Visible="false" />
                                </label>
                                <div>
                                    <insite:TextBox ID="PhoneMobile" runat="server" MaxLength="32" Width="100%" />
                                </div>
                            </div>

                            <div class="form-group mb-3" runat="server" id="PhoneOtherField">
                                <label class="form-label" for="<%# PhoneOther.ClientID %>">
                                    <insite:Literal runat="server" Text="Other" />
                                    <insite:RequiredValidator runat="server" ID="PhoneOtherRequiredValidator" ControlToValidate="PhoneOther" Display="None" Visible="false" />
                                </label>
                                <div>
                                    <insite:TextBox ID="PhoneOther" runat="server" MaxLength="32" Width="100%" />
                                </div>
                            </div>

                        </div>
                    </div>

                </div>

                <div class="col-md-4">

                    <div runat="server" id="PersonalGroup" class="card border-0 shadow mb-4">
                        <div class="card-body">
                            <h5 class="card-title">
                                <insite:Literal runat="server" Text="Personal" />
                            </h5>

                            <div class="form-group mb-3" runat="server" id="FirstNameField">
                                <label class="form-label" for="<%# FirstName.ClientID %>">
                                    <insite:Literal runat="server" Text="First Name" />
                                    <insite:RequiredValidator runat="server" ID="FirstNameRequiredValidator" ControlToValidate="FirstName" Display="None" />
                                </label>
                                <div>
                                    <insite:TextBox ID="FirstName" runat="server" MaxLength="32" Width="100%" />
                                </div>
                            </div>

                            <div class="form-group mb-3" runat="server" id="MiddleNameField">
                                <label class="form-label" for="<%# MiddleName.ClientID %>">
                                    <insite:Literal runat="server" Text="Middle Name" />
                                    <insite:RequiredValidator runat="server" ID="MiddleNameRequiredValidator" ControlToValidate="MiddleName" Display="None" Visible="false" />
                                </label>
                                <div>
                                    <insite:TextBox ID="MiddleName" runat="server" MaxLength="32" Width="100%" />
                                </div>
                            </div>

                            <div class="form-group mb-3" runat="server" id="LastNameField">
                                <label class="form-label" for="<%# LastName.ClientID %>">
                                    <insite:Literal runat="server" Text="Last Name" />
                                    <insite:RequiredValidator runat="server" ID="LastNameRequiredValidator" ControlToValidate="LastName" Display="None" />
                                </label>
                                <div>
                                    <insite:TextBox ID="LastName" runat="server" MaxLength="32" Width="100%" />
                                </div>
                            </div>

                            <div class="form-group mb-3" runat="server" id="EmailField">
                                <label class="form-label" for="<%# Email.ClientID %>">
                                    <insite:Literal runat="server" Text="Email / Login Name" />
                                    <insite:RequiredValidator runat="server" ID="EmailRequiredValidator" ControlToValidate="Email" Display="None" />
                                    <insite:EmailValidator runat="server" ID="EmailPatternValidator" ControlToValidate="Email" Display="None" />
                                    <insite:CustomValidator runat="server" ID="EmailUniqueValidator" ControlToValidate="Email" Display="None" />
                                </label>
                                <insite:TextBox ID="Email" runat="server" MaxLength="128" />
                            </div>

                            <div class="form-group mb-3" runat="server" id="LanguageField">
                                <label class="form-label">
                                    Preferred Language
                                </label>
                                <insite:LanguageComboBox runat="server" ID="Language" AllowBlank="false" />
                            </div>

                            <div class="form-group mb-3" runat="server" id="TimeZoneField">
                                <label class="form-label" for="<%# TimeZone.ClientID %>">
                                    <insite:Literal runat="server" Text="Time Zone" />
                                    <insite:RequiredValidator runat="server" ID="TimeZoneRequiredValidator" ControlToValidate="TimeZone" Display="None" />
                                </label>
                                <div>
                                    <insite:TimeZoneComboBox runat="server" ID="TimeZone" />
                                </div>
                            </div>

                            <div class="form-group mb-3" runat="server" id="FirstLanguageField">
                                <div>
                                    <insite:CheckBox ID="FirstLanguage" runat="server" Text="English Language Learner" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <div>
                                    <asp:HyperLink runat="server" Text="Terms of Use Agreement"
                                        NavigateUrl="/ui/portal/identity/license?review=true" />
                                </div>
                            </div>

                        </div>
                    </div>

                    <div runat="server" id="FeatureFlagCard" class="card border-0 shadow mb-4">
                        <div class="card-body">

                            <h5 runat="server" id="FeatureFlagHeading" class="card-title"></h5>

                            <div class="text-muted fs-sm mb-3">
                                Experimental features may change, break, or inexplicably disappear at any time.
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Light/dark mode switch</label>
                                <div>
                                    <uc:ModeSwitch runat="server" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Sidebar switch</label>
                                <div>
                                    <uc:SidebarSwitch runat="server" />
                                </div>
                            </div>

                        </div>
                    </div>

                </div>

                <div class="col-md-5">

                    <div runat="server" id="DeveloperPanel" class="card border-0 shadow mb-4">
                        <div class="card-body">

                            <h5 class="card-title">
                                <insite:Literal runat="server" Text="Developer Settings" />
                            </h5>

                            <div class="form-group mb-3">
                                <div class="float-end">
                                    <insite:IconButton runat="server" ID="RegenerateSecret" ToolTip="Generate a new client secret" Name="user-secret" />
                                </div>
                                <label class="form-label">
                                    API Client Secret
                                </label>
                                <div class="px-3">
                                    <asp:Literal runat="server" ID="PersonSecretValue" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    API Endpoint (Base URL)
                                </label>
                                <div class="px-3">
                                    <table>
                                        <tr>
                                            <td class="p-1">v1</td>
                                            <td class="p-1"><asp:Literal runat="server" ID="ApiBaseUrl1" /></td>
                                        </tr>
                                        <tr>
                                            <td class="p-1">v2</td>
                                            <td class="p-1"><asp:Literal runat="server" ID="ApiBaseUrl2" /></td>
                                        </tr>
                                    </table>
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Developer Documentation
                                </label>
                                <div class="px-3">
                                    <a runat="server" id="DeveloperDocsUrl" href="#"><i class="fa-regular fa-arrow-up-right fs-sm ps-2"></i></a>
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <div class="float-end">
                                    <insite:IconButton runat="server" ID="GenerateBearer" ToolTip="Generate a new bearer token" Name="key" />
                                </div>
                                <label class="form-label">
                                    API Access Token
                                </label>
                                <div runat="server" id="ApiAccessTokenPanel" visible="false" class="mt-2">
                                    
                                    <asp:Literal runat="server" ID="ApiAccessTokenInstruction" />

                                    <div class="api-key-container">
                                        <div class="api-key-display">
                                            <span class="api-key-text" runat="server" id="ApiAccessToken"></span>
                                            <a href="javascript:void(0)" class="copy-link" onclick="copyApiKey('api-key-text', this); return false;">Copy</a>
                                        </div>
                                        <div class="copy-feedback" id="feedback">Copied to clipboard!</div>
                                    </div>

                                    <script>
                                        async function copyApiKey(elementClass, button) {
                                            
                                            const apiKeyElement = document.getElementsByClassName(elementClass)[0];
                                            const apiKey = apiKeyElement.textContent;
                                            try {
                                                // Modern clipboard API
                                                await navigator.clipboard.writeText(apiKey);
                                                showCopySuccess(button);
                                            } catch (err) {
                                                // Fallback for older browsers
                                                fallbackCopy(apiKey, button);
                                            }
                                        }

                                        function fallbackCopy(text, button) {
                                            const textArea = document.createElement('textarea');
                                            textArea.value = text;
                                            textArea.style.position = 'fixed';
                                            textArea.style.opacity = '0';
                                            document.body.appendChild(textArea);
                                            textArea.focus();
                                            textArea.select();

                                            try {
                                                document.execCommand('copy');
                                                showCopySuccess(button);
                                            } catch (err) {
                                                console.error('Failed to copy: ', err);
                                                button.textContent = 'Failed';
                                                setTimeout(() => {
                                                    button.textContent = 'Copy';
                                                }, 2000);
                                            }

                                            document.body.removeChild(textArea);
                                        }

                                        function showCopySuccess(button) {
                                            const originalText = button.textContent;

                                            // Update button
                                            button.textContent = 'Copied!';
                                            button.classList.add('copied');

                                            // Show feedback message (for full style)
                                            const feedback = document.getElementById('feedback');
                                            if (feedback) {
                                                feedback.classList.add('show');
                                            }

                                            // Reset after 2 seconds
                                            setTimeout(() => {
                                                button.textContent = originalText;
                                                button.classList.remove('copied');
                                                if (feedback) {
                                                    feedback.classList.remove('show');
                                                }
                                            }, 2000);
                                        }
                                    </script>

                                </div>
                            </div>

                        </div>
                    </div>

                    <div runat="server" id="EmploymentGroup" class="card border-0 shadow mb-4">
                        <div class="card-body">

                            <h5 class="card-title">
                                <insite:Literal runat="server" Text="Employment" />
                            </h5>

                            <div class="form-group mb-3" runat="server" id="EmployerGroupIdentifierField">
                                <label class="form-label" for="<%# EmployerGroupIdentifier.ClientID %>">
                                    <insite:Literal runat="server" Text="Employer" />
                                    <insite:RequiredValidator runat="server" ID="EmployerGroupIdentifierRequiredValidator" ControlToValidate="EmployerGroupIdentifier" Display="None" Visible="false" />
                                </label>
                                <div>
                                    <insite:FindEmployer runat="server" ID="EmployerGroupIdentifier" />
                                </div>
                            </div>

                            <div class="form-group mb-3" runat="server" id="JobTitleField">
                                <label class="form-label" for="<%# JobTitle.ClientID %>">
                                    <insite:Literal runat="server" Text="Job Title" />
                                    <insite:RequiredValidator runat="server" ID="JobTitleRequiredValidator" ControlToValidate="JobTitle" Display="None" Visible="false" />
                                </label>
                                <div>
                                    <insite:TextBox ID="JobTitle" runat="server" MaxLength="256" Width="100%" />
                                </div>
                                <div class="form-text">
                                    <insite:Literal runat="server" Text="Primary employment occupation." />
                                </div>
                            </div>

                            <div class="form-group mb-3" runat="server" id="PersonCodeField">
                                <label class="form-label" for="<%# PersonCode.ClientID %>">
                                    <insite:ContentTextLiteral runat="server" ContentLabel="Person Code"/>
                                    <insite:RequiredValidator runat="server" ID="PersonCodeRequiredValidator" ControlToValidate="PersonCode" Display="None" Visible="false" />
                                    <insite:CustomValidator runat="server" ID="PersonCodeUniqueValidator"
                                        ControlToValidate="PersonCode"
                                        Display="None"
                                        ErrorMessage="Tradeworker Number entered is assigned to another user" />
                                </label>
                                <div>
                                    <insite:TextBox ID="PersonCode" runat="server" Width="100%" MaxLength="20" />
                                </div>
                            </div>

                            <div class="form-group mb-3" runat="server" id="UnionInfoField">
                                <label class="form-label" for="<%# UnionInfo.ClientID %>">
                                    <insite:Literal runat="server" Text="Union Info" />
                                    <insite:RequiredValidator runat="server" ID="UnionInfoRequiredValidator" ControlToValidate="UnionInfo" Display="None" Visible="false" />
                                </label>
                                <div>
                                    <insite:TextBox ID="UnionInfo" runat="server" Width="100%" MaxLength="32" />
                                </div>
                                <div class="form-text">
                                    <insite:Literal runat="server" Text="Local number, if applicable." />
                                </div>
                            </div>

                        </div>
                    </div>

                    <div runat="server" id="EmergencyContactGroup" class="card border-0 shadow">
                        <div class="card-body">

                            <h5 class="card-title">
                                <insite:Literal runat="server" Text="Emergency Contact" />
                            </h5>

                            <div class="form-group mb-3" runat="server" id="EmergencyContactNameField">
                                <label class="form-label" for="<%# EmergencyContactName.ClientID %>">
                                    <insite:Literal runat="server" Text="Name" />
                                    <insite:RequiredValidator runat="server" ID="EmergencyContactNameRequiredValidator" ControlToValidate="EmergencyContactName" Display="None" Visible="false" />
                                </label>
                                <div>
                                    <insite:TextBox ID="EmergencyContactName" runat="server" MaxLength="100" />
                                </div>
                            </div>

                            <div class="form-group mb-3" runat="server" id="EmergencyContactPhoneField">
                                <label class="form-label" for="<%# EmergencyContactPhone.ClientID %>">
                                    <insite:Literal runat="server" Text="Phone" />
                                    <insite:RequiredValidator runat="server" ID="EmergencyContactPhoneRequiredValidator" ControlToValidate="EmergencyContactPhone" Display="None" Visible="false" />
                                </label>
                                <div>
                                    <insite:TextBox ID="EmergencyContactPhone" runat="server" MaxLength="32" />
                                </div>
                            </div>

                            <div class="form-group mb-3" runat="server" id="EmergencyContactRelationshipField">
                                <label class="form-label" for="<%# EmergencyContactRelationship.ClientID %>">
                                    <insite:Literal runat="server" Text="Relationship" />
                                    <insite:RequiredValidator runat="server" ID="EmergencyContactRelationshipRequiredValidator" ControlToValidate="EmergencyContactRelationship" Display="None" Visible="false" />
                                </label>
                                <div>
                                    <insite:TextBox ID="EmergencyContactRelationship" runat="server" MaxLength="50" />
                                </div>
                            </div>

                        </div>
                    </div>

                </div>

            </div>

        </insite:NavItem>

        <insite:NavItem runat="server" Icon="far fa-home" Title="My Addresses">

            <div class="mb-3">
                <insite:SaveButton runat="server" ID="SaveButton" />
                <insite:CancelButton runat="server" ID="CancelButton" />
            </div>

            <uc:AddressList runat="server" ID="AddressList" ContactType="User" ValidationGroup="Person" />

        </insite:NavItem>

    </insite:Nav>



</asp:Content>
