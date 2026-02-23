<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OutlineNotifications.ascx.cs" Inherits="InSite.UI.Admin.Records.Achievements.Controls.OutlineNotifications" %>

<div class="row mb-3">
    <div class="col-md-6">

        <div class="card h-100 border-0 shadow-lg">
            <div class="card-body">
                <h3>Notification Before Expiry</h3>

                <insite:CustomValidator runat="server" ID="BeforeExpiryNotificationTimingValidator" ErrorMessage="Required field: Notification Timing (Before Expiry)" Display="None" />

                <div class="form-group mb-3">
                    <label class="form-label">To Learner</label>
                    <insite:FindMessage runat="server" ID="BeforeExpiryLearnerMessageIdentifier" />
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">To Administrator</label>
                    <insite:FindMessage runat="server" ID="BeforeExpiryAdministratorMessageIdentifier" />
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">Notification Timing</label>
                    <insite:NumericBox runat="server" ID="BeforeExpiryNotificationTiming" NumericMode="Integer" MinValue="1" Width="150px" />
                    <div class="form-text">
                        Enter the number of days before the expiry date when the notification should be sent.
                    </div>
                </div>

            </div>
        </div>

    </div>

    <div class="col-md-6">

        <div class="card h-100 border-0 shadow-lg">
            <div class="card-body">
                <h3>Notification After Expiry</h3>

                <insite:CustomValidator runat="server" ID="AfterExpiryNotificationTimingValidator" ErrorMessage="Required field: Notification Timing (After Expiry)" Display="None" />

                <div class="form-group mb-3">
                    <label class="form-label">To Learner</label>
                    <insite:FindMessage runat="server" ID="AfterExpiryLearnerMessageIdentifier" />
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">To Administrator</label>
                    <insite:FindMessage runat="server" ID="AfterExpiryAdministratorMessageIdentifier" />
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">Notification Timing</label>
                    <insite:NumericBox runat="server" ID="AfterExpiryNotificationTiming" NumericMode="Integer" MinValue="0" Width="150px" />
                    <div class="form-text">
                        Enter the number of days before the expiry date when the notification should be sent.
                    </div>
                </div>

            </div>
        </div>

    </div>
</div>

<div class="mt-4">
    <insite:SaveButton runat="server" ID="SaveButton" CausesValidation="true" />
</div>