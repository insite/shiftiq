<%@ Page Language="C#" CodeBehind="ModifyProctoring.aspx.cs" Inherits="InSite.Admin.Assessments.Forms.Forms.ModifyProctoring" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Assessment" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-window"></i>
            Modify Form Proctoring Rules
        </h2>
        <div class="row">

            <div class="col-lg-12">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        
                        <h3>Modify Form Proctoring Rules</h3>

                        <div class="row">

                            <div class="col-md-6">
                                <div class="form-group mb-3">
                                    <label class="form-label">Status</label>
                                    <div>
                                        <insite:CheckBox ID="IsSafeExamBrowserRequired" runat="server" Text="Safe Exam Browser Required" />
                                        <insite:CheckBox ID="IsKioskModeRequired" runat="server" Text="Kiosk Mode Required" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Schedule</label>
                                    <div class="form-group mb-3">
                                        <label class="form-label fw-light">Open Date</label>
                                        <insite:DateTimeOffsetSelector runat="server" ID="Opened" CssClass="mw-100" Width="360px" />
                                    </div>
                                    <div class="form-group mb-3">
                                        <label class="form-label fw-light">Close Date</label>
                                        <insite:DateTimeOffsetSelector runat="server" ID="Closed" CssClass="mw-100" Width="360px" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Time Limit
                                            <insite:RequiredValidator runat="server" ControlToValidate="TimeLimit" ValidationGroup="Assessment" />
                                    </label>
                                    <div>
                                        <insite:NumericBox runat="server" ID="TimeLimit" Width="120px" MinValue="0" MaxValue="262800" NumericMode="Integer" CssClass="d-inline-block" />
                                        minutes
                                    </div>
                                    <div class="form-text">
                                        This is the number of minutes allowed for each attempt on the exam.
                                            <span class="text-danger">Note this is required to enable the countdown timer and the autosave function for exam candidates.
                                            </span>
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Timer</label>
                                    <div>
                                        <insite:BooleanComboBox runat="server" ID="IsTimerVisible" TrueText="Show" FalseText="Hide" AllowBlank="false" Width="120px" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Attempt Limit
                                            <insite:RequiredValidator runat="server" ControlToValidate="AttemptLimit" ValidationGroup="Assessment" />
                                    </label>
                                    <div>
                                        <insite:NumericBox runat="server" ID="AttemptLimit" Width="120px" NumericMode="Integer" MinValue="0" MaxValue="999999" />
                                    </div>
                                    <div class="form-text">
                                        This is the maximum number of times each person is permitted attempt this exam. Enter zero (0) for unlimited attempts.
                                    </div>
                                </div>
                            </div>

                            <div class="col-md-6">
                                <div class="form-group mb-3" runat="server" id="AttemptLimitPerSessionField">
                                    <label class="form-label">
                                        Failed Attempts per Exam Session
                                            <insite:RequiredValidator runat="server" ControlToValidate="AttemptLimitPerSession" ValidationGroup="Assessment" />
                                    </label>
                                    <div>
                                        <insite:NumericBox runat="server" ID="AttemptLimitPerSession" Width="120px" NumericMode="Integer" MinValue="0" MaxValue="999999" />
                                    </div>
                                    <div class="form-text">
                                        This is the number of times a user is permitted to fail the exam before being disallowed another attempt.
                                            For example, if you set this field to 3 then the system will block the user from the exam after 3 failed attempts.
                                            It is important to note that if a user starts the exam and does not complete it then the system counts this as a failed attempt,
                                            regardless of whether or not the incomplete attempt is scored.
                                            (This latter scenario is analogous to a student leaving the room after beginning an exam form.)
                                    </div>
                                </div>

                                <div class="form-group mb-3" runat="server" id="TimeLimitPerSessionField">
                                    <label class="form-label">
                                        Minutes per Exam Session
                                            <insite:RequiredValidator runat="server" ControlToValidate="TimeLimitPerSession" ValidationGroup="Assessment" />
                                    </label>
                                    <div>
                                        <insite:NumericBox runat="server" ID="TimeLimitPerSession" Width="120px" NumericMode="Integer" MinValue="0" MaxValue="999999" CssClass="d-inline-block" />
                                        minutes
                                    </div>
                                    <div class="form-text">
                                        This is the number of minutes during which a user is permitted fail consecutive attempts at the exam. 
                                            For example, if you set this field to 60 minutes and you set the preceding field to 3 then the system will permit a maximum of 3 consecutive failed attempts during any 1 hour period. 
                                            If you leave this field empty then the system will consider all of the user's attempts without any time limits.
                                    </div>
                                </div>

                                <div class="form-group mb-3" runat="server" id="TimeLimitPerLockoutField">
                                    <label class="form-label">
                                        Minutes Locked Out
                                            <insite:RequiredValidator runat="server" ControlToValidate="TimeLimitPerLockout" ValidationGroup="Assessment" />
                                    </label>
                                    <div>
                                        <insite:NumericBox runat="server" ID="TimeLimitPerLockout" Width="120px" NumericMode="Integer" MinValue="0" MaxValue="999999" CssClass="d-inline-block" />
                                        minutes
                                    </div>
                                    <div class="form-text">
                                        This is the number of minutes that must elapse before the system will allow a user to retry an exam after failing too many consecutive attempts.
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>
            </div>

        </div>
    </section>

    <div class="row">
        <div class="col-md-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Assessment" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

</asp:Content>
