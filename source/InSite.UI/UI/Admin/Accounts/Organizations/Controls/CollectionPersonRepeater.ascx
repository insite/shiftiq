<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CollectionPersonRepeater.ascx.cs" Inherits="InSite.Admin.Accounts.Organizations.Controls.CollectionPersonRepeater" %>

<div runat="server" id="NoItemsMessage" style="margin-top:8px; padding:8px 16px; background-color:#f5f5f5; border:1px solid #ccc; border-radius:4px;" visible="false">
    No Items
</div>

<insite:CustomValidator runat="server" ID="RequiredValidator" Display="None" />
<insite:CustomValidator runat="server" ID="DuplicateValidator" Display="None" />

<asp:HiddenField runat="server" ID="ReorderData" />

<asp:Repeater runat="server" ID="Repeater" Visible="false">
    <HeaderTemplate><div id='<%# ClientID %>' class="collection-person-list"></HeaderTemplate>
    <FooterTemplate></div></FooterTemplate>
    <ItemTemplate>
        <div>
            <div class="commands commands-left">
                <span class="cmd-move"><i class="fas fa-arrows-alt-v"></i></span>
            </div>
            <div class="commands commands-right">
                <insite:IconButton runat="server" CommandName="Delete" CommandArgument='<%# Eval("ItemNumber") %>' Name="trash-alt" 
                    ToolTip="Remove Item" OnClientClick="if (!confirm('Are you sure you want to remove this contact?')) return false;" />
            </div>
            <insite:FindPerson runat="server" ID="PersonID" />
        </div>
    </ItemTemplate>
</asp:Repeater>

<insite:PageHeadContent runat="server" ID="CommonStyle">
    <style type="text/css">
        .collection-person-list {
        }

            .collection-person-list > div {
                margin-top: 8px;
                position: relative;
                padding: 0 38px;
            }

                .collection-person-list > div > .commands {
                    position: absolute;
                    line-height: 35px;
                }

                .collection-person-list > div > .commands-right {
                    right: 0;
                    text-align: right;
                }

                    .collection-person-list > div > .commands-right a {
                        padding: 8px;
                    }

                .collection-person-list > div > .commands-left {
                    left: 0;
                }

                    .collection-person-list > div > .commands-left > .cmd-move {
                        padding: 8px;
                        cursor: grab;
                        text-align: center;
                        vertical-align: baseline;
                    }
    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server" ID="CommonScript">
    <script type="text/javascript">
        (function () {
            var instance = window.collectionPersonRepeater = window.collectionPersonRepeater|| {};

            instance.init = function (containerId, inputId) {
                var $container = $('#' + String(containerId));
                var $input = $('#' + String(inputId));
                if ($container.length !== 1 || $container.data('inited') === true || $input.length !== 1)
                    return;

                $container.find('> div').each(function (i) {
                    $(this).data('index', i);
                });

                $container.sortable({
                    items: '> div',
                    containment: 'document',
                    cursor: 'grabbing',
                    forceHelperSize: true,
                    handle: '> .commands .cmd-move',
                    axis: 'y',
                    opacity: 0.65,
                    tolerance: 'pointer',
                    update: function (e, ui) {
                        var data = [];

                        $container.find('> div').each(function () {
                            data.push($(this).data('index'));
                        });

                        $input.val(data.join(','));
                    },
                    stop: function (e, ui) {
                        ui.item.removeAttr('style');
                    }
                }).disableSelection();

                $container.data('inited', true);
            };
        })();
    </script>
</insite:PageFooterContent>