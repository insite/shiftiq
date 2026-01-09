<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TranslationWindow.ascx.cs" Inherits="InSite.Admin.Assets.Contents.Controls.TranslationWindow" %>

<%@ Register Src="TranslationControl.ascx" TagName="TranslationControl" TagPrefix="uc" %>

<insite:Modal runat="server" ID="Window" Title="Edit Translation" Width="1000px" MinHeight="500px">
    <ContentTemplate>
        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />
        <insite:UpdatePanel runat="server" ID="UpdatePanel">
            <ContentTemplate>
                <div class="text-end">
                    <insite:SaveButton runat="server" ID="SaveButton" CausesValidation="false" ValidationGroup="TranslationControl" />
                    <insite:CloseButton runat="server" ID="CloseButton" />
                </div>

                <div class="mt-3">
                    <uc:TranslationControl runat="server" ID="TranslationControl" />
                </div>

                <insite:LoadingPanel runat="server" />
            </ContentTemplate>
        </insite:UpdatePanel>
    </ContentTemplate>
</insite:Modal>

<insite:PageFooterContent runat="server" ID="CommonScript">

    <script type="text/javascript">
        (function () {
            var instance = window.translationWindow = window.translationWindow || {};

            instance.open = function (id, key) {
                var wnd = modalManager.show(id);

                $(wnd)
                    .find('.loading-panel').show().end()
                    .find('.modal-body .InSiteUpdatePanel')[0].ajaxRequest(key);
            };

            instance.close = function (id) {
                modalManager.close(id);
            };
        })();
    </script>

</insite:PageFooterContent>