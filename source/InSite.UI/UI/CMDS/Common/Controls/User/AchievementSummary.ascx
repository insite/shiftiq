<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AchievementSummary.ascx.cs" Inherits="InSite.Cmds.Controls.Training.EmployeeAchievements.AchievementSummary" %>
<%@ Register Src="AttachmentList.ascx" TagName="AttachmentList" TagPrefix="uc" %>

<div class="card border-0 shadow-lg mb-3">
    <div class="card-body">
    
<div id="CoursePanel" runat="server">
    <h3><asp:Literal runat="server" ID="CourseType" /></h3>
    <div class="form-group mb-3">
        <label class="form-label">
            <asp:HyperLink runat="server" ID="CourseLink" />
        </label>
        <div class="mb-3">
            <asp:Literal runat="server" ID="CourseDescription" />
        </div>
        <div class="mb-3">
            <asp:HyperLink runat="server" ID="CourseLaunch" CssClass="btn btn-sm btn-primary"><i class="fa-solid fa-rocket me-2"></i>Launch</asp:HyperLink>
        </div>
    </div>
</div>

<asp:PlaceHolder ID="SignOffPanel" runat="server">
    <p>
        By clicking the "Sign Off" button I am confirming I have read the document, fully understand the content, and accept any requirements.
    </p>
    <insite:Button ID="SignOffButton" runat="server" Icon="fas fa-pen-field" Text="Sign Off" ButtonStyle="Success" ConfirmText="Are you sure you want to sign off on this training?" />
</asp:PlaceHolder>
                                
<asp:PlaceHolder ID="RenewSignOffPanel" runat="server">
    <p><asp:Literal ID="RenewSignOffText" runat="server" /></p>
    
    <insite:Button ID="RenewSignOffButton" runat="server" Icon="fas fa-pen-field" Text="Renew Sign Off" ButtonStyle="Success" ConfirmText="Do you want to renew your Sign Off date for this achievement?" />
</asp:PlaceHolder>

<div id="AttachmentPanel" runat="server" class="mt-4 mb-3">
    <h6 class="mt-4 mb-1">Downloads/Resources</h6>
    <uc:AttachmentList ID="AttachmentList" runat="server" />
</div>

    </div>
</div>