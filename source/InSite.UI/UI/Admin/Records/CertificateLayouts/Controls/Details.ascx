<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Details.ascx.cs" Inherits="InSite.UI.Admin.Records.AchievementLayouts.Details" %>

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

<div class="form-group mb-3">
    <label class="form-label">
        Layout Code
        <insite:RequiredValidator runat="server" ControlToValidate="CertificateLayoutCode" FieldName="Layout Code" />
    </label>
    <div>
        <insite:TextBox runat="server" ID="CertificateLayoutCode" Width="100%" MaxLength="100" />
    </div>
    <div class="form-text"></div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Layout Data
        <insite:RequiredValidator runat="server" ControlToValidate="CertificateLayoutData" FieldName="Layout Data" />
    </label>
    <div>
        <insite:TextBox runat="server" ID="CertificateLayoutData" TextMode="MultiLine" Width="100%" CssClass="json-editor" MaxLength="1200" />
    </div>
    <div class="form-text">
        This is a JSON representation of the organization.
        We use
        <a target="_blank" href="https://jsoneditoronline.org/"><i class="far fa-external-link"></i> JSON Editor Online</a>.
    </div>
</div>

<insite:PageFooterContent runat="server">

    <insite:ResourceLink runat="server" Type="JavaScript" Url="/UI/Layout/common/parts/plugins/ace.cloud9/ace.js" />

    <script type="text/javascript">

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
