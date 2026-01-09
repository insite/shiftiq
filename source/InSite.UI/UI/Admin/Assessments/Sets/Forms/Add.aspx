<%@ Page Language="C#" CodeBehind="Add.aspx.cs" Inherits="InSite.Admin.Assessments.Sets.Forms.Add" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Import Namespace="Shift.Common" %>

<%@ Register Src="~/UI/Admin/Assessments/Banks/Controls/BankInfo.ascx" TagName="BankDetails" TagPrefix="uc" %>
<%@ Register TagPrefix="uc" TagName="QuestionRepeater" Src="../../Questions/Controls/QuestionRepeater.ascx" %>
<%@ Register TagPrefix="assessments" Assembly="InSite.UI" Namespace="InSite.Admin.Assessments.Web.UI" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="CreatorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Assessment" />
    <insite:ValidationSummary runat="server" ValidationGroup="UploadMarkdown" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-th-list"></i>
            Add New Question Set
        </h2>

        <div class="row">

            <div class="col-lg-6 mb-3 mb-lg-0">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Question Bank</h3>
                        <uc:BankDetails runat="server" ID="BankDetails" />
                    </div>
                </div>
            </div>

            <div class="col-lg-6">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h3>Question Set</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Set Name
                            <insite:RequiredValidator runat="server" ControlToValidate="SetName" ValidationGroup="Assessment" />
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="SetName" Text="Default" />
                            </div>
                            <div class="form-text">
                                The name that uniquely identifies this question set within the bank.
                            </div>
                        </div>

                        <div runat="server" id="UploadColumn">
                            
                            <h3>Upload Question Items</h3>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    File Format
                                </label>
                                <div>
                                    <insite:ComboBox runat="server" ID="FileFormat" AllowBlank="false" />
                                    <asp:RadioButtonList runat="server" ID="FileEncoding" RepeatDirection="Vertical" RepeatLayout="Table" CssClass="mt-2">
                                        <asp:ListItem Value="65001" Text="UTF-8" Selected="True" />
                                        <asp:ListItem Value="1252" Text="Windows-1252" />
                                    </asp:RadioButtonList>
                                </div>
                                <div runat="server" id="MarkdownFileFormatHelp" class="form-text">
                                    Download an example here:
                                    <a target="_blank" href="/ui/admin/assessments/sets/samples/markdown.txt" download="markdown.txt">Shift iQ Markdown Sample</a>
                                </div>
                                <div runat="server" id="LxrFileFormatHelp" class="form-text" style="display: none;">
                                    Download an example here:
                                    <a target="_blank" href="/ui/admin/assessments/sets/samples/lxrmerge.txt" download="lxr-merge.txt">LXR Merge Sample</a>
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    <asp:Label runat="server" ID="UploadLabel" />
                                    <insite:RequiredValidator runat="server" ControlToValidate="FileUpload" FieldName="Upload File" Display="Dynamic" ValidationGroup="UploadMarkdown" />
                                    <insite:CustomValidator runat="server" ID="MarkdownFileUploadExtensionValidator"
                                        ControlToValidate="FileUpload" Display="Dynamic" ValidateEmptyText="false" ValidationGroup="UploadMarkdown"
                                        ErrorMessage="Invalid file type. File types supported: .md, .txt"
                                        ClientValidationFunction="setCreator.ValidateMarkdownFileUpload" />
                                    <insite:CustomValidator runat="server" ID="ICEMSAnswerKeyFileUploadExtensionValidator"
                                        ControlToValidate="FileUpload" Display="Dynamic" ValidateEmptyText="false" ValidationGroup="UploadMarkdown"
                                        ErrorMessage="Invalid file type. File types supported: .xml"
                                        ClientValidationFunction="setCreator.ValidateICEMSAnswerKeyFileUpload" />
                                    <insite:CustomValidator runat="server" ID="LxrMergeFileUploadExtensionValidator"
                                        ControlToValidate="FileUpload" Display="Dynamic" ValidateEmptyText="false" ValidationGroup="UploadMarkdown"
                                        ErrorMessage="Invalid file type. File types supported: .lxrmerge, .txt"
                                        ClientValidationFunction="setCreator.ValidateLxrMergeFileUpload" />
                                </label>
                                <div>
                                    <div class="input-group">
                                        <asp:TextBox runat="server" ReadOnly="true" CssClass="form-control" />
                                        <button class="btn btn-default border btn-icon" data-upload="#<%= FileUpload.ClientID %>" title="Browse" type="button"><i class="far fa-folder-open"></i></button>
                                        <asp:LinkButton runat="server" ID="UploadButton"
                                            ToolTip="Upload" CssClass="btn btn-default border btn-icon"
                                            CausesValidation="true" ValidationGroup="UploadMarkdown"><i class="far fa-upload"></i></asp:LinkButton>
                                    </div>
                                    <div class="d-none">
                                        <asp:FileUpload runat="server" ID="FileUpload" />
                                    </div>
                                </div>
                                <div class="form-text">
                                    Click browse to select a file on your computer, then click the Upload button.
                                </div>
                            </div>
                        </div>

                        <div runat="server" id="QuestionsColumn">
                            <h3 runat="server" id="QuestionsColumnHeader"></h3>

                            <div class="text-end" style="margin-bottom: 10px;">
                                <insite:SaveButton runat="server" ID="SaveButton2" ValidationGroup="Assessment" />
                                <insite:Button runat="server" ID="ClearUploadButton" ToolTip="Clear Questions" ButtonStyle="Default" Text="Clear Questions" Icon="far fa-undo" />
                            </div>

                            <uc:QuestionRepeater runat="server" ID="QuestionRepeater" />

                            <asp:Repeater runat="server" ID="SetRepeater">
                                <ItemTemplate>

                                    <h3><%# Eval("Name") %></h3>

                                    <assessments:AssetTitleDisplay runat="server"
                                        AssetID='<%# Eval("Standard") %>'
                                        Format="<div class='alert alert-warning'><i class='fas fa-clipboard-list'></i> {0}</div>"
                                        Visible='<%# (Guid)Eval("Standard") != Guid.Empty %>' />

                                    <uc:QuestionRepeater runat="server" ID="QuestionRepeater" />

                                </ItemTemplate>
                            </asp:Repeater>
                        </div>


                    </div>

                </div>

            </div>
        </div>

    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton1" ValidationGroup="Assessment" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

