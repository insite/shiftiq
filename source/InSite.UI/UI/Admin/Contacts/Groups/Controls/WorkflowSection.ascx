<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkflowSection.ascx.cs" Inherits="InSite.UI.Admin.Contacts.Groups.Controls.WorkflowSection" %>

<div class="row outline">

    <div class="col-6">

        <h3>Notifications</h3>

        <div class="form-group mb-3">
            <div class="float-end">
                <insite:IconLink Name="pencil" runat="server" ID="ChangeMessageToAdminWhenMembershipStarted" ToolTip="Change Group Membership Started - Notification to Administrator(s)" NavigateUrl="#" />
            </div>
			<label class="form-label">Group Membership Started - Notification to Administrator(s)</label>
			<div>
                <asp:Literal runat="server" ID="MessageToAdminWhenMembershipStarted" />
            </div>
            <div class="form-text">
                Send an email message to administrators when someone joins this group.
            </div>
        </div>

        <div class="form-group mb-3">
            <div class="float-end">
                <insite:IconLink Name="pencil" runat="server" ID="IconLink2" ToolTip="Change Group Membership Ended - Notification to Administrator(s)" NavigateUrl="#" />
            </div>
	        <label class="form-label">Group Membership Ended - Notification to Administrator(s)</label>
	        <div>
                <asp:Literal runat="server" ID="MessageToAdminWhenMembershipEnded" />
            </div>
            <div class="form-text">
                Send an email message to administrators when someone leaves this group.
            </div>
        </div>

        <div class="form-group mb-3">
            <div class="float-end">
                <insite:IconLink Name="pencil" runat="server" ID="ChangeMessageToUserWhenMembershipStarted" ToolTip="Change Group Membership Started - Notification to User" NavigateUrl="#" />
            </div>
			<label class="form-label">Group Membership Started - Notification to User</label>
			<div>
                <asp:Literal runat="server" ID="MessageToUserWhenMembershipStarted" />
            </div>
            <div class="form-text">Send an email message to someone who joins this group.</div>
        </div>

        <div class="form-group mb-3">
            <div class="float-end">
                <insite:IconLink Name="pencil" runat="server" ID="IconLink1" ToolTip="Change Group Membership Ended - Notification to User" NavigateUrl="#" />
            </div>
	        <label class="form-label">Group Membership Ended - Notification to User</label>
	        <div>
                <asp:Literal runat="server" ID="MessageToUserWhenMembershipEnded" />
            </div>
            <div class="form-text">Send an email message to someone who leaves this group.</div>
        </div>

        <h3>Events</h3>

        <div class="form-group mb-3">
            <div class="float-end">
                <insite:IconLink Name="pencil" runat="server" ID="ChangeMessageToAdminWhenEventVenueChanged" ToolTip="Change Event Venue Changed - Notification to Administrator(s)" NavigateUrl="#" />
            </div>
			<label class="form-label">Event Venue Changed - Notification to Administrator(s)</label>
			<div>
                <asp:Literal runat="server" ID="MessageToAdminWhenEventVenueChanged" />
            </div>
            <div class="form-text">
                Send an email message to administrators when this group is selected as the venue for an event.
            </div>
        </div>

        <h3>Forms</h3>

        <div class="form-group mb-3">
            <div class="float-end">
                <insite:IconLink Name="pencil" runat="server" ID="ChangeMandatorySurveyFormIdentifier" ToolTip="Change Mandatory Form Submission" NavigateUrl="#" />
            </div>
			<label class="form-label">Mandatory Form Submission</label>
			<div>
                <asp:Literal runat="server" ID="MandatorySurveyFormIdentifier" />
            </div>
            <div class="form-text">Every user who joins this group must submit a submission to the selected form.</div>
        </div>

    </div>

</div>