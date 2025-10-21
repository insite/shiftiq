<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StandardSelector.ascx.cs" Inherits="InSite.Admin.Assessments.Banks.Controls.StandardSelector" %>
<%@ Register Src="StandardSelectorModal.ascx" TagName="StandardSelectorModal" TagPrefix="uc" %>

<insite:PageHeadContent runat="server">
    <style type="text/css">
        .standard-popup-selector-input {
        }

            .standard-popup-selector-input input[type="text"] {
                background-color: #fff !important;
            }

            .standard-popup-selector-input a.btn-open,
            .standard-popup-selector-input a.btn-clear {
                display: block;
                position: absolute;
                line-height: 44px;
                padding: 0 6px;
            }
    </style>
</insite:PageHeadContent>

<uc:StandardSelectorModal runat="server" ID="StandardSelectorModal" />

<div>
    <div style="position: relative;" class="standard-popup-selector-input">
        <a runat="server" id="ClearButton" href="#" class="btn-clear" style="right: 38px; display: none;" title="Clear"><i class="fas fa-times"></i></a>
        <a runat="server" id="OpenButton" href="#" class="btn-open" style="right: 10px;" title="Open Popup"><i class="fas fa-search"></i></a>
        <input runat="server" id="TextInput" type="text" class="insite-text form-control" readonly="readonly" style="padding-right: 60px; width: 100%;" />
        <asp:HiddenField runat="server" ID="ValueInput" />
    </div>
</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            var $textInput = $('#<%= TextInput.ClientID %>');
            var $valueInput = $('#<%= ValueInput.ClientID %>');
            var $clearButton = $('#<%= ClearButton.ClientID %>');

            if ($valueInput.val().length > 0) {
                $clearButton.show();
            }

            $('#<%= OpenButton.ClientID %>').on('click', onPopupOpenClick);
            $clearButton.on('click', onClearClick);

            function onPopupOpenClick(e) {
                e.preventDefault();

                standardPopupSelectorWindow.<%= StandardSelectorModal.ClientID %>.open({
                    value: $valueInput.val(),
                    onSelected: function (data) {
                        $textInput.val(data.text).trigger('focus');
                        $valueInput.val(data.id);
                        $clearButton.show();
                    }
                });
            }

            function onClearClick(e) {
                e.preventDefault();

                $textInput.val("").trigger('focus');
                $valueInput.val("");
                $clearButton.hide();
            }
        })();
    </script>
</insite:PageFooterContent>
