<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.Admin.Standards.Collections.Forms.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/Detail.ascx" TagName="Detail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="CreatorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Collection" />

    <section class="mb-3">

        <h2 class="h4 mb-3">
            <i class="far fa-box-open"></i>
            Collection
        </h2>
        <div class="row mb-3">
            <div class="col-lg-6">
                <insite:CreationTypeComboBox runat="server" ID="CreationType" TypeName="Collections" />
            </div>
        </div>

        <asp:MultiView runat="server" ID="MultiView">

            <asp:View runat="server" ID="OneView">

                <div class="row">
                    <div class="col-md-6">

                        <div class="card border-0 shadow-lg">
                            <div class="card-body">

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Title
                                    <insite:RequiredValidator runat="server" ControlToValidate="NewTitle" FieldName="NewTitle" ValidationGroup="Collection" />
                                    </label>
                                    <div>
                                        <insite:TextBox runat="server" ID="NewTitle" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Tag
                                    <insite:RequiredValidator runat="server" ControlToValidate="NewLabel" FieldName="NewLabel" ValidationGroup="Collection" />
                                    </label>
                                    <div>
                                        <insite:TextBox runat="server" ID="NewLabel" MaxLength="100" />
                                    </div>
                                </div>

                            </div>
                        </div>

                    </div>
                </div>

            </asp:View>

            <asp:View runat="server" ID="UploadView">

                <div class="row">

                    <div class="col-md-6">

                        <div class="card border-0 shadow-lg h-100">
                            <div class="card-body">

                                <insite:FileUploadV1 runat="server"
                                    ID="CreateUploadFile"
                                    LabelText="Select and Upload JSON File"
                                    FileUploadType="Unlimited"
                                    OnClientFileUploaded="collectionCreate.onFileUploaded"
                                />
                                <asp:Button runat="server" ID="UploadFileUploaded" CssClass="d-none" />
                            </div>

                        </div>
                    </div>

                    <div class="col-md-6">

                        <div class="card border-0 shadow-lg">
                            <div class="card-body">

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Uploaded JSON
                                        <insite:RequiredValidator runat="server" ControlToValidate="UploadJsonInput" FieldName="Uploaded JSON" Display="Dynamic" ValidationGroup="Collection" />
                                    </label>
                                    <insite:TextBox runat="server" ID="UploadJsonInput" TextMode="MultiLine" Rows="15" />
                                    <div class="form-text">
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>

            </asp:View>

            <asp:View runat="server" ID="CopyView">

                <div class="row">

                    <div class="col-md-6">

                        <div class="card border-0 shadow-lg">
                            <div class="card-body">

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Collection
                                    <insite:RequiredValidator runat="server" ID="StandardComboBoxValidator" ControlToValidate="StandardComboBox" FieldName="Collection" ValidationGroup="Collection" />
                                    </label>
                                    <div>
                                        <insite:FindStandard runat="server" ID="StandardComboBox" />
                                    </div>
                                    <div class="form-text">This collection is used for copy.</div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Title
                                    <insite:RequiredValidator runat="server" ControlToValidate="CopyTitle" FieldName="CopyTitle" ValidationGroup="Collection" />
                                    </label>
                                    <div>
                                        <insite:TextBox runat="server" ID="CopyTitle" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Tag
                                    <insite:RequiredValidator runat="server" ControlToValidate="CopyLabel" FieldName="CopyLabel" ValidationGroup="Collection" />
                                    </label>
                                    <div>
                                        <insite:TextBox runat="server" ID="CopyLabel" MaxLength="100" />
                                    </div>
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
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Collection" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">

            (function () {
                var collectionCreate = window.collectionCreate = window.collectionCreate || {};

                collectionCreate.onFileUploaded = function () {
                    __doPostBack('<%= UploadFileUploaded.UniqueID %>', '')
                };
            })();

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
                var gradebookCreator = window.gradebookCreator = window.gradebookCreator || {};

                gradebookCreator.ValidateJsonFileUpload = function (s, e) {
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
