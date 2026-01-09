<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ItemGrid.ascx.cs" Inherits="InSite.Admin.Utilities.Collections.Controls.ItemGrid" %>

<insite:PageHeadContent runat="server">
    <style type="text/css">
        .color-example {
            display: inline-block;
            height: 16px;
            width: 16px;
            border-radius: 8px;
            margin: 0 3px 2px 0;
            vertical-align: middle;
        }

        .ui-sortable {
        }

        .ui-sortable > tr {
            cursor: move !important;
        }

        .ui-sortable > tr > td:first-child i.move {
            display: inline-block !important;
        }

        .ui-sortable > tr:hover {
            outline: 1px solid #666666;
        }

        .ui-sortable > tr.ui-sortable-helper {
            outline: 1px solid #666666;
        }

        .ui-sortable > tr.ui-sortable-helper > td:first-child {
            background-image: none !important;
        }

        .ui-sortable > tr.ui-sortable-placeholder {
            visibility: visible !important;
            outline: 1px dashed #666666 !important;
        }

        .ui-sortable > tr.ui-sortable-placeholder > td:first-child {
            background-image: none !important;
        }
    </style>
</insite:PageHeadContent>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

<insite:UpdatePanel runat="server" ID="UpdatePanel">
    <ContentTemplate>
        <div class="mb-3">
            <div class="reorder-trigger reorder-hide">
                <insite:Button runat="server" ID="AddButton" Text="Add New" Icon="fas fa-plus-circle" ButtonStyle="Default" />
                <insite:Button ID="ReorderButton" runat="server" Icon="fas fa-sort" Text="Reorder"
                    ButtonStyle="Default" OnClientClick="collectionItemGrid.reorder.start(); return false;" Visible="false" />
            </div>

            <div class="reorder-trigger reorder-visible reorder-inactive">
                <insite:SaveButton runat="server" OnClientClick="collectionItemGrid.reorder.save(); return false;" />
                <insite:CancelButton runat="server" OnClientClick="collectionItemGrid.reorder.cancel(); return false;" />
            </div>
        </div>

        <insite:Grid runat="server" ID="Grid" DataKeyNames="ItemNumber">
            <Columns>

                <asp:TemplateField ItemStyle-Width="40px" ItemStyle-Wrap="false" ItemStyle-CssClass="reorder-remove" HeaderStyle-CssClass="reorder-remove">
                    <ItemTemplate>
                        <insite:IconLink runat="server" Name="pencil" ToolTip="Edit"
                            NavigateUrl='<%# "/ui/admin/assets/collections/edit-item?" + Eval("Item.ItemIdentifier", "item={0}") %>' />
                        <insite:IconLink runat="server" Name="trash-alt" ToolTip="Delete"
                            NavigateUrl='<%# "/ui/admin/assets/collections/delete-item?" + Eval("Item.ItemIdentifier", "item={0}") %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Organization">
                    <ItemTemplate>
                        <%# Eval("Organization.CompanyName") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField HeaderText="Folder" DataField="Item.ItemFolder" />

                <asp:TemplateField HeaderText="Name">
                    <ItemTemplate>
                        <%# Eval("Item.ItemName") %>
                        <div class="form-text"><%# GetItemDescription() %></div>
                        <%# (Eval("Item.ItemNameTranslation") != null ? "<span class='badge bg-success'>Translated</span>" : "") %>
                        <%# (Eval("Item.ItemIcon") != null ? "<i class='" + (string)Eval("Item.ItemIcon") + "'></i>" : "") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Color" ItemStyle-Wrap="false">
                    <ItemTemplate>
                        <%# Eval("ItemColorBoxHtml") %>
                        <%# Eval("ItemColorName") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField HeaderText="Hours" DataField="Item.ItemHours" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />

                <asp:TemplateField HeaderText="Disabled" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Wrap="false">
                    <ItemTemplate>
                        <%# (bool)Eval("Item.ItemIsDisabled") ? "Yes" : "No" %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="References">
                    <ItemTemplate>
                        <%# GetReferenceCount() %>
                    </ItemTemplate>
                </asp:TemplateField>

            </Columns>
        </insite:Grid>
    </ContentTemplate>
</insite:UpdatePanel>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">

        (function () {
            var instance = window.collectionItemGrid = window.collectionItemGrid || {};

            instance.reorder = {
                start: function () {
                    inSite.common.gridReorderHelper.startReorder('collectionItems');
                },
                save: function () {
                    inSite.common.gridReorderHelper.saveReorder(true);
                },
                cancel: function () {
                    inSite.common.gridReorderHelper.cancelReorder(true);
                },
                updateItemStyle: function (item, index) {
                    if (index % 2 === 0) {
                        $(item).removeClass('rgAltRow').addClass('rgRow');
                    } else {
                        $(item).removeClass('rgRow').addClass('rgAltRow');
                    }
                },
                onStart: function (item, helper, placeholder) {
                    placeholder.removeClass('rgAltRow').removeClass('rgRow');
                },
            };
        })();

    </script>
</insite:PageFooterContent>
