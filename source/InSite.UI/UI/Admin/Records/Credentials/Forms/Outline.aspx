<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.Admin.Achievements.Credentials.Forms.Outline" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="StatusAlert" />

	<section class="pb-5 mb-md-2">        
	    <div class="row mb-3">
            <div class="col-lg-12">
                <insite:Button runat="server" ID="ViewHistoryLink" Text="History" Icon="fas fa-history" ButtonStyle="Default" />
                <insite:DeleteButton runat="server" ID="DeleteLink" />
                <insite:DownloadButton runat="server" ID="DownloadLink" Visible="false" ButtonStyle="Primary"/>
            </div>
        </div>

        <div class="row mb-3">
            <div class="col-md-4">

				<div class="card border-0 shadow-lg h-100">
					<div class="card-body">

                        <h3>Achievement</h3>

                        <div class="form-group mb-3">
                            <asp:Label runat="server" ID="AchievementTitleLabel" AssociatedControlID="AchievementTitle" Text="Title" CssClass="form-label" />
                            <div>
                                <a runat="server" ID="Achievementlink">
                                    <asp:Literal runat="server" ID="AchievementTitle" />
                                </a>
                            </div>
                            <div class="form-text">The descriptive title for this achievement.</div>
                        </div>

                        <div class="form-group mb-3">
                            <asp:Label runat="server" ID="AchievementLabelLabel" AssociatedControlID="AchievementLabel" Text="Tag" CssClass="form-label" />
                            <div>
                                <asp:Literal runat="server" ID="AchievementLabel" />
                            </div>
                            <div class="form-text">The descriptive tag for this achievement.</div>
                        </div>
            
                        <div class="form-group mb-3">
                            <asp:Label runat="server" ID="AchievementDescriptionLabel" AssociatedControlID="AchievementDescription" Text="Description" CssClass="form-label" />
                            <div>
                                <span style="white-space:pre-wrap;"><asp:Literal runat="server" ID="AchievementDescription" /></span>
                            </div>
                            <div class="form-text">The description for this achievement.</div>
                        </div>  
                        
                        <div class="form-group mb-3">
                            <asp:Label runat="server" ID="AchievementExpiryLabel" AssociatedControlID="AchievementExpiry" Text="Expiry" CssClass="form-label" />
                            <div>
                                <asp:Literal runat="server" ID="AchievementExpiry" />
                            </div>
                            <div class="form-text">The expiration for this achievement.</div>
                        </div>

                        <div class="form-group mb-3">
                            <asp:Label runat="server" ID="AchievementStatusLabel" AssociatedControlID="AchievementStatus" Text="Status" CssClass="form-label" />
                            <div>
                                <asp:Literal runat="server" ID="AchievementStatus" />
                            </div>
                            <div class="form-text">The locked/unlcoked status for this achievement.</div>
                        </div>

					</div>
				</div>

            </div>

            <div class="col-md-4">
				<div class="card border-0 shadow-lg h-100">
					<div class="card-body">

                        <h3>Authority</h3>

                        <div class="form-group mb-3">
                            <asp:Label runat="server" ID="AuthorityNameLabel" AssociatedControlID="AuthorityName" Text="Name" CssClass="form-label" />
                            <div>
                                <asp:Literal runat="server" ID="AuthorityName" />
                            </div>
                            <div class="form-text">The name for this authority.</div>
                        </div>

                        <div class="form-group mb-3">
                            <asp:Label runat="server" ID="AuthorityTypeLabel" AssociatedControlID="AuthorityType" Text="Type" CssClass="form-label" />
                            <div>
                                <asp:Literal runat="server" ID="AuthorityType" />
                            </div>
                            <div class="form-text">The type for this authority.</div>
                        </div>

                        <div class="form-group mb-3">
                            <asp:Label runat="server" ID="AuthorityLocationLabel" AssociatedControlID="AuthorityLocation" Text="Location" CssClass="form-label" />
                            <div>
                                <asp:Literal runat="server" ID="AuthorityLocation" />
                            </div>
                            <div class="form-text">The location for this authority.</div>
                        </div>

                        <div class="form-group mb-3">
                            <asp:Label runat="server" ID="AuthorityReferenceLabel" AssociatedControlID="AuthorityReference" Text="Reference" CssClass="form-label" />
                            <div>
                                <asp:Literal runat="server" ID="AuthorityReference" />
                            </div>
                            <div class="form-text">The reference for this authority.</div>
                        </div>

                    </div>
                </div>
            </div>

            <div class="col-md-4">
				<div class="card border-0 shadow-lg h-100">
					<div class="card-body">

                        <h3>User</h3>

                        <div class="form-group mb-3">
                            <asp:Label runat="server" ID="UserNameLabel" AssociatedControlID="UserName" Text="User Name" CssClass="form-label" />
                            <div>
                                <a runat="server" ID="UserLink">
                                    <asp:Literal runat="server" ID="UserName" />
                                </a>
                            </div>
                            <div class="form-text">The credential user full name.</div>
                        </div>

                        <div class="form-group mb-3">
                            <asp:Label runat="server" ID="UserEmailLabel" AssociatedControlID="UserEmail" Text="User Email" CssClass="form-label" />
                            <div>
                                <asp:Literal runat="server" ID="UserEmail" />
                                <asp:Literal runat="server" ID="UserEmailStatus" />
                            </div>
                            <div class="form-text">The credential user email.</div>
                        </div>

                        <div class="form-group mb-3">
                            <asp:Label runat="server" ID="UserRegionLabel" AssociatedControlID="UserRegion" Text="User Region" CssClass="form-label" />
                            <div>
                                <asp:Literal runat="server" ID="UserRegion" />
                            </div>
                            <div class="form-text">The credential user region.</div>
                        </div>

                        <div class="form-group mb-3">
                            <asp:Label runat="server" ID="UserArchivedLabel" AssociatedControlID="UserArchived" Text="User Archived" CssClass="form-label" />
                            <div>
                                <asp:Literal runat="server" ID="UserArchived" />
                            </div>
                            <div class="form-text">The credential user has been archived.</div>
                        </div>

                        <div class="form-group mb-3" runat="server" id="UserAccessField">
                            <asp:Label runat="server" ID="UserAccessLabel" AssociatedControlID="UserAccess" Text="User Access" CssClass="form-label" />
                            <div>
                                <asp:Literal runat="server" ID="UserAccess" />
                            </div>
                            <div class="form-text">The user access for this credential.</div>
                        </div>

                        <div class="form-group mb-3">
                            <asp:Label runat="server" ID="EmployerNameLabel" AssociatedControlID="EmployerName" Text="Employer Name" CssClass="form-label" />
                            <div>
                                <asp:Literal runat="server" ID="EmployerName" />
                            </div>
                            <div class="form-text">The employer name for this credential.</div>
                        </div>

                        <div class="form-group mb-3" runat="server" id="OrganizationAccessField">
                            <asp:Label runat="server" ID="OrganizationAccessLabel" AssociatedControlID="OrganizationAccess" Text="Organization Access" CssClass="form-label" />
                            <div>
                                <asp:Literal runat="server" ID="OrganizationAccess" />
                            </div>
                            <div class="form-text">The organization access for this credential.</div>
                        </div>

                    </div>

                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-md-12">

				<div class="card border-0 shadow-lg h-100">
					<div class="card-body">

                        <h3>Credential</h3>

                        <div class="row">
                            <div class="col-md-4">

                                <div class="form-group mb-3">
                                    <asp:Label runat="server" ID="CredentialStatusLabel" AssociatedControlID="CredentialStatus" Text="Status" CssClass="form-label" />
                                    <div>
                                        <asp:Literal runat="server" ID="CredentialStatus" />
                                    </div>
                                    <div class="form-text">The credential status.</div>
                                </div>

                                <div class="form-group mb-3">
                                    <asp:Label runat="server" ID="CredentialDescriptionLabel" AssociatedControlID="CredentialDescription" Text="Description" CssClass="form-label" />
                                    <div>
                                        <asp:Literal runat="server" ID="CredentialDescription" />
                                    </div>
                                    <div class="form-text">The credential description.</div>
                                </div>  

                                <div class="form-group mb-3">
                                    <asp:Label runat="server" ID="CredentialHoursLabel" AssociatedControlID="CredentialHours" Text="Hours" CssClass="form-label" />
                                    <div>
                                        <asp:Literal runat="server" ID="CredentialHours" />
                                    </div>
                                    <div class="form-text">The credential hours.</div>
                                </div>
                        
                                <div class="form-group mb-3">
                                    <asp:Label runat="server" ID="CredentialIdentifierLabel" AssociatedControlID="CredentialIdentifier" Text="Identifier" CssClass="form-label" />
                                    <div>
                                        <asp:Literal runat="server" ID="CredentialIdentifier" />
                                    </div>
                                    <div class="form-text">A globally unique identifier for this credential.</div>
                                </div>

                            </div>

                            <div class="col-md-4">

                                <div class="form-group mb-3">
                                    <div class="float-end">
                                        <insite:IconLink runat="server" id="CredentialExpiryLink" style="padding:8px" ToolTip="Configure credential" Name="pencil" />
                                    </div>
                                    <asp:Label runat="server" ID="CredentialExpiryLabel" AssociatedControlID="CredentialExpiry" Text="Expiry" CssClass="form-label" />
                                    <div>
                                        <asp:Literal runat="server" ID="CredentialExpiry" />
                                    </div>
                                    <div class="form-text">If credential expires, select whether this is a fixed date or relative to when issued.</div>
                                </div>

                                <div class="form-group mb-3">
                                    <div class="float-end">
                                        <insite:IconLink runat="server" id="CredentialNecessityLink" style="padding:8px" ToolTip="Configure credential" Name="pencil" />
                                    </div>
                                    <asp:Label runat="server" ID="CredentialNecessityLabel" AssociatedControlID="CredentialNecessity" Text="Necessity" CssClass="form-label" />
                                    <div>
                                        <asp:Literal runat="server" ID="CredentialNecessity" />
                                    </div>
                                    <div class="form-text">The credential necessity.</div>
                                </div>  

                                <div class="form-group mb-3">
                                    <div class="float-end">
                                        <insite:IconLink runat="server" id="CredentialPriorityLink" style="padding:8px" ToolTip="Configure credential" Name="pencil" />
                                    </div>
                                    <asp:Label runat="server" ID="CredentialPriorityLabel" AssociatedControlID="CredentialPriority" Text="Priority" CssClass="form-label" />
                                    <div>
                                        <asp:Literal runat="server" ID="CredentialPriority" />
                                    </div>
                                    <div class="form-text">The credential priority.</div>
                                </div>  

                            </div>

                            <div class="col-md-4">

                                <div class="form-group mb-3">
                                    <asp:Label runat="server" ID="CredentialAssignedLabel" AssociatedControlID="CredentialAssigned" Text="Assigned" CssClass="form-label" />
                                    <div>
                                        <asp:Literal runat="server" ID="CredentialAssigned" />
                                    </div>
                                    <div class="form-text">The credential has been assigned.</div>
                                </div>

                                <div class="form-group mb-3">
                                    <div class="float-end">
                                        <insite:IconLink runat="server" id="GrantCredential" style="padding:8px" ToolTip="Grant credential" Name="pencil" />
                                    </div>
                                    <asp:Label runat="server" ID="CredentialGrantedLabel" AssociatedControlID="CredentialGranted" Text="Granted" CssClass="form-label" />
                                    <div>
                                        <asp:Literal runat="server" ID="CredentialGranted" />
                                    </div>
                                    <div class="form-text">The credential has been granted.</div>
                                </div>

                                <div class="form-group mb-3">
                                    <div class="float-end">
                                        <insite:IconLink runat="server" id="RevokeCredential" style="padding:8px" ToolTip="Revoke credential" Name="pencil" />
                                    </div>
                                    <asp:Label runat="server" ID="CredentialRevokedLabel" AssociatedControlID="CredentialRevoked" Text="Revoked" CssClass="form-label" />
                                    <div>
                                        <asp:Literal runat="server" ID="CredentialRevoked" />
                                    </div>
                                    <div class="form-text">The credential has been revoked.</div>
                                </div>

                                <div class="form-group mb-3" runat="server" id="RevokedReasonField">
                                    <asp:Label runat="server" ID="CredentialRevokedReasonLabel" AssociatedControlID="CredentialRevokedReason" Text="Revoked Reason" CssClass="form-label" />
                                    <div>
                                        <span style="white-space:pre-wrap;"><asp:Literal runat="server" ID="CredentialRevokedReason" /></span>
                                    </div>
                                    <div class="form-text">The reason why credential has been revoked.</div>
                                </div>

                                <div class="form-group mb-3">
                                    <asp:Label runat="server" ID="CredentialExpiredLabel" AssociatedControlID="CredentialExpired" Text="Expired" CssClass="form-label" />
                                    <div>
                                        <asp:Literal runat="server" ID="CredentialExpired" />
                                    </div>
                                    <div class="form-text">The credential has been expired.</div>
                                </div>
                            </div>

                        </div>

                    </div>
                </div>
            </div>
        </div>

	</section>

</asp:Content>
