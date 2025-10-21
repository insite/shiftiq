<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="JsonEditor.ascx.cs" Inherits="InSite.UI.Admin.Reports.Controls.JsonEditor" %>

<insite:TextBox runat="server" ID="Input" TextMode="MultiLine" Rows="6" />

<insite:PageHeadContent runat="server" ID="CommonStyle">

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

<insite:PageFooterContent runat="server" ID="CommonScript">

    <insite:ResourceLink runat="server" Type="JavaScript" Url="/UI/Layout/common/parts/plugins/ace.cloud9/ace.js" />

    <script type="text/javascript">
        (function () {
            var instance = window.reportJsonEditor = window.reportJsonEditor || {};
            var initPropName = 'inited_' + String(Date.now());

            instance.init = function (id) {
                var input = document.querySelector('textarea#' + String(id));
                if (input == null || input[initPropName] === true)
                    return;

                input.style.display = 'none';

                var editor = document.createElement('DIV');

                var wrapper = document.createElement('DIV');
                wrapper.className = 'json-editor';
                wrapper.appendChild(editor);

                input.parentNode.insertBefore(wrapper, input);

                var editor = ace.edit(editor, {
                    minLines: 15
                });

                editor.$blockScrolling = Infinity;
                editor.setFontSize(15);
                editor.setShowPrintMargin(false);
                editor.session.setMode('ace/mode/json');

                editor.session.setValue(input.value);
                editor.session.on('change', function () {
                    input.value = editor.session.getValue();
                });

                input[initPropName] = true;
            };
        })();

    </script>

</insite:PageFooterContent>