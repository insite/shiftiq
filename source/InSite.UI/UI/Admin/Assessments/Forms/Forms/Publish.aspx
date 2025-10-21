<%@ Page Language="C#" CodeBehind="Publish.aspx.cs" Inherits="InSite.Admin.Assessments.Forms.Forms.Publish" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="assessments" Assembly="InSite.UI" Namespace="InSite.Admin.Assessments.Web.UI" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ID="AssessmentValidationSummary" ValidationGroup="Assessment" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-window"></i>
            Form
        </h2>
        <div class="row">

            <div class="col-lg-12">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-6">

                                <h3>Publication</h3>

                                <div class="form-group mb-3" runat="server" id="PublicationOptionField">
                                    <label class="form-label">Action</label>
                                    <div>
                                        <asp:RadioButtonList runat="server" ID="PublicationOption">
                                            <asp:ListItem Value="Unpublish" Text="Unpublish Form" Selected="true" />
                                            <asp:ListItem Value="Republish" Text="Republish Form" />
                                        </asp:RadioButtonList>
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Standard</label>
                                    <div>
                                        <assessments:AssetTitleDisplay runat="server" ID="FormStandard" />
                                    </div>
                                    <div class="form-text">
                                        The standard evaluated by questions on the form.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Form Title
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="FormTitle" />
                                    </div>
                                    <div class="form-text">
                                        The exam form to be published.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Asset Number and Version</label>
                                    <div>
                                        <asp:Literal runat="server" ID="Version" />
                                    </div>
                                    <div class="form-text">
                                        The inventory asset number (and version) for this form.
                                    </div>
                                </div>

                                <div class="form-group mb-3" runat="server" id="CandidateFeedbackField">
                                    <label class="form-label">Learner/Assessor Feedback</label>
                                    <insite:RequiredValidator runat="server" ControlToValidate="AllowFeedbackFromCandidates" FieldName="Candidate Feedback" ValidationGroup="Assessment" />
                                    <div>
                                        <insite:ComboBox runat="server" ID="AllowFeedbackFromCandidates" Width="200px">
                                            <Items>
                                                <insite:ComboBoxOption Selected="true" />
                                                <insite:ComboBoxOption Text="Enabled" Value="True" />
                                                <insite:ComboBoxOption Text="Disabled" Value="False" />
                                            </Items>
                                        </insite:ComboBox>
                                    </div>
                                    <div class="form-text">
                                        Allow exam candidates to submit feedback on the questions in this exam.
                                    </div>
                                </div>

                                <div class="form-group mb-3" runat="server" id="InstructorRationaleField">
                                    <label class="form-label">Instructor Rationale</label>
                                    <div>
                                        <insite:CheckBox ID="AllowRationaleForCorrectAnswers" runat="server" Text="Show for Correct Answers" />
                                        <insite:CheckBox ID="AllowRationaleForIncorrectAnswers" runat="server" Text="Show for Incorrect Answers" />
                                    </div>
                                    <div class="form-text">
                                        Show (to candidates) the rationale behind the questions in this exam for correct and/or incorrect answers.
                                    </div>
                                </div>

                                <div class="form-group mb-3" runat="server" id="DownloadAssessmentResultsField">
                                    <label class="form-label">Download Assessment Results</label>
                                    <div>
                                        <insite:CheckBox ID="AllowDownloadAssessmentsQA" runat="server" Text="Allow download of Assessment Questions and Answers" />
                                    </div>
                                    <div class="form-text">
                                        Allow exam candidate to download the Assessment Questions and Answers after their response is submitted.
                                    </div>
                                </div>

                                <div class="form-group mb-3" runat="server" id="FirstPublishedField">
                                    <label class="form-label">First Published</label>
                                    <div>
                                        <insite:DateTimeOffsetSelector runat="server" ID="FirstPublished" Width="300" />
                                    </div>
                                </div>

                                <insite:Container runat="server" ID="AccessField">
                                    <h3>Access
                                            <sup class="text-danger"><i class="far fa-asterisk fa-xs"></i></sup>
                                        <insite:CustomValidator runat="server" ID="AccessFieldValidator" ClientValidationFunction="publish.validateAccess" ValidationGroup="Assessment" ErrorMessage="Access: Please select one." Display="None" />
                                    </h3>

                                    <div class="form-group mb-3">
                                        <div>
                                            <div class="answer-container">
                                                <insite:RadioButton runat="server" ID="PortalAccessModule" GroupName="PortalAccess" Text="<strong>Training Program</strong><br><div class='form-text'>The exam is delivered in the context of a specific e-learning module.</div>" />
                                            </div>
                                            <div class="answer-container">
                                                <insite:RadioButton runat="server" ID="PortalAccessStandalone" GroupName="PortalAccess" Text="<strong>Standalone</strong><br><div class='form-text'>The exam is delivered directly to candidates.</div>" />
                                            </div>
                                        </div>
                                    </div>
                                </insite:Container>

                                <div class="form-group mb-3" runat="server" id="PermissionListPanel" visible="false">
                                    <label class="form-label">Permission Lists</label>
                                    <div>
                                        <asp:CheckBoxList runat="server" ID="PermissionList" DataValueField="GroupIdentifier" DataTextField="GroupName" />
                                    </div>
                                    <div class="form-text">
                                        Who should be able to see the program tile for this exam form in the portal? 
                                            If you leave all of these boxes unchecked then it will be visible to everyone.
                                    </div>
                                </div>

                            </div>
                            <div class="col-md-6">

                                <div runat="server" id="ProgramTilePanel" visible="true">

                                    <h3>Tile</h3>

                                    <div class="form-group mb-3">
                                        <label class="form-label">Tag</label>
                                        <div>
                                            <insite:TextBox runat="server" ID="ProgramTileLabel" Text="" />
                                        </div>
                                        <div class="form-text">
                                            The text to be used for the program tile on the portal home page.
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">Icon</label>
                                        <div>
                                            <insite:TextBox runat="server" ID="ProgramTileIcon" Text="far fa-check-double" MaxLength="30"/>
                                        </div>
                                        <div class="form-text">
                                            The <a href="https://fontawesome.com/icons?d=gallery" target="_blank">FontAwesome icon</a> to be used for the program tile on the portal home page.
                                        </div>
                                    </div>

                                </div>

                                <div runat="server" id="DeveloperPanel" visible="false">

                                    <h3>InSite Administration</h3>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Publication Container
                                        </label>
                                        <div>
                                            <asp:Repeater runat="server" ID="AssessmentRepeater">
                                                <ItemTemplate>
                                                    <%# Eval("Title") %>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                            <asp:Label runat="server" ID="AssessmentMissing" CssClass="text-danger">
                                                    This form is not yet published in an assessment.
                                            </asp:Label>
                                        </div>
                                        <div class="form-text">
                                            The <strong>Page</strong> used to deliver the published assessment form in the Portal.
                                        </div>
                                    </div>

                                    <div class="alert alert-info">
                                        Please note this panel is visible only for testing and troubleshooting purposes.
                                    </div>

                                    <div runat="server" id="FormTranslationsField" class="form-group mb-3">
                                        <label class="form-label">
                                            Translations
                                        </label>
                                        <div>
                                            <insite:CheckBoxList runat="server" ID="FormTranslations" RepeatDirection="Horizontal" />
                                        </div>
                                        <div class="form-text">
                                            Selecting a language here is for labeling purposes only. It does not translate the form automatically.
                                        </div>
                                    </div>

                                </div>

                                <div runat="server" id="PreviousVersionsPanel" visible="false">
                                    <h3>Previous Versions</h3>

                                    <div class="form-group mb-3">
                                        <div>
                                            <ul>
                                                <asp:Repeater runat="server" ID="PreviousVersions">
                                                    <ItemTemplate>
                                                        <li>
                                                            <a href='<%# "/ui/admin/assessments/banks/outline?bank=" + Eval("Form.Specification.Bank.Identifier") + "&form=" + Eval("Form.Identifier") %>'>
                                                                <%# Eval("Form.Asset") %>.<%# Eval("Form.AssetVersion") %>
                                                            </a>
                                                            <asp:Label runat="server" ID="PublicationStatus" />
                                                        </li>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </ul>
                                        </div>
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

            <div class="alert alert-warning">
                <i class="fas fa-exclamation-triangle"></i><strong>Warning:</strong>
                <asp:Literal runat="server" ID="Instruction" />
            </div>

            <insite:Button runat="server" ID="PublishButton" Text="Publish" Icon="fas fa-cloud-upload" ValidationGroup="Assessment" ButtonStyle="Success" />
            <insite:Button runat="server" ID="RepublishButton" Text="Republish" Icon="fas fa-cloud-upload" ValidationGroup="Assessment" ButtonStyle="Success" />
            <insite:Button runat="server" ID="UnpublishButton" Text="Unpublish" Icon="fas fa-eraser" ValidationGroup="Assessment" ButtonStyle="Danger" />
            <insite:CancelButton runat="server" ID="CancelButton" />

        </div>
    </div>


    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            (function () {
                var instance = window.publish = window.publish || {};

                instance.validateAccess = function (s, e) {
                    var portalAccessModule = document.getElementById('<%= PortalAccessModule.ClientID %>');
                    var portalAccessStandalone = document.getElementById('<%= PortalAccessStandalone.ClientID %>');

                    if (portalAccessModule && portalAccessStandalone)
                        e.IsValid = portalAccessModule.checked || portalAccessStandalone.checked;
                };
            })();
        </script>
    </insite:PageFooterContent>

</asp:Content>
