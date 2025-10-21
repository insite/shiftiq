<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.Admin.Events.Registrations.Forms.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">	

    <insite:Alert runat="server" ID="Status" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" />
            
    <div class="row settings">
        <div class="col-lg-6">
            <h3>Registration</h3>

            <dl class="row">
                <dt class="col-sm-3">Registration #</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="RegistrationSequence" /></dd>
            
                <dt class="col-sm-3">Registration Requested</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="RegistrationRequestedOn" /></dd>
            
                <dt class="col-sm-3">Approval Status</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="RegistrationApprovalStatus" /></dd>
            
                <dt class="col-sm-3">Attendance Status</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="RegistrationAttendanceStatus" /></dd>
            
                <dt class="col-sm-3">Registration Fee</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="RegistrationFee" /></dd>

                <dt class="col-sm-3">Participant Name:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="FullName" /></dd>

                <dt class="col-sm-3">Email:</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="Email" /></dd>

                <dt class="col-sm-3">Achievement</dt>
                <dd class="col-sm-9"><asp:Literal runat="server" ID="AchievementTitle" /></dd>

                <dt class="col-sm-3">Event Title</dt>
                <dd class="col-sm-9">
                    <a runat="server" id="EventLink">
                        <asp:Literal runat="server" ID="EventTitle" />
                    </a>
                </dd>
            </dl>

            <div class="alert alert-danger" role="alert">
                 <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                Are you sure you want to delete this registration?	
            </div>	

            <p style="padding-bottom:10px;">	
                <insite:DeleteButton runat="server" ID="DeleteButton" />	
                <insite:CancelButton runat="server" ID="CancelButton" />	
            </p>
        </div>

        <div class="col-lg-6">

            <h3>Impact</h3>

            <div class="alert alert-warning">
                <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                This is a permanent change that cannot be undone. 
                The registration will be deleted from all forms, queries, and reports.
            </div>
        </div>
    </div>
</div>
</asp:Content>
