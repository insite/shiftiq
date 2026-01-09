<%@ Page Language="C#" CodeBehind="Settings.aspx.cs" Inherits="InSite.UI.Portal.Home.Settings" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="./Controls/DashboardNavigation.ascx" TagName="DashboardNavigation" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">

    <uc:DashboardNavigation runat="server" ID="DashboardNavigation" />

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    
    <insite:Alert runat="server" ID="StatusAlert" />

    <insite:Toast runat="server" ID="ProfileToast" Visible="false" />

    <div class="accordion-item" style="border:none;">
        <div class="bg-secondary rounded-3 p-4 mb-4">
        
            <div class="d-block d-sm-flex align-items-center">
                <asp:Image runat="server" ID="ProfileImage" CssClass="d-block rounded-circle mx-sm-0 mx-auto mb-3 mb-sm-0" Style="max-height:100px;max-width:100px;" />
                <div class="ps-sm-3 text-center text-sm-start">
                    <button class="btn btn-outline-secondary shadow btn-sm mb-2" type="button" data-bs-toggle="collapse" data-bs-target='<%= "#" + ProfileUploadPanel.ClientID %>' aria-expanded="false" aria-controls='<%= ProfileUploadPanel.ClientID %>'>
                        <i class="far fa-arrows-rotate me-2"></i>Change photo</button>
                    <div class="p mb-0 fs-ms text-body-secondary">Upload JPG or PNG image. Recommended image size is <%= InSite.Web.Helpers.ProfilePictureHelper.MaxProfileImageSize %>x<%= InSite.Web.Helpers.ProfilePictureHelper.MaxProfileImageSize %>px.</div>
                </div>
            </div>
            <div class="accordion-collapse collapse" runat="server" id="ProfileUploadPanel" aria-labelledby='item-header-1234' data-bs-parent="#orders-accordion" enableviewstate="false">
                <div class="accordion-body pt-4 bg-secondary rounded-top-0 rounded-3" style="padding-bottom: 0;">             
                    <div class="pb-4">
                        <insite:FileUploadV2 runat="server" ID="ProfilePictureToUploadV2" LabelText="Select profile picture to upload" AllowedExtensions=".jpg,.jpeg,.gif,.png" FileUploadType="Image" />
                        <div class="pt-2">
                            <insite:SaveButton runat="server" ID="UploadProfilePicture" />
                            <insite:CancelButton runat="server" ID="CancelProfilePicture" ButtonStyle="OutlineSecondary" OnClientClick="return myAccount.onCancelProfilePicture();" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-sm-6">
            <div class="mb-3 pb-1">
                <label class="form-label" for="<%# FirstName.ClientID %>">
                    <insite:Literal runat="server" Text="First Name" />
                    <insite:RequiredValidator runat="server" ID="FirstNameRequiredValidator" ControlToValidate="FirstName" Display="None" ValidationGroup="Change" />
                </label>
                <div>
                    <insite:TextBox ID="FirstName" runat="server" MaxLength="32" Width="100%" />
                </div>
            </div>
        </div>
        <div class="col-sm-6">
            <div class="mb-3 pb-1">
                <label class="form-label" for="<%# LastName.ClientID %>">
                    <insite:Literal runat="server" Text="Last Name" />
                    <insite:RequiredValidator runat="server" ID="LastNameRequiredValidator" ControlToValidate="LastName" Display="None" ValidationGroup="Change" />
                </label>
                <div>
                    <insite:TextBox ID="LastName" runat="server" MaxLength="32" Width="100%" />
                </div>
            </div>
        </div>
        <div class="col-sm-6">
            <div class="mb-3 pb-1">
                <label class="form-label" for="<%# Email.ClientID %>">
                    <insite:Literal runat="server" Text="Email" />
                    <insite:RequiredValidator runat="server" ID="EmailRequiredValidator" ControlToValidate="Email" Display="None" ValidationGroup="Change" />
                    <insite:EmailValidator runat="server" ID="EmailPatternValidator" ControlToValidate="Email" Display="None" ValidationGroup="Change" />
                </label>
                <div>
                    <div>
                        <insite:TextBox ID="Email" runat="server" MaxLength="128" />
                    </div>
                </div>
                <div class="form-text">
                    <insite:Literal runat="server" Text="Preferred email address." />
                </div>
            </div>
        </div>
        <div class="col-sm-6">
            <div class="mb-3 pb-1">
                <label class="form-label" for="<%# PhoneMobile.ClientID %>">
                    <insite:Literal runat="server" Text="Mobile" />
                    <insite:RequiredValidator runat="server" ID="PhoneMobileRequiredValidator" ControlToValidate="PhoneMobile" Display="None" Visible="false" ValidationGroup="Change" />
                </label>
                <div>
                    <insite:TextBox ID="PhoneMobile" runat="server" MaxLength="32" Width="100%" />
                </div>
            </div>
        </div>
        <div class="col-sm-6">
            <div class="mb-3 pb-1">
                <label for="<%# Street1.ClientID %>" class="form-label">
                    <insite:Literal runat="server" Text="Home Address" />
                </label>
                <div>
                    <insite:TextBox ID="Street1" runat="server" MaxLength="128" />
                </div>
            </div>
        </div>
        <div class="col-sm-6">
            <div class="mb-3 pb-1">
                <label for="<%# City.ClientID %>" class="form-label">
                    <insite:Literal runat="server" Text="City" />
                </label>
                <div>
                    <insite:TextBox ID="City" runat="server" MaxLength="128" />
                </div>
            </div>
        </div>
        <div class="col-sm-6">
            <div class="mb-3 pb-1">
                <label class="form-label">
                    <insite:Literal runat="server" Text="Country" />
                    <insite:RequiredValidator runat="server" Display="None" ControlToValidate="Country" ValidationGroup="Change" />
                </label>
                <insite:CountryComboBox runat="server" ID="Country" />
            </div>
        </div>
        <div class="col-sm-6">
            <div class="mb-3 pb-1">
                <label for="<%# PostalCode.ClientID %>" class="form-label">
                    <insite:Literal runat="server" ID="PostalCodeLabel" Text="Postal Code" />
                </label>
                <div>
                    <insite:TextBox ID="PostalCode" runat="server" MaxLength="128" />
                </div>
            </div>
        </div>
        <div class="col-12">
            <hr class="mt-2 mb-4">
            <div class="d-flex flex-wrap justify-content-between align-items-center">
                <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Change" />
                <insite:Button runat="server" ID="ChangePasswordButton" Text="Change Password" ButtonStyle="Default" Icon="fas fa-sync" CausesValidation="false" CssClass="mb-3 float-end" NavigateUrl="/ui/portal/identity/password"/>
            </div>
        </div>
    </div>
    
    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            (function () {
                var instance = window.myAccount = window.myAccount || {};

                myAccount.onCancelProfilePicture = function () {
                    var panel = document.getElementById('<%= ProfileUploadPanel.ClientID %>');
                    var instance = bootstrap.Collapse.getOrCreateInstance(panel);

                    instance.hide();

                    return false;
                };
            })();
        </script>
    </insite:PageFooterContent>

</asp:Content>
