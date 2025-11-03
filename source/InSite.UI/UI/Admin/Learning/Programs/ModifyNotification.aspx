<%@ Page Language="C#" CodeBehind="ModifyNotification.aspx.cs" Inherits="InSite.Admin.Records.Programs.ModifyNotification" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Notification" />

    <section class="mb-3">
        <div class="row mb-3">
            <div class="col-md-6">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Progress Stalled</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">Send to Learner</label>
                            <div>
                                <insite:FindMessage runat="server" ID="NotificationStalledLearnerMessageIdentifier" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Send to Administrator</label>
                            <div>
                                <insite:FindMessage runat="server" ID="NotificationStalledAdministratorMessageIdentifier" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Days Without Task Completion</label>
                            <div>
                                <insite:NumericBox runat="server" ID="NotificationStalledTriggerDay" NumericMode="Integer" />
                            </div>
                            <div class="form-text">
                                This is the number of days a learner can go without completing a task before the program is considered stalled.
                            </div>
                        </div>
    
                        <div class="form-group mb-3">
                            <label class="form-label">Reminder Limit</label>
                            <div>
                                <insite:NumericBox runat="server" ID="NotificationStalledReminderLimit" NumericMode="Integer" />
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
                        <h3>Progress Completed</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">Send to Learner</label>
                            <div>
                                <insite:FindMessage runat="server" ID="NotificationCompletedLearnerMessageIdentifier" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Send to Administrator</label>
                            <div>
                                <insite:FindMessage runat="server" ID="NotificationCompletedAdministratorMessageIdentifier" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Send On Completion of this Task</label>
                            <div>
                                <insite:ComboBox runat="server" ID="TaskInProgram" AllowBlank="true" />
                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </section>

    <div class="mt-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Notification" />
        <insite:CancelButton runat="server" ID="CancelButton" CausesValidation="false" />
    </div>

</asp:Content>