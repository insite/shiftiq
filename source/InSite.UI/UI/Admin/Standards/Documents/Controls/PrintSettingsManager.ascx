<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PrintSettingsManager.ascx.cs" Inherits="InSite.Admin.Standards.Documents.Controls.PrintSettingsManager" %>

<div class="form-group mb-3">
    <label class="form-label">
        New Saved Settings
    </label>
    <div class="d-inline-block align-middle" style="width:calc(100% - 96px);">
        <insite:TextBox runat="server" ID="NewFilterText" />
    </div>
    <insite:Button runat="server" ID="NewFilterButton" Size="Default"
        ButtonStyle="Default" Icon="fas fa-plus-circle" ToolTip="Add a saved settings" />
    <insite:Button runat="server" ID="SetDefaultButton" Size="Default" Visible="false"
        ButtonStyle="Success" Icon="fas fa-cloud-upload" />
</div>

<div runat="server" id="SavedFilterField" class="form-group mb-3">
    <label class="form-label">
        Load Saved Settings
    </label>
    <div class="d-inline-block align-middle" style="width:calc(100% - 96px);">
        <insite:ComboBox runat="server" ID="SavedFilterSelector" />
    </div>
    <insite:Button runat="server" ID="SaveFilterButton" Size="Default"
        ButtonStyle="Default" Icon="fas fa-save" ToolTip="Save the selected saved settings" />
    <insite:Button runat="server" ID="RemoveFilterButton" Size="Default"
        ConfirmText="Are you sure you want to delete this saved settings?"
        ButtonStyle="Default" Icon="fas fa-trash-alt" ToolTip="Delete the selected saved settings" />
</div>

<insite:PageFooterContent runat="server" ID="Script">
    <script type="text/javascript">
        (function () {
            var $newFilterText, $newFilterButton, $saveFilterButton, $removeFilterButton;

            function onLoad() {
                var $textInput = $('#<%= NewFilterText.ClientID %>');
                if ($textInput.data('inited') === true)
                    return;

                $newFilterText = $textInput.data('inited', true).on('keyup changed', onTextChanged);
                $newFilterButton = $('#<%= NewFilterButton.ClientID %>');
                $saveFilterButton = $('#<%= SaveFilterButton.ClientID %>');
                $removeFilterButton = $('#<%= RemoveFilterButton.ClientID %>');

                onTextChanged();
                onFilterSelected();
            }

            function onTextChanged() {
                var hasText = $newFilterText.val().trim().length > 0;

                enableButton($newFilterButton, hasText);
            }

            function onFilterSelected() {
                var $combo = $('#<%= SavedFilterSelector.ClientID %>');
                if ($combo.length == 1) {
                    var hasValue = !!$combo.selectpicker('val');

                    enableButton($saveFilterButton, hasValue);
                    enableButton($removeFilterButton, hasValue);
                }
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
