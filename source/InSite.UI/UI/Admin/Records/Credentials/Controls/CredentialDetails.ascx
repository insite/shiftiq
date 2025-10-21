<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CredentialDetails.ascx.cs" Inherits="InSite.Admin.Achievements.Credentials.Controls.CredentialDetails" %>

<%@ Register Src="~/UI/Admin/Contacts/People/Controls/PersonInfo.ascx" TagName="PersonDetail" TagPrefix="uc" %>	
<%@ Register Src="~/UI/Admin/Records/Achievements/Controls/AchievementDetailsV2.ascx" TagName="AchievementDetail" TagPrefix="uc" %>	

<div class="row mb-3">
    <div class="col-12">
        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <h3>Credential</h3>

                <div runat="server" id="AssignedDiv" class="form-group mb-3">
                    <label class="form-label">
                        Assigned
                    </label>
                    <div>
                        <asp:Literal runat="server" ID="Assigned" />
                    </div>
                </div>

                <div runat="server" id="GrantedDiv" class="form-group mb-3">
                    <label class="form-label">
                        Granted
                    </label>
                    <div>
                        <asp:Literal runat="server" ID="Granted" />
                    </div>
                </div>

                <div runat="server" id="RevokedDiv" class="form-group mb-3">
                    <label class="form-label">
                        Revoked
                    </label>
                    <div>
                        <asp:Literal runat="server" ID="Revoked" />
                    </div>
                </div>

                <div runat="server" id="ExpiredDiv" class="form-group mb-3">
                    <label class="form-label">
                        Expired
                    </label>
                    <div>
                        <asp:Literal runat="server" ID="Expired" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div class="row">
    <div class="col-md-6">
        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <h3>User</h3>
                <uc:PersonDetail runat="server" ID="PersonDetail" />
            </div>
        </div>
    </div>
    <div class="col-md-6">
        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <h3>Achievement</h3>
                <uc:AchievementDetail runat="server" ID="AchievementDetail" />
            </div>
        </div>
    </div>
</div>


            
