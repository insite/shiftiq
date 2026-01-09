<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NotificationSetup.ascx.cs" Inherits="InSite.UI.Admin.Courses.Outlines.Controls.NotificationSetup" %>

<div class="row mb-3">
    <div class="col-md-6">

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <h3>Course Stalled Notifications</h3>

                <div class="form-group mb-3">
                    <label class="form-label">To Learner</label>
                    <div>
                        <insite:FindMessage runat="server" ID="StalledToLearnerMessageIdentifier" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">To Administrators</label>
                    <div>
                        <insite:FindMessage runat="server" ID="StalledToAdministratorMessageIdentifier" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">Send Notification After Days</label>
                    <div>
                        <insite:NumericBox runat="server" ID="SendMessageStalledAfterDays" NumericMode="Integer" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">Max Notification Count</label>
                    <div>
                        <insite:NumericBox runat="server" ID="SendMessageStalledMaxCount" NumericMode="Integer" />
                    </div>
                </div>
            </div>
        </div>

    </div>

    <div class="col-md-6">

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <h3>Course Completed Notifications</h3>

                <div class="form-group mb-3">
                    <label class="form-label">To Learner</label>
                    <div>
                        <insite:FindMessage runat="server" ID="CompletedToLearnerMessageIdentifier" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">To Administrators</label>
                    <div>
                        <insite:FindMessage runat="server" ID="CompletedToAdministratorMessageIdentifier" />
                    </div>
                </div>

                <asp:Panel runat="server" ID="ReminderCompletionActivity" CssClass="alert alert-warning" Visible="false">
                    <i class="fas fa-exclamation-triangle"></i> <strong>Reminder:</strong>
                    Don't forget to select the Course Completed activity under the Course Setup tab, in the Completion card.
                </asp:Panel>

            </div>
        </div>

    </div>
</div>

<div class="row mb-3">
    <div class="col-md-12">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="NotificationSetup" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>
</div>
