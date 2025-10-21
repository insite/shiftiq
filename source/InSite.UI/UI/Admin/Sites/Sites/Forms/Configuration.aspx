<%@ Page Language="C#" CodeBehind="Configuration.aspx.cs" Inherits="InSite.Admin.Sites.Sites.Forms.Configuration" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

<insite:PageHeadContent runat="server">

    <style type="text/css">
        div.json-editor {
            height: 500px;
        }

        div.json-editor > div {
            padding: 6px 12px;
            border: 1px solid #ccc;
            border-radius: 4px;
            width: 100%;
            height: 100%;
        }
    </style>

</insite:PageHeadContent>

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="SiteSetupChange" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-cloud me-1"></i>
            Site
        </h2>
        <div class="row">

            <div class="col-lg-12">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Details</h3>
                        <insite:TextBox runat="server" ID="ConfigurationText" TextMode="MultiLine"  Width="100%" CssClass="json-editor" />
                        <asp:CustomValidator runat="server" ID="ConfigurationTextValidator" ControlToValidate="ConfigurationText" Display="None" ErrorMessage="Configuration JSON is invalid" ValidationGroup="SiteSetupChange" />
                    </div>
                </div>
            </div>
        </div>

    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="SiteSetupChange" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

<insite:PageFooterContent runat="server">

    <insite:ResourceLink runat="server" Type="JavaScript" Url="/UI/Layout/common/parts/plugins/ace.cloud9/ace.js" />

    <script>

        (function () {
            formatJson();

            function formatJson() {
                $('textarea.json-editor').each(function () {
                    var $input = $(this).hide();
                    var $wrapper = $('<div class="json-editor">');
                    var $editor = $('<div>');

                    $wrapper.insertAfter($input);
                    $wrapper.append($editor);

                    var editor = ace.edit($editor[0], {
                        minLines: 15
                    });

                    editor.$blockScrolling = Infinity;
                    editor.setFontSize(15);
                    editor.setShowPrintMargin(false);
                    editor.session.setMode('ace/mode/json');

                    editor.session.setValue($input.val());
                    editor.session.on('change', function () {
                        $input.val(editor.session.getValue());
                    });
                });
            }
        })();

    </script>

</insite:PageFooterContent>


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
