<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FastProgramSection.ascx.cs" Inherits="InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls.FastProgramSection" %>

<div class="row mb-3">

    <div class="col-12">

        <div class="card">

            <div class="card-body">

                <h4 class="card-title mb-3">FAST Program</h4>

                <div class="row">
                    <div class="col-6">

                        <div runat="server" id="UploadForm" class="form-group mb-3">
                            <label class="form-label">
                                Please upload certificate of completion here
                            </label>
                            <insite:TextBox runat="server" ID="FastDocumentUrl" MaxLength="256" Width="240px" EmptyMessage="Fast Document URL" ReadOnly="true" style="display: inline;" />
                            <div style="display: none;">
                                <insite:FileUploadV2 runat="server" ID="FastDocumentUpload"
                                    MaxFileSize="5242880"
                                    OnClientFileUploaded="profileDetail.onFileUploaded"
                                    OnClientFileUploadFailed="profileDetail.onFileUploadFailed" />
                            </div>
                            <a href="#upload" data-upload='<%= FastDocumentUpload.ClientID %>' class="ms-1" title="Upload Fast Document File"><i class="fa fa-upload"></i></a>
                            <a href="#view" data-view='#<%= FastDocumentUrl.ClientID %>' class="ms-1" title="View Fast Document"><i class="fa fa-eye"></i></a>
                            <div class="form-text">
                                5MB max size
                            </div>
                        </div>

                    </div>

                </div>

            </div>
            
        </div>

    </div>

</div>