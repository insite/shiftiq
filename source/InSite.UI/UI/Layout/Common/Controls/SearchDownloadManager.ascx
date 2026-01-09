<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchDownloadManager.ascx.cs" Inherits="InSite.UI.Layout.Common.Controls.SearchDownloadManager" %>

<div class="mb-3">
    <div class="d-inline-block align-middle" style="width:calc(100% - 48px);">
        <insite:TextBox runat="server" ID="NewEntityText" Width="100%" />
    </div>
    <insite:Button runat="server" ID="CreateButton" Size="Default"
        ButtonStyle="Default" Icon="fas fa-plus-circle" ToolTip="Add a saved settings" />
</div>

<div runat="server" id="SavedField" class="mb-3">
    <div style="width:calc(100% - 87px); display:inline-block;">
        <insite:ComboBox runat="server" ID="SettingsSelector" Width="100%" />
    </div>
    <insite:Button runat="server" ID="SaveButton" style="padding:0.625rem;margin-bottom:0.2rem" 
        ButtonStyle="Default" Icon="fas fa-save" ToolTip="Save the selected saved settings" />
    <insite:Button runat="server" ID="RemoveButton" style="padding:0.625rem;margin-bottom:0.2rem"
        ConfirmText="Are you sure you want to delete this saved settings?"
        ButtonStyle="Default" Icon="fas fa-trash-alt" ToolTip="Delete the selected saved settings" />
</div>

<insite:PageFooterContent runat="server" ID="Script">
    <script type="text/javascript">
        (function () {
            var $newText, $createButton, $saveButton, $removeButton, $settingsSelector;

            function onLoad() {
                var $textInput = $('#<%= NewEntityText.ClientID %>');
                if ($textInput.data('inited') === true)
                    return;

                $newText = $textInput.data('inited', true).on('keyup changed', onTextChanged);
                $createButton = $('#<%= CreateButton.ClientID %>');
                $saveButton = $('#<%= SaveButton.ClientID %>');
                $removeButton = $('#<%= RemoveButton.ClientID %>');
                $settingsSelector = $('#<%= SettingsSelector.ClientID %>');

                onTextChanged();
                onSettingsSelected();
            }

            function onTextChanged() {
                var hasText = $newText.val().trim().length > 0;

                enableButton($createButton, hasText);
            }

            function onSettingsSelected() {
                var hasValue = !!$settingsSelector.selectpicker('val');

                enableButton($saveButton, hasValue);
                enableButton($removeButton, hasValue);
            }

            function enableButton($btn, enable) {
                if (enable) {
                    if ($btn.parent().is('span'))
                        $btn.unwrap();

                    $btn.removeClass('disabled');
                } else {
                    $btn.addClass('disabled');

                    if (!$btn.parent().is('span'))
                        $btn.wrap($('<span style="cursor:not-allowed;">').prop('title', $btn.prop('title')));
                }
            }

            Sys.Application.add_load(onLoad);
        })();
    </script>
</insite:PageFooterContent>