<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.Admin.Records.Programs.EnrollmentRequestOutline" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />

    <section class="pb-5 mb-md-2">

        <div class="row">

            <div class="col-lg-8 mb-3">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h3>Request</h3>

                        <div class="row">

                            <div class="col-md-6">

                                <div class="form-group mb-3">
                                    <label class="form-label">Status</label>
                                    <div><asp:Label runat="server" ID="RequestStatus" /></div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Program</label>
                                    <div><asp:Literal runat="server" ID="ProgramName" /></div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Funding path</label>
                                    <div><asp:Literal runat="server" ID="RequestFundingPath" /></div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Fee</label>
                                    <div><asp:Literal runat="server" ID="RequestFee" /></div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Submitted</label>
                                    <div><asp:Literal runat="server" ID="RequestSubmittedAt" /></div>
                                </div>

                            </div>

                            <div class="col-md-6">

                                <div class="form-group mb-3">
                                    <label class="form-label">Decided</label>
                                    <div><asp:Literal runat="server" ID="RequestDecidedAt" /></div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Decided by</label>
                                    <div><asp:Literal runat="server" ID="RequestDecidedBy" /></div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Funder contact</label>
                                    <div><asp:Literal runat="server" ID="RequestFunderEmail" /></div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Request identifier</label>
                                    <div class="font-monospace"><asp:Literal runat="server" ID="RequestIdentifier" /></div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Enrollment identifier</label>
                                    <div class="font-monospace"><asp:Literal runat="server" ID="ProgramEnrollmentIdentifier" /></div>
                                </div>

                            </div>

                        </div>

                    </div>
                </div>

            </div>

            <div class="col-lg-4 mb-3">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h3>Learner</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">Name</label>
                            <div><asp:Literal runat="server" ID="LearnerName" /></div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Email</label>
                            <div><asp:Literal runat="server" ID="LearnerEmail" /></div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Organization</label>
                            <div><asp:Literal runat="server" ID="OrganizationName" /></div>
                        </div>

                    </div>
                </div>

            </div>

        </div>

        <div runat="server" id="CompanySponsoredTerms" class="alert alert-info mb-0" visible="false">
            <i class="fas fa-file-contract me-2"></i>
            Program fees are non-refundable. Once you approve a request, the learner is enrolled, the fee is added to
            <asp:Literal runat="server" ID="CompanySponsoredTermsOrganization" />'s quarterly program billing, and the fee
            cannot be refunded, transferred, or credited, including if the learner withdraws from the program or does not
            complete it.
        </div>

        <div runat="server" id="SelfFundedTerms" class="alert alert-warning mb-0" visible="false">
            <i class="fas fa-money-check-alt me-2"></i>
            Note: Please ensure payment has been collected before approving this request.
        </div>

        <div class="mt-3">
            <insite:Button runat="server" ID="ApproveButton" Text="Approve" Icon="fas fa-thumbs-up" ButtonStyle="Success" />
            <insite:Button runat="server" ID="RejectButton" Text="Reject" Icon="fas fa-thumbs-down" ButtonStyle="Danger" ConfirmText="Reject this enrollment request?" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>

    </section>

</asp:Content>
