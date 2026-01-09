<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FormDetails.ascx.cs" Inherits="InSite.Admin.Assessments.Forms.Controls.FormDetails" %>

<%@ Register TagPrefix="assessments" Assembly="InSite.UI" Namespace="InSite.Admin.Assessments.Web.UI" %>

<div class="row mb-3">

    <div class="col-lg-6 mb-3 mb-lg-0">

        <h3>Identification</h3>
            
        <div runat="server" id="FormSpecificationField" class="form-group mb-3">
            <label class="form-label">
                Specification
            </label>
            <div>
                <asp:Literal runat="server" ID="FormSpecification" />
            </div>
            <div class="form-text">
                The specification that contains and drives the form.
            </div>
        </div>

        <div runat="server" id="FormStandardField" class="form-group mb-3">
            <label class="form-label">
                Standard
            </label>
            <div>
                <assessments:AssetTitleDisplay runat="server" ID="FormStandard" />
            </div>
            <div class="form-text">
                The standard evaluated by questions on the form.
            </div>
        </div>

        <div runat="server" id="FormNameField" class="form-group mb-3">
            <label class="form-label">
                Form Name
                <insite:IconLink Name="pencil" runat="server" id="ChangeNameLink" CssClass="form-details-link" ToolTip="Change Form Name" />
            </label>
            <div>
                <asp:Literal runat="server" ID="Name" />
            </div>
            <div class="form-text">
                The internal name used to uniquely identify the form for filing purposes.
            </div>
        </div>

        <div runat="server" id="FormAssetField" class="form-group mb-3">
            <label class="form-label">
                Asset Number and Version
                <insite:IconLink Name="trash-alt" runat="server" id="DeleteFormLink" CssClass="form-details-link" ToolTip="Delete Form" />
            </label>
            <div>
                <asp:Literal runat="server" ID="Version" />
            </div>
            <div class="form-text">
                The inventory asset number (and version) for this form.
            </div>
        </div>

        <div runat="server" id="GradebookField" class="form-group mb-3">
            <label class="form-label">
                Gradebook
                <insite:IconLink Name="pencil" runat="server" id="ChangeGradebookLink" CssClass="form-details-link" ToolTip="Change Gradebook" />
            </label>
            <div>
                <asp:Literal runat="server" ID="Gradebook" />
            </div>
            <div class="form-text">
                The gradebook where results are stored for answers to the questions in this form.
            </div>
        </div>

        <div runat="server" id="CodeField" class="form-group mb-3">
            <label class="form-label">
                Code
                <insite:IconLink Name="pencil" runat="server" id="ChangeCodeLink" CssClass="form-details-link" ToolTip="Change Form Code" />
            </label>
            <div>
                <asp:Literal runat="server" ID="Code" />
            </div>
            <div class="form-text">
                Alpha numeric catalog reference code for the form (required for Exam event scheduling).
            </div>
        </div>

        <div runat="server" id="SourceField" class="form-group mb-3">
            <label class="form-label">
                Source
            </label>
            <div>
                <asp:Literal runat="server" ID="Source" />
            </div>
            <div class="form-text">
                Reference to the source of the content and/or configuration for this form.
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Origin
            </label>
            <div>
                <asp:Literal runat="server" ID="Origin" />
            </div>
            <div class="form-text">
                Identifies the originating platform and/or record for this form. When this property is used, it should 
                ideally contain a fully qualified URL or API path.
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Hook / Integration Code
                <insite:IconLink Name="pencil" runat="server" id="ChangeHookLink" CssClass="form-details-link" ToolTip="Change Form Hook" />
            </label>
            <div>
                <asp:Literal runat="server" ID="Hook" />
            </div>
            <div class="form-text">
                Unique code for integration with internal toolkits and external systems.
            </div>
        </div>

        <div runat="server" id="FormIdentifierField" class="form-group mb-3">
            <label class="form-label">
                Form Identifier
            </label>
            <div>
                <asp:Literal runat="server" ID="Identifier" />
            </div>
            <div class="form-text">
                A globally unique identifier for this asset.
            </div>
        </div>

    </div>

    <div runat="server" id="PublicationColumn" class="col-lg-6">

        <h3>Publication</h3>

        <div runat="server" id="PublicationStatusField" class="form-group mb-3">
            <label class="form-label">
                Publication Status
            </label>
            <div>
                <asp:Literal runat="server" ID="PublicationStatus" />
            </div>
            <div class="form-text">
                The process to publish assessment forms has 3 steps. This is the current step in the process.
            </div>
        </div>

        <div runat="server" id="FirstPublishedField" class="form-group mb-3">
            <label class="form-label">
                First Published
            </label>
            <div>
                <asp:Literal runat="server" ID="FirstPublished" />
            </div>
            <div class="form-text">
            </div>
        </div>
            
        <div runat="server" id="FeedbackField" class="form-group mb-3">
            <label class="form-label">
                Learner/Assessor Feedback
            </label>
            <div>
                <asp:Literal runat="server" ID="AllowFeedbackFromCandidates" />
            </div>
            <div class="form-text">
                Allow the person completing the assessment to submit feedback on the questions.
            </div>
        </div>

        <div runat="server" id="RationaleField" class="form-group mb-3">
            <label class="form-label">
                Instructor Rationale
            </label>
            <div>
                <asp:Literal runat="server" ID="ShowRationale" />
            </div>
            <div class="form-text">
                Show (to candidates) the rationale behind the questions in this exam for correct and/or incorrect answers.
            </div>
        </div>

        <div runat="server" class="form-group mb-3">
            <div class="float-end">
                <asp:LinkButton runat="server" ID="FormThirdPartyAssessmentToggle" ToolTip="Enable third-party assessment"><i class="fas fa-toggle-off"></i></asp:LinkButton>
            </div>
            <label class="form-label">
                Third-Party Assessment
            </label>
            <div>
                <asp:Literal runat="server" ID="FormThirdPartyAssessment" />
            </div>
            <div class="form-text">
                Allow a third party assessor to use this form in the evaluation of a learner.
            </div>
        </div>

        <div runat="server" id="LanguageField" class="form-group mb-3">
            <label class="form-label">
                Language
            </label>
            <div runat="server" id="LanguageOutput"></div>
        </div>

        <div runat="server" id="SimulateField" class="form-group mb-3">
            <label class="form-label">
                Simulate
            </label>
            <div>
                <insite:Button runat="server" id="BuildScantronCsv" title="Scantron CSV" ButtonStyle="Default" Text="Scantron CSV" Icon="far fa-upload" />
            </div>
            <div class="form-text">
                Generate a text file to simulate the Scantron CSV output for answers to this exam form.
            </div>
        </div>

        <div runat="server" id="StaticQuestionOrderPanel" class="form-group mb-3" visible="false">

            <div class="float-end">
                <insite:IconButton runat="server" Name="badge-check" ID="VerifyStaticQuestionOrder" ToolTip="Verify Question Order"
                    ConfirmText="Are you sure you want to update the verified question order?" />
            </div>

            <label class="form-label">
                Static Question Order
            </label>

            <div>
            
                <asp:Literal runat="server" ID="StaticQuestionOrder" />

                <insite:Container runat="server" ID="StaticQuestionOrderDetail" Visible="false">

                    <insite:IconButton runat="server" Name="search" ID="StaticQuestionOrderZoom" CssClass="ms-1" data-bs-toggle="modal" />

                    <insite:Modal runat="server" ID="ModalStaticQuestionOrder" Title="Verified Question Order" Size="Large" Scrollable="true">
                        <ContentTemplate>
                            <table class="table table-striped">
                            <asp:Repeater runat="server" ID="StaticQuestionOrderRepeater">
                                <ItemTemplate>
                                    <tr>
                                        <td style="width: 30px;">
                                            <span class="badge bg-primary"><%# Eval("Sequence") %></span>
                                            <span class="badge bg-info"><%# Eval("Tag") %></span>
                                        </td>
                                        <td class="mw-0">
                                            <p class="text-truncate"><%# Eval("Text") %></p>
                                            <div class="fs-sm text-body-secondary"><%# Eval("Code") %></div>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            </table>
                        </ContentTemplate>
                        <FooterTemplate>
                            <button class="btn btn-secondary btn-sm" type="button" data-bs-dismiss="modal">Close</button>
                        </FooterTemplate>
                    </insite:Modal>
                
                    <div runat="server" id="StaticQuestionOrderMatchSuccess" visible="false" class="fs-xs text-success mt-1">
                        <i class="fas fa-check me-1"></i>The current question order matches the expected, verified order.
                    </div>

                    <div runat="server" id="StaticQuestionOrderMatchFailure" class="fs-xs text-danger mt-1">
                        <i class="fas fa-times me-1"></i>The current question order does not match the expected, verified order.
                    </div>

                </insite:Container>

            </div>
            <div class="form-text">
                Indicates when the sequence of questions on the form was last verified. 
                This is useful for static forms that are printed and stored outside the system for future use.
            </div>
        </div>

    </div>
