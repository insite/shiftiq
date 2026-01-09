<%@ Page Title="" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" AutoEventWireup="true" CodeBehind="Archive.aspx.cs" Inherits="InSite.UI.Admin.Contacts.People.Archive" %>
<%@ Register Src="Controls/PersonInfo.ascx" TagName="PersonDetail" TagPrefix="uc" %>	

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <section class="pb-5 mb-md-2">

        <asp:Panel runat="server" ID="ErrorPanel" Visible="false">	
            <div class="alert alert-danger" role="alert">
                <i class="fas fa-stop-circle"></i>
                <asp:Literal runat="server" ID="ErrorText" />	
            </div>	
        </asp:Panel>
        
        <div class="row">                       

            <div class="col-lg-6">                                   
                
                <h2 class="h4 mb-3">Person</h2>
                
                <uc:PersonDetail runat="server" ID="PersonDetail" />

                <div runat="server" id="ArchiveOptions" class="form-group mb-3">
                    <label class="form-label">
                        Archive Options
                    </label>
                    <div>
                        <insite:CheckBox runat="server" ID="DeactivateContact"       Checked="true"  Text="Deactivate Contact"        Subtext="Mark this contact as Archived, so their history and records remain in the system, but they no longer appear as an active contact." />
                        <insite:CheckBox runat="server" ID="RemoveFromGroups"        Checked="true"  Text="Remove from Groups"        Subtext="Remove this contact from all groups in which they are currently members." />
                        <insite:CheckBox runat="server" ID="DisableNotifications"    Checked="true"  Text="Disable Notifications"     Subtext="Disable this contact's email address, so they no longer receive any alerts or messages." />
                        <insite:CheckBox runat="server" ID="RevokeAccess"            Checked="true"  Text="Revoke Access"             Subtext="Revoke this contact's access to the system so they can no longer sign in." />
                        <insite:CheckBox runat="server" ID="RemoveFromOrganizations" Checked="false" Text="Remove from Organizations" Subtext="Remove this contact from all organizations they are currently connected to, and into the Archived organization. I understand that by doing so, all this contact's history and records will no longer be available." />
                    </div>
                </div>

                <div runat="server" id="ArchiveConfirm" class="alert alert-danger mt-4" role="alert">
                    <i class="fas fa-stop-circle"></i> 
                    Are you sure you want to archive this person?
                    Click the <strong>Archive</strong> button to confirm.
                </div>

                <div runat="server" id="UnarchiveConfirm" class="alert alert-success mt-4" role="alert">
                    <i class="fas fa-stop-circle"></i> 
                    Are you sure you want to unarchive this person?
                    Click the <strong>Unarchive</strong> button to confirm.
                </div>

                <div>
                    <insite:ArchiveButton runat="server" ID="ArchiveButton" />
                    <insite:UnarchiveButton runat="server" ID="UnarchiveButton" />
                    <insite:CancelButton runat="server" ID="CancelButton" />	
                </div>
                
            </div>

            <div class="col-lg-6">

                
            </div>
            
        </div>

    </section>
    
    
</asp:Content>
