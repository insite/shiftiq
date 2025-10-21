<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DocumentSection.ascx.cs" Inherits="InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls.DocumentSection" %>

<div id='<%= ClientID %>' class="card">
    <div class="card-body">

        <h4 class="card-title mb-3">Documents</h4>

        <div class="row">
            <div class="col-6">

                <div class="form-group mb-3">
                    <label class="form-label">
                        CV or Resume (pdf, docx, doc, txt)
                    </label>

                    <insite:TextBox runat="server" ID="ResumeUrl" MaxLength="256" Width="240px" EmptyMessage="Resume URL" ReadOnly="true" style="display: inline;" />
                    <div style="display: none;">
                        <insite:FileUploadV2 runat="server" ID="ResumeUpload"
                            MaxFileSize="5242880" AllowedExtensions=".pdf,.docx,.doc,.txt"
                            OnClientFileUploaded="profileDetail.onFileUploaded"
                            OnClientFileUploadFailed="profileDetail.onFileUploadFailed" />
                    </div>
                    <a href="#upload" data-upload='<%= ResumeUpload.ClientID %>' style="margin-left: 6px;" title="Upload Resume File"><i class="fa fa-upload"></i></a>
                    <a href="#view" data-view='#<%= ResumeUrl.ClientID %>' style="margin-left: 6px;" title="View Resume"><i class="fa fa-eye"></i></a>

                    <div class="form-text">
                        A resume or CV is a written overview of your experience and qualifications and is generally what you use to apply for a job.
                        <br />
                        5MB max size
                    </div>
                </div>

            </div>
            <div class="col-6">

                <div class="form-group mb-3">
                    <label class="form-label">
                        Cover Letter (pdf, docx, doc, txt)
                    </label>

                    <insite:TextBox runat="server" ID="CoverLetterUrl" MaxLength="256" Width="240px" EmptyMessage="Cover Letter URL" ReadOnly="true" style="display: inline;" />
                    <div style="display: none;">
                        <insite:FileUploadV2 runat="server" ID="CoverLetterUpload"
                            MaxFileSize="2097152" AllowedExtensions=".pdf,.docx,.doc,.txt"
                            OnClientFileUploaded="profileDetail.onFileUploaded"
                            OnClientFileUploadFailed="profileDetail.onFileUploadFailed" />
                    </div>
                    <a href="#upload" data-upload='<%= CoverLetterUpload.ClientID %>' style="margin-left: 6px;" title="Upload Cover Letter File"><i class="fa fa-upload"></i></a>
                    <a href="#view" data-view='#<%= CoverLetterUrl.ClientID %>' style="margin-left: 6px;" title="View Cover Letter"><i class="fa fa-eye"></i></a>
                    <div class="form-text">
                        A cover letter is a document that you send in with your resume to provide additional information on your skills and experience.
                        <br />
                        2MB max size
                    </div>
                </div>

            </div>
        </div>

        <div class="row">
            <div class="col-6">

                <div class="form-group mb-3">
                    <label class="form-label">
                        Reference Letter(s) (pdf, docx, doc, txt)
                    </label>

                    <div class="mb-3">
                        <insite:TextBox runat="server" ID="ReferenceLetterUrl1" MaxLength="256" Width="240px" EmptyMessage="Reference Letter URL" ReadOnly="true" style="display: inline;" />
                        <div style="display: none;">
                            <insite:FileUploadV2 runat="server" ID="ReferenceLetterUpload1"
                                MaxFileSize="2097152" AllowedExtensions=".pdf,.docx,.doc,.txt"
                                OnClientFileUploaded="profileDetail.onFileUploaded"
                                OnClientFileUploadFailed="profileDetail.onFileUploadFailed" />
                        </div>
                        <a href="#upload" data-upload='<%= ReferenceLetterUpload1.ClientID %>' style="margin-left: 6px;" title="Upload Reference Letter File"><i class="fa fa-upload"></i></a>
                        <a href="#view" data-view='#<%= ReferenceLetterUrl1.ClientID %>' style="margin-left: 6px;" title="View Reference Letter"><i class="fa fa-eye"></i></a>
                    </div>

                    <div class="mb-3">
                        <insite:TextBox runat="server" ID="ReferenceLetterUrl2" MaxLength="256" Width="240px" EmptyMessage="Reference Letter URL" ReadOnly="true" style="display: inline;" />
                        <div style="display: none;">
                            <insite:FileUploadV2 runat="server" ID="ReferenceLetterUpload2"
                                MaxFileSize="2097152" AllowedExtensions=".pdf,.docx,.doc,.txt"
                                OnClientFileUploaded="profileDetail.onFileUploaded"
                                OnClientFileUploadFailed="profileDetail.onFileUploadFailed" />
                        </div>
                        <a href="#upload" data-upload='<%= ReferenceLetterUpload2.ClientID %>' style="margin-left: 6px;" title="Upload Reference Letter File"><i class="fa fa-upload"></i></a>
                        <a href="#view" data-view='#<%= ReferenceLetterUrl2.ClientID %>' style="margin-left: 6px;" title="View Reference Letter"><i class="fa fa-eye"></i></a>
                    </div>

                    <div class="mb-3">
                        <insite:TextBox runat="server" ID="ReferenceLetterUrl3" MaxLength="256" Width="240px" EmptyMessage="Reference Letter URL" ReadOnly="true" style="display: inline;" />
                        <div style="display: none;">
                            <insite:FileUploadV2 runat="server" ID="ReferenceLetterUpload3"
                                MaxFileSize="2097152" AllowedExtensions=".pdf,.docx,.doc,.txt"
                                OnClientFileUploaded="profileDetail.onFileUploaded"
                                OnClientFileUploadFailed="profileDetail.onFileUploadFailed" />
                        </div>
                        <a href="#upload" data-upload='<%= ReferenceLetterUpload3.ClientID %>' style="margin-left: 6px;" title="Upload Reference Letter File"><i class="fa fa-upload"></i></a>
                        <a href="#view" data-view='#<%= ReferenceLetterUrl3.ClientID %>' style="margin-left: 6px;" title="View Reference Letter"><i class="fa fa-eye"></i></a>
                    </div>

                    <div class="mb-3">
                        <insite:TextBox runat="server" ID="ReferenceLetterUrl4" MaxLength="256" Width="240px" EmptyMessage="Reference Letter URL" ReadOnly="true" style="display: inline;" />
                        <div style="display: none;">
                            <insite:FileUploadV2 runat="server" ID="ReferenceLetterUpload4"
                                MaxFileSize="2097152" AllowedExtensions=".pdf,.docx,.doc,.txt"
                                OnClientFileUploaded="profileDetail.onFileUploaded"
                                OnClientFileUploadFailed="profileDetail.onFileUploadFailed" />
                        </div>
                        <a href="#upload" data-upload='<%= ReferenceLetterUpload4.ClientID %>' style="margin-left: 6px;" title="Upload Reference Letter File"><i class="fa fa-upload"></i></a>
                        <a href="#view" data-view='#<%= ReferenceLetterUrl4.ClientID %>' style="margin-left: 6px;" title="View Reference Letter"><i class="fa fa-eye"></i></a>
                    </div>

                    <div class="mb-3">
                        <insite:TextBox runat="server" ID="ReferenceLetterUrl5" MaxLength="256" Width="240px" EmptyMessage="Reference Letter URL" ReadOnly="true" style="display: inline;" />
                        <div style="display: none;">
                            <insite:FileUploadV2 runat="server" ID="ReferenceLetterUpload5"
                                MaxFileSize="2097152" AllowedExtensions=".pdf,.docx,.doc,.txt"
                                OnClientFileUploaded="profileDetail.onFileUploaded"
                                OnClientFileUploadFailed="profileDetail.onFileUploadFailed" />
                        </div>
                        <a href="#upload" data-upload='<%= ReferenceLetterUpload5.ClientID %>' style="margin-left: 6px;" title="Upload Reference Letter File"><i class="fa fa-upload"></i></a>
                        <a href="#view" data-view='#<%= ReferenceLetterUrl5.ClientID %>' style="margin-left: 6px;" title="View Reference Letter"><i class="fa fa-eye"></i></a>
                    </div>

                    <div class="form-text">
                        This is a letter of recommendation from your current employer or previous employers about your qualities, characteristics, and capabilities.
                        <br />
                        2MB max size each (up to 5 files)
                    </div>
                </div>

            </div>
            <div class="col-6">

                <div class="form-group mb-3">
                    <label class="form-label">
                        Certificate(s) (png, jpg, pdf, docx, doc)
                    </label>

                    <div class="mb-3">
                        <insite:TextBox runat="server" ID="CertificateUrl1" MaxLength="256" Width="240px" EmptyMessage="Certificate URL" ReadOnly="true" style="display: inline;" />
                        <div style="display: none;">
                            <insite:FileUploadV2 runat="server" ID="CertificateUpload1"
                                MaxFileSize="5242880"
                                AllowedExtensions=".png,.jpg,.jpeg,.pdf,.docx,.doc"
                                OnClientFileUploaded="profileDetail.onFileUploaded"
                                OnClientFileUploadFailed="profileDetail.onFileUploadFailed" />
                        </div>
                        <a href="#upload" data-upload='<%= CertificateUpload1.ClientID %>' style="margin-left: 6px;" title="Upload Certificate File"><i class="fa fa-upload"></i></a>
                        <a href="#view" data-view='#<%= CertificateUrl1.ClientID %>' style="margin-left: 6px;" title="View Certificate"><i class="fa fa-eye"></i></a>
                    </div>

                    <div class="mb-3">
                        <insite:TextBox runat="server" ID="CertificateUrl2" MaxLength="256" Width="240px" EmptyMessage="Certificate URL" ReadOnly="true" style="display: inline;" />
                        <div style="display: none;">
                            <insite:FileUploadV2 runat="server" ID="CertificateUpload2"
                                MaxFileSize="5242880" AllowedExtensions=".png,.jpg,.jpeg,.pdf,.docx,.doc"
                                OnClientFileUploaded="profileDetail.onFileUploaded"
                                OnClientFileUploadFailed="profileDetail.onFileUploadFailed" />
                        </div>
                        <a href="#upload" data-upload='<%= CertificateUpload2.ClientID %>' style="margin-left: 6px;" title="Upload Certificate File"><i class="fa fa-upload"></i></a>
                        <a href="#view" data-view='#<%= CertificateUrl2.ClientID %>' style="margin-left: 6px;" title="View Certificate"><i class="fa fa-eye"></i></a>
                    </div>

                    <div class="mb-3">
                        <insite:TextBox runat="server" ID="CertificateUrl3" MaxLength="256" Width="240px" EmptyMessage="Certificate URL" ReadOnly="true" style="display: inline;" />
                        <div style="display: none;">
                            <insite:FileUploadV2 runat="server" ID="CertificateUpload3"
                                MaxFileSize="5242880" AllowedExtensions=".png,.jpg,.jpeg,.pdf,.docx,.doc"
                                OnClientFileUploaded="profileDetail.onFileUploaded"
                                OnClientFileUploadFailed="profileDetail.onFileUploadFailed" />
                        </div>
                        <a href="#upload" data-upload='<%= CertificateUpload3.ClientID %>' style="margin-left: 6px;" title="Upload Certificate File"><i class="fa fa-upload"></i></a>
                        <a href="#view" data-view='#<%= CertificateUrl3.ClientID %>' style="margin-left: 6px;" title="View Certificate"><i class="fa fa-eye"></i></a>
                    </div>

                    <div class="mb-3">
                        <insite:TextBox runat="server" ID="CertificateUrl4" MaxLength="256" Width="240px" EmptyMessage="Certificate URL" ReadOnly="true" style="display: inline;" />
                        <div style="display: none;">
                            <insite:FileUploadV2 runat="server" ID="CertificateUpload4"
                                MaxFileSize="5242880" AllowedExtensions=".png,.jpg,.jpeg,.pdf,.docx,.doc"
                                OnClientFileUploaded="profileDetail.onFileUploaded"
                                OnClientFileUploadFailed="profileDetail.onFileUploadFailed" />
                        </div>
                        <a href="#upload" data-upload='<%= CertificateUpload4.ClientID %>' style="margin-left: 6px;" title="Upload Certificate File"><i class="fa fa-upload"></i></a>
                        <a href="#view" data-view='#<%= CertificateUrl4.ClientID %>' style="margin-left: 6px;" title="View Certificate"><i class="fa fa-eye"></i></a>
                    </div>

                    <div class="mb-3">
                        <insite:TextBox runat="server" ID="CertificateUrl5" MaxLength="256" Width="240px" EmptyMessage="Certificate URL" ReadOnly="true" style="display: inline;" />
                        <div style="display: none;">
                            <insite:FileUploadV2 runat="server" ID="CertificateUpload5"
                                MaxFileSize="5242880" AllowedExtensions=".png,.jpg,.jpeg,.pdf,.docx,.doc"
                                OnClientFileUploaded="profileDetail.onFileUploaded"
                                OnClientFileUploadFailed="profileDetail.onFileUploadFailed" />
                        </div>
                        <a href="#upload" data-upload='<%= CertificateUpload5.ClientID %>' style="margin-left: 6px;" title="Upload Certificate File"><i class="fa fa-upload"></i></a>
                        <a href="#view" data-view='#<%= CertificateUrl5.ClientID %>' style="margin-left: 6px;" title="View Certificate"><i class="fa fa-eye"></i></a>
                    </div>

                    <div class="form-text">
                        If you have completed a program of study or education courses resulting in a degree, diploma, or certificate, you can upload a copy of it here.
                        <br />
                        5MB max size each (up to 5 files)
                    </div>
                </div>

            </div>
        </div>

    </div>
</div>
