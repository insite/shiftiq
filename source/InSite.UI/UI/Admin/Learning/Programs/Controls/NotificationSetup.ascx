<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NotificationSetup.ascx.cs" Inherits="InSite.Admin.Records.Programs.Controls.NotificationSetup" %>

<div class="row mb-3">
    <div class="col-md-6">

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <div class="float-end">
                    <insite:IconLink runat="server" ID="EditLink1" CssClass="p-2" ToolTip="Modify notification" Name="pencil" />
                </div>

                <h3>Progress Stalled</h3>

                <div class="form-group mb-3">
                    <label class="form-label">Send to Learner</label>
                    <div>
                        <asp:Literal runat="server" ID="NotificationStalledLearnerMessageName" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">Send to Administrator</label>
                    <div>
                        <asp:Literal runat="server" ID="NotificationStalledAdministratorMessageName" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">Days Without Task Completion</label>
                    <div>
                        <asp:Literal runat="server" ID="NotificationStalledTriggerDay" />
                    </div>
                    <div class="form-text">
                        This is the number of days a learner can go without completing a task before the program is considered stalled.
                    </div>
                </div>
    
                <div class="form-group mb-3">
                    <label class="form-label">Reminder Limit</label>
                    <div>
                        <asp:Literal runat="server" ID="NotificationStalledReminderLimit" />
                    </div>
                    <div class="form-text">
                        This is the maximum number of reminder notifications to send to a learner who is stalled.
                    </div>
                </div>
            </div>
        </div>

    </div>

    <div class="col-md-6">

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <div class="float-end">
                    <insite:IconLink runat="server" ID="EditLink2" CssClass="p-2" ToolTip="Modify notification" Name="pencil" />
                </div>

                <h3>Progress Completed</h3>

                <div class="form-group mb-3">
                    <label class="form-label">Send to Learner</label>
                    <div>
                        <asp:Literal runat="server" ID="NotificationCompletedLearnerMessageName" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">Send to Administrator</label>
                    <div>
                        <asp:Literal runat="server" ID="NotificationCompletedAdministratorMessageName" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">Send On Completion of this Task</label>
                    <div>
                        <asp:Literal runat="server" ID="TaskInProgram" />
                    </div>
                </div>
            </div>
        </div>

    </div>
</div>
