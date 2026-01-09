<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteriaFilterManager.ascx.cs" Inherits="InSite.UI.Layout.Common.Controls.SearchCriteriaFilterManager" %>

<div class="mb-3">
    <div class="d-inline-block align-middle" style="width:calc(100% - 48px);">
        <insite:TextBox runat="server" ID="NewFilterText" Width="100%" />
    </div>
    <insite:Button runat="server" ID="NewFilterButton" Size="Default"
        ButtonStyle="Default" Icon="fas fa-plus-circle" ToolTip="Add a saved filter" />
</div>

<div runat="server" id="SavedFilterField" class="mb-3">
    <div style="width:calc(100% - 87px); display:inline-block;">
        <insite:ComboBox runat="server" ID="SavedFilterSelector" Width="100%" />
    </div>
    <insite:Button runat="server" ID="SaveFilterButton" style="padding:0.625rem;margin-bottom:0.2rem" 
        ButtonStyle="Default" Icon="fas fa-save" ToolTip="Save the selected saved filter" />
    <insite:Button runat="server" ID="RemoveFilterButton" style="padding:0.625rem;margin-bottom:0.2rem"
        ConfirmText="Are you sure you want to delete this saved filter?"
        ButtonStyle="Default" Icon="fas fa-trash-alt" ToolTip="Delete the selected saved filter" />
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
                $savedFilterSelector = $('#<%= SavedFilterSelector.ClientID %>');

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