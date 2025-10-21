<%@ Page CodeBehind="Edit.aspx.cs" Inherits="InSite.Admin.Accounts.Users.Forms.Edit" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Contacts/People/Controls/MembershipGrid.ascx" TagName="MembershipGrid" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Contacts/People/Controls/UserOrganizationList.ascx" TagName="UserOrganizationList" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <link href="/UI/Admin/Accounts/Users/Forms/Edit.css" rel="stylesheet" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:Alert runat="server" ID="InfoStatus" Indicator="Information"  />
    <insite:ValidationSummary runat="server" ValidationGroup="Person" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="UserSection" Title="User" Icon="far fa-user" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">

                    <h4 class="card-title mb-3">User</h4>

                    <div class="row">
                        <div class="col-lg-6">

                            <div class="row">
                                <div class="col-md-6">

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            First Name
                                            <insite:RequiredValidator runat="server" ControlToValidate="FirstName" FieldName="First Name" ValidationGroup="Person" />
                                        </label>
                                        <insite:TextBox ID="FirstName" runat="server" MaxLength="32" Width="100%" />
                                    </div>

                                </div>
                                <div class="col-md-6">

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Last Name
                                            <insite:RequiredValidator runat="server" ControlToValidate="LastName" FieldName="Last Name" ValidationGroup="Person" />
                                        </label>
                                        <insite:TextBox ID="LastName" runat="server" MaxLength="32" Width="100%" />
                                    </div>

                                </div>
                            </div>

                            <div class="row">
                                <div class="col-lg-12">

                                    <div class="form-group mb-3">
                                        <div class="float-end">
                                            <insite:IconLink runat="server" ID="EmailCommand" CssClass="field-icon" Name="paper-plane" ToolTip="Send Email" Visible="false" />
                                        </div>
                                        <label class="form-label">
                                            Email
                                            <insite:RequiredValidator runat="server" ID="EmailRequiredValidator" ControlToValidate="Email" FieldName="Email" ValidationGroup="Person" Display="Dynamic" />
                                        </label>
                                        <div class="mb-2">
                                            <insite:TextBox ID="Email" runat="server" MaxLength="254" />
                                        </div>
                                        <div class="mb-2">
                                            <insite:CheckBox runat="server" ID="EmailDisabled" Text="Disable sending to this email" />
                                        </div>
                                        <div>
                                            <insite:Button runat="server" 
                                                ID="SendWelcomeEmailButton" 
                                                ButtonStyle="OutlinePrimary"
                                                ConfirmText="Are you sure you want to send Welcome Email?"
                                                Icon="fas fa-paper-plane"
                                                Text="Send Welcome Email" />
                                        </div>
                                    </div>

                                </div>
                            </div>

                            <div runat="server" id="PhoneField" class="form-group mb-3">
                                <label class="form-label">
                                    Phone
                                </label>
                                <insite:TextBox ID="Phone" runat="server" MaxLength="20" Width="100%" ClientEvents-OnKeyUp="phoneKeyPress(event)" />
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Time Zone
                                    <insite:RequiredValidator runat="server" ControlToValidate="TimeZone" FieldName="Time Zone" ValidationGroup="Person" Display="Dynamic" />
                                </label>
                                <insite:TimeZoneComboBox runat="server" ID="TimeZone" />
                                <div class="form-text">
                                    All time zones in Canada and United States are supported.
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <div class="float-end">
                                    <insite:IconButton runat="server" ID="ViewHistoryLink" Text="History" Name="history" />
                                </div>
                                <label class="form-label">
                                    Identifier
                                </label>
                                <div runat="server" id="UserIdentifier"></div>
                            </div>

                        </div>
                        <div class="col-lg-6">

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Status
                                </label>
                                <div>
                                    <insite:CheckBox ID="IsApproved" runat="server" Text="Access Granted" />
                                    <insite:CheckBox ID="IsLicensed" runat="server" Text="Terms Accepted" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Login Email
                                </label>
								<div class="mb-2">
                                    <div runat="server" id="LoginEmail"></div>
								</div>
                                <div>
                                    <insite:Button runat="server" 
                                        ID="ImpersonateButton"
                                        ButtonStyle="OutlinePrimary"
                                        Icon="fas fa-user-secret"
                                        Text="Impersonate" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Login Organization Code
                                </label>
                                <div>
                                    <insite:TextBox ID="LoginOrganizationCode" runat="server" MaxLength="30" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Password Expiry
                                </label>
								<div class="mb-2">
									<insite:DateTimeOffsetSelector runat="server" ID="PasswordExpires" Width="100%" />
								</div>
                                <div class="mb-2 form-text">
                                    After a user's password expires, they will be required to input a new password next time they sign in.
                                </div>
                                <div class="">
                                    <insite:Button runat="server" 
                                        ID="ResetPasswordButton" 
                                        ButtonStyle="OutlineWarning" 
                                        ConfirmText="Are you sure you want to reset this user's password?" 
                                        Icon="fas fa-sync" 
                                        Text="Reset Password" 
                                        />
                                </div>
                                
                            </div>

                        </div>
                    </div>

                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="RolesSection" Title="Roles" Icon="far fa-users" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">
                    <h4 class="card-title mb-3">Roles</h4>

                    <uc:MembershipGrid runat="server" ID="MembershipGrid" />
                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="OrganizationSection" Title="Organizations" Icon="far fa-city" IconPosition="BeforeText">
            <div class="row mt-3">
                <div class="col-lg-12">

                    <div class="card border-0 shadow-lg">
                        <div class="card-body">
                            <div class="float-end">
                                <insite:CheckBox runat="server" ID="IsAccessGrantedToCmds" Text="CMDS" />
                            </div>
                            <h4 class="card-title mb-3">Organizations</h4>

                            <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="OrganizationUpdatePanel" />

                            <insite:UpdatePanel runat="server" ID="OrganizationUpdatePanel">
                                <ContentTemplate>
                                    <uc:UserOrganizationList runat="server" ID="OrganizationList" />
                                </ContentTemplate>
                            </insite:UpdatePanel>
                        </div>
                    </div>

                </div>
            </div>
        </insite:NavItem>

    </insite:Nav>
        
    <div class="row mt-3">
        <div class="col-lg-6">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Person" />
            <insite:DeleteButton runat="server" ID="DeleteButton" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

    <insite:Modal runat="server" ID="HistoryViewerWindow" Title="Change History" Width="710px" MinHeight="520px" />

    <insite:PageFooterContent runat="server">
    <script type="text/javascript">

        function phoneKeyPress(event) {
            var input = $("#<%= Phone.ClientID %>");
            var text = input.val();
            input.val(text.replace(/[^ 0-9\-\(\)x-]/g, ''));
        }

        (function () {
            var personEditor = window.personEditor = window.personEditor || {};
            var scrollToElementId = null;

            personEditor.scrollTo = function (id) {
                scrollToElementId = id;
            };

            personEditor.onViewHistoryClick = function () {
                modalManager.load('<%= HistoryViewerWindow.ClientID %>', '/ui/admin/reports/changes/history?id=<%= UserID %>&type=user');
            };

            $(document).ready(function () {
                if (!scrollToElementId)
                    return;

                var el = document.getElementById(scrollToElementId);
                if (el == null)
                    return;

                var headerHeight = $('header.navbar:first').outerHeight();
                var scrollTo = $(el).offset().top - headerHeight;

                if (isNaN(scrollTo) || scrollTo < 0)
                    return;

                $('html, body').scrollTop(scrollTo);

            });
        })();

    </script>
    </insite:PageFooterContent>

</asp:Content>

