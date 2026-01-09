<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditTabSignIn.ascx.cs" Inherits="InSite.UI.Admin.Contacts.People.Controls.EditTabSignIn" %>

<div class="float-end text-end mb-3">
    <insite:Button runat="server" ID="ImpersonateAnchor" ButtonStyle="Default" Text="Impersonate User" Icon="fas fa-user-secret" />
    <insite:Button runat="server" ID="ResetPassword" ButtonStyle="Default" ToolTip="Reset Password" OnClientClick="personEditor.onResetPasswordClick(); return false;" Text="Reset Password" Icon="fas fa-sync" />
</div>

<div class="clearfix"></div>

<div class="row">
    <div class="col-lg-6">

        <div class="card mb-3">
            <div class="card-body">

                <h3>Access Control</h3>

                <div class="form-group mb-3">
                    <div>
                        <label class="form-label">
                            System Access
                        </label>
                        <div>
                            <insite:CheckBox ID="IsUserAccessGranted" runat="server" Text="Access Granted" />
                            <p class="px-5"><asp:Literal runat="server" ID="IsUserAccessGrantedDateTime" Visible="false" /></p>
                        </div>
                    </div>
                    <div>
                        <insite:CheckBox ID="IsUserAccountApprovedCmds" runat="server" Text="Account Approved (CMDS)" />
                    </div>
                </div>

                <div runat="server" id="UserAccountApprovedField" class="form-group mb-3" visible="false">
                    <label class="form-label">
                        Date Reviewed and Approved (Job Profile)
                    </label>
                    <insite:DateTimeOffsetSelector runat="server" ID="UserAccountApproved" />
                    <div class="form-text">
                        The date and time the user account was <strong>approved</strong>.
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Terms and Conditions
                    </label>
                    <div>
                        <insite:CheckBox ID="IsUserLicenseAccepted" runat="server" Text="Terms and Conditions Accepted" />
                        <p class="px-5"><asp:Literal runat="server" ID="IsUserLicenseAcceptedDateTime" Visible="false"  /></p>
                    </div>
                    <div class="form-text">
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Password Expiry
                    </label>
                    <insite:DateTimeOffsetSelector runat="server" ID="PasswordExpires" />
                    <div class="form-text">
                        After password expiry this user will be required to change their password the next time they sign in.
                    </div>
                </div>

                <div class="form-group mb-3">
                    <div class="float-end">
                        <insite:IconLink runat="server" ID="ModifyArchiveStatus" ToolTip="Modify archive status" Name="archive" />
                    </div>
                    <label class="form-label">
                        Archive Status
                    </label>
                    <div class="px-3">
                        <asp:Literal runat="server" ID="ArchiveStatus" />
                    </div>
                    <div class="form-text">
                        <asp:Literal runat="server" ID="ArchiveStatusHelp" />
                    </div>
                </div>

                <div runat="server" id="AccountTypeField" visible="false" class="form-group mb-3">
                    <label class="form-label">
                        Account Type
                    </label>
                    <div>
                        <asp:Literal runat="server" ID="AccountType" />
                    </div>
                    <div class="form-text">
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        <asp:Literal runat="server" ID="AccountCreatedLabel" />
                    </label>
                    <div class="px-3">
                        <asp:Literal runat="server" ID="AccountCreatedInfo" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <div class="float-end">
                        <insite:IconLink runat="server" ID="DeleteLink" ToolTip="Delete Contact" Name="trash-alt" />
                    </div>
                    <label class="form-label">
                        User Identifier
                    </label>
                    <div class="px-3">
                        <asp:Literal runat="server" ID="UserIdentifierOutput" />
                    </div>
                </div>

            </div>
        </div>

        <div class="card">
            <div class="card-body">

                <h3>Multi-Factor Authentication</h3>

                <div class="form-group mb-3">
                    <div class="mb-2">
                        <insite:RadioButton runat="server" ID="MfaDisabled" Text="Optional" GroupName="MultiFactorAuthentication" />
                        <div class="form-text">
                            Allow user to choose between enabling, disabling multi-factor authentication
                        </div>
                    </div>

                    <div class="mb-2">
                        <insite:RadioButton runat="server" ID="MfaEnabled" Text="Mandatory" GroupName="MultiFactorAuthentication" />
                        <div class="form-text">
                            Forces user to activate multi-factor authentication upon next successful login
                        </div>
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Current MFA Mode
                    </label>
                    <div>
                        <asp:Literal runat="server" ID="CurrentMfaMode" />
                    </div>
                    <div class="form-text">
                    </div>
                </div>

                <div runat="server" id="MfaDisableField" class="form-group mb-3">
                    <label class="form-label">
                    </label>
                    <insite:Button runat="server" ID="MfaDisableButton" Text="Disable MFA" ButtonStyle="Default" Icon="fas fa-sync" ConfirmText="Are you sure you want to disable multi-factor authentication for this user?" />
                    <div class="form-text">
                        This option will reset multi-factor authentication settings for this user, 
						to be used when user loses access to their mfa option.
                    </div>
                </div>

            </div>
        </div>

    </div>
    <div class="col-lg-6">

        <div class="card h-100">
            <div class="card-body">

                <h3>Roles</h3>

                <div class="form-group mb-3">
                    <insite:CheckBoxList ID="RoleCheckList" runat="server" />
                </div>

                <div runat="server" id="PermissionInfo" class="alert d-flex alert-info" role="alert">
				    <i class="fa-solid fa-info-square fs-xl me-2"></i>
                    <div>
					    Contact <asp:Literal runat="server" ID="SupportEmail" /> to grant admin permissions
					</div>
	            </div>

            </div>
        </div>

    </div>
</div>
