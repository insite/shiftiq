<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.UI.Admin.Events.Attendees.Forms.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../../../Contacts/People/Controls/PersonInfo.ascx" TagName="PersonDetail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">	

    <insite:Alert runat="server" ID="Status" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" />
            
    <div class="row settings">
        <div class="col-md-6">
            <h3>Attendee</h3>
            <uc:PersonDetail runat="server" ID="PersonDetail" />

            <dl class="row">
                <dt runat="server" id="AttendeeRoleDiv" class="col-sm-3">Attendee Role</dt>
                <dd class="col-sm-9">
                    <asp:Literal runat="server" ID="AttendeeRole" />
                </dd>

                <dt runat="server" id="AssignedDiv" class="col-sm-3">Assigned</dt>
                <dd class="col-sm-9">
                    <asp:Literal runat="server" ID="Assigned" />
                </dd>

                <dt runat="server" id="EventTitleDiv" class="col-sm-3">Event Title</dt>
                <dd class="col-sm-9">
                    <asp:Literal runat="server" ID="EventTitle" />
                </dd>
            </dl>

            <div class="alert alert-danger">
                <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                Are you sure you want to delete this attendee event?
            </div>

            <div>
                <insite:DeleteButton runat="server" ID="DeleteButton" />	
                <insite:CancelButton runat="server" ID="CancelButton" />
            </div>
        </div>

        <div class="col-md-6">
            <h3>Impact</h3>

            <div class="alert alert-warning">
                <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                This is a permanent change that cannot be undone. 
                The attendee will be deleted from all forms, queries, and reports.
                Here is a summary of the data that will be erased if you proceed.
            </div>
        </div>
    </div>

</div>
</asp:Content>
