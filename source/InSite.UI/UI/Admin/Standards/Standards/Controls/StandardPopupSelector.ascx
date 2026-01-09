<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StandardPopupSelector.ascx.cs" Inherits="InSite.Admin.Standards.Standards.Controls.StandardPopupSelector" %>

<%@ Register Src="StandardPopupSelectorWindow.ascx" TagName="PopupWindow" TagPrefix="uc" %>

<insite:PageHeadContent runat="server" ID="StyleLiteral">
    <style type="text/css">
        .asset-popup-selector-input {
        }

            .asset-popup-selector-input input[type="text"] {
                background-color: #fff !important;
            }

            .asset-popup-selector-input a.btn-open,
            .asset-popup-selector-input a.btn-clear {
                display: block;
                position: absolute;
                line-height: 44px;
                padding: 0 6px;
            }
    </style>
</insite:PageHeadContent>

<uc:PopupWindow runat="server" ID="PopupWindow" />

<div>
    <div style="position: relative;" class="asset-popup-selector-input">
        <a runat="server" id="ClearButton" href="#" class="btn-clear" style="right: 38px; display: none;" title="Clear"><i class="fas fa-times"></i></a>
        <a runat="server" id="OpenButton" href="#" class="btn-open" style="right: 10px;" title="Open Popup"><i class="fas fa-search"></i></a>
        <input runat="server" id="TextInput" type="text" class="insite-text form-control w-100" style="padding-right:60px;" readonly="readonly" />
        <asp:HiddenField runat="server" ID="ValueInput" />
        <asp:HiddenField runat="server" ID="SubTypeInput" />
    </div>
</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            var $textInput = $('#<%= TextInput.ClientID %>');
            var $valueInput = $('#<%= ValueInput.ClientID %>');
            var $subTypeInput = $('#<%= SubTypeInput.ClientID %>');
            var $clearButton = $('#<%= ClearButton.ClientID %>');

            if ($valueInput.val().length > 0) {
                $clearButton.show();
            }

            $('#<%= OpenButton.ClientID %>').on('click', onPopupOpenClick);
            $clearButton.on('click', onClearClick);

            function onPopupOpenClick(e) {
                e.preventDefault();

                standardPopupSelectorWindow.<%= PopupWindow.ClientID %>.open({
                    value: $valueInput.val(),
                    subtype: $subTypeInput.val(),
                    onSelected: function (data) {
                        $textInput.val(data.text).trigger('focus');
                        $valueInput.val(data.id);
                        $subTypeInput.val(data.subtype);
                        $clearButton.show();
                    }
                });
            }

            function onClearClick(e) {
                e.preventDefault();

                $textInput.val('').trigger('focus');
                $valueInput.val('');
                $clearButton.hide();
            }
        })();
    </script>
</insite:PageFooterContent>
