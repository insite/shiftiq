<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailOrganization.ascx.cs" Inherits="InSite.UI.Admin.Accounts.Organizations.Controls.DetailOrganization" %>

<div class="row">
    <div class="col-md-6">
                                    
        <h3>Identification</h3>

        <div class="form-group mb-3">
            <label class="form-label">
                Parent Organization
                <insite:IconLink runat="server" ID="ParentLink" Name="pencil" ToolTip="Edit " />
            </label>
            <insite:FindOrganization runat="server" ID="ParentOrganizationIdentifier" />
            <div class="form-text">You can select another organization as the container for this organization. This is optional, and it allows you to nest organizations into suborganizations. </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Organization Code
                <insite:RequiredValidator runat="server" ControlToValidate="OrganizationCode" FieldName="Organization Code" ValidationGroup="Organization" />
                <insite:PatternValidator runat="server" ControlToValidate="OrganizationCode" ValidationGroup="Organization" ErrorMessage="Organization code supports only alphanumeric characters" ValidationExpression="^[A-Za-z0-9-]+$" />
                <asp:CustomValidator runat="server" ID="OrganizationCodeValidator" ControlToValidate="OrganizationCode" ValidationGroup="Organization" Display="None" />
            </label>
            <insite:TextBox runat="server" ID="OrganizationCode" MaxLength="30" />
            <div class="form-text">
                An account code is an alphanumeric code that uniquely identifies this organization.
                This is used as the sub-domain in the application URL for all organization account activity.
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Organization Name
                <insite:RequiredValidator runat="server" ControlToValidate="CompanyName" FieldName="Organization Name" ValidationGroup="Organization" />
            </label>
            <insite:TextBox runat="server" ID="CompanyName" MaxLength="50" />
            <div class="form-text">The name of the client organization that owns this organization account.</div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Organization Domain
                <insite:RequiredValidator runat="server" ControlToValidate="CompanyDomain" FieldName="Organization Domain" ValidationGroup="Organization" />
                <insite:PatternValidator runat="server" ID="DomainPatternValidator" ControlToValidate="CompanyDomain" ValidationGroup="Organization" ErrorMessage="Incorrect domain format" />
            </label>
            <insite:TextBox runat="server" ID="CompanyDomain" MaxLength="50" />
            <div class="form-text">The Internet domain name registered by the organization for its corporate web site and email.</div>
        </div>

        <insite:Container runat="server" ID="SetupContainer">
            <h3>Setup</h3>

            <div class="form-group mb-3">
                <div class="float-end" >
                    <insite:IconButton runat="server" Name="folder-open" ID="ReopenOrganizationButton" style="padding:8px" title="Reopen organization" OnClientClick="return confirm('Are you sure to open this organization account?')" />
                    <insite:IconButton runat="server" Name="folder" style="padding:8px" id="CloseOrganizationLink" title="Close organization" OnClientClick="return confirm('Are you sure to close this organization account?')" />
                </div>
                <label class="form-label">
                    Account Status
                </label>
                <asp:Label runat="server" ID="AccountStatus" />
                <div class="form-text">
                    <asp:Label runat="server" ID="AccountStatusHelp" />
                </div>
            </div>

            <div class="form-group mb-3">
                <label class="form-label">
                    Organization Announcement
                </label>
                <insite:TextBox runat="server" ID="OrganizationAnnouncement" TextMode="MultiLine" rows="2" />
                <div class="form-text">Any text you input here is displayed on the Admin home page for every user in this organization's account.</div>
            </div>
        </insite:Container>
    </div>
                                
    <div class="col-md-6">
        <h3>Description</h3>

        <div class="form-group mb-3">
            <label class="form-label">
                Organization Administrator
            </label>
            <insite:FindPerson runat="server" ID="CompanyAdministratorIdentifier" />
            <div class="form-text">The name of the primary administrative contact person at the company.</div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Organization Size
            </label>
            <insite:ComboBox runat="server" ID="CompanySize">
                <Items>
                    <insite:ComboBoxOption Value="Small" Text="Small" />
                    <insite:ComboBoxOption Value="Medium" Text="Medium" />
                    <insite:ComboBoxOption Value="Large" Text="Large" />
                </Items>
            </insite:ComboBox>
            <div class="form-text">The size of the organization.</div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Full Legal Name
                <insite:RequiredValidator runat="server" ControlToValidate="CompanyTitle" FieldName="Full Legal Name" ValidationGroup="Organization" />
            </label>
            <insite:TextBox runat="server" ID="CompanyTitle" MaxLength="100" />
            <div class="form-text">
                The organization's full legal name for contract purposes.
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Organization Description
            </label>
            <insite:TextBox runat="server" ID="Description" TextMode="MultiLine" Rows="3" />
            <div class="form-text">A summary description of the company.</div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Organization Web Site URL
            </label>
            <insite:TextBox runat="server" ID="CompanyWebSiteUrl" MaxLength="100" />
        </div>

        <h4 class="card-title mb-3">Localization</h4>

        <div class="form-group mb-3">
            <label class="form-label">
                Primary Language
            </label>
            <div>
                English
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Secondary Languages
            </label>
            <insite:LanguageMultiComboBox runat="server" ID="Languages" Multiple-Format="Values">
                <Settings IncludeLanguage="" ExcludeLanguage="en" />
            </insite:LanguageMultiComboBox>
            <div class="form-text">The languages supported for this organization account.</div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Time Zone
                <insite:RequiredValidator runat="server" ControlToValidate="TimeZone" FieldName="Time Zone" ValidationGroup="Organization" />
            </label>
            <insite:TimeZoneComboBox runat="server" ID="TimeZone" AllowBlank="false" />
            <div class="form-text">The primary time zone in which the organization operates.</div>
        </div>

    </div>
</div>
