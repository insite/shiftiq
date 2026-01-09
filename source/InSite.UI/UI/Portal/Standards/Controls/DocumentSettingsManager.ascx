<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DocumentSettingsManager.ascx.cs" Inherits="InSite.UI.Portal.Standards.Controls.DocumentSettingsManager" %>

<div class="form-group mb-3">
    <label class="form-label">
        <insite:Literal runat="server" Text="New Saved Settings" />
    </label>
    <div>
        <div class="d-inline-block align-middle" style="width:calc(100% - 96px);">
            <insite:TextBox runat="server" ID="NewFilterText" Width="100%" />
        </div>
        <insite:Button runat="server" ID="NewFilterButton" Size="Default"
            ButtonStyle="Default" Icon="fas fa-plus-circle" ToolTip="Add a saved settings" />
        <insite:Button runat="server" ID="SetDefaultButton" Size="Default" Visible="false"
            ButtonStyle="Success" icon="fas fa-cloud-upload" />
    </div>
</div>

<div runat="server" id="SavedFilterField" class="form-group mb-3">
    <label class="form-label">
        <insite:Literal runat="server" Text="Load Saved Settings" />
    </label>
    <div>
        <div class="d-inline-block align-middle" style="width:calc(100% - 96px);">
            <insite:ComboBox runat="server" ID="SavedFilterSelector" Width="100%" />
        </div>
        <insite:Button runat="server" ID="SaveFilterButton" Size="Default"
            ButtonStyle="Default" Icon="fas fa-save" ToolTip="Save the selected saved settings" />
        <insite:Button runat="server" ID="RemoveFilterButton" Size="Default"
            ButtonStyle="Default" Icon="fas fa-trash-alt" ToolTip="Delete the selected saved settings" />
    </div>
</div>

<insite:PageFooterContent runat="server" ID="Script">
    <script type="text/javascript">
        (function () {
            var $newFilterText, $newFilterButton, $saveFilterButton, $removeFilterButton, $savedFilterSelector;

            function onLoad() {
                var $textInput = $('#<%= NewFilterText.ClientID %>');
                if ($textInput.data('inited') === true)
                    return;

                $newFilterText = $textInput.data('inited', true).on('keyup changed', onTextChanged);
                $newFilterButton = $('#<%= NewFilterButton.ClientID %>');
                $saveFilterButton = $('#<%= SaveFilterButton.ClientID %>');
                $removeFilterButton = $('#<%= RemoveFilterButton.ClientID %>');
                $savedFilterSelector = $('#<%= SavedFilterSelector.ClientID %>').on('change', onFilterSelected);

                onTextChanged();
                onFilterSelected();
            }

            function onTextChanged() {
                var hasText = $newFilterText.val().trim().length > 0;

                enableButton($newFilterButton, hasText);
            }

            function onFilterSelected() {
                var hasValue = !!$savedFilterSelector.selectpicker('val');

                enableButton($saveFilterButton, hasValue);
                enableButton($removeFilterButton, hasValue);
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