</div>

<div class="row">
    <div runat="server" id="ProctoringColumn" class="col-lg-6 mb-3 mb-lg-0">
                                
        <h3>
            Proctoring
            <insite:IconLink Name="pencil" runat="server" ID="EditProctoringLink" CssClass="form-details-link fs-6" ToolTip="Edit form proctoring" />
        </h3>

        <div runat="server" id="SafeExamBrowserField" class="form-group mb-3">
            <label class="form-label">
                Safe Exam Browser
            </label>
            <div>
                <asp:Literal runat="server" ID="SafeExamBrowserState" />
            </div>
            <div class="form-text">
                If Safe Exam Browser is required then candidates must use this browser to submit answers to the exam.
                <a target="_blank" href="https://safeexambrowser.org/download_en.html">Download SEB</a>
            </div>
        </div>

        <div runat="server" id="KioskModeField" class="form-group mb-3">
            <label class="form-label">
                Kiosk Mode
            </label>
            <div>
                <asp:Literal runat="server" ID="KioskModeState" />
            </div>
            <div class="form-text">
                If Kiosk mode is enabled then candidates are <strong>NOT</strong> expected to have access to the portal outside the exam.
            </div>
        </div>

        <div runat="server" id="ScheduleOpenDateField" class="form-group mb-3">
            <label class="form-label">
                Date/Time Opened
            </label>
            <div>
                <asp:Literal runat="server" ID="ScheduleOpenDate" />
            </div>
            <div class="form-text">
                The earliest date and time when submissions are permitted.
            </div>
        </div>

        <div runat="server" id="ScheduleCloseDateField" class="form-group mb-3">
            <label class="form-label">
                Date Closed
            </label>
            <div>
                <asp:Literal runat="server" ID="ScheduleCloseDate" />
            </div>
            <div class="form-text">
                Submissions are not permitted after the exam form closes.
            </div>
        </div>

        <div runat="server" id="TimeLimitField" class="form-group mb-3">
            <label class="form-label">
                Time Limit
            </label>
            <div>
                <asp:Literal runat="server" ID="TimeLimit" /> minute(s)
            </div>
            <div class="form-text">
                This is the number of minutes permitted for each attempt on the exam. 
                <span class="text-danger">
                    This is required to enable the countdown timer and the autosave function for exam candidates.
                </span>
            </div>
        </div>

        <div runat="server" id="AttemptLimitField" class="form-group mb-3">
            <label class="form-label">
                Attempt Limit
            </label>
            <div>
                <asp:Literal runat="server" ID="AttemptLimit" />
            </div>
            <div class="form-text">
                The maximum number of times each candidate is permitted attempt this exam. Zero (0) indicates unlimited attempts.
            </div>
        </div>

        <div class="form-group mb-3" runat="server" id="AttemptLimitPerSessionField">
            <label class="form-label">
                Failed Attempts per Exam Session
            </label>
            <div>
                <asp:Literal runat="server" ID="AttemptLimitPerSession" />
            </div>
            <div class="form-text">
                This is the number of times a user is permitted to fail the exam before being disallowed another attempt. 
                For example, if you set this field to 3 then the system will block the user from the exam after 3 failed attempts. 
                It is important to note that if a user starts the exam and does not complete it then the system counts this as a failed attempt, regardless of whether or not the incomplete attempt is scored. 
                (This latter scenario is analogous to a student leaving the room after beginning an exam form.)
            </div>
        </div>

        <div class="form-group mb-3" runat="server" id="TimeLimitPerSessionField">
            <label class="form-label">
                Minutes per Exam Session
            </label>
            <div>
                <asp:Literal runat="server" ID="TimeLimitPerSession" />
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
            </label>
            <div>
                <asp:Literal runat="server" ID="TimeLimitPerLockout" />
            </div>
            <div class="form-text">
                This is the number of minutes that must elapse before the system will allow a user to retry an exam after failing too many consecutive attempts.
            </div>
        </div>

    </div>

    <div runat="server" id="ClassificationColumn" class="col-lg-6">

        <h3>
            Classification
            <insite:IconLink Name="pencil" runat="server" id="EditClassificationLink" CssClass="form-details-link fs-6" ToolTip="Edit Form Classification" />
        </h3>

        <div runat="server" id="InstrumentField" class="form-group mb-3">
            <label class="form-label">
                Instrument
            </label>
            <div>
                <asp:Literal runat="server" ID="Instrument" />
            </div>
            <div class="form-text">
                What type of assessment instrument is this (e.g. diagnostic pre-test, post-test evaluation)?
            </div>
        </div>

        <div runat="server" id="ThemeField" class="form-group mb-3">
            <label class="form-label">
                Theme
            </label>
            <div>
                <asp:Literal runat="server" ID="Theme" />
            </div>
            <div class="form-text">
                What is the general topic or subject matter under evaluation?
            </div>
        </div>

    </div>

</div>

<insite:PageFooterContent runat="server" ID="FooterLiteral">
    <script type="text/javascript">
        $(document).ready(function () {
            $('.form-details-link').each(function () {
                var $this = $(this);

                var section = $this.closest('[data-section]').data('section');
                if (section)
                    $this.prop('href', $this.prop('href') + '&section=' + String(section));
            });
        });
    </script>
</insite:PageFooterContent>