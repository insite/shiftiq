<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.Admin.Sites.Sites.Forms.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Site" />

    <insite:ValidationSummary runat="server" ValidationGroup="WebSite" />
    <insite:ValidationSummary runat="server" ValidationGroup="Outline" />
    <insite:ValidationSummary runat="server" ValidationGroup="Review" />
    <insite:ValidationSummary runat="server" ValidationGroup="UploadMarkdown" />
    <insite:ValidationSummary runat="server" ValidationGroup="UploadJson" />
    <insite:ValidationSummary runat="server" ValidationGroup="Upload2ValidationGroup" />

    <section class="mb-3">
        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <h2 class="h4 mb-3">
                    <i class="far fa-calendar-alt me-1"></i>
                    New Site
                </h2>
                <div class="row mb-3">
                    <div class="col-lg-4">

                        <div class="form-group mb-3">
                            <insite:CreationTypeComboBox runat="server" ID="CreationType" TypeName="Site" />
                        </div>
                    </div>

                    <asp:MultiView runat="server" ID="MultiView">

                        <asp:View runat="server" ID="OneView">
                            <div class="row">

                                <div class="col-md-6">
                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Title
                                            <insite:RequiredValidator runat="server" ControlToValidate="SingleTitle" Display="Dynamic" FieldName="Title" ValidationGroup="Site" />
                                        </label>
                                        <div>
                                            <insite:TextBox ID="SingleTitle" runat="server" MaxLength="128" />
                                        </div>
                                        <div class="form-text">
                                            A descriptive title that uniquely identifies this web site.
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Domain
                                            <insite:RequiredValidator runat="server" ControlToValidate="SingleName" Display="Dynamic" FieldName="Domain" ValidationGroup="Site" />
                                        </label>
                                        <div>
                                            <insite:TextBox ID="SingleName" runat="server" MaxLength="128" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </asp:View>

                        <asp:View runat="server" ID="CopyView">
                            <div class="row">

                                <div class="col-md-6">
                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Existing Sites
                                            <insite:RequiredValidator runat="server" ControlToValidate="SiteComboBox" Display="Dynamic" FieldName="Existing Sites" ValidationGroup="Site" />
                                        </label>
                                        <div>
                                            <insite:SiteComboBox runat="server" ID="SiteComboBox" />
                                        </div>
                                    </div>
                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Title
                                            <insite:RequiredValidator runat="server" ControlToValidate="CopyTitle" Display="Dynamic" FieldName="Title" ValidationGroup="Site" />
                                        </label>
                                        <div>
                                            <insite:TextBox ID="CopyTitle" runat="server" MaxLength="128" />
                                        </div>
                                        <div class="form-text">
                                            A descriptive title that uniquely identifies this web site.
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Domain
                                            <insite:RequiredValidator runat="server" ControlToValidate="CopyName" Display="Dynamic" FieldName="Domain" ValidationGroup="Site" />
                                        </label>
                                        <div>
                                            <insite:TextBox ID="CopyName" runat="server" MaxLength="128" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </asp:View>

                        <asp:View runat="server" ID="UploadView">

                            <div class="row">
                                <div class="col-lg-4">
                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            File Type
                                        </label>
                                        <div>
                                            <insite:ComboBox runat="server" ID="UploadFileType">
                                                <Items>
                                                    <insite:ComboBoxOption Text="JSON" Value="JSON" Selected="true" />
                                                </Items>
                                            </insite:ComboBox>
                                        </div>
                                    </div>

                                    <insite:FileUploadV1 runat="server"
                                        ID="CreateUploadFile"
                                        LabelText="Select and Upload JSON File"
                                        FileUploadType="Unlimited"
                                        OnClientFileUploaded="siteCreate.onFileUploaded" />
                                    <asp:Button runat="server" ID="UploadFileUploaded" CssClass="d-none" />
                                </div>

                                <div class="col-lg-8">

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Uploaded JSON
                                            <insite:RequiredValidator runat="server"
                                                ControlToValidate="UploadJsonInput"
                                                FieldName="Uploaded JSON"
                                                Display="Dynamic"
                                                ValidationGroup="Site"
                                            />
                                        </label>
                                        <insite:TextBox runat="server" ID="UploadJsonInput" TextMode="MultiLine" Rows="15" AllowHtml="true" />
                                    </div>

                                </div>
                            </div>

                        </asp:View>

                    </asp:MultiView>

                </div>

            </div>
        </div>
    </section>

    <section class="mb-3" runat="server" visible="false" id="PreviewSection">
        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <h4 class="card-title mb-3">
                    <i class="far fa-calendar-alt me-1"></i>
                    Review
                </h4>

                <div class="row">
                    <div class="col-md-12">
                        <asp:Repeater runat="server" ID="TreeViewRepeater">
                            <ItemTemplate>
                                <%# Eval("HtmlPrefix") %>
                                <div>
                                    <div>
                                        <div class="node-title">
                                            <span class='text'>
                                                <%# Eval("Icon") == null ? string.Empty : Eval("Icon", "<i class='align-middle {0}'></i>") %>
                                                <span><%# Eval("Title") %></span>
                                            </span>
                                        </div>
                                        <div class="node-inputs node-inputs-sm">
                                            <insite:WebPageTypeComboBox runat="server" ID="TypeSelector" Width="150px" AllowBlank="false" ButtonSize="Small" />
                                        </div>
                                    </div>
                                </div>
                                <%# Eval("HtmlPostfix") %>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>

            </div>
        </div>
    </section>

    <section>
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Site" />
        <insite:NextButton runat="server" ID="NextButton" ValidationGroup="Site" Visible="false" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </section>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">

            (function () {
                var siteCreate = window.siteCreate = window.siteCreate || {};

                siteCreate.onFileUploaded = function () {
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
                var instance = window.creator = window.creator || {};

                instance.ValidateMarkdownFileUpload = function (s, e) {
                    if (!e.Value)
                        return;

                    var ext = '';
                    var index = e.Value.lastIndexOf('.');
                    if (index > 0)
                        ext = e.Value.substring(index).toLowerCase();

                    e.IsValid = ext === '.txt' || ext === '.zip';
                };

                instance.ValidateJsonFileUpload = function (s, e) {
                    if (!e.Value)
                        return;

                    var ext = '';
                    var index = e.Value.lastIndexOf('.');
                    if (index > 0)
                        ext = e.Value.substring(index).toLowerCase();

                    e.IsValid = ext === '.json' || ext === '.zip';
                };
            })();
        </script>
    </insite:PageFooterContent>
</asp:Content>
