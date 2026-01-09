<%@ Page Language="C#" CodeBehind="Upload.aspx.cs" Inherits="InSite.Admin.Assessments.Attempts.Forms.Upload" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/UploadAttemptList.ascx" TagName="UploadAttemptList" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Step1" />
    <insite:ValidationSummary runat="server" ValidationGroup="Step2" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="AttemptTab" Title="Attempt" Icon="far fa-tasks" IconPosition="BeforeText">
            <div class="row">
            
                <div class="col-lg-6 mb-3 mb-lg-0">
                    <div class="card border-0 shadow-lg">
                        <div class="card-body">

                            <h3>Exam Event</h3>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Event Number
                                </label>
                                <insite:FindEvent runat="server" ID="EventIdentifier" />
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Exam Form
                                    <insite:RequiredValidator runat="server" ControlToValidate="FormIdentifier" FieldName="Exam Form" ValidationGroup="Step1" Display="Dynamic" />
                                </label>
                                <insite:FindBankForm runat="server" ID="FormIdentifier" />
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Attempt Graded
                                    <insite:RequiredValidator runat="server" ControlToValidate="AttemptGraded" FieldName="Attempt Graded" ValidationGroup="Step1" Display="Dynamic" />
                                </label>
                                <insite:DateTimeOffsetSelector ID="AttemptGraded" runat="server" />
                            </div>

                            <insite:Container runat="server" ID="EventInfoFields" Visible="false">
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Class/Session Code
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="ClassSessionCode" />
                                    </div>
                                </div>
                            
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Venue
                                    </label>
                                    <div class="mb-2">
                                        <asp:Literal runat="server" ID="VenueName" />
                                    </div>
                                    <div>
                                        <asp:Literal runat="server" ID="VenueRoom" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Scheduled Start Time
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="StartTime" />
                                    </div>
                                </div>
                            </insite:Container>
                            
                        </div>
                    </div>
                </div>

                <div runat="server" id="ExamCandidateCol" class="col-lg-6" visible="false">
                    <div class="card border-0 shadow-lg">
                        <div class="card-body">

                            <h3>Candidates / Learners / Students</h3>

                            <asp:Repeater runat="server" ID="ExamCandidateRepeater">
                                <HeaderTemplate>
                                    <table class="table table-striped table-bordered">
                                    <tr><th>Name</th><th>Email</th></tr>
                                </HeaderTemplate>
                                <FooterTemplate></table></FooterTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <a href='/ui/admin/contacts/people/edit?contact=<%# Eval("CandidateIdentifier") %>'><%# Eval("Candidate.UserFullName") %></a>
                                            <span class="form-text"><%# Eval("Candidate.PersonCode") %></span>
                                        </td>
                                        <td>
                                            <a href="mailto:<%# Eval("Candidate.UserEmail") %>">
                                                <%# Eval("Candidate.UserEmail") %>
                                            </a>
                                            
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            
                        </div>
                    </div>
                </div>

            </div>

            <div class="mt-3">
                <insite:NextButton runat="server" ID="Step1NextButton" ValidationGroup="Step1" CausesValidation="true" />
                <insite:CancelButton runat="server" ID="Step1CancelButton" />
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="UploadTab" Title="Upload" Icon="far fa-upload" IconPosition="BeforeText" Visible="false">
            <div class="row mt-3">
                <div class="col-lg-6 mb-3 mb-lg-0">
                    <div class="card border-0 shadow-lg">
                        <div class="card-body">

                            <h3>Attempts</h3>
                            
                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Upload File Format
                                    <insite:RequiredValidator runat="server" ControlToValidate="UploadFileFormat" FieldName="Upload File Format" Display="Dynamic" ValidationGroup="Step2" />
                                </label>
                                <insite:ComboBox runat="server" ID="UploadFileFormat" AllowBlank="false" />
                            </div>

                            <div runat="server" id="UploadFileFormatTypeField" class="form-group mb-3" visible="false">
                                <label runat="server" id="UploadFileFormatTypeLabel">
                                </label>
                                <insite:ComboBox runat="server" ID="UploadFileFormatType" AllowBlank="false" />
                            </div>

                            <div runat="server" id="FileUploadField" class="form-group mb-3">
                                <label class="form-label">
                                    <span runat="server" id="FileUploadLabel"></span>
                                    <insite:RequiredValidator runat="server" ControlToValidate="FileUpload" FieldName="Upload File" Display="Dynamic" ValidationGroup="Step2" />
                                    <insite:CustomValidator runat="server" ID="FileUploadExtensionValidator" ErrorMessage="Invalid file type"
                                        ControlToValidate="FileUpload" Display="Dynamic" ValidateEmptyText="false" ValidationGroup="Step2"
                                        ClientValidationFunction="uploadForm.validateFileUpload" />
                                </label>
                                <div>
                                    <div class="input-group">
                                        <insite:TextBox runat="server" ReadOnly="true" style="background-color:#fff;" />
                                        <button class="btn btn-outline-secondary btn-icon" data-upload="#<%= FileUpload.ClientID %>" title="Browse" type="button"><i class="far fa-folder-open"></i></button>
                                        <insite:Button runat="server" ID="FileUploadButton" ToolTip="Upload" ButtonStyle="OutlineSecondary" Size="Default" Icon="far fa-upload"
                                            CausesValidation="true" ValidationGroup="Step2" />
                                    </div>
                                    <div class="d-none">
                                        <asp:FileUpload runat="server" ID="FileUpload" />
                                    </div>
                                </div>
                            </div>
                            
                        </div>
                    </div>
                </div>

                <div class="col-md-6">
                    <asp:MultiView runat="server" ID="InstructionMultiView">

                        <asp:View runat="server" ID="InstructionCsvView">
                            <div class="card border-0 shadow-lg">
                                <div class="card-body">

                                    <h3>Instructions</h3>

                                    <div class="form-group mb-3">
                                        <label class="form-label">Follow these steps to upload your exam attempt.</label>
                                        <div>
                                            <ul>
                                                <li>Your upload file format must be Comma Separated Values (CSV). You can save any Excel spreadsheet as a CSV file.</li>
                                                <li>Your file must contain titles on every column.</li>
                                                <li>If dates are being uploaded (i.e., attempt completed), ensure the date format in the CSV file is DD/MM/YYYY.</li>
                                                <li><a href="/UI/Admin/Assessments/Attempts/Content/Upload-Attempt.csv" target="_blank">Click here to download a template</a></li>
                                            </ul>
                                            <p>Here is a simple example:</p>
                                            <table class="table table-striped table-bordered">
                                                <tr><td>Candidate Code</td><td>Candidate Name</td><td>1</td><td>2</td><td>3</td><td>...</td><td>100</td></tr>
                                                <tr><td>13579</td><td>BROWN CHARLIE</td><td>D</td><td>B</td><td>A</td><td>...</td><td>C</td></tr>
                                                <tr><td>24680</td><td>VAN PELT LUCY</td><td>A</td><td>B</td><td>D</td><td>...</td><td>D</td></tr>
                                            </table>
                                        </div>
                                    </div>

                                </div>
                            </div>
                        </asp:View>

                        <asp:View runat="server" ID="InstructionScantronView">
                            <div class="card border-0 shadow-lg">
                                <div class="card-body">

                                    <h3>Instructions</h3>

                                    <div class="form-group mb-3">
                                        <label class="form-label">Follow these steps to upload your exam attempt.</label>
                                        <div>
                                            <ul>
                                                <li>Your upload file format must be a text file output by Scantron configured to use answer sheet template #994.</li>
                                                <li>
                                                    This is a text file with fixed-width columns, defined as follows:
                                                    <ol>
                                                        <li>Scantron Reference: characters 1 - 40</li>
                                                        <li>Candidate Code: characters 41 - 49</li>
                                                        <li>Candidate Date: characters 50 - 59</li>
                                                        <li>Candidate Name: characters 60 - 82</li>
                                                        <li>Answers: characters 83 - 183</li>
                                                    </ol>
                                                </li>
                                                <li><a href="/UI/Admin/Assessments/Attempts/Content/Upload-Attempt.txt" target="_blank">Click here to download a template</a></li>
                                            </ul>
                                            <p>Here is a simple example:</p>
                                            <table class="table table-striped table-bordered">
                                                <tr><td>Scantron Reference</td><td>Code</td><td>Date</td><td>Name</td><td>Answers</td></tr>
                                                <tr><td>994000006001042419001   5383     1    N </td><td>54321</td><td>2019/03/01</td><td>BROWN CHARLIE</td><td>BBADCDBBBBCBA...</td></tr>
                                                <tr><td>994000007001042419001   5383     1    N </td><td>00246</td><td>2019/03/01</td><td>VAN PELT LUCY</td><td>ABABBDCBBBACD...</td></tr>
                                            </table>
                                        </div>
                                    </div>

                                </div>
                            </div>
                        </asp:View>

                        <asp:View runat="server" ID="InstructionLxrMergeView">

                        </asp:View>

                        <asp:View runat="server" ID="InstructionSummaryView">
                            <div class="card border-0 shadow-lg">
                                <div class="card-body">

                                    <h3>Uploaded File Summary</h3>

                                    <div class="form-group mb-3">
                                        <ul>
                                            <asp:Repeater runat="server" ID="SummaryRepeater">
                                                <ItemTemplate>
                                                    <li><%# Eval("Text") %>: <strong><%# Eval("Value") %></strong></li>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </ul>
                                    </div>

                                    <asp:Repeater runat="server" ID="WarningRepeater">
                                        <HeaderTemplate>
                                            <div class="alert alert-warning" role="alert"><i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong><ul>
                                        </HeaderTemplate>
                                        <FooterTemplate></ul></div></FooterTemplate>
                                        <ItemTemplate>
                                            <li><%# Container.DataItem %></li>
                                        </ItemTemplate>
                                    </asp:Repeater>

                                </div>
                            </div>
                        </asp:View>

                    </asp:MultiView>

                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="DataTab" Title="Attempt Records" Icon="far fa-poll-people" IconPosition="BeforeText" Visible="false">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">

                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UploadAttemptListUpdatePanel" />
                    <insite:UpdatePanel runat="server" ID="UploadAttemptListUpdatePanel">
                        <ContentTemplate>
                            <uc:UploadAttemptList runat="server" ID="UploadAttemptList" />
                        </ContentTemplate>
                    </insite:UpdatePanel>

                </div>
            </div>

            <div class="mt-3">
                <insite:SaveButton runat="server" ID="SaveButton" Text="Save Attempts"
                    OnClientClick="uploadForm.confirmationWindow.show(); return false;" />
            </div>
        </insite:NavItem>

    </insite:Nav>

    <insite:Modal runat="server" ID="ConfirmationWindow" Title="Confirmation" Width="950px">
        <ContentTemplate>
            <div class="p-2">
                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="ConfirmationUpdatePanel" />

                <insite:UpdatePanel runat="server" ID="ConfirmationUpdatePanel">
                    <ContentTemplate>
                        <p>
                            <asp:Literal runat="server" ID="ConfirmTitle" />
                        </p>

                        <div class="mt-3">
                            <asp:Repeater runat="server" ID="SelectedAttemptRepeater">
                                <HeaderTemplate>
                                    <table class="table table-striped table-bordered">
                                        <thead>
                                            <tr>
                                                <th>Exam Candidate</th>
                                                <th>Completed</th>
                                                <th>Status</th>
                                                <th class="text-end">Score</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <%# Eval("LearnerName") %>
                                            <span class='form-text'><%# Eval("LearnerCode") %></span>
                                        </td>
                                        <td>
                                            <insite:DateTimeOffsetSelector runat="server" ID="AttemptGraded" Value='<%# Eval("AttemptGraded") %>' />
                                            <asp:Literal runat="server" ID="AttemptSequence" Text='<%# Eval("Sequence") %>' Visible="false" />
                                        </td>
                                        <td>
                                            <span class='<%# !(bool)Eval("HasUserAccount") ? "contact-not-found text-danger" : "contact-found text-success" %>'>
                                                <%# !(bool)Eval("HasUserAccount") ? "(No User Account)" : "Import" %>
                                            </span>
                                            <span class='<%# !(bool)Eval("HasEventRegistration") ? "contact-not-found text-danger" : "contact-found text-success" %>'>
                                                <%# !(bool)Eval("HasEventRegistration") ? "(No Event Registration)" : "Import" %>
                                            </span>
                                            <span runat="server" class="text-warning" visible='<%# Eval("HasAttemptMatch") != null %>'>
                                                (Duplicate)
                                            </span>
                                        </td>
                                        <td class="text-end">
                                            <%# Eval("Percent", "{0:p0}") %>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                        </tbody>
                                    </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>

                        <div class="mt-3">
                            <insite:CheckBox runat="server" ID="ConfirmAllowDuplicates" Text="Allow duplicate upload" Checked="true" />
                        </div>
                    </ContentTemplate>
                </insite:UpdatePanel>

                <insite:UpdatePanel runat="server" ID="ConfirmButtonsUpdatePanel" CssClass="mt-3" ChildrenAsTriggers="false" UpdateMode="Conditional">
                    <Triggers>
                        <asp:PostBackTrigger ControlID="ConfirmSaveButton" />
                    </Triggers>
                    <ContentTemplate>
                        <insite:Button runat="server" ID="ConfirmSaveButton" Text="OK" ButtonStyle="Success" Icon="fas fa-check" DisableAfterClick="true" />
                        <insite:CancelButton runat="server" OnClientClick="uploadForm.confirmationWindow.close(); return false;" />
                    </ContentTemplate>
                </insite:UpdatePanel>
            </div>
        </ContentTemplate>
    </insite:Modal>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">

            (function () {
                $('[data-upload]').each(function () {
                    var $btn = $(this);
                    var uploadSelector = $btn.data('upload');
                    $(uploadSelector).on('change', function () {
                        var fileName = '';

                        if (this.files) {
                            if (this.files.length > 0) {
                                fileName = this.files[0].name;
                            }
                        } else if (this.value) {
                            fileName = this.value.split(/(\\|\/)/g).pop();
                        }

                        $btn.closest('.input-group').find('input[type="text"]').val(fileName);
                    });
                }).on('click', function () {
                    var uploadSelector = $(this).data('upload');
                    $(uploadSelector).click();
                });
            })();

            (function () {
                var instance = window.uploadForm = window.uploadForm || {};

                instance.confirmationWindow = {
                    show: function () {
                        if (!validateAllAttempts()) {
                            alert("Errors should be fixed before saving.");
                            return;
                        }

                        modalManager.show('<%= ConfirmationWindow.ClientID %>');
                        document.getElementById('<%= ConfirmationUpdatePanel.ClientID %>').ajaxRequest();
                    },
                    close: function () {
                        modalManager.close('<%= ConfirmationWindow.ClientID %>');
                    },
                };

                instance.validateFileUpload = function (s, e) {
                    if (!e.Value)
                        return;

                    var exts = <%= FormatExtensionsJson %>;
                    if (!exts)
                        return;

                    e.IsValid = false;

                    var index = e.Value.lastIndexOf('.');
                    if (index < 0)
                        return;

                    var ext = e.Value.substring(index).toLowerCase();
                    for (var i = 0; i < exts.length; i++) {
                        if (exts[i] === ext) {
                            e.IsValid = true;
                            break;
                        }
                    }
                };
            })();

        </script>
    </insite:PageFooterContent>

</asp:Content>