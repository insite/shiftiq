<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Detail.ascx.cs" Inherits="InSite.UI.Admin.Jobs.Applications.Controls.Detail" %>

<style>
    .form-control {
        display: inline;
    }
</style>

<div class="form-group mb-3">
    <label class="form-label">
        Job
        <insite:RequiredValidator runat="server" FieldName="Job"
            ControlToValidate="OpportunityID"
            ValidationGroup="Job Application"
            Display="Dynamic" />

    </label>
    <insite:FindOpportunity runat="server" ID="OpportunityID" />
    <div class="form-text">(Help)</div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Candidate
        <insite:RequiredValidator runat="server" FieldName="Candidate"
            ControlToValidate="CandidateUserID"
            ValidationGroup="Job Application"
            Display="Dynamic" />
    </label>
    <insite:FindPerson runat="server" ID="CandidateUserID" />
    <div class="form-text">(Help)</div>
</div>

<div runat="server" id="CandidateCoverLetterField" class="form-group mb-3">
    <label class="form-label">
        Candidate Cover Letter
        <insite:RequiredValidator runat="server" FieldName="Cover Letter File"
            ControlToValidate="CoverLetterUrl"
            ValidationGroup="Job Application"
            Display="Dynamic" />
        <insite:FileExtensionValidator runat="server" FileExtensions="pdf,doc,docx,txt"
            ControlToValidate="CoverLetterUpload"
            ValidationGroup="CoverLetterUpload"
            Display="Dynamic" />
    </label>
    <div style="min-width:660px;">
        <insite:TextBox runat="server" ID="CoverLetterUrl" MaxLength="256" Width="240px" EmptyMessage="Cover Letter URL" />
        <div style="display:none;">
            <asp:FileUpload runat="server" ID="CoverLetterUpload" Width="600px" />
        </div>
        <insite:IconButton runat="server" ID="UploadCoverLetter" Name="upload" style="margin-left:6px;" ToolTip="Upload Cover Letter File" ValidationGroup="CoverLetterUpload" Visible="true" OnClientClick="jobApplicationDetail.onUploadCoverLetter(); return false;" />
        <insite:IconButton runat="server" ID="ViewCoverLetter" Name="eye" style="margin-left:6px;" ToolTip="View Cover Letter" CausesValidation="false" Visible="false" OnClientClick="jobApplicationDetail.onViewCoverLetter(); return false;" />
    </div>
    <div class="form-text">(Help)</div>
</div>

<div runat="server" id="CandidateResumeField" class="form-group mb-3">
    <label class="form-label">
        Candidate Resume
        <insite:RequiredValidator runat="server" FieldName="Resume File"
            ControlToValidate="ResumeUrl"
            ValidationGroup="Job Application"
            Display="Dynamic" />
        <insite:FileExtensionValidator runat="server" FileExtensions="pdf,doc,docx,txt"
            ControlToValidate="ResumeUpload"
            ValidationGroup="ResumeUpload"
            Display="Dynamic" />
    </label>
    <div>
        <div style="min-width:660px;">
            <insite:TextBox runat="server" ID="ResumeUrl" MaxLength="256" Width="240px" EmptyMessage="Resume URL" />
            <div style="display:none;">
                <asp:FileUpload runat="server" ID="ResumeUpload" Width="600px" />
            </div>
            <insite:IconButton runat="server" ID="UploadResume" Name="upload" style="margin-left:6px;" ToolTip="Upload Resume File" ValidationGroup="ResumeUpload" Visible="true" OnClientClick="jobApplicationDetail.onUploadResume(); return false;" />
            <insite:IconButton runat="server" ID="ViewResume" Name="eye" style="margin-left:6px;" ToolTip="View Resume" CausesValidation="false" Visible="false" OnClientClick="jobApplicationDetail.onViewResume(); return false;" />
        </div>
    </div>
</div>                       

<insite:PageFooterContent runat="server">

    <script type="text/javascript">

        (function () {
            var jobApplicationDetail = window.jobApplicationDetail = window.jobApplicationDetail || {};
            var coverLetterUrl = null;
            var resumeUrl = null;

            // Cover Letter

            var $coverLetterUploadInput = $('#<%= CoverLetterUpload.ClientID %>').on('change', function () {
                if (typeof this.files != 'object' || this.files.length > 0) {
                    WebForm_DoPostBackWithOptions(
                        new WebForm_PostBackOptions(
                            '<%= UploadCoverLetter.UniqueID %>',
                            '',
                            true,
                            'CoverLetterUpload',
                            '',
                            false,
                            true));
                }
            });

            jobApplicationDetail.onViewCoverLetter = function () {
                var win = window.open(coverLetterUrl, '_blank');
                win.focus();
            };

            jobApplicationDetail.onUploadCoverLetter = function () {
                $coverLetterUploadInput.val('').click();
            };

            // Resume

            var $resumeUploadInput = $('#<%= ResumeUpload.ClientID %>').on('change', function () {
                if (typeof this.files != 'object' || this.files.length > 0) {
                    WebForm_DoPostBackWithOptions(
                        new WebForm_PostBackOptions(
                            '<%= UploadResume.UniqueID %>',
                            '',
                            true,
                            'ResumeUpload',
                            '',
                            false,
                            true));
                }
            });

            jobApplicationDetail.onViewResume = function () {
                var win = window.open(resumeUrl, '_blank');
                win.focus();
            };

            jobApplicationDetail.onUploadResume = function () {
                $resumeUploadInput.val('').click();
            };

            function onApplicationLoad() {
                Sys.Application.remove_load(onApplicationLoad);

                coverLetterUrl = $('#<%= CoverLetterUrl.ClientID %>').val();
                resumeUrl = $('#<%= ResumeUrl.ClientID %>').val();
            }

            // other

            Sys.Application.add_load(onApplicationLoad);
        })();

    </script>

</insite:PageFooterContent>