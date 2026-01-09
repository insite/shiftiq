<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.Admin.Achievements.Achievements.Forms.Outline" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Contacts/Groups/Controls/DepartmentChecklist.ascx" TagName="DepartmentChecklist" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Records/Achievements/Controls/CredentialGrid.ascx" TagName="CredentialGrid" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">

    <insite:Alert runat="server" ID="StatusAlert" />

	<div class="row mb-3">
        <div class="col-lg-12">
            <insite:Button runat="server" ID="NewAchievementLink" Text="New" Icon="fas fa-file" ButtonStyle="Default" NavigateUrl="/ui/admin/records/achievements/define" />

            <insite:ButtonSpacer runat="server" />

            <insite:Button runat="server" ID="LockButton" Text="Lock"  Icon="fas fa-lock" ConfirmText="Are you sure to lock the achievement?" ButtonStyle="Default" />
            <insite:Button runat="server" ID="UnlockButton" Text="Unlock" Icon="fas fa-lock-open" ConfirmText="Are you sure to unlock the achievement?" ButtonStyle="Default" />

            <insite:ButtonSpacer runat="server" />

            <insite:Button runat="server" ID="ViewHistoryLink" Text="History" Icon="fas fa-history" ButtonStyle="Default" />
            <insite:DeleteButton runat="server" ID="DeleteLink" />
        </div>
    </div>

    <insite:Nav runat="server">               
	    <insite:NavItem runat="server" Title="Learners" Icon="far fa-users" IconPosition="BeforeText">

	        <section class="pb-5 mb-md-2">        
		        <div class="row">
			        <div class="col-lg-12">
				        <div class="card border-0 shadow-lg">
					        <div class="card-body">
						        <uc:CredentialGrid runat="server" ID="CredentialGrid" />

                                <div class="text-muted fs-sm">
                                    <i class="far fa-lightbulb-on me-1"></i>
                                    A specific achievement assigned to a specific learner is also known as a credential.
                                </div>

					        </div>
				        </div>
			        </div>
		        </div>
	        </section>

	    </insite:NavItem>

        <insite:NavItem runat="server" ID="DepartmentSection" Title="Departments" Icon="far fa-building" IconPosition="BeforeText">
            <section>
                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <uc:DepartmentChecklist runat="server" ID="DepartmentChecklist" />
                    </div>
                </div>
            </section>
        </insite:NavItem>

	    <insite:NavItem runat="server" ID="AchievementSetupTab" Title="Achievement Settings" Icon="far fa-trophy" IconPosition="BeforeText">

	        <section class="pb-5 mb-md-2">        
		        <div class="row">
			        <div class="col-lg-12">
				        <div class="card border-0 shadow-lg">
					        <div class="card-body">

                                <div class="row">
                        
                                    <div class="col-md-6">
                                    
                                        <div class="settings">

                                            <div class="form-group mb-3">
                                                <div class="float-end">
                                                    <insite:IconLink runat="server" id="DescribeTitle" ToolTip="Rename title" Name="pencil" />
                                                </div>
                                                <asp:Label runat="server" ID="AchievementTitleLabel" AssociatedControlID="AchievementTitle" Text="Title" CssClass="form-label" />
                                                <div>
                                                    <asp:Literal runat="server" ID="AchievementTitle" />
                                                </div>
                                                <div class="form-text">User-friendly name or short-form description for this achievement or microcredential.</div>
                                            </div>

                                            <div class="form-group mb-3">
                                                <div class="float-end">
                                                    <insite:IconLink runat="server" id="DescribeLabel" ToolTip="Change tag" Name="pencil" />
                                                </div>
                                                <asp:Label runat="server" ID="AchievementLabelLabel" AssociatedControlID="AchievementLabel" Text="Tag" CssClass="form-label" />
                                                <div>
                                                    <asp:Literal runat="server" ID="AchievementLabel" />
                                                </div>
                                                <div class="form-text">Tags are used to to classify/categorize achievements.</div>
                                            </div>
            
                                            <div runat="server" id="BadgePanel" class="form-group mb-3" visible="false">
                                                <div class="float-end">
                                                    <insite:IconLink runat="server" id="DescribeBadge" ToolTip="Change badge" Name="pencil" />
                                                </div>
                                                <asp:Label runat="server" ID="AchievementBadgeLabel" AssociatedControlID="AchievementBadge" Text="Badge Image" CssClass="form-label" />
                                                <div>
                                                    <asp:Literal runat="server" ID="AchievementBadge" />
                                                </div>
                                                <div class="form-text"></div>
                                            </div>

                                            <div class="form-group mb-3">
                                                <div class="float-end">
                                                    <insite:IconLink runat="server" id="DescribeDescription" ToolTip="Change description" Name="pencil" />
                                                </div>
                                                <asp:Label runat="server" ID="AchievementDescriptionLabel" AssociatedControlID="AchievementDescription" Text="Description" CssClass="form-label" />
                                                <div>
                                                    <span style="white-space:pre-wrap;"><asp:Literal runat="server" ID="AchievementDescription" /></span>
                                                </div>
                                                <div class="form-text">A long-form, detailed description of the achievement.</div>
                                            </div>  
                        
                                            <div class="form-group mb-3">
                                                <asp:Label runat="server" ID="AchievementIdentifierLabel" AssociatedControlID="AchievementIdentifier" Text="Identifier" CssClass="form-label" />
                                                <div>
                                                    <asp:Literal runat="server" ID="AchievementIdentifier" />
                                                </div>
                                                <div class="form-text">A globally unique identifier for this achievement.</div>
                                            </div>

                                            <div class="form-group mb-3">
                                                <div class="float-end">
                                                    <insite:IconLink runat="server" id="AchievementIsReportedLink" ToolTip="Change Reporting Status" Name="pencil" />
                                                </div>
                                                <label class="form-label">Reporting Status</label>
                                                <div>
                                                    <asp:Literal runat="server" ID="AchievementIsReported" />
                                                </div>
                                            </div> 

                                            <div runat="server" id="AchievementTypeField" class="form-group mb-3">
                                                <div class="float-end">
                                                    <insite:IconLink runat="server" id="ChangeAchievementType" ToolTip="Change Achievement Type" Name="pencil" />
                                                </div>
                                                <asp:Label runat="server" Text="Achievement Type" CssClass="form-label" />
                                                <div>
                                                    <asp:Literal runat="server" ID="AchievementType" />
                                                </div>
                                            </div>

                                            <div class="form-group mb-3">
                                                <div class="float-end">
                                                    <insite:IconLink runat="server" ID="AchievementAllowSelfDeclaredLink" ToolTip="Allow or disallow self-declaration" Name="pencil" />
                                                </div>
                                                <asp:Label runat="server" Text="Self-Declaration" CssClass="form-label" />
                                                <div>
                                                    <asp:Literal runat="server" ID="AchievementAllowSelfDeclared" />
                                                </div>
                                            </div>

                                        </div>

                                    </div>

                                    <div class="col-md-6">
                                    
                                        <div class="settings">

                                            <div class="form-group mb-3">
                                                <div class="float-end">
                                                    <insite:IconLink runat="server" id="ChangeExpiration" ToolTip="Change expiry setup" Name="pencil" />
                                                </div>
                                                <asp:Label runat="server" ID="AchievementExpiryLabel" AssociatedControlID="AchievementExpiry" Text="Expiry" CssClass="form-label" />
                                                <div>
                                                    <asp:Literal runat="server" ID="AchievementExpiry" />
                                                </div>
                                                <div class="form-text">If the achievement expires, then the expiry date can be a one-time fixed date, or a recurring date that is relative to the date when each individual certificate is issued.</div>
                                            </div>
                        
                                            <div class="form-group mb-3">
                                                <div class="float-end">
                                                    <insite:IconLink runat="server" id="ChangeCertificateLayout" ToolTip="Change certificate layout" Name="pencil" />
                                                </div>
                                                <asp:Label runat="server" ID="CertificateLayoutLabel" AssociatedControlID="CertificateLayout" Text="Certificate Layout" CssClass="form-label" />
                                                <div>
                                                    <asp:Literal runat="server" ID="CertificateLayout" />
                                                </div>
                                                <div class="form-text">What layout should be used when a certificate for this achievement is granted and then printed?</div>
                                            </div>

                                            <div class="form-group mb-3">
                                                <asp:Label runat="server" ID="AchievementStatusLabel" AssociatedControlID="AchievementStatus" Text="Current Status" CssClass="form-label" />
                                                <div>
                                                    <asp:Literal runat="server" ID="AchievementStatus" />
                                                </div>
                                                <div class="form-text">Changes to a locked achievement are not permitted.</div>
                                            </div>

                                        </div>
                                    </div>
                                </div>
					        </div>
				        </div>
			        </div>
		        </div>
	        </section>

	    </insite:NavItem>
    </insite:Nav>

</div>

<insite:PageHeadContent runat="server">
    <style type="text/css">
        .achievement-prerequisite {
            padding:10px;
        }
        .achievement-prerequisite .text-success { margin-bottom:5px; }
    </style>
</insite:PageHeadContent>
</asp:Content>
