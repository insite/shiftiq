<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PagePopupSelector.ascx.cs" Inherits="InSite.Admin.Sites.Pages.Controls.PagePopupSelector" %>

<%@ Register Src="PagePopupSelectorWindow.ascx" TagName="PopupWindow" TagPrefix="uc" %>

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
        <insite:IconButton Name="times" runat="server" ID="ClearButton" CssClass="btn-clear" style="right: 38px; display: none;"  ToolTip="Clear" />
        <insite:IconButton Name="search" runat="server" ID="OpenButton" CssClass="btn-open" style="right: 10px;" ToolTip="Open Popup" />
        <input runat="server" id="TextInput" type="text" class="insite-text form-control" readonly="readonly" style="padding-right:10px; width: 100%;" />
        <asp:HiddenField runat="server" ID="ValueInput" />
        <asp:HiddenField runat="server" ID="TypeInput" />
        <asp:HiddenField runat="server" ID="FixedTypeInput" />
        <asp:HiddenField runat="server" ID="SiteInput" />
    </div>
</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            var $textInput = $('#<%= TextInput.ClientID %>');
            var $valueInput = $('#<%= ValueInput.ClientID %>');
            var $typeInput = $('#<%= TypeInput.ClientID %>');
            var $fixedTypeInput = $('#<%= FixedTypeInput.ClientID %>');
            var $siteInput = $('#<%= SiteInput.ClientID %>');
            var $clearButton = $('#<%= ClearButton.ClientID %>');

            if ($valueInput.val().length > 0) {
                $clearButton.show();
            }

            $('#<%= OpenButton.ClientID %>').on('click', onPopupOpenClick);
            $clearButton.on('click', onClearClick);

            function onPopupOpenClick(e) {
                e.preventDefault();

                pagePopupSelectorWindow.<%= PopupWindow.ClientID %>.open({
                    value: $valueInput.val(),
                    type: $typeInput.val(),
                    fixedType: $fixedTypeInput.val(),
                    site: $siteInput.val(),
                    onSelected: function (data) {
                        $textInput.val(data.text).trigger('focus');
                        $valueInput.val(data.key);
                        $typeInput.val(data.type);
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
