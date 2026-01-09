<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.Admin.Workflow.Forms.CreateSurveyForm" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="CreatorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="BlankValidationGroup" />
    <insite:ValidationSummary runat="server" ValidationGroup="Copy" />
    <insite:ValidationSummary runat="server" ValidationGroup="Upload1ValidationGroup" />
    <insite:ValidationSummary runat="server" ValidationGroup="Upload2ValidationGroup" />

    <section runat="server" id="SurveySection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-check-square me-1"></i>
            Form
        </h2>

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">

                <div class="row mb-3">
                    <div class="col-md-6">
                        <insite:CreationTypeComboBox runat="server" ID="CreationType" TypeName="Form" />
                    </div>
                </div>

                <asp:Panel runat="server" ID="NewSection">

                    <div class="row">
                        <div class="col-md-6">
                            <div>
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Internal Name
                                        <insite:RequiredValidator runat="server" ControlToValidate="Name" FieldName="Internal Name" Display="Dynamic" ValidationGroup="BlankValidationGroup" />
                                        <insite:CustomValidator runat="server" ID="NameDuplicateValidator"
                                            ControlToValidate="Name"
                                            ErrorMessage="The system contains multiple forms having the same name. Please give each form a unique name."
                                            Display="Dynamic"
                                            ValidationGroup="BlankValidationGroup" />
                                    </label>
                                    <div>
                                        <insite:TextBox runat="server" ID="Name" MaxLength="256" />
                                    </div>
                                    <div class="form-text">
                                        The internal name is used as an internal reference for filing the form. It is required field, and it is not visible to the form respondent.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        External Title
                                    </label>
                                    <div>
                                        <insite:TextBox runat="server" ID="SurveyTitle" MaxLength="200" />
                                    </div>
                                    <div class="form-text">
                                        This is the form title displayed to the form respondent.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Language
                                    </label>
                                    <div>
                                        English
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-md-6">
                            <div>
                                <div class="form-group mb-3">
                                    <label class="form-label">Limit Submissions per Respondent</label>
                                    <div>
                                        <insite:ComboBox runat="server" ID="IsSubmissionsLimitedSelector" CssClass="w-50">
                                            <Items>
                                                <insite:ComboBoxOption Value="True" Text="Limited" />
                                                <insite:ComboBoxOption Value="False" Text="Not Limited" />
                                            </Items>
                                        </insite:ComboBox>
                                    </div>
                                    <div class="form-text">
                                        If you want to ensure each respondent answers the form no more than once then select Limited.
                                    </div>
                                </div>

                                <div runat="server" id="EnableAnonymousSaveGroup" class="form-group mb-3" style="display: none;">
                                    <label class="form-label">Allow Anonymous Submissions</label>
                                    <div>
                                        <insite:BooleanComboBox runat="server" ID="EnableAnonymousSave" TrueText="Permitted" FalseText="Not Permitted" AllowBlank="false" CssClass="w-50"/>
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Confidentiality for Respondents</label>
                                    <div>
                                        <insite:BooleanComboBox runat="server" ID="EnableUserConfidentiality" TrueText="Enabled" FalseText="Disabled" AllowBlank="false" CssClass="w-50" />
                                    </div>
                                    <div class="form-text">
                                        Enable this setting if you do not want to disclose confidential/personal information 
                                        about form respondents to form administrators.
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>

                    <div>
                        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="BlankValidationGroup" />
                        <insite:CancelButton runat="server" ID="CancelButton" />
                    </div>

                </asp:Panel>

                <asp:Panel runat="server" ID="CopySection">
                    <div class="row">
                        <div class="col-md-6">
                            <div>
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Form
                                        <insite:RequiredValidator runat="server" ID="CopySurveyFormValidator" ControlToValidate="CopySurveyFormSelector" FieldName="Form" ValidationGroup="Copy" />
                                    </label>
                                    <div>
                                        <insite:FindWorkflowForm runat="server" ID="CopySurveyFormSelector" />
                                    </div>
                                    <div class="form-text">This form is use for copy.</div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Internal Name
                                        <insite:RequiredValidator runat="server" ControlToValidate="CopyName" FieldName="Internal Name" Display="Dynamic" ValidationGroup="Copy" />
                                        <insite:CustomValidator runat="server" ID="CopyNameDuplicateValidator"
                                            ControlToValidate="CopyName"
                                            ErrorMessage="The system contains multiple forms having the same name. Please give each form a unique name."
                                            Display="Dynamic"
                                            ValidationGroup="Copy" />
                                    </label>
                                    <div>
                                        <insite:TextBox runat="server" ID="CopyName" MaxLength="256" />
                                    </div>
                                    <div class="form-text">
                                        The internal name is used as an internal reference for filing the form. It is required field, and it is not visible to the form respondent.
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-lg-12">
                            <insite:SaveButton runat="server" ID="CopyButton" ValidationGroup="Copy" />
                            <insite:CancelButton runat="server" ID="CopyCancelButton" />
                        </div>
                    </div>
                </asp:Panel>

                <asp:Panel runat="server" ID="UploadSection">

                    <div class="row settings">
                        <div class="col-md-5">

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Select and Upload JSON File
                                    <insite:RequiredValidator runat="server" ControlToValidate="JsonFileUpload" FieldName="JSON File" Display="Dynamic" ValidationGroup="Upload1ValidationGroup" />
                                    <insite:CustomValidator runat="server" ID="JsonFileUploadExtensionValidator"
                                        ControlToValidate="JsonFileUpload" Display="Dynamic" ValidateEmptyText="false" ValidationGroup="Upload1ValidationGroup"
                                        ErrorMessage="Invalid file type. File types supported: .json"
                                        ClientValidationFunction="surveyCreator.ValidateJsonFileUpload" />
                                </label>
                                <div>
                                    <div class="input-group">
                                        <insite:TextBox runat="server" ReadOnly="true" CssClass="bg-white" />
                                        <button class="btn btn-icon btn-outline-secondary" data-upload="#<%= JsonFileUpload.ClientID %>" title="Browse" type="button"><i class="far fa-folder-open"></i></button>
                                        <insite:Button runat="server" ID="JsonFileUploadButton" Size="Default"
                                            ToolTip="Upload" ButtonStyle="OutlineSecondary" Icon="far fa-upload"
                                            CausesValidation="true" ValidationGroup="Upload1ValidationGroup" />
                                    </div>
                                    <div class="d-none">
                                        <asp:FileUpload runat="server" ID="JsonFileUpload" />
                                    </div>
                                </div>
                            </div>

                        </div>
                        <div class="col-md-7">

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Uploaded JSON
                                    <insite:RequiredValidator runat="server" ControlToValidate="JsonInput" FieldName="Uploaded JSON" Display="Dynamic" ValidationGroup="Upload2ValidationGroup" />
                                </label>
                                <div>
                                    <insite:TextBox runat="server" ID="JsonInput" TextMode="MultiLine" Rows="15" AllowHtml="true" />
                                </div>
                            </div>

                        </div>
                    </div>

                    <div>
                        <insite:SaveButton runat="server" ID="UploadSaveButton" ValidationGroup="Upload2ValidationGroup" />
                        <insite:CancelButton runat="server" ID="UploadCancelButton" />
                    </div>

                </asp:Panel>

            </div>
        </div>
    </section>

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
                var surveyCreator = window.surveyCreator = window.surveyCreator || {};
    
                surveyCreator.ValidateJsonFileUpload = function (s, e) {
                    if (!e.Value)
                        return;
    
                    var ext = '';
                    var index = e.Value.lastIndexOf('.');
                    if (index > 0)
                        ext = e.Value.substring(index).toLowerCase();
    
                    e.IsValid = ext === '.json';
                };
            })();
    
            (function () {
                Sys.Application.add_load(function () {
                    $('#<%= IsSubmissionsLimitedSelector.ClientID %>')
                        .off('change', onIsSubmissionsLimitedChanged)
                        .on('change', onIsSubmissionsLimitedChanged);
    
                    onIsSubmissionsLimitedChanged();
                });
    
                function onIsSubmissionsLimitedChanged() {
                    var $combo = $('#<%= IsSubmissionsLimitedSelector.ClientID %>');
                        var $field = $('#<%= EnableAnonymousSaveGroup.ClientID %>');
                    if ($combo.selectpicker('val') === 'True') {
                        $field.hide();
                    } else {
                        $field.show();
                    }
                }
            })();
    
        </script>
    </insite:PageFooterContent>

</asp:Content>
