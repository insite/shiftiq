<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CollectionItemRepeater.ascx.cs" Inherits="InSite.Admin.Accounts.Organizations.Controls.CollectionItemRepeater" %>

<insite:PageHeadContent runat="server" ID="HeaderContent">
    <style type="text/css">
        .res-cont-list {
            width: 100%;
        }

        .res-cont-list > h4:first-child {
            margin-top: 0;
        }

        .res-cont-list > h4 > span.small-print strong {
            color: #808080;
        }

        .res-cont-list > table {
            width: 100%;
        }

        .res-cont-list > table > tr > td {
            vertical-align: baseline !important;
        }

        .res-cont-list .ui-sortable {
        }

        .res-cont-list .ui-sortable > tr,
        .res-cont-list .ui-sortable > tbody > tr {
            cursor: move !important;
        }

        .res-cont-list .ui-sortable > tr > td:first-child i.move,
        .res-cont-list .ui-sortable > tbody > tr > td:first-child i.move {
            display: inline-block !important;
        }

        .res-cont-list .ui-sortable > tr:hover,
        .res-cont-list .ui-sortable > tbody > tr:hover {
            outline: 1px solid #666666;
        }

        .res-cont-list .ui-sortable > tr.ui-sortable-helper,
        .res-cont-list .ui-sortable > tbody > tr.ui-sortable-helper {
            outline: 1px solid #666666;
        }

        .res-cont-list .ui-sortable > tr.ui-sortable-helper > td:first-child,
        .res-cont-list .ui-sortable > tbody > tr.ui-sortable-helper > td:first-child {
            background-image: none !important;
        }

        .res-cont-list .ui-sortable > tr.ui-sortable-placeholder,
        .res-cont-list .ui-sortable > tbody > tr.ui-sortable-placeholder {
            visibility: visible !important;
            outline: 1px dashed #666666 !important;
        }

        .res-cont-list .ui-sortable > tr.ui-sortable-placeholder > td:first-child,
        .res-cont-list .ui-sortable > tbody > tr.ui-sortable-placeholder > td:first-child {
            background-image: none !important;
        }
    </style>
</insite:PageHeadContent>


<div runat="server" id="NoItemsMessage" style="margin-top: 8px; padding: 8px 16px; background-color: #f5f5f5; border: 1px solid #ccc; border-radius: 4px;" visible="false">
    No Items
</div>

<insite:CustomValidator runat="server" ID="RequiredValidator" Display="None" />
<insite:CustomValidator runat="server" ID="DuplicateValidator" Display="None" />

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

<insite:UpdatePanel runat="server" ID="UpdatePanel">
    <ContentTemplate>
        <div class="mb-3 text-end">
            <div id="CommandButtons" runat="server" class="reorder-trigger reorder-hide">
                <insite:Button runat="server" ID="ReorderButton" Text="Reorder" Icon="fas fa-sort" ButtonStyle="Default" />
            </div>
            <asp:Panel ID="ReorderCommandButtons" runat="server" CssClass="reorder-trigger reorder-visible reorder-inactive">
                <insite:SaveButton runat="server" OnClientClick="inSite.common.gridReorderHelper.saveReorder(true); return false;" />
                <insite:CancelButton runat="server" OnClientClick="inSite.common.gridReorderHelper.cancelReorder(true); return false;" />
            </asp:Panel>
        </div>

        <div runat="server" id="SectionControl">

            <div class="res-cont-list">

                <asp:Repeater runat="server" ID="Repeater" Visible="false">
                    <HeaderTemplate>
                        <table class="table">
                            <thead>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <FooterTemplate>
                        </tbody>
                        </table>
                    </FooterTemplate>
                    <ItemTemplate>
                        <tr>
                            <td class="res-name">
                                <div style="clear: both; padding-top: 8px;">
                                    <div style="display: inline-block; width: calc(100% - 30px);">
                                        <insite:TextBox runat="server" ID="ItemName" Text='<%# Eval("Name") %>' />
                                    </div>
                                    <insite:IconButton runat="server" CommandName="Delete" Name="trash-alt" CommandArgument='<%# Eval("Number") %>' ToolTip="Remove Item" OnClientClick="if (!confirm('Are you sure you want to remove this item?')) return false;" />
                                </div>
                            </td>
                        </tr>

                    </ItemTemplate>
                </asp:Repeater>

            </div>
        </div>

    </ContentTemplate>
</insite:UpdatePanel>

<asp:Button runat="server" ID="RefreshButton" Style="display: none;" />