<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CollectionFolderRepeater.ascx.cs" Inherits="InSite.Admin.Accounts.Organizations.Controls.CollectionFolderRepeater" %>

<div runat="server" id="NoFoldersMessage" style="margin-top:8px; padding:8px 16px; background-color:#f5f5f5; border:1px solid #ccc; border-radius:4px;" visible="false">
    No Folders
</div>

<insite:CustomValidator runat="server" ID="FolderNameRequiredValidator" Display="None" />
<insite:CustomValidator runat="server" ID="FolderNameDuplicateValidator" Display="None" />
<insite:CustomValidator runat="server" ID="FolderItemRequiredValidator" Display="None" />
<insite:CustomValidator runat="server" ID="ItemNameRequiredValidator" Display="None" />
<insite:CustomValidator runat="server" ID="ItemNameDuplicateValidator" Display="None" />

<asp:HiddenField runat="server" ID="ReorderData" />

<asp:Repeater runat="server" ID="FolderRepeater" Visible="false">
    <HeaderTemplate><div id='<%# ClientID %>' class="folder-list"></HeaderTemplate>
    <FooterTemplate></div></FooterTemplate>
    <ItemTemplate>
        <div data-index='<%# Container.ItemIndex %>'>
            <div class="commands">
                <insite:IconButton runat="server" CommandName="AddItem" ToolTip="Add Item" Name="plus-circle" />
                <insite:IconButton runat="server" CommandName="RemoveFolder" ToolTip="Remove Folder" OnClientClick="if (!confirm('Are you sure you want to remove this folder?')) return false;" Name="trash-alt" />
            </div>

            <insite:TextBox runat="server" ID="FolderName" Text='<%# Eval("Name") %>' EmptyMessage="Folder Name" MaxLength="250" />

            <asp:Repeater runat="server" ID="ItemRepeater">
                <HeaderTemplate><div class="items-list"></HeaderTemplate>
                <FooterTemplate></div></FooterTemplate>
                <ItemTemplate>
                    <div data-num='<%# Eval("Number") %>'>
                        <div class="commands commands-left">
                            <span class="cmd-move"><i class="fas fa-arrows-alt-v"></i></span>
                        </div>
                        <div class="commands commands-right">
                            <insite:IconButton runat="server" Name="trash-alt" CommandName="RemoveItem" CommandArgument='<%# Eval("Number") %>' ToolTip="Remove Item" OnClientClick="if (!confirm('Are you sure you want to remove this item?')) return false;" />
                        </div>
                        
                        <insite:TextBox runat="server" ID="ItemName" Text='<%# Eval("Name") %>' EmptyMessage="Item Name" MaxLength="250" />
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </ItemTemplate>
</asp:Repeater>

<insite:PageHeadContent runat="server" ID="CommonStyle">
    <style type="text/css">
        .folder-list {
            margin-top: 8px;
        }

            .folder-list span[data-val-controltovalidate] {
                position: absolute;
                line-height: 35px;
            }

            .folder-list .commands {
                position: absolute;
                line-height: 35px;
            }

            .folder-list > div {
                position: relative;
                padding-right: 55px;
                margin-bottom: 16px;
            }

                .folder-list > div > .commands {
                    right: 0;
                    text-align: right;
                }

                    .folder-list > div > .commands a {
                        text-decoration: none !important;
                    }

                .folder-list > div > span[data-val-controltovalidate] {
                    right: 63px;
                }

                .folder-list > div > .items-list {
                    padding-left: 38px;
                }

                    .folder-list > div > .items-list > div {
                        margin-top: 8px;
                        position: relative;
                    }

                        .folder-list > div > .items-list > div > span[data-val-controltovalidate] {
                            right: 8px;
                        }

                        .folder-list > div > .items-list > div > .commands-right {
                            right: -55px;
                            text-align: right;
                        }

                        .folder-list > div > .items-list > div > .commands-left {
                            left: -38px;
                        }

                            .folder-list > div > .items-list > div > .commands-left > .cmd-move {
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
            var instance = window.collectionFolderRepeater = window.collectionFolderRepeater || {};

            instance.init = function (containerId, inputId) {
                var $container = $('#' + String(containerId));
                var $input = $('#' + String(inputId));
                if ($container.length !== 1 || $container.data('inited') === true || $input.length !== 1)
                    return;
               
                $container.find('> div > .items-list').sortable({
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
                            var $collection = $(this);
                            var collectionData = [];

                            $collection.find('> .items-list > div').each(function () {
                                collectionData.push($(this).data('num'));
                            });

                            data.push(String($collection.data('index')) + ':' + collectionData.join(','));
                        });

                        $input.val(data.join(';'));
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