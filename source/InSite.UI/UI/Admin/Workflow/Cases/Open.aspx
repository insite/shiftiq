<%@ Page Language="C#" CodeBehind="Open.aspx.cs" Inherits="InSite.Admin.Issues.Forms.Open" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Case" />
    <insite:ValidationSummary runat="server" ValidationGroup="Upload1ValidationGroup" />
    <insite:ValidationSummary runat="server" ValidationGroup="Upload2ValidationGroup" />

    <section class="mb-3">

        <div class="row mb-3">
            <div class="col-lg-5">
                <insite:CreationTypeComboBox runat="server" ID="CreationType" TypeName="Case" />
            </div>
        </div>

        <asp:MultiView runat="server" ID="MultiView">

            <asp:View runat="server" ID="OneView">

                    <div class="row">
                        <div class="col-lg-6">

                            <div class="card border-0 shadow-lg h-100">
                            <div class="card-body">

                                <h3>Details</h3>
                
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Case Type
                                        <insite:RequiredValidator runat="server" ControlToValidate="IssueType" ValidationGroup="Case" />
                                    </label>
                                    <div>
                                        <insite:ItemNameComboBox runat="server" ID="IssueType" AllowBlank="true" />
                                    </div>
                                    <div class="form-text">
                                        The type of case determines the steps in the workflow to manage it.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Case Summary
                                        <insite:RequiredValidator runat="server" ControlToValidate="IssueTitle" ValidationGroup="Case" />
                                    </label>
                                    <div>
                                        <insite:TextBox ID="IssueTitle" runat="server" MaxLength="200" />
                                    </div>
                                    <div class="form-text">A brief summary of the case.</div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Case Description
                                    </label>
                                    <div>
                                        <insite:TextBox runat="server" ID="IssueDescription" TextMode="MultiLine" Rows="5" MaxLength="6400" />
                                    </div>
                                    <div class="form-text">
                                        A detailed description of the case.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Current Status
                                    </label>
                                    <div>
                                        <insite:IssueStatusComboBox runat="server" ID="IssueStatus" />
                                    </div>
                                    <div class="form-text">
                                        The current status of the issue.
                                    </div>
                                </div>

                            </div>
                            </div>
        
                        </div>
                        <div class="col-lg-6">

                            <div class="card border-0 shadow-lg h-100">
                            <div class="card-body">

                                <h3>Contacts</h3>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Case Administrator
                                        <insite:RequiredValidator runat="server" ControlToValidate="AdministratorIdentifier" FieldName="Administrator" ValidationGroup="Case" />
                                    </label>
                                    <div>
                                        <insite:FindPerson runat="server" ID="AdministratorIdentifier" />
                                    </div>
                                    <div class="form-text">
                                        The internal staff responsible for managing the case.
                                    </div>
                                </div>
                
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Member
                                        <insite:RequiredValidator runat="server" ControlToValidate="TopicIdentifier" FieldName="Topic" ValidationGroup="Case" />
                                    </label>
                                    <div>
                                        <insite:FindPerson runat="server" ID="TopicIdentifier" />
                                    </div>
                                    <div runat="server" id="TopicHelp" class="form-text">
                                        The person upon whose behalf the case is raised.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Employer/Referrer
                                        <insite:RequiredValidator runat="server" ControlToValidate="EmployerIdentifier" FieldName="Employer" ValidationGroup="Case" Enabled="false" />
                                    </label>
                                    <div>
                                        <insite:FindGroup runat="server" ID="EmployerIdentifier" />
                                    </div>
                                    <div runat="server" id="EmployerHelp" class="form-text">
                                        The organization that employed or referred the member at the time the issue was reported.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <div class="float-end" runat="server" id="OwnerHelp"></div>
                                    <label class="form-label">
                                        Current Owner
                                    </label>
                                    <div>
                                        <insite:FindPerson runat="server" ID="OwnerIdentifier" />
                                    </div>
                                    <div class="form-text">
                                        The person to whom responsibility is currently assigned.
                                    </div>
                                </div>

                                <hr class="mt-4 mb-3" />

                                <h3>Dates</h3>
                
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Reported
                                    </label>
                                    <div>
                                        <insite:DateTimeOffsetSelector ID="DateReported" runat="server" />
                                    </div>
                                    <div class="form-text">
                                        When the issue was first reported.
                                    </div>
                                </div>

                            </div>
                            </div>
        
                        </div>
                    </div>

                </asp:View>

                <asp:View runat="server" ID="CopyView">

                    <div class="row" >
                        <div class="col-lg-5">

                            <div class="card border-0 shadow-lg h-100">
                            <div class="card-body">

                                <h3>Details</h3>
                
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Case
                                    </label>
                                    <div>
                                        <insite:FindCase runat="server" ID="ExistingCaseIdentifier" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Case Type
                                        <insite:RequiredValidator runat="server" ControlToValidate="DuplicateIssueType" ValidationGroup="Case" />
                                    </label>
                                    <div>
                                        <insite:ItemNameComboBox runat="server" ID="DuplicateIssueType" AllowBlank="true" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Case Summary
                                        <insite:RequiredValidator runat="server" ControlToValidate="DuplicateIssueTitle" ValidationGroup="Case" />
                                    </label>
                                    <div>
                                        <insite:TextBox ID="DuplicateIssueTitle" runat="server" MaxLength="200" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Case Description
                                    </label>
                                    <div>
                                        <insite:TextBox runat="server" ID="DuplicateIssueDescription" TextMode="MultiLine" Rows="5" MaxLength="6400"/>
                                    </div>
                                </div>

                            </div>
                            </div>
        
                        </div>
                        <div class="col-lg-4">

                            <div class="card border-0 shadow-lg h-100">
                            <div class="card-body">

                                <h3>Contacts</h3>
                
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Administrator
                                        <insite:RequiredValidator runat="server" ControlToValidate="DuplicateAdministratorID" FieldName="Administrator" ValidationGroup="Case" />
                                    </label>
                                    <div>
                                        <insite:FindPerson runat="server" ID="DuplicateAdministratorID" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Topic (Member/Account)
                                        <insite:RequiredValidator runat="server" ControlToValidate="DuplicateAssigneeID" FieldName="Member" ValidationGroup="Case" />
                                    </label>
                                    <div>
                                        <insite:FindPerson runat="server" ID="DuplicateAssigneeID" />
                                    </div>
                                    <div class="form-text" runat="server" id="DuplicateAssigneeHelp"></div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Employer
                                        <insite:RequiredValidator runat="server" ControlToValidate="DuplicateEmployerGroup" FieldName="Employer" ValidationGroup="Case" />
                                    </label>
                                    <div>
                                        <insite:FindGroup runat="server" ID="DuplicateEmployerGroup" />
                                    </div>
                                    <div class="form-text" runat="server" id="DuplicateEmployerHelp">School</div>
                                </div>

                            </div>
                            </div>
        
                        </div>
                        <div class="col-lg-3">

                            <div class="card border-0 shadow-lg h-100">
                            <div class="card-body">

                                <h3>Dates</h3>
                
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Reported
                                    </label>
                                    <div>
                                        <insite:DateSelector ID="DuplicateDateReported" runat="server" />
                                    </div>
                                </div>

                            </div>
                            </div>
        
                        </div>
                    </div>

                </asp:View>

                <asp:View runat="server" ID="UploadView">

                    <div>
                        <div class="settings row">
                            <div class="col-md-6">

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Select and Upload Case JSON File
                                        <insite:RequiredValidator runat="server" ControlToValidate="JsonFileUpload" FieldName="JSON File" Display="Dynamic" ValidationGroup="Upload1ValidationGroup" />
                                        <insite:CustomValidator runat="server" ID="JsonFileUploadExtensionValidator"
                                            ControlToValidate="JsonFileUpload" Display="Dynamic" ValidateEmptyText="false" ValidationGroup="Upload1ValidationGroup"
                                            ErrorMessage="Invalid file type. File types supported: .json"
                                            ClientValidationFunction="jsonCreator.ValidateJsonFileUpload" />
                                    </label>
                                    <div>
                                        <div class="input-group">
                                            <insite:TextBox runat="server" ReadOnly="true" style="background-color:#fff;" />
                                            <button class="btn btn-icon btn-outline-secondary" data-upload="#<%= JsonFileUpload.ClientID %>" title="Browse" type="button"><i class="far fa-folder-open"></i></button>
                                            <insite:Button runat="server" ID="JsonFileUploadButton" Size="Default"
                                                ToolTip="Upload" ButtonStyle="OutlineSecondary" Icon="far fa-upload"
                                                ValidationGroup="Upload1ValidationGroup" />
                                        </div>
                                        <div class="d-none">
                                            <asp:FileUpload runat="server" ID="JsonFileUpload" />
                                        </div>
                                    </div>
                                    <div class="form-text"></div>
                                </div>

                            </div>
                            <div class="col-md-6">

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Uploaded JSON
                                        <insite:RequiredValidator runat="server" ControlToValidate="JsonInput" FieldName="Uploaded JSON" Display="Dynamic" ValidationGroup="Upload2ValidationGroup" />
                                    </label>
                                    <div>
                                        <insite:TextBox runat="server" ID="JsonInput" TextMode="MultiLine" Rows="15" />
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>

                </asp:View>

            </asp:MultiView>

    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Case" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

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
                var jsonCreator = window.jsonCreator = window.jsonCreator || {};

                jsonCreator.ValidateJsonFileUpload = function (s, e) {
                    if (!e.Value)
                        return;

                    var ext = '';
                    var index = e.Value.lastIndexOf('.');
                    if (index > 0)
                        ext = e.Value.substring(index).toLowerCase();

                    e.IsValid = ext === '.json';
                };
            })();

        </script>
    </insite:PageFooterContent>

</asp:Content>
