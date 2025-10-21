<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NotificationSetup.ascx.cs" Inherits="InSite.UI.Admin.Courses.Outlines.Controls.NotificationSetup" %>

<div class="row mb-3">
    <div class="col-md-6">

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <h3>Course Stalled Notifications</h3>

                <div class="form-group mb-3">
                    <label class="form-label">To Learner</label>
                    <div>
                        <insite:FindMessage runat="server" ID="CourseMessageStalledToLearner" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">To Administrators</label>
                    <div>
                        <insite:FindMessage runat="server" ID="CourseMessageStalledToAdministrator" />
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

                <asp:CustomValidator runat="server" ID="CompletedNotificationValidator"
                    Display="None"
                    ValidationGroup="NotificationSetup"
                    ErrorMessage="Required field: Send On Completion of this Activity"
                />

                <div class="form-group mb-3">
                    <label class="form-label">To Learner</label>
                    <div>
                        <insite:FindMessage runat="server" ID="CourseMessageCompletedToLearner" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">To Administrators</label>
                    <div>
                        <insite:FindMessage runat="server" ID="CourseMessageCompletedToAdministrator" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">Send On Completion of this Activity</label>
                    <div>
                        <insite:ActivityComboBox runat="server" ID="CompletionActivityIdentifier" />
                    </div>
                </div>

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
