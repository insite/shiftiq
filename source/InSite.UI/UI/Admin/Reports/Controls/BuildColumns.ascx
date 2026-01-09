<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BuildColumns.ascx.cs" Inherits="InSite.UI.Admin.Reports.Controls.BuildColumns" %>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

<insite:UpdatePanel runat="server" ID="UpdatePanel">
    <ContentTemplate>
        <insite:Alert runat="server" ID="ControlStatus" />

        <div class="row settings">

            <div class="col-md-6">

                <insite:ComboBox runat="server" ID="ViewSelector" AllowBlank="false" />

                <div runat="server" id="ViewPanel" style="padding-top: 15px;">
                    <div class="mb-3">
                        <insite:TextBox runat="server" ID="FilterText" EmptyMessage="Filter" />
                    </div>

                    <table class="column-selector">
                        <tbody>
                            <tr><td>
                                <insite:CheckBox runat="server" ID="SelectAll" />
                            </td></tr>

                            <asp:Repeater runat="server" ID="ColumnRepeater">
                                <ItemTemplate>
                                    <tr><td>
                                        <insite:CheckBox runat="server" ID="IsSelected" AutoPostBack="true"
                                            Text='<%# Eval("Text") %>' Value='<%# Eval("Value") %>'
                                            OnCheckedChanged="IsSelected_CheckedChanged" />
                                    </td></tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>

            </div>

            <div class="col-md-6">

                <div class="mb-3">
                    <insite:ClearButton runat="server" ID="ClearColumnSelectionButton" CausesValidation="false" />
                </div>

                <asp:Repeater runat="server" ID="SelectedColumnRepeater">
                    <HeaderTemplate>
                        <ul class="column-list">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li class="item-column">
                            <span><%# Container.DataItem %></span>
                            <insite:Button runat="server" Icon="far fa-trash-alt" ToolTip='<%# Translate("Delete Column") %>'
                                CommandName="DeleteColumn" CommandArgument='<%# Container.DataItem %>' />
                        </li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>

                <div style="margin-bottom:10px">

                    <div class="reorder-remove">
                        <insite:Button ID="ReorderColumnStartButton" runat="server" Icon="fas fa-sort" Text="Reorder"
                            ButtonStyle="Default" OnClientClick="buildReport.columnReorder.start(); return false;" />
                    </div>

                    <div class="reorder-visible" style="display:none;">
                        <insite:SaveButton runat="server" ID="ReorderColumnSaveButton" OnClientClick="buildReport.columnReorder.stop(); return true;" />
                        <insite:CancelButton runat="server" ID="ReorderColumnCancelButton" OnClientClick="buildReport.columnReorder.stop(); return true;" />
                        <asp:HiddenField runat="server" ID="ReorderColumnState" />
                    </div>

                </div>

            </div>

        </div>
    </ContentTemplate>
</insite:UpdatePanel>

<insite:PageHeadContent runat="server">
    <style type="text/css">
        ul.column-list {
        }

            ul.column-list li.item-column > span {
                display: inline-block;
                min-width: 50%;

            }
    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server">
    <script type="text/javascript"> 

        Sys.Application.add_load(function () {
            $('#<%= FilterText.ClientID %>')
                .off('keyup', onKeyUp)
                .on('keyup', onKeyUp);

            onKeyUp();
        });

        function onKeyUp() {

            var text = $('#<%= FilterText.ClientID %>').val().toLowerCase();

            $(".column-selector label").each(function () {
                
                if ($(this).text().toLowerCase().search(text) < 0) {
                    $(this).parent().parent().hide();
                }
                else {
                    $(this).parent().parent().show();
                }
            });
        }

    </script>

    <script type="text/javascript">

        (function () {
            var instance = window.buildReport = window.buildReport || {};

            instance.columnReorder = {
                start: function () {
                    $('#<%= ReorderColumnState.ClientID %>').val("");

                    $('ul.column-list > li.item-column').each(function (columnIndex) {
                        $(this).data('order-index', String(columnIndex));
                    });

                    $('ul.column-list').sortable({
                        items: '> li.item-column',
                        connectWith: 'ul.column-list',
                        containment: 'document',
                        cursor: 'grabbing',
                        forceHelperSize: true,
                        axis: 'y',
                        opacity: 0.65,
                        tolerance: 'pointer',
                        dropOnEmpty: true,
                        update: function (e, ui) {
                            var data = '';

                            $('ul.column-list > li.item-column').each(function (columnIndex) {
                                data += $(this).data('order-index') + ':' + String(columnIndex) + ';';
                            });

                            $('#<%= ReorderColumnState.ClientID %>').val(data);
                        },
                    }).disableSelection();

                    $('.reorder-remove').hide();
                    $('.reorder-visible').show();
                },

                stop: function () {
                    $('.reorder-remove').show();
                    $('.reorder-visible').hide();
                }
            };
        })();
    </script>
</insite:PageFooterContent>