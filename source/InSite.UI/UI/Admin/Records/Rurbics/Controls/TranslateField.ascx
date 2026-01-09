<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TranslateField.ascx.cs" Inherits="InSite.UI.Admin.Records.Rurbics.Controls.TranslateField" %>

<div class="row pe-5 me-1" data-type="field-translate">
    <div class="col-lg-6 mb-3">
        <div class="form-group">
            <label class="form-label">
                Original <asp:Literal runat="server" ID="FieldNameOriginal" ViewStateMode="Disabled" />
            </label>
            <div runat="server" ID="OriginalValue" enableviewstate="false" class="output-translate border py-2 px-3 rounded-2 lh-lg"></div>
        </div>
    </div>
    <div class="col-lg-6 mb-3">
        <div class="form-group">
            <label class="form-label">
                Translated <asp:Literal runat="server" ID="FieldNameTranslated" ViewStateMode="Disabled" />
            </label>
            <div class="position-absolute end-0 btn-translate">
                <insite:Button runat="server" ID="TranslateButton" Icon="fas fa-language" ButtonStyle="OutlineSecondary" Size="Default" ToolTip="Translate" />
            </div>
            <div runat="server" ID="TranslatedValue" enableviewstate="false" class="output-translate border py-2 px-3 rounded-2 lh-lg bg-secondary-subtle"></div>
        </div>
    </div>
</div>

<insite:PageHeadContent runat="server" ID="CommonStyle">
    <style type="text/css">
        .btn-translate {
            margin-right: var(--ar-card-spacer-x);
        }

        .output-translate {
            white-space: pre-line;
        }
    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server" ID="CommonScript">
    <script type="text/javascript">
        (function () {
            window.addEventListener('resize', onWindowResize);
            document.addEventListener("DOMContentLoaded", onWindowResize);

            Sys.Application.add_load(onWindowResize);

            onWindowResize();

            function onWindowResize() {
                const fields = document.querySelectorAll('[data-type="field-translate"]');
                for (let i = 0; i < fields.length; i++) {
                    const outputs = fields[i].querySelectorAll('.output-translate');
                    const fromOutput = outputs[0];
                    const toOutput = outputs[1];

                    fromOutput.style.minHeight = null;
                    toOutput.style.minHeight = null;
                    
                    const fromHeight = fromOutput.offsetHeight;
                    const toHeight = toOutput.offsetHeight;

                    if (fromHeight > toHeight)
                        toOutput.style.minHeight = fromHeight + 'px';
                    else
                        fromOutput.style.minHeight = toHeight + 'px';
                }
            }
        })();
    </script>
</insite:PageFooterContent>