<insite:PageHeadContent runat="server">
    <style type="text/css">

        table.table-question {
        }

            table.table-question > tbody > tr > td.qnum {
                width: 40px;
                text-align: right;
            }

            table.table-question > tbody > tr > td.qtxt table.table-option {
            }

                table.table-question > tbody > tr > td.qtxt table.table-option > tbody > tr > td {
                    border-top: none;
                }

                    table.table-question > tbody > tr > td.qtxt table.table-option > tbody > tr > td.onum {
                        width: 30px;
                        color: #919191;
                    }

    </style>
</insite:PageHeadContent>

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
            var setCreator = window.setCreator = window.setCreator || {};
            var validExtsMarkdown = <%= JsonHelper.SerializeJsObject(FormatInfo.ShiftiQMarkdown.Extensions) %>;
            var validExtsICEMSAnswerKey = <%= JsonHelper.SerializeJsObject(FormatInfo.ICEMSAnswerKey.Extensions) %>;
            var validExtsLxrMerge = <%= JsonHelper.SerializeJsObject(FormatInfo.LxrMerge.Extensions) %>;

            setCreator.ValidateMarkdownFileUpload = function (s, e) {
                e.IsValid = validateFileExtension(e.Value, validExtsMarkdown);
            };

            setCreator.ValidateICEMSAnswerKeyFileUpload = function (s, e) {
                e.IsValid = validateFileExtension(e.Value, validExtsICEMSAnswerKey);
            };

            setCreator.ValidateLxrMergeFileUpload = function (s, e) {
                e.IsValid = validateFileExtension(e.Value, validExtsLxrMerge);
            };

            var $fileFormat = $('#<%= FileFormat.ClientID %>').on('change', onFileFormatChange);

            $(onFileFormatChange);

            function onFileFormatChange() {
                const value = $fileFormat.selectpicker('val');
                const isMarkdown = value === '<%= FormatInfo.ShiftiQMarkdown.ID %>';
                const isLxrMerge = value === '<%= FormatInfo.LxrMerge.ID %>';
                const isIcemsAnswerKey = value === '<%= FormatInfo.ICEMSAnswerKey.ID %>';

                var labelText = isMarkdown
                    ? '<%= FormatInfo.ShiftiQMarkdown.Label %>'
                    : isLxrMerge
                        ? '<%= FormatInfo.LxrMerge.Label %>'
                        : '<%= FormatInfo.ICEMSAnswerKey.Label %>';

                $('#<%= UploadLabel.ClientID %>').html(labelText);
                $('#<%= MarkdownFileFormatHelp.ClientID %>').toggle(isMarkdown);
                $('#<%= LxrFileFormatHelp.ClientID %>').toggle(isLxrMerge);

                ValidatorEnable(document.getElementById('<%= MarkdownFileUploadExtensionValidator.ClientID %>'), isMarkdown);
                ValidatorEnable(document.getElementById('<%= ICEMSAnswerKeyFileUploadExtensionValidator.ClientID %>'), isIcemsAnswerKey);
                ValidatorEnable(document.getElementById('<%= LxrMergeFileUploadExtensionValidator.ClientID %>'), isLxrMerge);
            }

            function validateFileExtension(value, validExts) {
                if (!value)
                    return true;

                var ext = '';
                var index = value.lastIndexOf('.');
                if (index > 0)
                    ext = value.substring(index).toLowerCase();

                for (var i = 0; i < validExts.length; i++) {
                    if (validExts[i] === ext)
                        return true;
                }

                return false;
            }
        })();

    </script>
</insite:PageFooterContent>
</asp:Content>
