<%@ Page Title="Jobs | Candidates | Opportunities | View" Language="C#" CodeBehind="Apply.aspx.cs" Inherits="InSite.UI.Portal.Jobs.Candidates.Apply" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="StatusAlert" />

    <asp:CustomValidator runat="server" ID="ItemsValidator" ErrorMessage="Minimum 2 and maximum 10 duties should be specified" ValidationGroup="Profile" Display="Dynamic" />

    <asp:Label runat="server" ID="Instructions" />

    <div runat="server" id="Content" class="row"></div>

    <div class="row mb-3">

        <div class="col-12">

            <div class="card">

                <div class="card-body">

                    <h4 class="card-title mb-3"><asp:Literal runat="server" ID="JobTitle" /></h4>

                    <div class="row">
                        <div class="col-6">
                            <div class="row mb-3">
                                <h5 class="mt-2 mb-1">
                                    Your name
                                </h5>
                                <div>
                                    <asp:Literal runat="server" ID="CandidateName" />
                                </div>
                            </div>

                            <div class="row mb-3">
                                <h5 class="mt-2 mb-1">Your phone number
                                    <insite:RequiredValidator runat="server" FieldName="Industry" ControlToValidate="CandidatePhoneNumber" ValidationGroup="Profile" Display="Dynamic" />
                                </h5>
                                <div>
                                    <insite:TextBox runat="server" ID="CandidatePhoneNumber" MaxLength="100" EmptyMessage="Please specify your phone number" />
                                </div>
                            </div>

                            <div class="row mb-3">
                                <h5 class="mt-2 mb-1">Your email address
                                    <insite:RequiredValidator runat="server" FieldName="Industry" ControlToValidate="CandidateEmail" ValidationGroup="Profile" Display="Dynamic" />
                                </h5>
                                <div>
                                    <insite:TextBox runat="server" ID="CandidateEmail" MaxLength="100" EmptyMessage="Please specify your email" />
                                </div>
                            </div>

                        </div>
                        <div class="col-6">

                            <div class="row mb-3">
                                <h5 class="mt-2 mb-1">Resume
                                    <span runat="server" id="ResumeRequired">
                                        <sup class="text-danger"><i class="far fa-asterisk fa-xs"></i></sup>
                                    </span>
                                    <insite:CustomValidator runat="server" ID="ResumeUploadValidator"
                                        Display="Dynamic" ValidateEmptyText="false" ValidationGroup="Profile"
                                        ErrorMessage="Invalid file type. File types supported: .pdf .doc .docx .txt" />

                                </h5>
                                <div class="mb-2">
                                    <asp:FileUpload runat="server" ID="ResumeUpload" />
                                </div>
                                <div class="form-text">
                                    Attach a custom resume for this application (pdf, doc, docx, txt). 2MB max size.
                                </div>
                                
                            </div>

                            <div class="row mb-3">
                                <h5 class="mt-2 mb-1">Cover letter
                                    <span runat="server" id="CoverLetterRequired">
                                        <sup class="text-danger"><i class="far fa-asterisk fa-xs"></i></sup>
                                    </span>
                                    <insite:CustomValidator runat="server" ID="CoverLetterUploadValidator"
                                        ControlToValidate="CoverLetterUpload" Display="Dynamic" ValidateEmptyText="false" ValidationGroup="Profile"
                                        ErrorMessage="Invalid file type. File types supported: .pdf .doc .docx .txt" />
                                </h5>
                                <div class="mb-2">
                                    <asp:FileUpload runat="server" ID="CoverLetterUpload" />
                                </div>
                                <div class="form-text">
                                    Attach a custom cover letter for this application (pdf, doc, docx, txt). 2MB max size.
                                </div>
                                
                            </div>

                        </div>
                    </div>

                </div>
            
            </div>

        </div>

    </div>

    <div class="row">
        <div class="col-10">
            <p runat="server" id="NotApproved" visible='<%# !IsUserApproved %>'>
                You can apply when your profile is approved.
            </p>
            <insite:Button runat="server" ButtonStyle="Default" Icon="fas fa-arrow-left" Text="Back"
                NavigateUrl="/ui/portal/job/candidates/opportunities/search" />
            <insite:Button runat="server" ID="SubmitButton" ButtonStyle="Success" Text="Submit" Icon="fas fa-cloud-upload" 
                NavigateUrl='<%# "/ui/portal/job/candidates/opportunities/apply?id=" + Eval("JobIdentifier") %>' 
                Visible='<%# IsUserApproved %>' />
        </div>
    </div>

</asp:Content>
