<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClassNotificationTab.ascx.cs" Inherits="InSite.UI.Admin.Events.Classes.Controls.ClassNotificationTab" %>

<insite:Alert runat="server" ID="TriggerAlert" />

<div class="row mb-3">
    <div class="col-md-6 mb-3 mb-md-0">

		<div class="card mb-3">
			<div class="card-body">

                <h3>Class Reminder Notifications</h3>

                <div class="form-group mb-3">
                    <div class="float-end">
                        <insite:IconLink Name="pencil" runat="server" id="ToLearnerLink" style="padding:8px" ToolTip="Change Message" />
                    </div>
                    <label class="form-label">
                        To Learner
                    </label>
                    <div>
                        <asp:Literal runat="server" ID="ReminderLearnerMessage" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <div class="float-end">
                        <insite:IconLink Name="pencil" runat="server" id="ToInstructorsLink" style="padding:8px" ToolTip="Change Message" />
                    </div>
                    <label class="form-label">
                        To Instructors
                    </label>
                    <div>
                        <asp:Literal runat="server" ID="ReminderInstructorMessage" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <div class="float-end">
                        <insite:IconLink Name="pencil" runat="server" id="SendReminderBeforeDaysLink" style="padding:8px" ToolTip="Change Value" />
                    </div>
                    <label class="form-label">
                        Send notification prior to start of Class (in days)
                    </label>
                    <div>
                        <asp:Literal runat="server" ID="SendReminderBeforeDays" />
                    </div>
                </div>

            </div>
        </div>
                                    
    </div>

    <div runat="server" id="TestPanel" visible="false" class="col-md-6 mb-3 mb-md-0">

		<div class="card mb-3">
			<div class="card-body">

                <h3>
                    Trigger Notifications Manually

                    <span class="float-end text-info fs-6 fw-normal">
                        <i class="fa-solid fa-circle-info me-2"></i>
                        This panel is visible only to Operators
                    </span>
                </h3>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Last Sent Message
                    </label>
                    <div>
                        <asp:Literal runat="server" ID="ReminderMessageSent" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Ignore when the class is scheduled and always send notifications
                    </label>
                    <div>
                        <asp:RadioButtonList runat="server" ID="IgnoreScheduleStart">
                            <asp:ListItem Value="Yes" Text="Yes (always send)" />
                            <asp:ListItem Value="No" Text="No (use scheduled start field)" Selected="true" />
                        </asp:RadioButtonList>
                    </div>
                </div>

                <insite:Button runat="server"
                    ID="TriggerButton"
                    CausesValidation="false"
                    ButtonStyle="Default"
                    Icon="fas fa-bolt"
                    Text="Trigger Notifications"
                    ConfirmText="Are you sure to trigger notifications?"
                    DisableAfterClick="true"
                />

            </div>
        </div>
                                    
    </div>
</div>
