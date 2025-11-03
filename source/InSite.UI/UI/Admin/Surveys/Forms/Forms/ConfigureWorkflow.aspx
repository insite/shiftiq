<%@ Page Language="C#" CodeBehind="ConfigureWorkflow.aspx.cs" Inherits="InSite.Admin.Surveys.Forms.Forms.ConfigureWorkflow" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/SurveyFormInfo.ascx" TagName="SurveyDetails" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Survey" />

    <section runat="server" id="NameSection" class="mb-3">
        
        <h2 class="h4 mb-3"><i class="far fa-cogs me-2"></i>Survey Workflow</h2>

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <div class="row">
                    <div class="col-md-6">
                        <h3>Survey Details</h3>
                        <uc:SurveyDetails runat="server" ID="SurveyDetail" />
                    </div>
                    <div class="col-md-6">
                        <div>

                            <h3>Email Notifications</h3>

                            <div class="form-group mb-3">
                                <label class="form-label">Survey Invitation</label>
                                <div>
                                    <insite:FindMessage runat="server" ID="SurveyInvitationMessageIdentifier" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Response Started (Administrator)</label>
                                <div>
                                    <insite:FindMessage runat="server" ID="ResponseStartedAdministratorMessageIdentifier" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Response Completed (Administrator)</label>
                                <div>
                                    <insite:FindMessage runat="server" ID="ResponseCompletedAdministratorMessageIdentifier" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Response Completed (Respondent)</label>
                                <div>
                                    <insite:FindMessage runat="server" ID="ResponseCompletedRespondentMessageIdentifier" />
                                </div>
                            </div>

                            <h3 class="mt-4">Case Workflow</h3>

                            <asp:Panel runat="server" ID="OpenIssueEnabledWarning" class="alert alert-warning" Visible="false">
                                When Case Workflows are enabled for a survey, you must select the Create Case checkbox on at least one question.
                                A case will only be created if the respondent answers that question.
                            </asp:Panel>

                            <div class="form-group mb-3">
                                <div>
                                    <insite:CheckBox runat="server" ID="OpenIssueEnabled" Text="Open a new case when a response to this survey is completed" />
                                </div>
                            </div>

                            <insite:Container runat="server" ID="IssueWorkflowContainer">

                                <div class="form-group mb-3">
                                    <label class="form-label">Case Type</label>
                                    <div>
                                        <insite:ItemNameComboBox runat="server" ID="IssueType" AllowBlank="false" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Case Status</label>
                                    <div>
                                        <insite:IssueStatusComboBox runat="server" ID="IssueStatus" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Case Administrator</label>
                                    <div>
                                        <insite:FindPerson runat="server" ID="IssueAdministratorIdentifier" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Case Owner</label>
                                    <div>
                                        <insite:FindPerson runat="server" ID="IssueOwnerIdentifier" />
                                    </div>
                                </div>

                            </insite:Container>

                        </div>
                    </div>
                </div>
            </div>
        </div>

    </section>

    <div>
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Survey" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>
