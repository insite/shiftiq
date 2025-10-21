<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FieldOutputContent.ascx.cs" Inherits="InSite.Admin.Messages.Messages.Controls.FieldOutputContent" %>

<div runat="server" id="MessageContentOutput" class="d-none"></div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            init();

            Sys.Application.add_load(() => {
                init();
            });

            function init() {
                const container = document.getElementById('<%= MessageContentOutput.ClientID %>');
                if (container.classList.contains('d-none'))
                    inSite.common.contentToIFrame(container, { disableAnchors: true });
            }
        })();
    </script>
</insite:PageFooterContent>